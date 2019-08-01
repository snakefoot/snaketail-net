﻿#region License statement
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SnakeTail
{
    partial class TailForm : Form, ITailForm
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
        bool _formTitleMatchFilename = false;
        string _formTitle = "";
        string _formIconFile = "";
        DateTime _lastFormTitleUpdate = DateTime.Now;
        bool _formMinimizedAtBottom = false;
        Icon _formCustomIcon = null;
        Icon _formMaximizedIcon = null;
        string _configPath = "";
        List<TailKeywordConfig> _keywordHighlight;
        int _loghitCounter = -1;
        bool _displayTabIcon = false;
        List<ExternalToolConfig> _externalTools;
        Color _bookmarkTextColor = Color.Yellow;    // Default bookmark text color
        Color _bookmarkBackColor = Color.Green;     // Default bookmark background color
        List<int> _bookmarks = new List<int>();
        ThreadPoolQueue _threadPoolQueue = null;

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

            try
            {
                new DirectoryInfo(Path.GetDirectoryName(Path.Combine(configPath, tailConfig.FilePath)));
            }
            catch (System.ArgumentException ex)
            {
                MessageBox.Show(this, String.Format("Failed to open file:\n\n{0}\n\nError:{1}", tailConfig.FilePath, ex.Message), "Invalid filename", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
                return;
            }

            if (tailConfig.FileCacheSize <= 0)
                tailConfig.FileCacheSize = 1000;

            if (tailConfig.FormBackColor != null)
                _tailListView.BackColor = tailConfig.FormBackColor.Value;
            if (tailConfig.FormTextColor != null)
                _tailListView.ForeColor = tailConfig.FormTextColor.Value;

            if (tailConfig.FormFont != null)
                _tailListView.Font = tailConfig.FormFont;

            if (tailConfig.FormBookmarkBackColor != null)
                _bookmarkBackColor = tailConfig.FormBookmarkBackColor.Value;
            if (tailConfig.FormBookmarkTextColor != null)
                _bookmarkTextColor = tailConfig.FormBookmarkTextColor.Value;

            if (tailConfig.FileChangeCheckInterval > 0)
                _tailTimer.Interval = tailConfig.FileChangeCheckInterval;

            _externalTools = tailConfig.ExternalTools;
            externalToolsToolStripMenuItem.DropDownItems.Clear();
            externalToolsToolStripMenuItem.Enabled = false;
            if (_externalTools != null)
            {
                foreach (ExternalToolConfig externalTool in _externalTools)
                {
                    ToolStripMenuItem toolItem = externalToolsToolStripMenuItem.DropDownItems.Add(externalTool.Name) as ToolStripMenuItem;
                    if (toolItem != null)
                    {
                        toolItem.Tag = externalTool;
                        toolItem.Click += new EventHandler(externalToolMenuItem_Click);
                        if (externalTool.ShortcutKeyEnum.HasValue)
                            toolItem.ShortcutKeys = externalTool.ShortcutKeyEnum.Value;
                    }
                    externalToolsToolStripMenuItem.Enabled = true;
                }
            }

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

                    if (!string.IsNullOrEmpty(keyword.ExternalToolName))
                    {
                        keyword.ExternalToolConfig = _externalTools.Find((externalTool) => string.Compare(externalTool.Name, keyword.ExternalToolName) == 0);
                        if (_threadPoolQueue == null)
                            _threadPoolQueue = new ThreadPoolQueue();   // Prepare the threadpool for use
                    }
                }
            }

            Encoding fileEncoding = tailConfig.EnumFileEncoding;

            if (_logTailStream != null)
                _logTailStream.Reset();

            if (_logFileStream == null || _logFileStream.FilePath != tailConfig.FilePath || _logFileStream.FileEncoding != fileEncoding || _logFileStream.FileCheckInterval != tailConfig.FileCheckInterval || _logFileStream.FileCheckPattern != tailConfig.FileCheckPattern)
            {
                if (_logFileStream != null)
                    _logFileStream.Dispose();
                _logFileStream = new LogFileStream(configPath, tailConfig.FilePath, fileEncoding, tailConfig.FileCheckInterval, tailConfig.FileCheckPattern);
            }
            if (_logTailStream == null || _logTailStream.FilePath != tailConfig.FilePath || _logTailStream.FileEncoding != fileEncoding || _logTailStream.FileCheckInterval != tailConfig.FileCheckInterval || _logTailStream.FileCheckPattern != tailConfig.FileCheckPattern)
            {
                if (_logTailStream != null)
                    _logTailStream.Dispose();
                _logTailStream = new LogFileStream(configPath, tailConfig.FilePath, fileEncoding, tailConfig.FileCheckInterval, tailConfig.FileCheckPattern);
                if (_logTailStream.Length > 500 * 1024 * 1024)
                {
                    if (MessageBox.Show(this, String.Format("The file is very large, sure you want to open it?\n\nFile Name: {0}\nFile Size: {1} Megabytes", _logTailStream.FilePath, _logTailStream.Length / 1024 / 1024), "Large file detected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        Close();
                        return;
                    }
                }
                if (_logFileCache != null)
                {
                    _logFileCache.Reset();
                    _logFileCache = null;   // Reset Cache, as the file contents can have changed
                }
            }

            _logTailStream.FileReloadedEvent += new EventHandler(_logTailStream_FileReloadedEvent);

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

            if (_taskMonitor != null)
            {
                _taskMonitor.Dispose();
                _taskMonitor = null;
            }
            if (!string.IsNullOrEmpty(tailConfig.ServiceName))
                _taskMonitor = new TaskMonitor(tailConfig.ServiceName);

            _formTitleMatchFilename = tailConfig.TitleMatchFilename;
            if (_formTitleMatchFilename)
                _formTitle = Path.GetFileName(_logTailStream.Name);
            else
            if (tailConfig.Title != null)
                _formTitle = tailConfig.Title;
            else
                _formTitle = Path.GetFileName(tailConfig.FilePath);

            UpdateFormTitle(true);

            _displayTabIcon = tailConfig.DisplayTabIcon;

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
                    MessageBox.Show(this, "Failed to load icon\n\n   " + formIconFilePathAbsolute + "\n\n" + ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            UpdateFormTitle(true);

            if (Visible)
            {
                if (_tailListView.VirtualListSize > 0)
                {
                    _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
                    _tailListView.FocusedItem = _tailListView.Items[_tailListView.VirtualListSize - 1];
                    _tailListView.SelectedIndices.Add(_tailListView.VirtualListSize - 1);
                    _tailListView.Invalidate();
                    _tailListView.Update();
                }
            }
        }

        void _logTailStream_FileReloadedEvent(object sender, EventArgs e)
        {
            // If new file has arrived, then update form-title to display this
            if (_formTitleMatchFilename)
            {
                string fileName = Path.GetFileName(_logTailStream.Name);
                if (!String.IsNullOrEmpty(fileName))
                    _formTitle = fileName;
                UpdateFormTitle(true);
            }

            // Reset bookmarks
            if (_bookmarks.Count > 0)
            {
                _bookmarks.Clear();
                _tailListView.Invalidate();
            }

            SetStatusBar(null);
        }

        void UpdateFormTitle(bool force)
        {
            if (!force && DateTime.Now.Subtract(_lastFormTitleUpdate) < TimeSpan.FromSeconds(1))
                return;

            string title = _formTitle;

            TabPage parentTab = this.Tag as TabPage;
            if (parentTab != null)
            {
                if (parentTab.Text != title)
                    parentTab.Text = title;

                string fileStreamPath = _logTailStream != null ? _logTailStream.Name : string.Empty;
                if (!String.IsNullOrEmpty(fileStreamPath))
                    parentTab.ToolTipText = fileStreamPath;
                else
                    parentTab.ToolTipText = _logTailStream != null ? _logTailStream.FilePathAbsolute : string.Empty;
            }

            StringBuilder sb = null;

            if (_loghitCounter != -1)
            {
                sb = sb != null ? sb : new StringBuilder(title);
                sb.Append(" Hits=").Append(_loghitCounter);
                _loghitCounter = 0;
            }

            if (_taskMonitor != null)
            {
                sb = sb != null ? sb : new StringBuilder(title);

                try
                {
                    float cpuUtilization = _taskMonitor.ProcessorUsage;
                    Process process = _taskMonitor.Process;
                    if (process != null)
                    {
                        sb.AppendFormat(" CPU={0:F0}", cpuUtilization);

                        process.Refresh();
                        sb.Append(" RAM=").Append(process.PrivateMemorySize64 / (1024 * 1024));
                        sb.Append(_taskMonitor.ServiceRunning ? " (Started)" : " (Stopped)");
                    }
                    else
                    {
                        sb.Append(" (Stopped)");
                    }
                }
                catch (Exception ex)
                {
                    sb.Append(" (").Append(ex.Message).Append(")");
                }
            }

            string logSizeText = GetFileSizeStatusText();
            if (!string.IsNullOrEmpty(logSizeText))
            {
                if (sb != null)
                {
                    sb.Append(" LogSize=");
                    sb.Append(logSizeText);
                }
                else
                {
                    title = string.Format("{0} LogSize={1}", title, logSizeText);
                }
            }

            _lastFormTitleUpdate = DateTime.Now;

            if (sb != null)
                title = sb.ToString();

            if (Text != title)
                Text = title;

            SetStatusBar(null);
        }

        public void SetStatusBar(string text)
        {
            SetStatusBar(text, 0, 0);
        }

        private static string ConvertFileSize(long fileSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = fileSize;
            int order = 0;
            while (len >= 1024 && ++order < sizes.Length)
            {
                len = len / 1024;
            }
            if (order == 0)
                return string.Format("{0:0} {1}", len, sizes[order]);
            else if (order == 1)
                return string.Format("{0:0.0} {1}", len, sizes[order]);
            else
                return string.Format("{0:0.00} {1}", len, sizes[order]);
        }

        private string GetFileSizeStatusText()
        {
            long fileSize = _logTailStream != null ? _logTailStream.Position : 0;
            if (fileSize > 0)
                return ConvertFileSize(fileSize);
            else
                return string.Empty;
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

                if (text != _statusTextBar.Text || progressValue != 0 || progressMax != 0)
                {
                    _statusTextBar.Text = text;
                    _statusStrip.Invalidate();
                    _statusStrip.Update();
                }
            }
            else if (MainForm.Instance != null)
            {
                if (text == null)
                {
                    if (MainForm.Instance.ActiveMdiChild == this)
                        MainForm.Instance.SetStatusBar(Paused ? "Paused" : "Ready", progressValue, progressMax);
                }
                else
                    MainForm.Instance.SetStatusBar(text, progressValue, progressMax);
            }
        }

        void _logFileCache_LoadingFileEvent(object sender, EventArgs e)
        {
            LogFileStream filestream = sender as LogFileStream;
            if (filestream != null)
            {
                double position = filestream.Length != 0 ? filestream.Position / (double)filestream.Length * 100 : 100;
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
            tailConfig.FormFont = _tailListView.Font;
            tailConfig.FormBackColor = _tailListView.BackColor;
            tailConfig.FormTextColor = _tailListView.ForeColor;

            tailConfig.FormBookmarkBackColor = _bookmarkBackColor;
            tailConfig.FormBookmarkTextColor = _bookmarkTextColor;

            tailConfig.KeywordHighlight = _keywordHighlight;

            tailConfig.ExternalTools = _externalTools;

            tailConfig.FileCacheSize = _logFileCache.Items.Count;
            tailConfig.EnumFileEncoding = _logTailStream.FileEncoding;
            tailConfig.FilePath = _logTailStream.FilePath;
            tailConfig.FileCheckInterval = _logTailStream.FileCheckInterval;
            tailConfig.FileChangeCheckInterval = _tailTimer.Interval;
            tailConfig.FileCheckPattern = _logTailStream.FileCheckPattern;
            tailConfig.TitleMatchFilename = _formTitleMatchFilename;
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

            tailConfig.DisplayTabIcon = _displayTabIcon;
        }

        public void CopySelectionToClipboard()
        {
            // Copy selected rows to clipboard
            StringBuilder selection = new StringBuilder();
            foreach (int itemIndex in _tailListView.SelectedIndices)
            {
                if (selection.Length > 0)
                    selection.AppendLine();
                selection.Append(_tailListView.Items[itemIndex].Text);
            }
            try
            {
                ClipboardHelper.CopyToClipboard(selection.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to copy to clipboard, maybe another application is locking the clipboard.\n\n" + ex.Message);
            }
        }

        private bool MatchTextSearch(int lineNumber, string lineText, string searchText, bool matchCase, bool lineHighlights)
        {
            if (lineHighlights)
            {
                if (MatchesBookmark(lineNumber))
                    return true;

                if (MatchesKeyword(lineText, true, false) != null)
                    return true;

                return false;
            }
            else
            if (matchCase)
            {
                if (lineText.Contains(searchText))
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

        private int SearchForTextForward(string searchText, bool matchCase, bool lineHighlights, int startIndex, int endIndex, ref LogFileCache searchFileCache)
        {
            int i = 0;

            try
            {
                for (i = startIndex; i < endIndex; ++i)
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

                    if (MatchTextSearch(i, lineText, searchText, matchCase, lineHighlights))
                        return i;
                }
            }
            catch (Exception ex)
            {
                string exceptionDetails = ex.Message + "\r\n";
                if (searchFileCache != null)
                {
                    exceptionDetails += "SearchFileCache = ";
                    if (searchFileCache.Items != null)
                        exceptionDetails += "Items = " + searchFileCache.Items.Count.ToString() + ", ";
                    else
                        exceptionDetails = "Items = null, ";
                }
                else
                {
                    exceptionDetails += "SearchFileCache = null, ";
                }
                if (_logFileCache != null)
                {
                    exceptionDetails += "LogFileCache = ";
                    if (searchFileCache.Items != null)
                        exceptionDetails += "Items = " + searchFileCache.Items.Count.ToString() + ", ";
                    else
                        exceptionDetails = "Items = null, ";
                }
                else
                {
                    exceptionDetails += "LogFileCache = null, ";
                }
                exceptionDetails += "VirtualListSize = " + _tailListView.VirtualListSize.ToString() + ", ";
                exceptionDetails += "StartIndex = " + startIndex.ToString() + ", ";
                exceptionDetails += "EndIndex = " + endIndex.ToString() + ", ";
                exceptionDetails += "CurrentIndex = " + i.ToString() + ", ";
                throw new InvalidOperationException(exceptionDetails, ex);
            }
            return -1;
        }

        public bool SearchForText(string searchText, bool matchCase, bool searchForward, bool lineHighlights, bool wrapAround)
        {
            if (_tailListView.VirtualListSize == 0)
                return false;

            if (lineHighlights && (_keywordHighlight == null || _keywordHighlight.Count == 0) && _bookmarks.Count == 0)
                return false;

            // Use selection if it is below top-index
            int startIndex = _tailListView.VirtualListSize - 1;
            if (_tailListView.SelectedIndices.Count > 0)
            {
                if (_tailListView.TopItem == null || _tailListView.TopItem.Index - 1 < _tailListView.SelectedIndices[0])
                    startIndex = _tailListView.SelectedIndices[0];
            }

            int matchFound = -1;
            if (!searchForward)
            {
                matchFound = SearchForTextBackward(searchText, matchCase, lineHighlights, startIndex - 1, 0);
                //Retry if not found
                if (matchFound == -1 && wrapAround)
                {
                    matchFound = SearchForTextBackward(searchText, matchCase, lineHighlights, _tailListView.VirtualListSize - 1, startIndex);
                }
            }
            else
            {
                LogFileCache searchFileCache = null;
                matchFound = SearchForTextForward(searchText, matchCase, lineHighlights, startIndex + 1, _tailListView.VirtualListSize, ref searchFileCache);
                //Retry if not found
                if (matchFound == -1 && wrapAround)
                {
                    matchFound = SearchForTextForward(searchText, matchCase, lineHighlights, 0, startIndex + 1, ref searchFileCache);
                }

                if (matchFound != -1)
                {
                    // Swap cache before displaying search result
                    if (searchFileCache != null)
                        _logFileCache = searchFileCache;
                }
            }

            SetStatusBar(null);
            if (matchFound != -1)
            {
                _tailListView.SelectedIndices.Clear();
                _tailListView.EnsureVisible(matchFound);
                _tailListView.SelectedIndices.Add(matchFound);
                _tailListView.Items[matchFound].Focused = true;
                return true;
            }

            return false;
        }

        private int SearchForTextBackward(string searchText, bool matchCase, bool lineHighlights, int startIndex, int endIndex)
        {
            // First use the visual cache, when that have failed, then revert to search from the beginning
            // and find the last match
            for (int i = startIndex; i >= endIndex; --i)
            {
                if (i % _logFileCache.Items.Count == 0)
                    SetStatusBar("Searching...", _tailListView.VirtualListSize - i, _tailListView.VirtualListSize);

                string lineText;
                ListViewItem lvi = _logFileCache.LookupCache(i);
                if (lvi != null)
                {
                    lineText = lvi.Text;
                    if (MatchTextSearch(i, lineText, searchText, matchCase, lineHighlights))
                    {
                        return i;
                    }
                }
                else
                {
                    int lastMatchFound = SearchForwardForLastMatch(searchText, matchCase, lineHighlights, i + 1);
                    if (lastMatchFound != -1 && endIndex <= lastMatchFound)
                    {
                        return lastMatchFound;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            return -1;
        }

        private int SearchForwardForLastMatch(string searchText, bool matchCase, bool lineHighlights, int endIndex)
        {
            LogFileCache searchFileCache = null;
            int matchFound = -1;
            int lastMatchFound = -1;
            int startIndex = 0;
            do
            {
                matchFound = SearchForTextForward(searchText, matchCase, lineHighlights, startIndex, endIndex, ref searchFileCache);
                if (matchFound != -1)
                {
                    lastMatchFound = matchFound;
                    startIndex = matchFound + 1;
                    _tailListView.SelectedIndices.Clear();
                    if (searchFileCache != null)
                    {
                        _logFileCache = searchFileCache; // Store the cache of the last match
                        searchFileCache = new LogFileCache(_logFileCache.Items.Count);
                        searchFileCache.Items = _logFileCache.Items.GetRange(0, _logFileCache.Items.Count);
                        searchFileCache.FirstIndex = _logFileCache.FirstIndex;
                    }
                }
            } while (matchFound != -1);

            return lastMatchFound;
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
                _tailListView.FocusedItem = _tailListView.Items[_tailListView.VirtualListSize - 1];
                _tailListView.SelectedIndices.Add(_tailListView.VirtualListSize - 1);
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

            Color? textColor = null;
            Color? backColor = null;

            // Bookmark coloring has higher priority, than keyword highlight
            if (MatchesBookmark(e.ItemIndex))
            {
                backColor = _bookmarkBackColor;
                textColor = _bookmarkTextColor;
            }
            else
            {
                TailKeywordConfig keyword = MatchesKeyword(e.Item.Text, false, true);
                if (keyword != null)
                {
                    if (keyword.FormBackColor.HasValue && keyword.FormTextColor.HasValue)
                    {
                        backColor = keyword.FormBackColor.Value;
                        textColor = keyword.FormTextColor.Value;
                    }
                }
            }

            //toggle colors if the item is highlighted 
            if (e.Item.Selected)
            {
                if (backColor.HasValue || textColor.HasValue)
                {
                    e.SubItem.BackColor = SystemColors.Highlight;
                    e.SubItem.ForeColor = Color.FromArgb(SystemColors.Highlight.A, (SystemColors.Highlight.R + 128) % 256, (SystemColors.Highlight.G + 128) % 256, (SystemColors.Highlight.B + 128) % 256);
                }
                else
                {
                    e.SubItem.BackColor = SystemColors.Highlight;
                    e.SubItem.ForeColor = SystemColors.HighlightText;
                }
            }
            else
            {
                if (backColor.HasValue)
                    e.SubItem.BackColor = backColor.Value;
                else
                    e.SubItem.BackColor = e.Item.ListView.BackColor;
                if (textColor.HasValue)
                    e.SubItem.ForeColor = textColor.Value;
                else
                    e.SubItem.ForeColor = e.Item.ListView.ForeColor;
            }

            // Draw the standard header background.
            e.DrawBackground();
            e.DrawFocusRectangle(e.Bounds);

            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.ExpandTabs | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix;
            if (e.Item.Text.Length > 1000)
                TextRenderer.DrawText(e.Graphics, e.Item.Text.Substring(0, 1000), e.Item.ListView.Font, e.Bounds, e.SubItem.ForeColor, flags);
            else
                TextRenderer.DrawText(e.Graphics, e.Item.Text, e.Item.ListView.Font, e.Bounds, e.SubItem.ForeColor, flags);
        }

        private bool MatchesBookmark(int lineNumber)
        {
            if (_bookmarkBackColor == null || _bookmarkTextColor == null)
                return false;

            if (_bookmarks.Count == 0)
                return false;

            return _bookmarks.Contains(lineNumber);
        }

        private TailKeywordConfig MatchesKeyword(string line, bool onlyKeywordHighlight, bool onlyTextColoring)
        {
            TailKeywordConfig matchKeyword = null;
            if (_keywordHighlight != null && _keywordHighlight.Count > 0)
            {
                foreach (TailKeywordConfig keyword in _keywordHighlight)
                {
                    if (onlyTextColoring)
                    {
                        if (!keyword.TextColoring.Value)
                            continue;
                    }
                    else if (matchKeyword != null)
                    {
                        // Ignore keywords that doesn't add extra detail to the existing keyword-match
                        if ((matchKeyword.ExternalToolConfig != null || keyword.ExternalToolConfig == null)
                          && (matchKeyword.LogHitCounter || !keyword.LogHitCounter)
                          && (matchKeyword.AlertHighlight.Value || !keyword.AlertHighlight.Value)
                            )
                            continue;
                    }

                    if ((keyword.KeywordRegex != null && keyword.KeywordRegex.IsMatch(line))
                      || (keyword.KeywordRegex == null && keyword.MatchCaseSensitive && line.IndexOf(keyword.Keyword, StringComparison.CurrentCulture) != -1)
                      || (keyword.KeywordRegex == null && !keyword.MatchCaseSensitive && line.IndexOf(keyword.Keyword, StringComparison.CurrentCultureIgnoreCase) != -1)
                       )
                    {
                        if (onlyKeywordHighlight)
                        {
                            // If high priority match is performing text-coloring, then no line match
                            if (keyword.TextColoring.Value && !keyword.AlertHighlight.Value)
                                return null;
                            else if (keyword.AlertHighlight.Value)
                                return keyword;
                        }
                        else if (onlyTextColoring)
                        {
                            return keyword;
                        }
                        else if (matchKeyword != null)
                        {
                            // Add extra detail to the existing keyword-match
                            matchKeyword = new TailKeywordConfig() { ExternalToolConfig = matchKeyword.ExternalToolConfig, LogHitCounter = matchKeyword.LogHitCounter, AlertHighlight = matchKeyword.AlertHighlight };
                            if (matchKeyword.ExternalToolConfig == null && keyword.ExternalToolConfig != null)
                                matchKeyword.ExternalToolConfig = keyword.ExternalToolConfig;
                            if (!matchKeyword.LogHitCounter && keyword.LogHitCounter)
                                matchKeyword.LogHitCounter = keyword.LogHitCounter;
                            if (!matchKeyword.AlertHighlight.Value && keyword.AlertHighlight.Value)
                                matchKeyword.AlertHighlight = keyword.AlertHighlight;
                        }
                        else
                        {
                            matchKeyword = keyword;
                        }

                        if (matchKeyword.LogHitCounter && matchKeyword.AlertHighlight.Value && matchKeyword.ExternalToolConfig != null)
                            return matchKeyword;	// We have all the details we need
                    }
                }
            }
            return matchKeyword;
        }

        private void _tailListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Add)
            {
                e.Handled = true;   // No auto resize
                return;
            }

            if (_tailListView.VirtualListSize > 0)
            {
                if (e.KeyCode == Keys.End)
                {
                    _logFileCache.PrepareCache(Math.Max(_tailListView.VirtualListSize - _logFileCache.Items.Count, 0), _tailListView.VirtualListSize, false);
                    _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
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
            }

            if (e.KeyCode == Keys.Pause)
            {
                Paused = !Paused;
                if (!Paused)
                {
                    if (_tailListView.VirtualListSize > 0)
                        _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
                }
            }
            else
            {
                if (Paused)
                {
                    Paused = false;
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                    {
                        if (_tailListView.VirtualListSize > 0)
                            _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
                    }
                }
            }
        }

        private bool ListAtBottom()
        {
            if (Paused)
                return false;

            if (_tailListView.VirtualListSize <= 5)
                return true;

            return IsItemVisible(_tailListView.VirtualListSize - 5);
        }

        private bool IsItemVisible(int index)
        {
            ListViewItem topItem = _tailListView.TopItem;
            if (topItem == null || topItem.Index > index)
                return false;

            int heightOfFirstItem = topItem.Bounds.Height;
            if (heightOfFirstItem == 0)
                return false;

            int nVisibleLines = _tailListView.Height / heightOfFirstItem;
            int lastVisibleIndexInDetailsMode = topItem.Index + nVisibleLines;
            return lastVisibleIndexInDetailsMode >= index;
        }

        private void _tailTimer_Tick(object sender, EventArgs e)
        {
            if (!_tailTimer.Enabled)
                return;

            UpdateFormTitle(false);

            CheckExternalToolResults();

            int lineCount = _tailListView.VirtualListSize;
            bool listAtBottom = ListAtBottom();
            bool warningIcon = false;

            if (_displayTabIcon)
            {
                TabPage parentTab = this.Tag as TabPage;
                if (parentTab != null && parentTab.ImageIndex == 1)
                    warningIcon = true;
            }
            else
                warningIcon = true;

            string line = _logTailStream.ReadLine(lineCount + 1);
            while (line != null)
            {
                ++lineCount;
                _logFileCache.AppendTailCache(line, lineCount);
                TailKeywordConfig keywordMatch = MatchesKeyword(line, false, false);
                if (keywordMatch != null)
                {
                    if (keywordMatch.LogHitCounter)
                        _loghitCounter++;
                    if (keywordMatch.ExternalToolConfig != null)
                    {
                        CheckExternalToolResults();
                        if (_threadPoolQueue != null)
                            _threadPoolQueue.QueueRequest(ExecuteExternalTool, GenerateExternalTool(keywordMatch.ExternalToolConfig, line, lineCount, keywordMatch.Keyword));
                    }
                    if (keywordMatch.AlertHighlight.Value)
                        warningIcon = true;
                }
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

            if (_displayTabIcon)
            {
                TabPage parentTab = this.Tag as TabPage;
                if (parentTab != null && parentTab.Parent != null && parentTab.Parent.Visible && !parentTab.Visible)
                {
                    if (warningIcon)
                        parentTab.ImageIndex = 1;
                    else
                        parentTab.ImageIndex = 0;
                }
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

        private void CheckExternalToolResults()
        {
            if (_threadPoolQueue != null)
            {
                try
                {
                    _threadPoolQueue.CheckResult();
                }
                catch (ApplicationException ex)
                {
                    MessageBox.Show(this, ex.Message);
                }
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
            else if (keyData == Keys.F3)
            {
                SearchForm.Instance.SearchAgain(this, true, false);
                return true;
            }

            if (keyData == (Keys.Alt | Keys.Up))
            {
                SearchForm.Instance.SearchAgain(this, false, true);
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.Down))
            {
                SearchForm.Instance.SearchAgain(this, true, true);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnResize(EventArgs e)
        {
            bool listAtBottom = ListAtBottom();
            base.OnResize(e);
            _tailListView.Invalidate();
            if (WindowState == FormWindowState.Minimized)
            {
                if (listAtBottom)
                    _formMinimizedAtBottom = listAtBottom;
            }
            else
            {
                if (_formMinimizedAtBottom)
                    listAtBottom = true;
                _formMinimizedAtBottom = false;
            }

            if (listAtBottom)
            {
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
                SetStatusBar(null);
            }
            catch (Exception ex)
            {
                SetStatusBar(null);
                MessageBox.Show(this, "Start service failed: " + ex.Message);
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
                MessageBox.Show(this, "Stop service failed: " + ex.Message);
            }
        }

        void externalToolMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem toolItem = sender as ToolStripItem;
            if (toolItem != null)
            {
                ExternalToolConfig toolConfig = toolItem.Tag as ExternalToolConfig;
                if (toolConfig != null)
                {
                    ExternalTool tool = null;
                    if (_tailListView.FocusedItem != null)
                        tool = GenerateExternalTool(toolConfig, _tailListView.FocusedItem.Text, _tailListView.FocusedItem.Index + 1, string.Empty);
                    else
                        tool = GenerateExternalTool(toolConfig, string.Empty, null, string.Empty);

                    try
                    {
                        tool.Execute();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "External Tool '" + toolConfig.Name + "' failed: " + ex.Message);
                    }
                }
            }
        }

        private ExternalTool GenerateExternalTool(ExternalToolConfig toolConfig, string line, int? lineNumber, string keywordMatch)
        {
            Dictionary<ExternalTool.ParameterName, string> fileParameters = new Dictionary<ExternalTool.ParameterName, string>();
            fileParameters[ExternalTool.ParameterName.FilePath] = _logTailStream != null ? _logTailStream.Name : string.Empty;
            fileParameters[ExternalTool.ParameterName.FileDirectory] = Path.GetDirectoryName(fileParameters[ExternalTool.ParameterName.FilePath]);
            fileParameters[ExternalTool.ParameterName.FileName] = Path.GetFileName(fileParameters[ExternalTool.ParameterName.FilePath]);
            fileParameters[ExternalTool.ParameterName.ServiceName] = _taskMonitor != null ? _taskMonitor.ServiceName : string.Empty;
            fileParameters[ExternalTool.ParameterName.SessionDirectory] = _configPath;
            fileParameters[ExternalTool.ParameterName.SessionPath] = MainForm.Instance.CurrenTailConfig;
            fileParameters[ExternalTool.ParameterName.SessionFileName] = Path.GetFileName(fileParameters[ExternalTool.ParameterName.SessionPath]);
            fileParameters[ExternalTool.ParameterName.SessionName] = Path.GetFileNameWithoutExtension(fileParameters[ExternalTool.ParameterName.SessionPath]);
            fileParameters[ExternalTool.ParameterName.ViewName] = _formTitle;
            fileParameters[ExternalTool.ParameterName.ProgramDirectory] = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            fileParameters[ExternalTool.ParameterName.LineText] = line != null ? line : string.Empty;
            fileParameters[ExternalTool.ParameterName.LineNumber] = lineNumber.HasValue ? lineNumber.Value.ToString() : string.Empty;
            fileParameters[ExternalTool.ParameterName.KeywordText] = keywordMatch != null ? keywordMatch : string.Empty;

            ExternalTool tool = new ExternalTool(toolConfig, fileParameters);
            return tool;
        }

        private void ExecuteExternalTool(object state)
        {
            ExternalTool tool = state as ExternalTool;

            try
            {
                if (tool != null)
                    tool.Execute();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("External Tool '" + tool.ToolConfig.Name + "' failed: " + ex.Message, ex);
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
                MessageBox.Show(this, "Pause service failed: " + ex.Message);
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
                MessageBox.Show(this, "Continue service failed: " + ex.Message);
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
            SearchForm.Instance.ActiveTailForm = this;
            TabPage parentTab = this.Tag as TabPage;
            if (parentTab != null)
                parentTab.ImageIndex = -1;
            _tailListView.Invalidate();
            _tailListView.Update();
            SetStatusBar(null);
            UpdateFormTitle(true);
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
            CopySelectionToClipboard();
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

        private void configureViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TailFileConfig configFile = new TailFileConfig();
            SaveConfig(configFile);
            TailConfigForm configForm = new TailConfigForm(configFile, true);
            switch (configForm.ShowDialog(this))
            {
                case DialogResult.OK:
                    {
                        LoadConfig(configForm.TailFileConfig, _configPath);
                        break;
                    }

                case DialogResult.Retry:
                    {
                        // Apply Config To All
                        LoadConfig(configForm.TailFileConfig, _configPath);
                        configFile = new TailFileConfig();
                        SaveConfig(configFile);
                        TailConfigApplyAllForm configFormApply = new TailConfigApplyAllForm();
                        if (configFormApply.ShowDialog(this) == DialogResult.OK)
                        {
                            // Then we loop through all forms (includes free floating)
                            foreach (Form childForm in Application.OpenForms)
                            {
                                TailForm tailForm = childForm as TailForm;
                                if (tailForm != null && tailForm != this)
                                {
                                    TailFileConfig configFileOther = new TailFileConfig();
                                    tailForm.SaveConfig(configFileOther);
                                    if (configFormApply._checkBoxColors.Checked)
                                    {
                                        configFileOther.FormBackColor = configFile.FormBackColor;
                                        configFileOther.FormTextColor = configFile.FormTextColor;
                                        configFileOther.FormBookmarkBackColor = configFile.FormBookmarkBackColor;
                                        configFileOther.FormBookmarkTextColor = configFile.FormBookmarkTextColor;
                                    }
                                    if (configFormApply._checkBoxFont.Checked)
                                        configFileOther.FontInvariant = configFile.FontInvariant;
                                    if (configFormApply._checkboxKeywords.Checked)
                                        configFileOther.KeywordHighlight = configFile.KeywordHighlight;
                                    if (configFormApply._checkboxTools.Checked)
                                        configFileOther.ExternalTools = configFile.ExternalTools;
                                    tailForm.LoadConfig(configFileOther, _configPath);
                                }
                            }
                        }
                        break;
                    }
            }
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
                if (_tailListView.VirtualListSize > 0)
                    _tailListView.EnsureVisible(_tailListView.VirtualListSize - 1);
            }
        }

        private void toggleBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_tailListView.VirtualListSize == 0)
                return;

            ListViewItem focusedItem = _tailListView.FocusedItem;
            if (focusedItem == null)
                return;

            if (!_bookmarks.Contains(focusedItem.Index))
                _bookmarks.Add(focusedItem.Index);
            else
                _bookmarks.Remove(focusedItem.Index);
            _tailListView.Invalidate();
            _tailListView.Update();
        }

        private void clearBookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_bookmarks.Count > 0)
            {
                _bookmarks.Clear();
                _tailListView.Invalidate();
                _tailListView.Update();
            }
        }

        private void nextBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_tailListView.VirtualListSize == 0)
                return;

            if (_bookmarks.Count == 0)
                return;

            int startIndex = _tailListView.VirtualListSize - 1;

            ListViewItem focusedItem = _tailListView.FocusedItem;
            if (focusedItem != null)
                startIndex = focusedItem.Index;

            // Search for the bookmark that is larger but closest to the index
            int matchFound = int.MaxValue;
            foreach (int lineNumber in _bookmarks)
            {
                if (lineNumber > startIndex && lineNumber < matchFound)
                    matchFound = lineNumber;
            }
            if (matchFound != int.MaxValue)
            {
                if (matchFound >= _tailListView.VirtualListSize)
                {
                    _bookmarks.Remove(matchFound);
                    _tailListView.Invalidate();
                    _tailListView.Update();
                    return; // something very weird has happened
                }

                _tailListView.SelectedIndices.Clear();
                _tailListView.EnsureVisible(matchFound);
                _tailListView.SelectedIndices.Add(matchFound);   // Set selection after having scrolled to avoid top-index cache miss
                _tailListView.Items[matchFound].Focused = true;
            }
        }

        private void previousBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_tailListView.VirtualListSize == 0)
                return;

            if (_bookmarks.Count == 0)
                return;

            int startIndex = _tailListView.VirtualListSize - 1;

            ListViewItem focusedItem = _tailListView.FocusedItem;
            if (focusedItem != null)
                startIndex = focusedItem.Index;

            if (startIndex == 0)
                return;

            // Search for the bookmark that is larger but closest to the index
            int matchFound = -1;
            foreach (int lineNumber in _bookmarks)
            {
                if (lineNumber < startIndex && lineNumber > matchFound)
                    matchFound = lineNumber;
            }
            if (matchFound != -1)
            {
                if (matchFound >= _tailListView.VirtualListSize)
                {
                    _bookmarks.Remove(matchFound);
                    _tailListView.Invalidate();
                    _tailListView.Update();
                    return; // something very weird has happened
                }

                _tailListView.SelectedIndices.Clear();
                _tailListView.EnsureVisible(matchFound);
                _tailListView.SelectedIndices.Add(matchFound);   // Set selection after having scrolled to avoid top-index cache miss
                _tailListView.Items[matchFound].Focused = true;
            }
        }

        private void copyAsPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyPath();
        }

        public void CopyPath()
        {
            string actualFileName = string.Empty;
            if (_logTailStream != null)
            {
                actualFileName = _logTailStream.Name;
                if (string.IsNullOrEmpty(actualFileName))
                    actualFileName = _logTailStream.FilePathAbsolute;
            }
            try
            {
                ClipboardHelper.CopyToClipboard(actualFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to copy to clipboard, maybe another application is locking the clipboard.\n\n" + ex.Message);
            }
        }

        private void openContainingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenExplorer();
        }

        public void OpenExplorer()
        {
            try
            {
                string actualFileName = string.Empty;
                if (_logTailStream != null)
                {
                    actualFileName = _logTailStream.Name;
                    if (string.IsNullOrEmpty(actualFileName))
                        actualFileName = _logTailStream.FilePathAbsolute;
                    if (!string.IsNullOrEmpty(actualFileName))
                        Process.Start("explorer.exe", "/select," + actualFileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to open Windows Explorer.\n\n" + ex.Message);
            }
        }
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
