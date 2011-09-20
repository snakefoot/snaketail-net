#region License statement
/* SnakeTail is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;

namespace SnakeTail
{
    public partial class TailForm : Form, ITailForm
    {
        // Todo Implement inline selection when owner-drawn
        // Todo Implement inline keyword highlight when owner-drawn
        // Todo Filter Text File
        // Todo Consider keeping file handle open, even when file is renamed / deleted (show history)
        // Todo Jump Lists
        // Todo Use background workerthread when searching (First visual cache, new file-stream, lock scrolling and timers, extra step to check for new lines, allow cancel search)
        // Todo Cache search results when searching up
        // Todo Scroll to the same matching line in all views
        // Todo Use background workerthread when checking for new file (avoid network timeout)
        //  - It needs to be a seperate thread that both does the reading and file checking
        //  - The ListViewItem cache should reside in the GUI context
        //  - Make active thread object, which the GUI context can contact
        //      - Read next line (Polls the next line if one exist)
        //      - Total number of lines (Attempt to make guess ?)
        //  - Active thread keeps internal cache of 100 lines
        //      - Only when lines has been read, then it continues
        //  - There is also a LogStreamObject object in the GUI context for searching and reading backwards
        //      - We close / reopen the second file handle for each operation
        //          - This will slow down the operation when scrolling down
        //              - Another work around is to keep a version on the LogStreamObject, and increase every time it reopens
        //              - We could fix this if enabling dynamic file positioning
        //              - Maybe just implement a solution where it is slow first ?
        // Todo Implement log stream that can read backwards
        //  - Can start at the bottom and display at once without loading entire file
        //  - Open file, read BOM (If default specified)
        //  - Seek to bottom - 64K
        //  - Start seeking for newlines using the encoding from the BOM
        //  - Guess the total number of lines based on the lines in the 64K
        //  - When scrolling how to lookup the line numbers requested
        //      - Perform file seek based on percentage
        //  - When using page-up to reach top then it will read backwards in 64K blocks
        //      - Should always expect the total number of lines as a guess
        //      - If too many lines are guessed when we reach file-start before top of list (adjust guess)
        //      - If too few lines are guessed before we reach file-start we are at top of lies (adjust guess)
        //      - Must support that scrolling have been performed so it is always an adjusted guess
        // Todo Implement log stream that skips to the end at beginning
        //  - Only when requested to search the file backwards beyond the cache, then it reads the file from the beginning
        //  - Has to make a good guess on the number of lines in the file
        //      - Based on the number of the lines in the last X bytes of the file
        //  - When requesting a line outside the cache, then it performs a proper line calculation
        //      - How do we merge the correct line-count with the one guessed when using arrow-up ?
        //          - File position is not a good friend when using file cache
        //              - We can make the file reader use chunks of 64K
        //          - Who should be responsible for conversion?
        //              - Extend cache class so it wraps the two file-streams
        //                  - Scroll to bottoom
        //                  - Scroll to top
        //                  - Search
        //                  - Check tail (and new file)
        //                  - Readline
        //                  - Line Count
        //                  - Prepare Cache
        //                  - All methods can modify the line count
        LogFileCache _logFileCache = null;
        LogFileStream _logFileStream = null;
        LogFileStream _logTailStream = null;
        TaskMonitor _taskMonitor = null;
        bool _topIndexHack = false;
        bool _pauseMode = false;
        string _formTitle = "";
        string _formIconFile = "";
        DateTime _lastFormTitleUpdate = DateTime.Now;
        Icon _formCustomIcon = null;
        Icon _formMaximizedIcon = null;
        string _configPath = "";
        List<TailKeywordConfig> _keywordHighlight;
        int _loghitCounter = -1;

        public TailForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.LogIcon;
        }

        public bool Paused
        {
            get
            {
                return _pauseMode;
            }
            set
            {
                _pauseMode = value;
                if (_pauseMode)
                    SetStatusBar("Paused");
                else
                    SetStatusBar(null);
            }
        }

        public Form TailWindow { get { return this; } }

        public void LoadFile(string filepath)
        {
            TailFileConfig tailConfig = new TailFileConfig();
            tailConfig.FilePath = filepath;
            LoadConfig(tailConfig, "");
        }

        public void LoadConfig(TailFileConfig tailConfig, string configPath)
        {
            _configPath = configPath;

            if (tailConfig.FileCacheSize <= 0)
                tailConfig.FileCacheSize = 1000;

            if (tailConfig.FormBackColor != null)
                _tailListView.BackColor = tailConfig.FormBackColor.Value;
            if (tailConfig.FormTextColor != null)
                _tailListView.ForeColor = tailConfig.FormTextColor.Value;

            if (tailConfig.FormFont != null)
                _tailListView.Font = tailConfig.FormFont;

            if (tailConfig.FileChangeCheckInterval > 0)
                _tailTimer.Interval = tailConfig.FileChangeCheckInterval;

            _loghitCounter = -1;
            _keywordHighlight = tailConfig.KeywordHighlight;
            if (_keywordHighlight != null)
            {
                foreach (TailKeywordConfig keyword in _keywordHighlight)
                {
                    if (keyword.MatchRegularExpression)
                    {
                        if (keyword.MatchCaseSensitive)
                            keyword.KeywordRegex = new System.Text.RegularExpressions.Regex(keyword.Keyword);
                        else
                            keyword.KeywordRegex = new System.Text.RegularExpressions.Regex(keyword.Keyword, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    }
                    else
                        keyword.KeywordRegex = null;

                    if (keyword.LogHitCounter)
                        _loghitCounter = 0;
                }
            }

            Encoding fileEncoding = tailConfig.EnumFileEncoding;

            if (_logFileStream == null || _logFileStream.FilePath != tailConfig.FilePath || _logFileStream.FileEncoding != fileEncoding || _logFileStream.FileCheckInterval != tailConfig.FileCheckInterval || _logFileStream.FileCheckPattern != tailConfig.FileCheckPattern)
                _logFileStream = new LogFileStream(configPath, tailConfig.FilePath, fileEncoding, tailConfig.FileCheckInterval, tailConfig.FileCheckPattern);
            if (_logTailStream == null || _logTailStream.FilePath != tailConfig.FilePath || _logTailStream.FileEncoding != fileEncoding || _logTailStream.FileCheckInterval != tailConfig.FileCheckInterval || _logTailStream.FileCheckPattern != tailConfig.FileCheckPattern)
                _logTailStream = new LogFileStream(configPath, tailConfig.FilePath, fileEncoding, tailConfig.FileCheckInterval, tailConfig.FileCheckPattern);

            if (_logFileCache != null)
                _logFileCache.Reset();

            if (_logFileCache == null || _logFileCache.Items.Count != tailConfig.FileCacheSize)
            {
                _logFileCache = new LogFileCache(tailConfig.FileCacheSize);
                _logFileCache.LoadingFileEvent += new EventHandler(_logFileCache_LoadingFileEvent);
                _logFileCache.FillCacheEvent += new EventHandler(_logFileCache_FillCacheEvent);
                // Add loading of cache while counting lines in file
                int lineCount = _logFileCache.FillTailCache(_logTailStream);
                _tailListView.VirtualListSize = lineCount;
            }
            else
            {
                _logFileCache.LoadingFileEvent += new EventHandler(_logFileCache_LoadingFileEvent);
                _logFileCache.FillCacheEvent += new EventHandler(_logFileCache_FillCacheEvent);
            }

            if (!string.IsNullOrEmpty(tailConfig.ServiceName))
                _taskMonitor = new TaskMonitor(tailConfig.ServiceName);

            if (tailConfig.Title != null)
                _formTitle = tailConfig.Title;
            else
                _formTitle = Path.GetFileName(tailConfig.FilePath);

            TabPage parentTab = this.Tag as TabPage;
            if (parentTab != null)
                parentTab.Text = _formTitle;

            if (!string.IsNullOrEmpty(tailConfig.IconFile))
            {
                _formIconFile = tailConfig.IconFile;
                string formIconFilePathAbsolute = System.IO.Path.Combine(configPath, _formIconFile);
                try
                {
                    _formCustomIcon = System.Drawing.Icon.ExtractAssociatedIcon(formIconFilePathAbsolute);
                    _formMaximizedIcon = Icon;
                    Icon = _formCustomIcon;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load icon\n\n   " + formIconFilePathAbsolute + "\n\n" + ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            UpdateFormTitle(true);

            if (Visible)
            {
                if (_tailListView.VirtualListSize > 0)
                {
                    _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
                    _tailListView.Invalidate();
                    _tailListView.Update();
                }
            }
        }

        void UpdateFormTitle(bool force)
        {
            if (!force && DateTime.Now.Subtract(_lastFormTitleUpdate) < TimeSpan.FromSeconds(1))
                return;

            string title = _formTitle;

            if (_loghitCounter != -1)
            {
                title += " Hits: " + _loghitCounter.ToString();
                _loghitCounter = 0;
            }

            if (_taskMonitor != null)
            {
                try
                {
                    float cpuUtilization = _taskMonitor.ProcessorUsage;
                    Process process = _taskMonitor.Process;
                    if (process != null)
                    {
                        if (!float.IsNaN(cpuUtilization))
                        {
                            TimeSpan cpuTime = DateTime.Now.Subtract(_lastFormTitleUpdate);
                            double cpuUsage = (double)cpuUtilization / cpuTime.TotalMilliseconds * 1000.0;
                            title += " CPU: " + cpuUtilization.ToString("F0");
                        }
                        process.Refresh();
                        title += " RAM: " + (process.PrivateMemorySize64 / (1024 * 1024)).ToString();
                        title += _taskMonitor.ServiceRunning ? " (Started)" : " (Stopped)";
                    }
                    else
                    {
                        title += " (Stopped)";
                    }
                }
                catch (Exception ex)
                {
                    title += " (" + ex.Message + ")";
                }
            }

            _lastFormTitleUpdate = DateTime.Now;
            Text = title;
        }

        public void SetStatusBar(string text)
        {
            SetStatusBar(text, 0, 0);
        }

        public void SetStatusBar(string text, int progressValue, int progressMax)
        {
            if (_statusStrip.Visible)
            {
                _statusProgressBar.Maximum = progressMax;
                _statusProgressBar.Value = progressValue;
                if (progressMax == 0 && progressValue == 0)
                    _statusProgressBar.Visible = false;
                else
                    _statusProgressBar.Visible = true;

                if (text == null)
                    _statusTextBar.Text = Paused ? "Paused" : "Ready";
                else
                    _statusTextBar.Text = text;

                _statusStrip.Invalidate();
                _statusStrip.Update();
            }
            else
            {
                if (text == null && Paused)
                    MainForm.Instance.SetStatusBar("Paused", progressValue, progressMax);
                else
                    MainForm.Instance.SetStatusBar(text, progressValue, progressMax);
            }
        }

        void _logFileCache_LoadingFileEvent(object sender, EventArgs e)
        {
            LogFileStream filestream = sender as LogFileStream;
            if (filestream != null)
            {
                double position = filestream.Position / (double)filestream.Length*100;
                SetStatusBar("Loading file...", (int)position, 100);
            }
            else
            {
                SetStatusBar(null);
            }
        }

        void _logFileCache_FillCacheEvent(object sender, EventArgs e)
        {
            if (sender != null)
            {
                SetStatusBar("Loading file cache...");
            }
            else
            {
                SetStatusBar(null);
            }
        }

        public void SaveConfig(TailFileConfig tailConfig)
        {
            tailConfig.FormBackColor = _tailListView.BackColor;
            tailConfig.FormTextColor = _tailListView.ForeColor;

            tailConfig.KeywordHighlight = _keywordHighlight;

            tailConfig.FormFont = _tailListView.Font;
            tailConfig.FileCacheSize = _logFileCache.Items.Count;
            tailConfig.EnumFileEncoding = _logTailStream.FileEncoding;
            tailConfig.FilePath = _logTailStream.FilePath;
            tailConfig.FileCheckInterval = _logTailStream.FileCheckInterval;
            tailConfig.FileChangeCheckInterval = _tailTimer.Interval;
            tailConfig.FileCheckPattern = _logTailStream.FileCheckPattern;
            tailConfig.Title = _formTitle;
            tailConfig.IconFile = _formIconFile;
            tailConfig.Modeless = MdiParent == null;
            tailConfig.WindowState = WindowState;
            if (WindowState == FormWindowState.Minimized)
            {
                tailConfig.WindowSize = RestoreBounds.Size;
                tailConfig.WindowPosition = RestoreBounds.Location;
            }
            else
            {
                tailConfig.WindowSize = Size;
                tailConfig.WindowPosition = DesktopLocation;
            }
            
            if (_taskMonitor != null)
                tailConfig.ServiceName = _taskMonitor.ServiceName;
            else
                tailConfig.ServiceName = "";
        }

        public void CopySelectionToClipboard()
        {
            // Copy selected rows to clipboard
            StringBuilder selection = new StringBuilder();
            foreach (int itemIndex in _tailListView.SelectedIndices)
            {
                if (selection.Length > 0)
                    selection.Append("\r\n");
                selection.Append(_tailListView.Items[itemIndex].Text);
            }
            Clipboard.SetText(selection.ToString());
        }

        private bool MatchTextSearch(string lineText, string searchText, bool matchCase, bool keywordHighlights)
        {
            if (keywordHighlights)
            {
                return MatchesKeyword(lineText, false) != null;
            }
            else
            if (matchCase)
            {
                if (0 <= lineText.IndexOf(searchText))
                {
                    return true;
                }
            }
            else
            {
                if (0 <= lineText.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private int SearchForTextForward(string searchText, bool matchCase, bool keywordHighlights, int startIndex, int endIndex, ref LogFileCache searchFileCache)
        {
            for (int i = startIndex; i < endIndex; ++i)
            {
                if (i % _logFileCache.Items.Count == 0)
                    SetStatusBar("Searching...", i, endIndex);

                string lineText = null;
                if (searchFileCache == null)
                {
                    ListViewItem lvi = _logFileCache.LookupCache(i);
                    if (lvi != null)
                        lineText = lvi.Text;
                    else
                    {
                        // Copy the current cache position, in case the search hit is the next line
                        searchFileCache = new LogFileCache(_logFileCache.Items.Count);
                        searchFileCache.Items = _logFileCache.Items.GetRange(0, _logFileCache.Items.Count);
                        searchFileCache.FirstIndex = _logFileCache.FirstIndex;
                    }
                }
                if (searchFileCache != null)
                {
                    ListViewItem lvi = searchFileCache.LookupCache(i);
                    if (lvi == null)
                    {
                        searchFileCache.PrepareCache(i, i + searchFileCache.Items.Count / 2, true);
                        searchFileCache.FillCache(_logFileStream, i + searchFileCache.Items.Count / 2);
                        lvi = searchFileCache.LookupCache(i);
                    }
                    lineText = lvi.Text;
                }

                if (MatchTextSearch(lineText, searchText, matchCase, keywordHighlights))
                    return i;
            }
            return -1;
        }

        public bool SearchForText(string searchText, bool matchCase, bool searchForward, bool keywordHighlights)
        {
            if (_tailListView.VirtualListSize == 0)
                return false;

            if (keywordHighlights && (_keywordHighlight == null || _keywordHighlight.Count == 0))
                return false;

            // Use selection if it is below top-index
            int startIndex = _tailListView.VirtualListSize - 1;
            if (_tailListView.SelectedIndices.Count > 0)
            {
                if (_tailListView.TopItem == null || _tailListView.TopItem.Index - 1 < _tailListView.SelectedIndices[0])
                    startIndex = _tailListView.SelectedIndices[0];
            }

            if (!searchForward)
            {
                // First use the visual cache, when that have failed, then revert to search from the beginning
                // and find the last match
                startIndex -= 1;
                for (int i = startIndex; i >= 0; --i)
                {
                    if (i % _logFileCache.Items.Count == 0)
                        SetStatusBar("Searching...", _tailListView.VirtualListSize - i, _tailListView.VirtualListSize);

                    string lineText;
                    ListViewItem lvi = _logFileCache.LookupCache(i);
                    if (lvi != null)
                        lineText = lvi.Text;
                    else
                    {
                        LogFileCache searchFileCache = null;
                        int matchFound = -1;
                        int lastMatchFound = -1;
                        startIndex = 0;
                        int endIndex = i + 1;
                        do
                        {
                            matchFound = SearchForTextForward(searchText, matchCase, keywordHighlights, startIndex, endIndex, ref searchFileCache);
                            if (matchFound != -1)
                            {
                                lastMatchFound = matchFound;
                                startIndex = matchFound + 1;
                                _tailListView.SelectedIndices.Clear();
                                _logFileCache = searchFileCache;    // Store the cache of the last match
                                searchFileCache = new LogFileCache(_logFileCache.Items.Count);
                                searchFileCache.Items = _logFileCache.Items.GetRange(0, _logFileCache.Items.Count);
                                searchFileCache.FirstIndex = _logFileCache.FirstIndex;
                            }
                        } while (matchFound != -1);

                        if (lastMatchFound != -1)
                        {
                            SetStatusBar(null);
                            _tailListView.SelectedIndices.Clear();
                            _tailListView.EnsureVisible(lastMatchFound);
                            _tailListView.SelectedIndices.Add(lastMatchFound);
                            _tailListView.Items[lastMatchFound].Focused = true;
                            return true;
                        }
                        else
                        {
                            SetStatusBar(null);
                            return false;
                        }
                    }

                    if (MatchTextSearch(lineText, searchText, matchCase, keywordHighlights))
                    {
                        SetStatusBar(null);
                        _tailListView.SelectedIndices.Clear();
                        _tailListView.EnsureVisible(i);
                        _tailListView.SelectedIndices.Add(i);
                        _tailListView.Items[i].Focused = true;
                        return true;
                    }
                }

                SetStatusBar(null);
                return false;
            }
            else
            {
                LogFileCache searchFileCache = null;
                startIndex += 1;
                int endIndex = _tailListView.VirtualListSize;

                int matchFound = SearchForTextForward(searchText, matchCase, keywordHighlights, startIndex, endIndex, ref searchFileCache);
                if (matchFound != -1)
                {
                    SetStatusBar(null);
                    _tailListView.SelectedIndices.Clear();  // Clear selection before changing cache to avoid cache miss
                    if (searchFileCache != null)
                        _logFileCache = searchFileCache;    // Swap cache before displaying search result
                    _tailListView.EnsureVisible(matchFound);
                    _tailListView.SelectedIndices.Add(matchFound);   // Set selection after having scrolled to avoid top-index cache miss
                    _tailListView.Items[matchFound].Focused = true;
                    return true;
                }
                else
                {
                    SetStatusBar(null);
                    return false;
                }
            }
        }

        private void _tailListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = _logFileCache.LookupCache(e.ItemIndex);
            if (e.Item != null)
                return;

            if (_logFileCache.FillCache(_logFileStream, e.ItemIndex) != -1)
            {
                e.Item = _logFileCache.LookupCache(e.ItemIndex);
                if (e.Item != null)
                    return;
            }

            ListViewItem lvi = new ListViewItem();
            lvi.SubItems.Add("");

            if (!_topIndexHack)
            {
                _topIndexHack = true;
                ListViewItem topItem = _tailListView.TopItem;
                _topIndexHack = false;
                if (topItem == null || e.ItemIndex < topItem.Index || e.ItemIndex > topItem.Index + 1000)
                {
                    // Ignore invalid requests outside the visible zone
                    lvi.Text = "";
                    e.Item = lvi;
                    return;
                }
            }

            lvi.Text = _logFileStream.ReadLine(e.ItemIndex + 1);      // assign the text to the item
            _logFileCache.SetLastCacheMiss(e.ItemIndex, lvi);
            e.Item = lvi;
        }

        private void TailForm_Load(object sender, EventArgs e)
        {
            if (MdiParent == null)
            {
                Icon = MainForm.Instance.Icon;
                // Fix issue where ListView scrollbar was hidden behind statusbar
                this.Controls.Remove(_statusStrip);
                _statusStrip.Visible = true;
                this.Controls.Add(_statusStrip);
            }
            else
            {
                _statusStrip.Visible = false;
            }

            _tailTimer.Enabled = true;

            _tailListView.EnableDoubleBuffer();

            using (Graphics graphics = _tailListView.CreateGraphics())
            {
                string textstring = new string('X', 1001);
                _tailListView.Columns[1].Width = (int)graphics.MeasureString(textstring, _tailListView.Font).Width;
            }

            if (_tailListView.VirtualListSize > 0)
            {
                _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
                _tailListView.Invalidate();
                _tailListView.Update();
            }
        }

        private void TailForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent event callbacks while we are closing down
            _tailTimer.Enabled = false;
            if (_logFileCache != null)
                _logFileCache.Reset();
        }

        private void _tailListView_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            _logFileCache.PrepareCache(e.StartIndex, e.EndIndex, false);
        }

        private void _tailListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
        }

        private void _tailListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex != 1)
                return;

            Color? textColor = null;;
            
            if (e.Item.Selected)
            {
                e.DrawFocusRectangle(e.Item.Bounds);
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }
            else
            {
                TailKeywordConfig keyword = MatchesKeyword(e.Item.Text, false);
                if (keyword != null)
                {
                    if (keyword.FormBackColor.HasValue && keyword.FormTextColor.HasValue)
                    {
                        using (Brush backBrush = new SolidBrush(keyword.FormBackColor.Value))
                        {
                            e.Graphics.FillRectangle(backBrush, e.Bounds);
                            textColor = keyword.FormTextColor.Value;
                        }
                    }
                }

                if (!textColor.HasValue)
                    e.DrawBackground();
            }

            if (!textColor.HasValue)
                textColor = _tailListView.ForeColor;

            using (Brush textBrush = e.Item.Selected ? null : new SolidBrush(textColor.Value))
            {
                if (e.Item.Text.Length > 1000)
                    e.Graphics.DrawString(e.Item.Text.Substring(0, 1000), _tailListView.Font, textBrush != null ? textBrush : SystemBrushes.HighlightText, e.Bounds);
                else
                    e.Graphics.DrawString(e.Item.Text, _tailListView.Font, textBrush != null ? textBrush : SystemBrushes.HighlightText, e.Bounds);
            }
        }

        private TailKeywordConfig MatchesKeyword(string line, bool logHitCounter)
        {
            if (_keywordHighlight != null && _keywordHighlight.Count > 0)
            {
                foreach (TailKeywordConfig keyword in _keywordHighlight)
                {
                    if (logHitCounter != keyword.LogHitCounter)
                        continue;

                    if ((keyword.KeywordRegex != null && keyword.KeywordRegex.IsMatch(line))
                      || (keyword.KeywordRegex == null && keyword.MatchCaseSensitive && line.IndexOf(keyword.Keyword, StringComparison.CurrentCulture) != -1)
                      || (keyword.KeywordRegex == null && !keyword.MatchCaseSensitive && line.IndexOf(keyword.Keyword, StringComparison.CurrentCultureIgnoreCase) != -1)
                       )
                    {
                        return keyword;
                    }
                }
            }
            return null;
        }

        private void _tailListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Add)
            {
                e.Handled = true;   // No auto resize
                return;
            }

            if (e.Control && e.KeyCode == Keys.Home)
            {
                _tailListView.SelectedIndices.Clear();
                if (_tailListView.VirtualListSize > 0)
                {
                    _tailListView.EnsureVisible(0);
                    _tailListView.SelectedIndices.Add(0);
                    _tailListView.Items[0].Focused = true;
                }
            }

            if (e.Control && e.KeyCode == Keys.End)
            {
                _tailListView.SelectedIndices.Clear();
                if (_tailListView.VirtualListSize > 0)
                {
                    _logFileCache.PrepareCache(Math.Max(_tailListView.VirtualListSize - _logFileCache.Items.Count,0), _tailListView.VirtualListSize, false);
                    _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
                    _tailListView.SelectedIndices.Add(_tailListView.VirtualListSize-1);
                    _tailListView.Items[_tailListView.VirtualListSize - 1].Focused = true;
                }
            }

            // For some weird reason the cache request for page-down / page-up comes after the item-requests
            if (e.KeyCode == Keys.PageDown)
            {
                int topIndex = _tailListView.TopItem.Index;
                _logFileCache.PrepareCache(topIndex + _logFileCache.Items.Count / 2, topIndex + _logFileCache.Items.Count / 2, false);
            }
            if (e.KeyCode == Keys.PageUp)
            {
                int topIndex = _tailListView.TopItem.Index;
                _logFileCache.PrepareCache(topIndex - _logFileCache.Items.Count / 2, topIndex + 100, false);
            }
            if (e.KeyCode == Keys.Pause)
            {
                if (!Paused)
                {
                    Paused = true;
                }
            }
            else
            {
                if (Paused)
                {
                    Paused = false;
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                        _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
                }
            }
        }

        private bool ListAtBottom()
        {
            if (Paused)
                return false;

            if (WindowState == FormWindowState.Minimized)
                return false;

            if (_tailListView.VirtualListSize <= 5)
                return true;

            if (_tailListView.TopItem == null)
                return false;   // There is no bottom

            return IsItemVisible(_tailListView.VirtualListSize - 5);
        }

        private bool IsItemVisible(int index)
        {
            if (_tailListView.TopItem == null || _tailListView.TopItem.Index > index)
                return false;

            int heightOfFirstItem = _tailListView.TopItem.Bounds.Height;
            int nVisibleLines = _tailListView.Height / heightOfFirstItem;
            int lastVisibleIndexInDetailsMode = _tailListView.TopItem.Index + nVisibleLines;
            return lastVisibleIndexInDetailsMode >= index;
        }

        private void _tailTimer_Tick(object sender, EventArgs e)
        {
            if (!_tailTimer.Enabled)
                return;

            UpdateFormTitle(false);

            int lineCount = _tailListView.VirtualListSize;
            bool listAtBottom = ListAtBottom();

            string line = _logTailStream.ReadLine(lineCount + 1);
            while(line != null)
            {
                ++lineCount;
                _logFileCache.AppendTailCache(line, lineCount);
                if (MatchesKeyword(line, true) != null)
                    _loghitCounter++;
                line = _logTailStream.ReadLine(lineCount + 1);
            }

            if (lineCount == _tailListView.VirtualListSize)
            {
                if (lineCount == 1 && _logTailStream.Length == 0)
                {
                    // Check if the open file error has changed
                    if (_tailListView.Items[0].Text == _logTailStream.ReadLine(1))
                        return;
                }
                else
                if (_logTailStream.ValidLineCount(lineCount))
                    return;

                lineCount = 0;
            }

            if (lineCount < _tailListView.VirtualListSize)
            {
                _logFileStream.CheckLogFile(true);
                _logFileCache = new LogFileCache(_logFileCache.Items.Count);
                _tailListView.TopItem = null;
                _tailListView.VirtualListSize = lineCount;
                _tailListView.Invalidate();
            }
            else
            {
                //_tailListView.VirtualListSize = linecount;
                ListViewUtil.SetVirtualListSizeWithoutRefresh(_tailListView, lineCount);
                if (listAtBottom && _tailListView.VirtualListSize > 0)
                    _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
            }
        }

        private void switchToModelessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TailForm newform = new TailForm();
            TailFileConfig tailConfig = new TailFileConfig();
            SaveConfig(tailConfig);
            if (this.MdiParent == null)
            {
                newform.MdiParent = MainForm.Instance;
                newform.ShowInTaskbar = false;
            }
            else
                newform.ShowInTaskbar = true;

            Close();

            newform.Show();
            newform._logFileCache = _logFileCache;
            newform._logFileStream = _logFileStream;
            newform._logTailStream = _logTailStream;
            newform._tailListView.VirtualListSize = _tailListView.VirtualListSize;
            newform.LoadConfig(tailConfig, _configPath);
            newform.BringToFront();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Shift | Keys.F3))
            {
                SearchForm.Instance.SearchAgain(this, false, false);
                return true;
            }
            else
            if (keyData == Keys.F3)
            {
                SearchForm.Instance.SearchAgain(this, true, false);
                return true;
            }
            if (keyData == (Keys.Control | Keys.Up))
            {
                SearchForm.Instance.SearchAgain(this, false, true);
                return true;
            }
            else
            if (keyData == (Keys.Control | Keys.Down))
            {
                SearchForm.Instance.SearchAgain(this, true, true);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnResize(EventArgs e)
        {
            bool listAtBottom = ListAtBottom();

            bool maximized = WindowState == FormWindowState.Maximized;
            base.OnResize(e);
            _tailListView.Invalidate();
            if (WindowState != FormWindowState.Minimized)
            {
                if (Paused || listAtBottom)
                {
                    Paused = false;
                    if (_tailListView.VirtualListSize > 0)
                        _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
                }
            }
            _tailListView.Update();
        }

        private void startServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_taskMonitor == null)
                    return;

                SetStatusBar("Starting service...", 1, 5);
                _taskMonitor.StartService();
                for (int i = 1; i <= 10; ++i)
                {
                    System.Threading.Thread.Sleep(500);
                    SetStatusBar("Starting service...", i / 2, 5);

                    if (_taskMonitor.ServiceRunning)
                        break;
                }
                SetStatusBar(null);
            }
            catch (Exception ex)
            {
                SetStatusBar(null);
                MessageBox.Show("Start service failed: " + ex.Message);
            }
        }

        private void stopServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_taskMonitor == null)
                    return;

                SetStatusBar("Stopping service...", 1, 5);
                _taskMonitor.StopService();
                for (int i = 1; i <= 10; ++i)
                {
                    System.Threading.Thread.Sleep(500);
                    SetStatusBar("Stopping service...", i / 2, 5);

                    if (!_taskMonitor.ServiceRunning)
                        break;
                }
                SetStatusBar(null);
            }
            catch (Exception ex)
            {
                SetStatusBar(null);
                MessageBox.Show("Stop service failed: " + ex.Message);
            }
        }

        private void pauseServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_taskMonitor == null)
                    return;

                SetStatusBar("Pause service...", 1, 5);
                _taskMonitor.PauseService();
                SetStatusBar(null);
            }
            catch (Exception ex)
            {
                SetStatusBar(null);
                MessageBox.Show("Pause service failed: " + ex.Message);
            }
        }

        private void resumeServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_taskMonitor == null)
                    return;

                SetStatusBar("Continue service...", 1, 5);
                _taskMonitor.ContinueService();
                SetStatusBar(null);
            }
            catch (Exception ex)
            {
                SetStatusBar(null);
                MessageBox.Show("Continue service failed: " + ex.Message);
            }
        }

        private void _contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            _contextMenuStrip_Opening(sender, (EventArgs)e);

            // We steal the items from the main menu (we restore them when closing again)
            ToolStripItem[] items = new ToolStripItem[_activeWindowMenuItem.DropDownItems.Count];
            _activeWindowMenuItem.DropDownItems.CopyTo(items, 0);
            _contextMenuStrip.Items.Clear();            // Clear the dummy item
            _contextMenuStrip.Items.AddRange(items);
        }

        private void _contextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            // Restore the items back to the main menu when closing
            ToolStripItem[] items = new ToolStripItem[_contextMenuStrip.Items.Count];
            _contextMenuStrip.Items.CopyTo(items, 0);
            _activeWindowMenuItem.DropDownItems.AddRange(items);
            _contextMenuStrip.Items.Clear();
            _contextMenuStrip.Items.Add(new ToolStripSeparator());  // Dummy item so menu is shown the next time
        }

        private void _contextMenuStrip_Opening(object sender, EventArgs e)
        {
            bool windowsService = _taskMonitor != null && _taskMonitor.ServiceController != null;
            bool pauseAndContinue = _taskMonitor != null && _taskMonitor.CanPauseAndContinue;
            startServiceToolStripMenuItem.Enabled = windowsService;
            stopServiceToolStripMenuItem.Enabled = windowsService;
            pauseServiceToolStripMenuItem.Visible = pauseAndContinue;
            resumeServiceToolStripMenuItem.Visible = pauseAndContinue;
            pauseWindowToolStripMenuItem.Checked = Paused;
        }

        private void TailForm_Activated(object sender, EventArgs e)
        {
            SetStatusBar(null);
            SearchForm.Instance.ActiveTailForm = this;
        }

        private void TailForm_Resize(object sender, EventArgs e)
        {
            if (MdiParent != null)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    if (_formMaximizedIcon != null)
                        Icon = _formMaximizedIcon;
                }
                else
                {
                    if (_formCustomIcon != null)
                        Icon = _formCustomIcon;
                }
            }
        }

        private void _copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Copy selected rows to clipboard
            StringBuilder selection = new StringBuilder();
            foreach (int itemIndex in _tailListView.SelectedIndices)
            {
                if (selection.Length > 0)
                    selection.Append("\r\n");
                selection.Append(_tailListView.Items[itemIndex].Text);
            }
            if (selection.Length > 0)
                Clipboard.SetText(selection.ToString());
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm.Instance.StartSearch(this);
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SearchForm.Instance.Visible)
            {
                SearchForm.Instance.SearchAgain(this, true, false);
            }
        }

        private void configureStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TailFileConfig configFile = new TailFileConfig();
            SaveConfig(configFile);
            TailConfigForm configForm = new TailConfigForm(configFile, true);
            if (configForm.ShowDialog() == DialogResult.OK)
                LoadConfig(configForm.TailFileConfig, _configPath);
        }

        private void gotoPreviousHighlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm.Instance.SearchAgain(this, false, true);
        }

        private void gotoNextHighlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm.Instance.SearchAgain(this, true, true);
        }

        private void pauseWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Paused)
            {
                Paused = true;
            }
            else
            {
                Paused = false;
                _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
            }
        }
    }

    public static class ListViewUtil
    {
        /// <summary>
        /// Performs <c>LVM_SETITEMCOUNT</c> on the given <c>ListView</c>.
        /// Also sets <c>LVSICF_NOINVALIDATEALL</c> and <c>LVSICF_NOSCROLL</c> flags
        /// to avoid expensive (and ugly) redrawing on frequent item additions.
        /// See http://msdn.microsoft.com/en-us/library/bb761188%28v=VS.85%29.aspx for more.
        /// </summary>
        public static void SetVirtualListSizeWithoutRefresh(ListView listView, int count)
        {
            SendMessage(listView.Handle,
                (int)ListViewMessages.LVM_SETITEMCOUNT,
                count,
                (int)(ListViewSetItemCountFlags.LVSICF_NOINVALIDATEALL |
                ListViewSetItemCountFlags.LVSICF_NOSCROLL));

            // The ListView.VirtualListSize property drives a private member
            // virtualListSize that is used in the implementation of
            // ListViewItemCollection (returned by ListView.Items) to validate
            // indices. If this is not updated, spurious ArgumentOutOfRangeExceptions
            // may be raised by functions and properties using the indexing
            // operator on ListView.Items, for instance FocusedItem.
            listViewVirtualListSizeField.SetValue(listView, count);
        }


        [Flags]
        private enum ListViewSetItemCountFlags
        {
            //#if (_WIN32_IE >= 0x0300)
            // these flags only apply to LVS_OWNERDATA listviews in report or list mode
            LVSICF_NOINVALIDATEALL = 0x00000001,
            LVSICF_NOSCROLL = 0x00000002,
            //#endif
        }

        private enum ListViewMessages
        {
            LVM_FIRST = 0x1000,      // ListView messages
            LVM_SETITEMCOUNT = (LVM_FIRST + 47),
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr handle, int messg, int wparam, int lparam);

        static ListViewUtil()
        {
            listViewVirtualListSizeField = typeof(ListView).GetField("virtualListSize", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            System.Diagnostics.Debug.Assert(listViewVirtualListSizeField != null, "System.Windows.Forms.ListView class no longer has a virtualListSize field.");
        }

        private static readonly System.Reflection.FieldInfo listViewVirtualListSizeField;
    }

    class LogFileListView : ListView
    {
        public LogFileListView()
        {
        }

        public void EnableDoubleBuffer()
        {
            DoubleBuffered = true;
        }
    }
}
