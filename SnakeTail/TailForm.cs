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
    public partial class TailForm : Form
    {
        // Todo Implement inline selection when owner-drawn
        // Todo EventLog Filtering
        //  - First count the rows in the entire data-set
        //  - Then parse the entire data-set backwards with the filter applied, 
        // Todo Filter Text File
        // Todo Highlight Text Filter
        // Todo Search highlight text (Extend Search dialog with highlight filters)
        // Todo Consider keeping file handle open, even when file is renamed / deleted (show history)
        // Todo Jump Lists
        // Todo Move CPUMeter and Task Monitor to own their own class files without knowledge of MainForm
        // Todo Use background workerthread when searching (First visual cache, new file-stream, lock scrolling and timers, extra step to check for new lines, allow cancel search)
        // Todo Cache search results when searching up
        // Todo Scroll to the same matching line in all views
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

        public TailForm()
        {
            InitializeComponent();
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

        public void LoadFile(string filepath)
        {
            TailFileConfig tailConfig = new TailFileConfig();
            tailConfig.FilePath = filepath;
            tailConfig.FileCacheSize = 1000;
            LoadConfig(tailConfig, "");
        }

        public void LoadConfig(TailFileConfig tailConfig, string configPath)
        {
            if (tailConfig.BackColor != null)
                _tailListView.BackColor = ColorTranslator.FromHtml(tailConfig.BackColor);
            if (tailConfig.TextColor != null)
                _tailListView.ForeColor = ColorTranslator.FromHtml(tailConfig.TextColor);

            if (tailConfig.Font != null)
            {
                TypeConverter fontConverter = TypeDescriptor.GetConverter(typeof(Font));
                _tailListView.Font = (Font)fontConverter.ConvertFromString(tailConfig.Font);
            }

            Encoding fileEncoding = Encoding.Default;
            if (tailConfig.FileEncoding == Encoding.UTF8.ToString())
                fileEncoding = Encoding.UTF8;
            else
                if (tailConfig.FileEncoding == Encoding.ASCII.ToString())
                    fileEncoding = Encoding.ASCII;
                else
                    if (tailConfig.FileEncoding == Encoding.Unicode.ToString())
                        fileEncoding = Encoding.Unicode;

            _logFileCache = new LogFileCache(tailConfig.FileCacheSize);
            _logFileStream = new LogFileStream(configPath, tailConfig.FilePath, fileEncoding);
            _logTailStream = new LogFileStream(configPath, tailConfig.FilePath, fileEncoding);

            _logFileCache.LoadingFileEvent += new EventHandler(_logFileCache_LoadingFileEvent);
            _logFileCache.FillCacheEvent += new EventHandler(_logFileCache_FillCacheEvent);

            // Add loading of cache while counting lines in file
            int lineCount = _logFileCache.FillCacheEndOfFile(_logTailStream, 0);

            _tailListView.VirtualListSize = lineCount;

            if (string.IsNullOrEmpty(tailConfig.LogHitText))
                _logTailStream.LogHitText = null;
            else
                _logTailStream.LogHitText = tailConfig.LogHitText;

            if (!string.IsNullOrEmpty(tailConfig.ServiceName))
                _taskMonitor = new TaskMonitor(tailConfig.ServiceName);

            if (tailConfig.Title != null)
                _formTitle = tailConfig.Title;
            else
                _formTitle = Path.GetFileName(tailConfig.FilePath);

            if (!string.IsNullOrEmpty(tailConfig.IconFile))
            {
                _formIconFile = tailConfig.IconFile;
                string formIconFilePathAbsolute = System.IO.Path.Combine(configPath, _formIconFile);
                _formCustomIcon = System.Drawing.Icon.ExtractAssociatedIcon(formIconFilePathAbsolute);
                _formMaximizedIcon = Icon;
                Icon = _formCustomIcon;
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

            if (_logTailStream.LogHitText != null)
            {
                title += " Hits: " + _logTailStream.LogHitTextCount.ToString();
                _logTailStream.LogHitTextCount = 0;
            }

            if (_taskMonitor != null)
            {
                try
                {
                    double cpuUtilization = _taskMonitor.ProcessorUsage;
                    Process process = _taskMonitor.Process;
                    if (process != null)
                    {
                        TimeSpan cpuTime = DateTime.Now.Subtract(_lastFormTitleUpdate);
                        cpuUtilization = cpuUtilization / cpuTime.TotalMilliseconds * 1000.0;
                        title += " CPU: " + cpuUtilization.ToString("F0");
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
                SetStatusBar("Loading file...", (int)filestream.Position, (int)filestream.Length);
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
            tailConfig.BackColor = ColorTranslator.ToHtml(_tailListView.BackColor);
            tailConfig.TextColor = ColorTranslator.ToHtml(_tailListView.ForeColor);

            TypeConverter fontConverter = TypeDescriptor.GetConverter(typeof(Font));
            tailConfig.Font = fontConverter.ConvertToString(_tailListView.Font);
            tailConfig.FileCacheSize = _logFileCache.Items.Count;
            tailConfig.FileEncoding = _logTailStream.FileEncoding.ToString();
            tailConfig.FilePath = _logTailStream.FilePath;
            tailConfig.Title = _formTitle;
            tailConfig.IconFile = _formIconFile;
            tailConfig.Modeless = MdiParent == null;
            tailConfig.WindowState = WindowState;
            tailConfig.WindowSize = Size;
            tailConfig.WindowPosition = DesktopLocation;
            if (_logTailStream.LogHitText != null)
                tailConfig.LogHitText = _logTailStream.LogHitText;
            else
                tailConfig.LogHitText = "";
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

        private int SearchForTextForward(string searchText, bool matchCase, int startIndex, int endIndex, ref LogFileCache searchFileCache)
        {
            for (int i = startIndex; i < endIndex; ++i)
            {
                if (i % _logFileCache.Items.Count == 0)
                    SetStatusBar("Searching...", i - startIndex, endIndex - startIndex);

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

                if (matchCase)
                {
                    if (0 <= lineText.IndexOf(searchText))
                    {
                        return i;
                    }
                }
                else
                {
                    if (0 <= lineText.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public bool SearchForText(string searchText, bool matchCase, bool searchForward)
        {
            if (_tailListView.VirtualListSize == 0)
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
                            matchFound = SearchForTextForward(searchText, matchCase, startIndex, endIndex, ref searchFileCache);
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
                            _tailListView.EnsureVisible(lastMatchFound);
                            _tailListView.SelectedIndices.Add(lastMatchFound);
                            return true;
                        }
                        else
                        {
                            SetStatusBar(null);
                            return false;
                        }
                    }

                    if (matchCase)
                    {
                        if (0 <= lineText.IndexOf(searchText))
                        {
                            SetStatusBar(null);
                            _tailListView.SelectedIndices.Clear();
                            _tailListView.EnsureVisible(i);
                            _tailListView.SelectedIndices.Add(i);
                            return true;
                        }
                    }
                    else
                    {
                        if (0 <= lineText.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase))
                        {
                            SetStatusBar(null);
                            _tailListView.SelectedIndices.Clear();
                            _tailListView.EnsureVisible(i);
                            _tailListView.SelectedIndices.Add(i);
                            return true;
                        }
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

                int matchFound = SearchForTextForward(searchText, matchCase, startIndex, endIndex, ref searchFileCache);
                if (matchFound != -1)
                {
                    SetStatusBar(null);
                    _tailListView.SelectedIndices.Clear();  // Clear selection before changing cache to avoid cache miss
                    if (searchFileCache != null)
                        _logFileCache = searchFileCache;    // Swap cache before displaying search result
                    _tailListView.EnsureVisible(matchFound);
                    _tailListView.SelectedIndices.Add(matchFound);   // Set selection after having scrolled to avoid top-index cache miss
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
                _statusStrip.Visible = true;
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

            if (e.Item.Selected)
            {
                e.DrawFocusRectangle(e.Item.Bounds);
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }
            else
            {
                e.DrawBackground();
            }

            using (Brush textBrush = e.Item.Selected ? null : new SolidBrush(_tailListView.ForeColor))
            {
                if (e.Item.Text.Length > 1000)
                    e.Graphics.DrawString(e.Item.Text.Substring(0, 1000), _tailListView.Font, textBrush != null ? textBrush : SystemBrushes.HighlightText, e.Bounds);
                else
                    e.Graphics.DrawString(e.Item.Text, _tailListView.Font, textBrush != null ? textBrush : SystemBrushes.HighlightText, e.Bounds);
            }
        }

        private void _tailListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Add)
            {
                e.Handled = true;   // No auto resize
                return;
            }

            // For some weird reason the cache request for page-down / page-up comes after the item-requests
            if (e.KeyCode == Keys.PageDown)
            {
                _logFileCache.PrepareCache(_tailListView.TopItem.Index + _logFileCache.Items.Count / 2, _tailListView.TopItem.Index + _logFileCache.Items.Count / 2, false);
            }
            if (e.KeyCode == Keys.PageUp)
            {
                _logFileCache.PrepareCache(_tailListView.TopItem.Index - _logFileCache.Items.Count / 2, _tailListView.TopItem.Index + _logFileCache.Items.Count / 2, false);
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

            // Hvis file cache foelger bunden af filen
            if (_logFileCache.Items.Count + _logFileCache.FirstIndex >= _tailListView.VirtualListSize)
            {
                lineCount = _logFileCache.FillCacheEndOfFile(_logTailStream, lineCount);
            }
            else
            {
                while (_logTailStream.ReadLine(lineCount + 1) != null)
                    lineCount++;
            }

            if (lineCount == _tailListView.VirtualListSize)
            {
                if (_logTailStream.ValidLineCount(lineCount))
                    return;
                else
                    lineCount = 0;
            }

            if (lineCount < _tailListView.VirtualListSize)
            {
                _logFileStream.CheckLogFile();
                _logFileCache = new LogFileCache(_logFileCache.Items.Count);
                _tailListView.TopItem = null;
                _tailListView.VirtualListSize = lineCount;
                _tailListView.Invalidate();
            }
            else
            {
                bool listAtBottom = ListAtBottom();
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
            newform.LoadConfig(tailConfig, Path.GetDirectoryName(_logFileStream.FilePathAbsolute));
            newform.BringToFront();
        }

        private void configureFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fdlgText = new FontDialog();
            fdlgText.Font = _tailListView.Font;
            fdlgText.Color = _tailListView.ForeColor;
            fdlgText.ShowColor = true;
            if (fdlgText.ShowDialog() == DialogResult.OK)
            {
                _tailListView.Font = fdlgText.Font;
                _tailListView.ForeColor = fdlgText.Color;
            }
        }

        private void configureBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.Color = _tailListView.BackColor;
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                _tailListView.BackColor = colorDlg.Color;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                SearchForm.Instance.StartSearch(this);
                return true;
            }
            if (keyData == Keys.F3)
            {
                if (SearchForm.Instance.Visible)
                {
                    SearchForm.Instance.SearchAgain(this, true);
                }
                return true;
            }
            if (keyData == (Keys.Shift | Keys.F3))
            {
                if (SearchForm.Instance.Visible)
                {
                    SearchForm.Instance.SearchAgain(this, false);
                }
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
            if (WindowState == FormWindowState.Minimized)
            {
                Paused = true;
            }
            else
                if (Paused || listAtBottom)
                {
                    Paused = false;
                    if (_tailListView.VirtualListSize > 0)
                        _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
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
            }
            catch (Exception ex)
            {
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
            }
            catch (Exception ex)
            {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Pause service failed: " + ex.Message);
            }
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_taskMonitor == null)
                    return;

                SetStatusBar("Continue service...", 1, 5);
                _taskMonitor.ContinueService();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Continue service failed: " + ex.Message);
            }
        }

        private void _contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            bool windowsService = _taskMonitor != null && _taskMonitor.ServiceController != null;
            bool pauseAndContinue = _taskMonitor != null && _taskMonitor.CanPauseAndContinue;
            foreach (ToolStripItem item in _contextMenuStrip.Items)
            {
                if (item.Text == "Start Service...")
                    item.Enabled = windowsService;
                if (item.Text == "Stop Service...")
                    item.Enabled = windowsService;
                if (item.Text == "Pause Service...")
                    item.Visible = pauseAndContinue;
                if (item.Text == "Continue Service...")
                    item.Visible = pauseAndContinue;
            }
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
