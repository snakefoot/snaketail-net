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
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SnakeTail
{
    public partial class EventLogForm : Form, ITailForm
    {
        EventLog _eventLog;
        List<List<Regex>> _columnFilters = new List<List<Regex>>();
        EventLogMesageLookup _messageLookup;
        bool _filterActive = false;
        int _lastEventLogEntry = -1;
        int _lastEventLogFilterIndex = -1;
        int _lastEventLogFilterEntry = -1;
        string _formTitle;
        bool _displayTabIcon;
        bool _topItemIndexHack = false;
        bool _formMinimizedAtBottom = false;

        public EventLogForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.LogIcon;
        }

        public void LoadFile(string eventLogFile)
        {
            TailFileConfig tailConfig = new TailFileConfig();
            tailConfig.FilePath = eventLogFile;
            LoadConfig(tailConfig, null);
        }

        public Form TailWindow { get { return this; } }

        public void SaveConfig(TailFileConfig tailConfig)
        {
            tailConfig.BackColor = ColorTranslator.ToHtml(_eventListView.BackColor);
            tailConfig.TextColor = ColorTranslator.ToHtml(_eventListView.ForeColor);

            tailConfig.FormFont = _eventListView.Font;
            tailConfig.FilePath = _eventLog.Log;
            tailConfig.Title = _formTitle;
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
            tailConfig.ColumnFilters = new List<List<string>>();
            foreach (List<Regex> filter in _columnFilters)
            {
                List<string> columnFilter = new List<string>();
                foreach(Regex regexPattern in filter)
                {
                    if (regexPattern!=null)
                        columnFilter.Add(regexPattern.ToString());
                    else
                        columnFilter.Add("");
                }
                tailConfig.ColumnFilters.Add(columnFilter);
            }
            tailConfig.ColumnFilterActive = _filterActive;
            tailConfig.DisplayTabIcon = _displayTabIcon;
        }

        public void LoadConfig(TailFileConfig tailConfig, string configPath)
        {
            try
            {
                _eventLog = new EventLog(tailConfig.FilePath);
                _eventLog.EntryWritten += new EntryWrittenEventHandler(_eventLog_EntryWritten);
                _eventLog.EndInit();
                if (_eventLog.Entries.Count == -1)
                    return; // Crazy check just to ensure we have permissions to read the log
            }
            catch (System.Security.SecurityException ex)
            {
                MessageBox.Show(this, "Access denied when opening log: " + tailConfig.FilePath + "\n\n" + ex.Message);
                Close();
                return;
            }

            _messageLookup = new EventLogMesageLookup(_eventLog);

            if (tailConfig.FormBackColor != null)
                _eventListView.BackColor = tailConfig.FormBackColor.Value;
            if (tailConfig.FormTextColor != null)
                _eventListView.ForeColor = tailConfig.FormTextColor.Value;

            if (tailConfig.FormFont != null)
            {
                _eventListView.Font = tailConfig.FormFont;
                _eventMessageText.Font = tailConfig.FormFont;
            }

            if (tailConfig.Title != null)
                _formTitle = tailConfig.Title;
            else
                _formTitle = "EventLog - " + _eventLog.LogDisplayName;

            TabPage parentTab = this.Tag as TabPage;
            if (parentTab != null)
                parentTab.Text = _formTitle;

            if (tailConfig.ColumnFilters != null)
            {
                foreach (List<string> filter in tailConfig.ColumnFilters)
                {
                    List<Regex> columnFilter = new List<Regex>();
                    foreach (string regexPattern in filter)
                    {
                        if (!string.IsNullOrEmpty(regexPattern))
                            columnFilter.Add(new Regex(regexPattern));
                        else
                            columnFilter.Add(null);
                    }
                    _columnFilters.Add(columnFilter);
                }
            }

            _filterActive = tailConfig.ColumnFilterActive;
            _displayTabIcon = tailConfig.DisplayTabIcon;

            if (Visible)
            {
                ConfigureColumnFilter(_filterActive);
            }
        }

        private bool MatchTextSearch(string itemText, string searchText, bool matchCase)
        {
            if (matchCase)
            {
                if (0 <= itemText.IndexOf(searchText))
                {
                    return true;
                }
            }
            else
            {
                if (0 <= itemText.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool MatchTextSearch(ListViewItem eventLogItem, string searchText, bool matchCase)
        {
            foreach (ListViewItem.ListViewSubItem subItem in eventLogItem.SubItems)
            {
                if (MatchTextSearch(subItem.Text, searchText, matchCase))
                    return true;
            }

            string eventMessage = LookupEventLogMessage(eventLogItem);
            if (MatchTextSearch(eventMessage, searchText, matchCase))
                return true;

            return false;
        }

        ListViewItem GetNextEntry(int previousRecordId, bool searchForward, int startIndex)
        {
            if (searchForward)
            {
                if (!_eventListView.VirtualMode)
                {
                    if (startIndex >= _eventListView.Items.Count)
                        return null;
                    else
                        return _eventListView.Items[startIndex];
                }
                else
                {
                    // Sanity checks
                    int endIndex = _eventListView.VirtualListSize;
                    if (endIndex == 0)
                        return null;
                    else if (startIndex < endIndex)
                    {
                        ListViewItem lvItem = _eventListView.Items[startIndex];
                        if ((int)lvItem.Tag != -1 && (int)lvItem.Tag != 0)
                        {
                            if (startIndex > 0)
                            {
                                ListViewItem prevItem = _eventListView.Items[startIndex - 1];
                                if ((int)prevItem.Tag == previousRecordId)
                                    return lvItem;
                            }
                        }
                    }
                    else if (startIndex > 0 && startIndex == endIndex)
                    {
                        if (previousRecordId == (int)_eventListView.Items[startIndex - 1].Tag)
                            return null;
                    }

                    endIndex = _eventListView.VirtualListSize;
                    if (startIndex * 0.75 > endIndex * 0.5)
                    {
                        // Faster to search backwards
                        ListViewItem lvItem = null;
                        for (int j = endIndex - 1; j >= 0; --j)
                        {
                            ListViewItem prevItem = _eventListView.Items[j];
                            if ((int)prevItem.Tag == -1 || (int)prevItem.Tag == 0)
                            {
                                endIndex = _eventListView.VirtualListSize;
                                j = endIndex - 2;
                                continue;   // Restart search
                            }

                            if ((int)prevItem.Tag == previousRecordId)
                            {
                                return lvItem;
                            }

                            lvItem = prevItem;
                        }
                    }
                    else
                    {
                        // Search from the beginning
                        ListViewItem prevItem = null;
                        for (int j = 0; j < endIndex; ++j)
                        {
                            ListViewItem lvItem = _eventListView.Items[j];
                            if ((int)lvItem.Tag == -1 || (int)lvItem.Tag == 0)
                            {
                                endIndex = _eventListView.VirtualListSize;
                                j = -1;
                                prevItem = null;
                                continue;   // Restart search
                            }

                            if (prevItem != null)
                                return lvItem;

                            if ((int)lvItem.Tag == previousRecordId)
                            {
                                prevItem = lvItem;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!_eventListView.VirtualMode)
                {
                    if (startIndex < 0)
                        return null;
                    else
                        return _eventListView.Items[startIndex];
                }
                else
                {
                    // Sanity checks
                    int endIndex = _eventListView.VirtualListSize;
                    if (endIndex == 0)
                        return null;
                    else if (startIndex < endIndex && startIndex >= 0)
                    {
                        ListViewItem lvItem = _eventListView.Items[startIndex];
                        if ((int)lvItem.Tag != -1 && (int)lvItem.Tag != 0)
                        {
                            if ((startIndex + 1) < endIndex)
                            {
                                ListViewItem prevItem = _eventListView.Items[startIndex + 1];
                                if ((int)prevItem.Tag == previousRecordId)
                                    return lvItem;
                            }
                        }
                    }
                    else if (startIndex < 0)
                        return null;

                    endIndex = _eventListView.VirtualListSize;
                    if (startIndex * 0.75 > endIndex * 0.5)
                    {
                        // Faster to search backwards
                        ListViewItem prevItem = null;
                        for (int j = endIndex - 1; j >= 0; --j)
                        {
                            ListViewItem lvItem = _eventListView.Items[j];
                            if ((int)lvItem.Tag == -1 || (int)lvItem.Tag == 0)
                            {
                                endIndex = _eventListView.VirtualListSize;
                                j = endIndex - 2;
                                prevItem = null;
                                continue;   // Restart search
                            }

                            if (prevItem != null)
                                return lvItem;

                            if ((int)lvItem.Tag == previousRecordId)
                            {
                                prevItem = lvItem;
                            }
                        }
                    }
                    else
                    {
                        // Search from the beginning
                        ListViewItem lvItem = null;
                        for (int j = 0; j < endIndex; ++j)
                        {
                            ListViewItem prevItem = _eventListView.Items[j];
                            if ((int)prevItem.Tag == -1 || (int)prevItem.Tag == 0)
                            {
                                endIndex = _eventListView.VirtualListSize;
                                j = -1;
                                continue;   // Restart search
                            }

                            if ((int)prevItem.Tag == previousRecordId)
                            {
                                return lvItem;
                            }

                            lvItem = prevItem;
                        }
                    }
                }
            }

            return null;    // Should never come here
        }

        public bool SearchForText(string searchText, bool matchCase, bool searchForward, bool lineHighlights)
        {
            int listCount = -1;

            if (_eventListView.VirtualMode)
            {
                listCount = _eventListView.VirtualListSize;
            }
            else
            {
                listCount = _eventListView.Items.Count;
            }

            if (listCount <= 1)
                return false;

            // Use selection if it is below top-index
            int startIndex = listCount - 1;
            if (_eventListView.SelectedIndices.Count > 0)
            {
                if (_eventListView.TopItem == null || _eventListView.TopItem.Index - 1 < _eventListView.SelectedIndices[0])
                    startIndex = _eventListView.SelectedIndices[0];
            }

            ListViewItem lvItem = null;

            try
            {
                // We need to handle that EventLog can get pruned
                //  - Make sure the previous index is as expected, else resync
                int previousRecordId = (int)_eventListView.Items[startIndex].Tag;
                using (new HourGlass(this))
                {
                    if (searchForward)
                    {
                        startIndex += 1;

                        // We should not reverse the search if the next startindex is already in cache
                        if (_eventListView.VirtualMode && startIndex < listCount)
                        {
                            _messageLookup.StartMessageCacheThread(_eventLog, searchForward, (int)_eventListView.Items[startIndex].Tag);
                        }

                        do
                        {
                            lvItem = GetNextEntry(previousRecordId, searchForward, startIndex);
                            if (lvItem != null)
                            {
                                if (lineHighlights)
                                {
                                    if (lvItem.ImageIndex < 2)
                                        break;
                                }
                                else if (MatchTextSearch(lvItem, searchText, matchCase))
                                    break;

                                previousRecordId = (int)lvItem.Tag;
                                startIndex = lvItem.Index + 1;
                            }
                        }
                        while (lvItem != null);
                    }
                    else
                    {
                        startIndex -= 1;

                        // We should not reverse the search if the next startindex is already in cache
                        if (_eventListView.VirtualMode && startIndex >= 0)
                        {
                            _messageLookup.StartMessageCacheThread(_eventLog, searchForward, (int)_eventListView.Items[startIndex].Tag);
                        }

                        do
                        {
                            lvItem = GetNextEntry(previousRecordId, searchForward, startIndex);
                            if (lvItem != null)
                            {
                                if (lineHighlights)
                                {
                                    if (lvItem.ImageIndex < 2)
                                        break;
                                }
                                else if (MatchTextSearch(lvItem, searchText, matchCase))
                                    break;

                                previousRecordId = (int)lvItem.Tag;
                                startIndex = lvItem.Index - 1;
                            }
                        }
                        while (lvItem != null);
                    }
                }
            }
            finally
            {
                _messageLookup.StopMessageCacheThread();
            }

            if (lvItem != null)
            {
                _eventListView.SelectedIndices.Clear();  // Clear selection before changing cache to avoid cache miss
                _eventListView.EnsureVisible(lvItem.Index);
                _eventListView.SelectedIndices.Add(lvItem.Index);   // Set selection after having scrolled to avoid top-index cache miss
                _eventListView.Items[lvItem.Index].Focused = true;
                return true;
            }
            return false;
        }

        private void EventLogForm_Load(object sender, EventArgs e)
        {
            if (MdiParent == null)
                Icon = MainForm.Instance.Icon;

            _eventListView.EnableDoubleBuffer();
            _eventListView.Columns.Add("Level", 100);
            _eventListView.Columns.Add("Date and Time", 125);
            _eventListView.Columns.Add("Source", 150);
            _eventListView.Columns.Add("EventId");
            _eventListView.Columns.Add("Category", 100);

            if (_eventLog != null)
                ConfigureColumnFilter(_filterActive);
        }

        private void EventLogForm_Activated(object sender, EventArgs e)
        {
            if (this.MdiParent != null && MainForm.Instance != null)
                MainForm.Instance.SetStatusBar(null, 0, 0);
            SearchForm.Instance.ActiveTailForm = this;
            TabPage parentTab = this.Tag as TabPage;
            if (parentTab != null)
                parentTab.ImageIndex = -1;
            _eventListView.Invalidate();
            _eventListView.Update();
        }

        protected override void OnResize(EventArgs e)
        {
            bool listAtBottom = ListAtBottom();
            base.OnResize(e);
            _eventListView.Invalidate();
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
                int listViewCount = 0;
                if (_eventListView.VirtualMode)
                    listViewCount = _eventListView.VirtualListSize;
                else
                    listViewCount = _eventListView.Items.Count;
                if (listViewCount > 0)
                    _eventListView.EnsureVisible(listViewCount - 1);
            }
            _eventListView.Update();
        }

        private bool ListAtBottom()
        {
            int listViewCount = 0;
            if (_eventListView.VirtualMode)
                listViewCount = _eventListView.VirtualListSize;
            else
                listViewCount = _eventListView.Items.Count;

            if (listViewCount <= 5)
                return true;

            return IsItemVisible(listViewCount - 5);
        }

        private bool IsItemVisible(int index)
        {
            try
            {
                _topItemIndexHack = true;
                ListViewItem topItem = _eventListView.TopItem;
                if (topItem == null || topItem.Index > index)
                    return false;

                int heightOfFirstItem = topItem.Bounds.Height;
                if (heightOfFirstItem == 0)
                    return false;

                int nVisibleLines = _eventListView.Height / heightOfFirstItem;
                int lastVisibleIndexInDetailsMode = topItem.Index + nVisibleLines;
                return lastVisibleIndexInDetailsMode >= index;
            }
            finally
            {
                _topItemIndexHack = false;
            }
        }

        string LookupEventLogMessage(ListViewItem listItem)
        {
            EventLogEntry entry = null;
            if (_eventListView.VirtualMode)
            {
                entry = _eventLog.Entries[listItem.Index];
                if (entry != null && entry.Index != (int)listItem.Tag)
                    entry = null;
            }

            using (new HourGlass(this))
            {
                return _messageLookup.LookupMessage(_eventLog, entry, (int)listItem.Tag, listItem.SubItems[3].Text);
            }
        }

        void _eventLog_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new EntryWrittenEventHandler(_eventLog_EntryWritten), new object[] { sender, e });
                return;
            }

            bool listAtBottom = ListAtBottom();
            if (_filterActive)
            {
                // Collect all relevant events that arrived since last check
                List<ListViewItem> eventLogEvents = new List<ListViewItem>();

                int lastEntryIndex = _lastEventLogEntry;
                int prevEntryIndex = -1;
                int count = _eventLog.Entries.Count;
                for (int i = count - 1; i >= 0; --i)
                {
                    EventLogEntry entry = null;

                    try
                    {
                        entry = _eventLog.Entries[i];
                        if (prevEntryIndex != -1)
                        {
                            // Sanity check to ensure the EventLog have not been pruned by Windows
                            if (_eventLog.Entries[i + 1].Index != prevEntryIndex)
                            {
                                eventLogEvents.Clear();
                                i = _eventLog.Entries.Count;    // Retry
                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Possible that the EventLog have been pruned by Windows
                        System.Diagnostics.Debug.WriteLine("EventLog Possible Pruned: " + ex.Message);
                        eventLogEvents.Clear();
                        i = _eventLog.Entries.Count;    // Retry
                        continue;
                    }

                    if (entry.Index == _lastEventLogEntry)
                        break;

                    if (lastEntryIndex == _lastEventLogEntry)
                        lastEntryIndex = entry.Index;

                    ListViewItem item = CreateListViewItem(entry);
                    if (AcceptFilterListViewItem(item))
                    {
                        _messageLookup.UpdateMesageCache(entry);
                        eventLogEvents.Add(item);
                    }

                    prevEntryIndex = entry.Index;
                }

                // Add any new entries to the list, in reported order
                for (int i = eventLogEvents.Count - 1; i >= 0; --i)
                    _eventListView.Items.Add(eventLogEvents[i]);

                _lastEventLogEntry = lastEntryIndex;

                if (listAtBottom && _eventListView.Items.Count > 0)
                {
                    _eventListView.Invalidate();
                    _eventListView.EnsureVisible(_eventListView.Items.Count - 1);
                    _eventListView.Update();
                }
            }
            else
            {
                // React only if actual change was detected
                int newVirtualListSize = _eventLog.Entries.Count;
                EventLogEntry entry = null;
                while (entry == null && _eventLog.Entries.Count > 0)
                {
                    try
                    {
                        newVirtualListSize = _eventLog.Entries.Count;
                        entry = _eventLog.Entries[newVirtualListSize-1];
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("EventLog Possible Pruned: " + ex.Message);
                        System.Threading.Thread.Sleep(5);	// No need to kick a dying man...too hard
                    }
                }

                if (entry == null && _eventLog.Entries.Count==0)
                {
                    _eventListView.VirtualListSize = 0;
                    return;
                }

                if (_lastEventLogEntry == entry.Index && _eventListView.VirtualListSize == newVirtualListSize)
                    return; // No change detected

                // If returned EventLogEntry.Index == 0, then _eventLog-object is broken, and we should just refresh
                if (entry.Index == 0)
                {
                    _eventListView.Invalidate();
                    this.BeginInvoke(new EntryWrittenEventHandler(_eventLog_EntryWritten), new object[] { sender, null });
                    return;
                }

                // Just refresh if we are at the bottom
                if (listAtBottom || newVirtualListSize<=0)
                {
                    ListViewUtil.SetVirtualListSizeWithoutRefresh(_eventListView, _eventLog.Entries.Count);
                    if (_eventListView.VirtualListSize > 0)
                        _eventListView.EnsureVisible(_eventListView.VirtualListSize - 1);
                    _lastEventLogEntry = entry.Index;
                    return;
                }
              
                try
                {
                    // List count has changed, need to re-position TopItem
                    _eventListView.BeginUpdate();   // Avoid redraw until we have set TopItem.Index
                    int oldVirtualListSize = _eventListView.VirtualListSize;
                    int oldTopItemIndex = GetTopItemIndex();
                    int oldFocusedItemIndex = GetFocusedItemIndex();
                    int oldSelectedItemIndex = (_eventListView.SelectedIndices.Count == 1 && _eventListView.SelectedIndices[0] == oldFocusedItemIndex) ? oldFocusedItemIndex : -1;

					ListViewUtil.SetVirtualListSizeWithoutRefresh(_eventListView, newVirtualListSize);

                    int newItemCount = 0;
                    if (_lastEventLogEntry != -1 && _lastEventLogEntry != entry.Index)
                    {
                        newItemCount = 1;   // Atleast 1 new element

                        ListViewItem lvItem = null;
                        int prevRecordId = entry.Index;
                        int startIndex = _eventListView.VirtualListSize - 2;
                        do
                        {
                            lvItem = GetNextEntry(prevRecordId, false, startIndex);
                            if (lvItem != null)
                            {
                                if (_lastEventLogEntry == (int)lvItem.Tag)
                                    break;

                                newItemCount++;
                                prevRecordId = (int)lvItem.Tag;
                                startIndex = lvItem.Index - 1;
                            }
                        } while (lvItem != null);
                    }
                    _lastEventLogEntry = entry.Index;

                    int newTopItemIndex = _eventListView.VirtualListSize - (oldVirtualListSize - oldTopItemIndex) - newItemCount;
                    if (newTopItemIndex < 0)
                        newTopItemIndex = 0;
                    _eventListView.TopItem = _eventListView.Items[newTopItemIndex];
                    if (GetTopItemIndex() != newTopItemIndex)
                    {
                        System.Threading.Thread.Sleep(5);  // Some times TopItem fails to set the first time (Little weird)
                        _eventListView.TopItem = _eventListView.Items[newTopItemIndex];
                    }
                    if (oldFocusedItemIndex != -1)
                    {
                        int newFocusedItemIndex = _eventListView.VirtualListSize - (oldVirtualListSize - oldFocusedItemIndex) - newItemCount;
                        if (newFocusedItemIndex < 0)
                            newFocusedItemIndex = 0;
                        if (newFocusedItemIndex != oldFocusedItemIndex)
                            _eventListView.FocusedItem = _eventListView.Items[newFocusedItemIndex];
                    }
                    if (oldSelectedItemIndex != -1)
                    {
                        int newSelectedItemIndex = _eventListView.VirtualListSize - (oldVirtualListSize - oldSelectedItemIndex) - newItemCount;
                        if (newSelectedItemIndex < 0)
                        {
                            if (oldFocusedItemIndex == oldSelectedItemIndex)
                                newSelectedItemIndex = 0;
                            else
                                newSelectedItemIndex = -1;
                        }
                        if (oldSelectedItemIndex != newSelectedItemIndex)
                        {
                            _eventListView.SelectedIndices.Clear();
							if (newSelectedItemIndex != -1)
                            	_eventListView.SelectedIndices.Add(newSelectedItemIndex);
                        }
                    }
                }
                finally
                {
                    _eventListView.EndUpdate();
                    _eventListView.Invalidate();
                    //_eventListView.Update();
                }
            }

            if (_displayTabIcon)
            {
                TabPage parentTab = this.Tag as TabPage;
                if (parentTab != null && parentTab.Parent != null && parentTab.Parent.Visible && !parentTab.Visible)
                    parentTab.ImageIndex = 0;
            }
        }

        private int GetFocusedItemIndex()
        {
            try
            {
                _topItemIndexHack = true;
                if (_eventListView.FocusedItem != null)
                    return _eventListView.FocusedItem.Index;
                else
                    return -1;
            }
            finally
            {
                _topItemIndexHack = false;
            }
        }

        private int GetTopItemIndex()
        {
            try
            {
                _topItemIndexHack = true;
                if (_eventListView.TopItem != null)
                    return _eventListView.TopItem.Index;
                else
                    return -1;
            }
            finally
            {
                _topItemIndexHack = false;
            }
        }

        public delegate void UpdateAction(); 

        private void _eventListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (_topItemIndexHack)
            {
                e.Item = new ListViewItem();
                return;
            }

            EventLogEntry entry = null;
            try
            {
                entry = _eventLog.Entries[e.ItemIndex];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EventLog Possible Pruned: " + ex.Message);
            }
            if (entry == null)
            {
                // The EventLog is pruned from time to time, meaning suddenly items will disappear
                bool listAtBottom = ListAtBottom();
                if (listAtBottom)
                {
                    ListViewUtil.SetVirtualListSizeWithoutRefresh(_eventListView, _eventLog.Entries.Count);
                    if (_eventListView.VirtualListSize > 0)
                        _eventListView.EnsureVisible(_eventListView.VirtualListSize - 1);
                }
                else
                {
                    _eventListView.Invalidate();
                    this.BeginInvoke(new EntryWrittenEventHandler(_eventLog_EntryWritten), new object[] { sender, null });
                }
            }
            else
            if (entry.Index == 0)
            {
                // The EventLog object sometimes get "broken" when pruned, and returns invalid entries
                _eventListView.Invalidate();
                this.BeginInvoke(new EntryWrittenEventHandler(_eventLog_EntryWritten), new object[] { sender, null });
            }

            ListViewItem lvi = CreateListViewItem(entry);
            e.Item = lvi;
        }

        private void _eventListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                _eventMessageText.Text = LookupEventLogMessage(e.Item);
                _eventMessageText.ScrollToCaret();  // Scroll to top of RichTextBox
            }
        }

        public static List<string> GetEventLogFiles()
        {
            List<string> logfiles = new List<string>();

            EventLog[] eventLogs = EventLog.GetEventLogs();
            foreach (EventLog eventLog in eventLogs)
                logfiles.Add(eventLog.Log);

            return logfiles;
        }

        private ListViewItem CreateListViewItem(EventLogEntry entry)
        {
            if (entry != null)
            {
                ListViewItem lvi = new ListViewItem(entry.EntryType.ToString());
                lvi.SubItems.Add(entry.TimeWritten.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
                lvi.SubItems.Add(entry.Source);
                int eventid = (int)(entry.InstanceId & 0x3fff);
                lvi.SubItems.Add(eventid.ToString());
                string category = "Invalid Category";
                try
                {
                    category = entry.Category;
                }
                catch (Exception ex)
                {
                    // Category lookup can cause 'Registry subkeys should not be greater than 255 characters' (System.ArgumentException)
                    System.Diagnostics.Debug.WriteLine("Invalid EventLog Category: " + ex.Message);
                }
                lvi.SubItems.Add(category);
                lvi.Tag = entry.Index;
                switch (entry.EntryType)
                {
                    case 0: lvi.ImageIndex = 2; lvi.Text = "Success"; break;
                    case EventLogEntryType.Error: lvi.ImageIndex = 0; break;
                    case EventLogEntryType.Warning: lvi.ImageIndex = 1; break;
                    case EventLogEntryType.Information: lvi.ImageIndex = 2; break;
                    default: lvi.ImageIndex = -1; break;
                }
                return lvi;
            }
            else
            {
                ListViewItem lvi = new ListViewItem("Out of bounds");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.Tag = -1;
                lvi.ImageIndex = -1;
                return lvi;
            }
        }

        private bool AcceptFilterListViewItem(ListViewItem item)
        {
            foreach (List<Regex> filter in _columnFilters)
            {
                bool accept = true;
                int subItemIndex = 0;
                foreach (Regex regexPattern in filter)
                {
                    if (regexPattern != null && !regexPattern.IsMatch(item.SubItems[subItemIndex].Text))
                    {
                        accept = false;
                        break;
                    }
                    subItemIndex++;
                }
                if (accept)
                    return true;
            }
            return false;
        }

        private void _contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            Point mousePosition = MousePosition;
            Point clientPosition = _eventListView.PointToClient(mousePosition);
            ListViewHitTestInfo hitTest = _eventListView.HitTest(clientPosition);
            int subItemIndex = -1;
            if (hitTest != null && hitTest.Item != null && hitTest.SubItem != null)
                subItemIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);

            _contextMenuStrip_Opening(sender, (EventArgs)e);

            // We steal the items from the main menu (we restore them when closing again)
            ToolStripItem[] items = new ToolStripItem[_activeWindowMenuItem.DropDownItems.Count];
            _activeWindowMenuItem.DropDownItems.CopyTo(items, 0);
            _contextMenuStrip.Items.Clear();            // Clear the dummy item
            _contextMenuStrip.Items.AddRange(items);

            foreach (ToolStripItem menuItem in _contextMenuStrip.Items)
                menuItem.Tag = subItemIndex;

            _addFilterToolStripMenuItem.Visible = subItemIndex != -1 && _eventListView.SelectedIndices.Count == 1;
            if (hitTest != null && hitTest.SubItem != null)
                _addFilterToolStripMenuItem.Text = "Add Filter '" + hitTest.SubItem.Text + "'";
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
            _addFilterToolStripMenuItem.Visible = false;
            _filterModeToolStripMenuItem.Checked = _filterActive;
            _filterModeToolStripMenuItem.Enabled = _columnFilters.Count > 0;
            _resetFilterToolStripMenuItem.Enabled = _columnFilters.Count > 0;
        }

        private void switchWindowModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventLogForm newform = new EventLogForm();
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

            newform.LoadConfig(tailConfig, null);
            newform.Show();
            newform.BringToFront();
        }

        private void _addFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == null)
                return;

            int subItemIndex = (int)menuItem.Tag;
            if (subItemIndex == -1)
                return;

            if (_eventListView.SelectedIndices.Count != 1)
                return;

            ListViewItem listViewItem = _eventListView.Items[_eventListView.SelectedIndices[0]];
            string filterText = listViewItem.SubItems[subItemIndex].Text;
            List<Regex> filter = new List<Regex>();
            foreach (ListViewItem.ListViewSubItem subitem in listViewItem.SubItems)
                filter.Add(null);
            filter[subItemIndex] = new Regex(filterText);
            _columnFilters.Add(filter);
            ConfigureColumnFilter(true);
        }

        private void ConfigureColumnFilter(bool enableFilter)
        {
            _filterActive = enableFilter;

            if (_eventLog.EnableRaisingEvents || _filterEventLogTimer.Enabled)
            {
                _eventLog.EnableRaisingEvents = false;
                _filterEventLogTimer.Enabled = false;
                Application.DoEvents(); // Process any pending updates from _eventLog or _filterThread
            }

            _lastEventLogEntry = -1;
            _eventListView.FocusedItem = null;
            _eventListView.SelectedIndices.Clear();

            if (!enableFilter)
            {
                _eventListView.Items.Clear();
                _eventListView.VirtualMode = true;
                _eventListView.VirtualListSize = _eventLog.Entries.Count;
                if (_eventListView.VirtualListSize > 0)
                {
                    _lastEventLogEntry = _eventLog.Entries[_eventLog.Entries.Count-1].Index;
                    _eventListView.SelectedIndices.Clear();
                    _eventListView.SelectedIndices.Add(_eventListView.VirtualListSize - 1);
                    _eventListView.EnsureVisible(_eventListView.VirtualListSize - 1);
                    _eventListView.FocusedItem = _eventListView.Items[_eventListView.VirtualListSize - 1];
                    _eventListView.Update();
                }
                Text = _formTitle;
            }
            else
            {
                _eventListView.VirtualListSize = 0;
                _eventListView.VirtualMode = false;
                _eventListView.Items.Clear();
                Text = _formTitle + " (Filter Mode)";

                _lastEventLogFilterIndex = _eventLog.Entries.Count - 1;
                _lastEventLogFilterEntry = -1;
                _lastEventLogEntry = _eventLog.Entries[_lastEventLogFilterIndex].Index;
                _filterEventLogTimer.Enabled = true;
            }

            // Allow the _eventLog to notify about new 
            _eventLog.EnableRaisingEvents = true;
        }

        private void _filterActiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureColumnFilter(!_filterActive);
        }

        private void resetFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureColumnFilter(false);
            _columnFilters.Clear();
        }

        private void EventLogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_messageLookup != null)
                _messageLookup.Dispose();
            _filterEventLogTimer.Enabled = false;
            _eventLog.EnableRaisingEvents = false;
        }

        private void _filterEventLogTimer_Tick(object sender, EventArgs e)
        {
            bool listAtBottom = ListAtBottom();
            int listCount = _eventListView.Items.Count;
            int topItemIndex = listCount > 0 ? _eventListView.TopItem.Index : -1;

            bool foundLastEntry = _lastEventLogFilterEntry == -1;

            // Loop through the next 50 messages
            // - When new messages arrrive, then the Count increases (ignores these)
            // - When new old messages are pruned, then the Count decreases
            // - Should only use _lastEventLogFilterIndex as hint and search for last position
            _eventListView.BeginUpdate();
            int lastEventLogFilterIndex = Math.Max(0, _lastEventLogFilterIndex - 50);
            for (int i = _lastEventLogFilterIndex; i >= lastEventLogFilterIndex; --i)
            {
                EventLogEntry entry = _eventLog.Entries[i];
                if (!foundLastEntry)
                {
                    if (entry.Index == _lastEventLogFilterEntry)
                        foundLastEntry = true;
                    continue;
                }

                ListViewItem item = CreateListViewItem(entry);
                if (AcceptFilterListViewItem(item))
                {
                    _messageLookup.UpdateMesageCache(entry);
                    if (_eventListView.Items.Count == 0 || entry.Index != (int)_eventListView.Items[0].Tag)
                        _eventListView.Items.Insert(0, item);
                }

                _lastEventLogFilterEntry = entry.Index;
            }
            _eventListView.EndUpdate();

            if (listCount == 0 && _eventListView.Items.Count > 0)
                _eventListView.SelectedIndices.Add(_eventListView.Items.Count - 1);

            if (_eventListView.FocusedItem != null)
                _eventListView.SelectedIndices.Add(_eventListView.FocusedItem.Index);

            if (topItemIndex != -1)
            {
                if (listAtBottom && topItemIndex == 0)
                {
                    _eventListView.Invalidate();
                    _eventListView.EnsureVisible(_eventListView.Items.Count - 1);
                    _eventListView.Update();
                }
                else
                if (_eventListView.Items.Count - listCount > 0)
                {
                    int newTopItemIndex = topItemIndex + _eventListView.Items.Count - listCount;
                    if (newTopItemIndex < 0)
                        newTopItemIndex = 0;
                    _eventListView.TopItem = _eventListView.Items[newTopItemIndex];
                    if (_eventListView.TopItem.Index != newTopItemIndex)
                    {
                        System.Threading.Thread.Sleep(5);  // Some times TopItem fails to set the first time (Little weird)
                        _eventListView.TopItem = _eventListView.Items[newTopItemIndex];
                    }
                }
            }

            _lastEventLogFilterIndex = lastEventLogFilterIndex;
            if (_lastEventLogFilterIndex == 0)
                _filterEventLogTimer.Enabled = false;
        }

        private void _copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Copy selected rows to clipboard
            StringBuilder selection = new StringBuilder();
            if (_eventMessageText.Focused)
            {
                if (_eventMessageText.SelectedText.Length != 0)
                    selection.AppendLine(_eventMessageText.SelectedText);
                else
                {
                    foreach (string line in _eventMessageText.Lines)
                        selection.AppendLine(line);
                }
            }
            else
            {
                if (_eventListView.SelectedIndices.Count > 1)
                {
                    string columnText = "";
                    foreach (ColumnHeader columnHeader in _eventListView.Columns)
                    {
                        if (columnText.Length > 0)
                            columnText += '\t';
                        columnText += columnHeader.Text;
                    }
                    columnText += '\t';
                    columnText += "Message";
                    selection.AppendLine(columnText);

                    foreach (int itemIndex in _eventListView.SelectedIndices)
                    {
                        string itemText = "";
                        ListViewItem item = _eventListView.Items[itemIndex];
                        foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                        {
                            if (itemText.Length > 0)
                                itemText += '\t';
                            itemText += subItem.Text;
                        }
                        itemText += '\t';
                        itemText += LookupEventLogMessage(item).Replace(Environment.NewLine, "").Replace("\n", "").Replace("\t", " ");
                        selection.AppendLine(itemText);
                    }
                }
                else
                if (_eventListView.SelectedIndices.Count == 1)
                {
                    foreach (int itemIndex in _eventListView.SelectedIndices)
                    {
                        ListViewItem item = _eventListView.Items[itemIndex];
                        foreach (ColumnHeader columnHeader in _eventListView.Columns)
                            selection.AppendLine(columnHeader.Text + ": " + item.SubItems[columnHeader.Index].Text);
                        selection.AppendLine("Message:");
                        // Fix unix-newlines to environment newlines
                        selection.Append( LookupEventLogMessage(item).Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine));
                    }
                }
                else
                if (_eventListView.FocusedItem != null)
                {
                    ListViewItem item = _eventListView.FocusedItem;
                    foreach (ColumnHeader columnHeader in _eventListView.Columns)
                        selection.AppendLine(columnHeader.Text + ": " + item.SubItems[columnHeader.Index].Text);
                    selection.AppendLine("Message:");
                    // Fix unix-newlines to environment newlines
                    selection.Append(LookupEventLogMessage(item).Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine));
                }
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

        private void _configTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TailFileConfig configFile = new TailFileConfig();
            SaveConfig(configFile);
            TailConfigForm configForm = new TailConfigForm(configFile, false);
            if (configForm.ShowDialog(this) == DialogResult.OK)
                LoadConfig(configForm.TailFileConfig, null);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm.Instance.StartSearch(this);
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
             SearchForm.Instance.SearchAgain(this, true, false);
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

        private void gotoPreviousHighlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm.Instance.SearchAgain(this, false, true);
        }

        private void gotoNextHighlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm.Instance.SearchAgain(this, true, true);
        }
    }

    class EventLogMesageLookup : IDisposable
    {
        Dictionary<int, string> _eventMessages = new Dictionary<int, string>();
        ManagementOperationObserver _eventMessagesObserver = null;
        System.Reflection.Assembly _eventLogReaderAssembly = null;
        Type _eventLogReaderQueryType = null;
        Type _eventLogReaderEnumType = null;
        Type _eventLogReaderType = null;
        Type _eventLogRecordType = null;
        Type _eventLogPropertyType = null;
        bool _eventLogReaderThreadContinue = true;
        System.Threading.Thread _eventLogReaderThread = null;
        bool _eventLogReaderReadForward = false;
        bool _eventLogReaderReadEverything = false;
        TimeSpan _eventLogReaderReadDelay = TimeSpan.FromMilliseconds(100);

        public EventLogMesageLookup(EventLog eventLog)
        {
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                //System.Diagnostics.Eventing.Reader.EventLogReader
                try
                {
                    _eventLogReaderAssembly = System.Reflection.Assembly.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken= b77a5c561934e089");
                    _eventLogReaderQueryType = _eventLogReaderAssembly.GetType("System.Diagnostics.Eventing.Reader.EventLogQuery");
                    _eventLogReaderEnumType = _eventLogReaderAssembly.GetType("System.Diagnostics.Eventing.Reader.PathType");
                    _eventLogReaderType = _eventLogReaderAssembly.GetType("System.Diagnostics.Eventing.Reader.EventLogReader");
                    _eventLogRecordType = _eventLogReaderAssembly.GetType("System.Diagnostics.Eventing.Reader.EventRecord");
                    _eventLogPropertyType = _eventLogReaderAssembly.GetType("System.Diagnostics.Eventing.Reader.EventProperty");

                    _eventLogReaderThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ThreadProc));
                    _eventLogReaderThread.Start(eventLog.Log);
                }
                catch (Exception ex)
                {
                    _eventLogReaderAssembly = null;
                    _eventLogReaderReadDelay = TimeSpan.FromMilliseconds(5);
                    System.Diagnostics.Debug.WriteLine("EventLogReader unavailable reverts to WMI query: " + ex.Message);
                    GetEventLogItemMessages(eventLog.MachineName, eventLog.Log);

                    if (eventLog.Entries.Count > 0)
                    {
                        // Wait for the first eventlog message to be read from WMI
                        while (true)
                        {
                            lock (_eventMessages)
                            {
                                if (_eventMessages.Count > 0)
                                    break;
                            }
                            System.Threading.Thread.Sleep(10);
                        }
                    }
                }
            }            
        }

        public void Dispose()
        {
            if (_eventMessagesObserver != null)
            {
                lock (_eventMessages)
                {
                    _eventMessagesObserver.ObjectReady -= OnEventLogEntryReady;
                    _eventMessagesObserver = null;
                }
            }
            if (_eventLogReaderThread != null)
            {
                _eventLogReaderThreadContinue = false;
                while (_eventLogReaderThread.IsAlive)
                {
                    System.Threading.Thread.Sleep(5);
                    continue;
                }
            }
        }

        public string LookupMessage(EventLog eventLog, EventLogEntry eventLogEntry, int eventRecordId, string eventId)
        {
            if (eventLogEntry != null)
                UpdateMesageCache(eventLogEntry);

            string eventMessage = null;
            lock (_eventMessages)
            {
                if (_eventMessages.TryGetValue(eventRecordId, out eventMessage))
                {
                    if (eventMessage != null)
                        return eventMessage;
                    else
                    if (eventLogEntry != null)
                    {
                        // Message lookup performed, but nothing found use default message
                        _eventMessages[eventRecordId] = eventLogEntry.Message;
                        return eventLogEntry.Message;
                    }
                }
            }

            if (_eventLogReaderAssembly != null)
            {
                object readerObj = null;
                try
                {
                    string queryStr = string.Format("*[System/EventRecordID={0}]", eventRecordId);
                    object queryObj = Activator.CreateInstance(_eventLogReaderQueryType, new Object[] { eventLog.Log, Enum.GetValues(_eventLogReaderEnumType).GetValue(0), queryStr });
                    readerObj = Activator.CreateInstance(_eventLogReaderType, new Object[] { queryObj });
                    object eventRecordObj = _eventLogReaderType.InvokeMember("ReadEvent", System.Reflection.BindingFlags.InvokeMethod, null, readerObj, null);
                    eventMessage = ExtractMessage(eventRecordObj);
                }
                catch (Exception ex)
                {
                    _eventLogReaderAssembly = null;
                    System.Diagnostics.Debug.WriteLine("EventLog Message Lookup Failed: " + ex.Message);
                    throw;
                }
                finally
                {
                    IDisposable disposeReader = readerObj as IDisposable;
                    if (disposeReader != null)
                        disposeReader.Dispose();
                }
            }
            else
            {
                eventMessage = GetEventLogItemMessage(eventLog.MachineName, eventLog.LogDisplayName, (uint)eventRecordId);
            }

            if (eventMessage != null)
            {
                lock (_eventMessages)
                {
                    if (!_eventMessages.ContainsKey(eventRecordId))
                        _eventMessages.Add(eventRecordId, eventMessage);
                }
                return eventMessage;
            }
            else
            if (eventLogEntry != null)
            {
                lock (_eventMessages)
                {
                    if (!_eventMessages.ContainsKey(eventRecordId))
                        _eventMessages.Add(eventRecordId, eventLogEntry.Message);
                }
                return eventLogEntry.Message;
            }

            return "";
        }

        public void UpdateMesageCache(EventLogEntry eventLogEntry)
        {
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                string eventMessage = eventLogEntry.Message;
                int eventId = (int)(eventLogEntry.InstanceId);
                if (eventMessage.IndexOf(" Event ID '" + eventId.ToString() + "' ") != -1)
                    return; // Cannot be used
            }

            lock (_eventMessages)
            {
                if (!_eventMessages.ContainsKey(eventLogEntry.Index))
                    _eventMessages.Add(eventLogEntry.Index, eventLogEntry.Message);
            }
        }

        public void StartMessageCacheThread(EventLog eventLog, bool readForward, int nextEventRecordId)
        {
            if (_eventLogReaderAssembly != null)
            {
                if (_eventLogReaderThread != null)
                {
                    if (_eventLogReaderThreadContinue && _eventLogReaderThread.IsAlive)
                    {
                        if (_eventLogReaderReadForward != readForward)
                        {
                            lock (_eventMessages)
                            {
                                if (readForward && _eventMessages.ContainsKey(nextEventRecordId))
                                {
                                    // Continue current search direction
                                    _eventLogReaderReadDelay = TimeSpan.FromMilliseconds(0);
                                    return;
                                }
                                else
                                {
                                    // Reverse search direction
                                    _eventLogReaderThreadContinue = false;
                                    while (_eventLogReaderThread.IsAlive)
                                    {
                                        System.Threading.Thread.Sleep(5);
                                        continue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            _eventLogReaderReadDelay = TimeSpan.FromMilliseconds(0);
                            return;
                        }
                    }
                }

                _eventLogReaderThreadContinue = true;
                _eventLogReaderReadForward = readForward;
                _eventLogReaderReadDelay = TimeSpan.FromMilliseconds(0);
                _eventLogReaderThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ThreadProc));
                _eventLogReaderThread.Start(eventLog.Log);
            }
            else
            {
                _eventLogReaderReadDelay = TimeSpan.FromMilliseconds(0);
            }
        }

        public void StopMessageCacheThread()
        {
            if (_eventLogReaderAssembly != null)
                _eventLogReaderReadDelay = TimeSpan.FromMilliseconds(1000);
            else
                _eventLogReaderReadDelay = TimeSpan.FromMilliseconds(5);
        }

        private string ExtractMessage(object eventRecordObj)
        {
            if (eventRecordObj == null)
                return null;

            object eventProperties = _eventLogRecordType.InvokeMember("Properties", System.Reflection.BindingFlags.GetProperty, null, eventRecordObj, null);
            System.Collections.IEnumerable eventPropertiesList = eventProperties as System.Collections.IEnumerable;

            List<string> eventPropertyValues = new List<string>();
            foreach (object property in eventPropertiesList)
            {
                string propertyValue = _eventLogPropertyType.InvokeMember("Value", System.Reflection.BindingFlags.GetProperty, null, property, null) as string;
                if (propertyValue != null)
                    eventPropertyValues.Add(propertyValue);
            }

            System.Globalization.CultureInfo backupCulture = System.Threading.Thread.CurrentThread.CurrentCulture;

            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                object eventMessageStr = _eventLogRecordType.InvokeMember("FormatDescription", System.Reflection.BindingFlags.InvokeMethod, null, eventRecordObj, null, new System.Globalization.CultureInfo("en-US"));
                if (eventMessageStr != null)
                    return eventMessageStr as string;
                else
                if (eventPropertyValues != null && eventPropertyValues.Count == 1)
                    return eventPropertyValues[0];
                else
                    return null;
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = backupCulture;
            }
        }

        private void ThreadProc(object logName)
        {
            if (_eventLogReaderReadEverything || _eventLogReaderAssembly == null)
                return;

            object readerObj = null;
            try
            {
                string queryStr = string.Format("*");
                object queryObj = Activator.CreateInstance(_eventLogReaderQueryType, new Object[] { logName.ToString(), Enum.GetValues(_eventLogReaderEnumType).GetValue(0), queryStr });
                _eventLogReaderQueryType.InvokeMember("ReverseDirection", System.Reflection.BindingFlags.SetProperty, null, queryObj, new object[] { !_eventLogReaderReadForward });
                readerObj = Activator.CreateInstance(_eventLogReaderType, new Object[] { queryObj });
                object eventRecordObj = _eventLogReaderType.InvokeMember("ReadEvent", System.Reflection.BindingFlags.InvokeMethod, null, readerObj, null);

                while (_eventLogReaderThreadContinue && eventRecordObj != null)
                {
                    object eventRecordId = _eventLogRecordType.InvokeMember("RecordId", System.Reflection.BindingFlags.GetProperty, null, eventRecordObj, null);
                    string eventMessage = ExtractMessage(eventRecordObj);
                    if (eventRecordId != null)
                    {
                        lock (_eventMessages)
                        {
                            if (!_eventMessages.ContainsKey((int)(long)eventRecordId))
                                _eventMessages.Add((int)(long)eventRecordId, eventMessage);
                        }
                    }

                    eventRecordObj = _eventLogReaderType.InvokeMember("ReadEvent", System.Reflection.BindingFlags.InvokeMethod, null, readerObj, null);
                    System.Threading.Thread.Sleep((int)_eventLogReaderReadDelay.TotalMilliseconds);
                }

                if (eventRecordObj == null)
                    _eventLogReaderReadEverything = true;
            }
            catch (Exception ex)
            {
                _eventLogReaderAssembly = null;
                System.Diagnostics.Debug.WriteLine("EventLog Message Lookup Failed: " + ex.Message);
            }
            finally
            {
                IDisposable disposeReader = readerObj as IDisposable;
                if (disposeReader != null)
                    disposeReader.Dispose();
            }
        }

        private static string GetStandardPath(string machinename)
        {
            return String.Concat(
                        @"\\",
                        machinename,
                        @"\root\CIMV2"
            );
        }

        private void GetEventLogItemMessages(string machinename, string logname)
        {
            if (_eventMessagesObserver == null)
            {
                ManagementScope messageScope = new ManagementScope(
                             GetStandardPath(machinename)
                 );

                messageScope.Connect();

                StringBuilder query = new StringBuilder();
                query.Append("select Message, InsertionStrings, RecordNumber from Win32_NTLogEvent where LogFile ='");
                query.Append(logname.Replace("'", "''"));
                query.Append("'");

                System.Management.ObjectQuery objectQuery = new System.Management.ObjectQuery(
                    query.ToString()
                );

                EnumerationOptions objectQueryOptions = new EnumerationOptions();
                objectQueryOptions.BlockSize = 100000;

                ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(messageScope, objectQuery);

                _eventMessagesObserver = new ManagementOperationObserver();
                _eventMessagesObserver.ObjectReady += new ObjectReadyEventHandler(OnEventLogEntryReady);
                _eventMessagesObserver.Completed += new CompletedEventHandler(OnEventLogEntryCompleted);
                objectSearcher.Get(_eventMessagesObserver);
            }
        }

        void OnEventLogEntryCompleted(object sender, CompletedEventArgs e)
        {
            _eventMessagesObserver = null;
        }

        static string ExtractMessage(ManagementBaseObject obj)
        {
            string message = (string)obj["Message"];
            string[] insertionStrings = (string[])obj["InsertionStrings"];

            if (message != null)
                return message;

            if (insertionStrings != null && insertionStrings.Length == 1)
                return insertionStrings[0];

            return null;
        }

        void OnEventLogEntryReady(object sender, ObjectReadyEventArgs e)
        {
            uint eventRecordId = (uint)e.NewObject["RecordNumber"];
            string eventMessage = ExtractMessage(e.NewObject);
            lock (_eventMessages)
            {
                if (!_eventMessages.ContainsKey((int)eventRecordId))
                    _eventMessages.Add((int)eventRecordId, eventMessage);
            }
            if ((int)_eventLogReaderReadDelay.TotalMilliseconds > 0)
                System.Threading.Thread.Sleep((int)_eventLogReaderReadDelay.TotalMilliseconds);
        }

        private static string GetEventLogItemMessage(string machinename, string logname, uint thisIndex)
        {
            ManagementScope messageScope = new ManagementScope(
                        GetStandardPath(machinename)
            );

            messageScope.Connect();

            StringBuilder query = new StringBuilder();
            query.Append("select Message, InsertionStrings from Win32_NTLogEvent where LogFile ='");
            query.Append(logname.Replace("'", "''"));
            query.Append("' AND RecordNumber='");
            query.Append(thisIndex);
            query.Append("'");

            System.Management.ObjectQuery objectQuery = new System.Management.ObjectQuery(
                query.ToString()
            );

            EnumerationOptions objectQueryOptions = new EnumerationOptions();
            objectQueryOptions.Rewindable = false;

            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(messageScope, objectQuery, objectQueryOptions))
            {
                // Execute the query
                using (ManagementObjectCollection collection = objectSearcher.Get())
                {
                    // Execute the query
                    using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = collection.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string message = ExtractMessage(enumerator.Current);
                            return message;
                        }
                    }
                }
            }

            return null;
        }
    }

    class EventLogListView : ListView
    {
        public EventLogListView()
        {
        }

        public void EnableDoubleBuffer()
        {
            DoubleBuffered = true;
        }

        // http://blogs.msdn.com/b/cumgranosalis/archive/2006/03/18/listviewvirtualmodebugs.aspx
        [DefaultValue(0)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public new int VirtualListSize
        {
            get { return base.VirtualListSize; }

            set
            {
                try
                {
                    base.VirtualListSize = value;
                }
                catch (ArgumentOutOfRangeException)
                {
                    // If current TopItem.Index is larger than the new value, then exception is thrown
                    base.Invalidate();
                    base.Update();
                }
            }
        }
    }
}
