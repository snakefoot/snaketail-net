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
    public partial class EventLogForm : Form
    {
        Dictionary<int, string> _eventMessages = new Dictionary<int, string>();
        ManagementOperationObserver _eventMessagesObserver = null;
        EventLog _eventLog;
        List<List<Regex>> _columnFilters = new List<List<Regex>>();
        bool _filterActive = false;
        int _lastEventLogEntry = -1;
        int _lastEventLogFilterIndex = -1;

        public EventLogForm()
        {
            InitializeComponent();
        }

        public void LoadFile(string eventLogFile)
        {
            if (MdiParent == null)
                Icon = MainForm.Instance.Icon;

            _eventLog = new EventLog(eventLogFile);
            _eventLog.EnableRaisingEvents = true;
            _eventLog.EntryWritten += new EntryWrittenEventHandler(_eventLog_EntryWritten);
            _eventLog.EndInit();

            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                GetEventLogItemMessages(_eventLog.MachineName, _eventLog.LogDisplayName);

                if (_eventLog.Entries.Count > 0)
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

        public void SaveConfig(TailFileConfig tailConfig)
        {
            tailConfig.BackColor = ColorTranslator.ToHtml(_eventListView.BackColor);
            tailConfig.TextColor = ColorTranslator.ToHtml(_eventListView.ForeColor);

            TypeConverter fontConverter = TypeDescriptor.GetConverter(typeof(Font));
            tailConfig.Font = fontConverter.ConvertToString(_eventListView.Font);
            tailConfig.FilePath = _eventLog.Log;
            tailConfig.Title = Text;
            tailConfig.Modeless = MdiParent == null;
            tailConfig.WindowState = WindowState;
            tailConfig.WindowSize = Size;
            tailConfig.WindowPosition = DesktopLocation;
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
        }

        public void LoadConfig(TailFileConfig tailConfig)
        {
            if (tailConfig.BackColor != null)
                _eventListView.BackColor = ColorTranslator.FromHtml(tailConfig.BackColor);
            if (tailConfig.TextColor != null)
                _eventListView.ForeColor = ColorTranslator.FromHtml(tailConfig.TextColor);

            if (tailConfig.Font != null)
            {
                TypeConverter fontConverter = TypeDescriptor.GetConverter(typeof(Font));
                _eventListView.Font = (Font)fontConverter.ConvertFromString(tailConfig.Font);
            }

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

            LoadFile(tailConfig.FilePath);
            _filterActive = tailConfig.ColumnFilterActive;

            if (Visible)
            {
                ConfigureColumnFilter(_filterActive);
            }
        }

        private void EventLogForm_Load(object sender, EventArgs e)
        {
            _eventListView.EnableDoubleBuffer();
            _eventListView.Columns.Add("Level", 100);
            _eventListView.Columns.Add("Date and Time", 125);
            _eventListView.Columns.Add("Source", 150);
            _eventListView.Columns.Add("EventId");
            _eventListView.Columns.Add("Category", 100);

            if (_eventLog != null)
                ConfigureColumnFilter(_filterActive);
        }

        protected override void OnResize(EventArgs e)
        {
            bool listAtBottom = ListAtBottom();
            base.OnResize(e);
            _eventListView.Invalidate();
            if (listAtBottom)
            {
                if (_eventListView.VirtualListSize > 0)
                    _eventListView.EnsureVisible(_eventListView.VirtualListSize - 1);
            }
            _eventListView.Update();
        }

        private bool ListAtBottom()
        {
            if (_eventListView.VirtualMode)
            {
                if (_eventListView.VirtualListSize <= 5)
                    return true;
            }
            else
            {
                if (_eventListView.Items.Count <= 5)
                    return true;
            }

            if (_eventListView.TopItem == null)
                return false;   // There is no bottom

            if (_eventListView.VirtualMode)
                return IsItemVisible(_eventListView.VirtualListSize - 5);
            else
                return IsItemVisible(_eventListView.Items.Count - 5);
        }

        private bool IsItemVisible(int index)
        {
            if (_eventListView.TopItem == null || _eventListView.TopItem.Index > index)
                return false;

            int heightOfFirstItem = _eventListView.TopItem.Bounds.Height;
            if (heightOfFirstItem == 0)
                return false;
            int nVisibleLines = _eventListView.Height / heightOfFirstItem;
            int lastVisibleIndexInDetailsMode = _eventListView.TopItem.Index + nVisibleLines;
            return lastVisibleIndexInDetailsMode >= index;
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
                // Add any new entries to the list
                int lastEventLogEntry = -1;
                for (int i = _eventLog.Entries.Count - 1; i >= 0; --i)
                {
                    EventLogEntry entry = _eventLog.Entries[i];
                    if (_lastEventLogEntry == entry.Index)
                        break;

                    if (lastEventLogEntry == -1)
                        lastEventLogEntry = entry.Index;

                    ListViewItem item = CreateListViewItem(entry);
                    if (AcceptFilterListViewItem(item))
                        _eventListView.Items.Add(item);
                }
                if (lastEventLogEntry != -1)
                    _lastEventLogEntry = lastEventLogEntry;

                if (listAtBottom && _eventListView.Items.Count > 0)
                {
                    _eventListView.Invalidate();
                    _eventListView.EnsureVisible(_eventListView.Items.Count - 1);
                    _eventListView.Update();
                }
            }
            else
            {
                // Just refresh the list
                _eventListView.VirtualListSize = _eventLog.Entries.Count;
                _eventListView.Invalidate();
                if (listAtBottom)
                    _eventListView.EnsureVisible(_eventListView.VirtualListSize - 1);
                _eventListView.Update();
            }
        }

        private void _eventListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            EventLogEntry entry = _eventLog.Entries[e.ItemIndex];
            if (entry == null)
                return;

            ListViewItem lvi = CreateListViewItem(entry);
            e.Item = lvi;
        }

        private void _eventListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                lock (_eventMessages)
                {
                    string message;
                    if (_eventMessages.TryGetValue((int)e.Item.Tag, out message))
                    {
                        _eventMessageText.Text = message;
                        return;
                    }
                }

                EventLogEntry entry = _eventLog.Entries[e.Item.Index];
                if (entry == null)
                    return;

                if (System.Environment.OSVersion.Version.Major >= 6)
                {
                    if (entry.Index == (int)e.Item.Tag && entry.Message.IndexOf(" Event ID '" + entry.InstanceId.ToString() + "' ") == -1)
                    {
                        _eventMessageText.Text = entry.Message;
                    }
                    else
                    {
                        using (new HourGlass(this))
                        {
                            _eventMessageText.Text = GetEventLogItemMessage(_eventLog.MachineName, _eventLog.LogDisplayName, (uint)(int)e.Item.Tag);
                        }
                    }

                    if (_eventMessageText.Text != null)
                    {
                        lock (_eventMessages)
                        {
                            if (!_eventMessages.ContainsKey((int)e.Item.Tag))
                                _eventMessages.Add((int)e.Item.Tag, _eventMessageText.Text);
                        }
                    }
                    else
                    {
                        _eventMessageText.Text = "";
                    }
                }
                else
                {
                    _eventMessageText.Text = entry.Message;
                }
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
            objectSearcher.Get(_eventMessagesObserver);
        }

        void OnEventLogEntryReady(object sender, ObjectReadyEventArgs e)
        {
            lock (_eventMessages)
            {
                uint messageIndex = (uint)e.NewObject["RecordNumber"];
                string message = (string)e.NewObject["Message"];
                string[] insertionStrings = (string[])e.NewObject["InsertionStrings"];

                if (message == null)
                {
                    if (insertionStrings.Length > 0)
                    {
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < insertionStrings.Length; i++)
                        {
                            sb.Append(insertionStrings[i]);
                            sb.Append(" ");
                        }

                        if (!_eventMessages.ContainsKey((int)messageIndex))
                            _eventMessages.Add((int)messageIndex, sb.ToString());
                    }
                }
                else
                {
                    if (!_eventMessages.ContainsKey((int)messageIndex))
                        _eventMessages.Add((int)messageIndex, message);
                }
            }
            System.Threading.Thread.Sleep(5);
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
                            string message = (string)enumerator.Current["Message"];
                            string[] insertionStrings = (string[])enumerator.Current["InsertionStrings"];

                            if (message == null)
                            {
                                if (insertionStrings.Length > 0)
                                {
                                    StringBuilder sb = new StringBuilder();

                                    for (int i = 0; i < insertionStrings.Length; i++)
                                    {
                                        sb.Append(insertionStrings[i]);
                                        sb.Append(" ");
                                    }

                                    return sb.ToString();
                                }
                            }
                            else
                            {
                                return message;
                            }
                        }
                    }
                }
            }

            return null;
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
            ListViewItem lvi = new ListViewItem(entry.EntryType.ToString());
            lvi.SubItems.Add(entry.TimeWritten.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
            lvi.SubItems.Add(entry.Source);
            int eventid = (int)(entry.InstanceId & 0x3fff);
            lvi.SubItems.Add(eventid.ToString());
            lvi.SubItems.Add(entry.Category);
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

            foreach (ToolStripMenuItem menuItem in _contextMenuStrip.Items)
                menuItem.Tag = subItemIndex;

            _addFilterToolStripMenuItem.Visible = subItemIndex != -1 && _eventListView.SelectedIndices.Count == 1;
            if (hitTest != null && hitTest.SubItem != null)
                _addFilterToolStripMenuItem.Text = "Add Filter '" + hitTest.SubItem.Text + "'";

            _filterActiveToolStripMenuItem.Checked = _filterActive;
            _filterActiveToolStripMenuItem.Enabled = _columnFilters.Count > 0;

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

            newform.LoadConfig(tailConfig);
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

            _eventLog.EnableRaisingEvents = false;
            _filterEventLogTimer.Enabled = false;

            Application.DoEvents(); // Process any pending updates from _eventLog or _filterThread
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
                    _eventListView.SelectedIndices.Clear();
                    _eventListView.SelectedIndices.Add(_eventListView.VirtualListSize - 1);
                    _eventListView.EnsureVisible(_eventListView.VirtualListSize - 1);
                    _eventListView.Update();
                }
                Text = "EventLog - " + _eventLog.LogDisplayName;
            }
            else
            {
                _eventListView.VirtualListSize = 0;
                _eventListView.VirtualMode = false;
                _eventListView.Items.Clear();
                Text = "EventLog - " + _eventLog.LogDisplayName + " (Filter Mode)";

                _lastEventLogFilterIndex = _eventLog.Entries.Count - 1;
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
            _filterEventLogTimer.Enabled = false;
            _eventLog.EnableRaisingEvents = false;
            if (_eventMessagesObserver != null)
            {
                lock (_eventMessages)
                {
                    _eventMessagesObserver.ObjectReady -= new ObjectReadyEventHandler(OnEventLogEntryReady);
                    _eventMessagesObserver = null;
                }
            }
        }

        private void _filterEventLogTimer_Tick(object sender, EventArgs e)
        {
            bool listAtBottom = ListAtBottom();
            bool listAtTop = _eventListView.TopItem != null && _eventListView.TopItem.Index == 0;
            int listCount = _eventListView.Items.Count;

            // Loop through the next 50 messages
            _eventListView.BeginUpdate();
            int lastEventLogFilterIndex = Math.Max(0, _lastEventLogFilterIndex - 50);
            for (int i = _lastEventLogFilterIndex; i > lastEventLogFilterIndex; --i)
            {
                EventLogEntry entry = _eventLog.Entries[i];
                ListViewItem item = CreateListViewItem(entry);
                if (AcceptFilterListViewItem(item))
                {
                    if (_eventListView.Items.Count == 0 || entry.Index != (int)_eventListView.Items[0].Tag)
                        _eventListView.Items.Insert(0, item);
                }
            }

            _eventListView.EndUpdate();

            if (listCount==0 && _eventListView.Items.Count > 0)
                _eventListView.SelectedIndices.Add(_eventListView.Items.Count-1);

            if (_eventListView.FocusedItem != null)
                _eventListView.SelectedIndices.Add(_eventListView.FocusedItem.Index);

            int itemsAdded = _eventListView.Items.Count - listCount;
            if (itemsAdded > 0)
            {
                _eventListView.Invalidate();
                if (listAtBottom)
                    _eventListView.EnsureVisible(_eventListView.Items.Count - 1);
                else
                if (listAtTop)
                    _eventListView.EnsureVisible(0);
                else
                if (_eventListView.SelectedIndices.Count > 0)
                    _eventListView.EnsureVisible(_eventListView.SelectedIndices[0]);
                _eventListView.Update();
            }

            _lastEventLogFilterIndex = lastEventLogFilterIndex;
            if (_lastEventLogFilterIndex == 0)
                _filterEventLogTimer.Enabled = false;
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
    }
}
