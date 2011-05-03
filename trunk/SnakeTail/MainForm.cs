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
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace SnakeTail
{
    public partial class MainForm : Form
    {
        private static MainForm _instance = null;
        public static MainForm Instance { get { return _instance; } }

        private TailFileConfig _defaultTailConfig = null;

        public MainForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.SnakeIcon;
            _trayIcon.Icon = Properties.Resources.SnakeIcon; 
            _instance = this;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                LoadSession(args[1]);
            }
        }

        public void SetStatusBar(string text)
        {
            SetStatusBar(text, 0, 0);
        }

        public void SetStatusBar(string text, int progressValue, int progressMax)
        {
            _statusProgressBar.Maximum = progressMax;
            _statusProgressBar.Increment(progressValue - _statusProgressBar.Value);
            if (progressMax == 0 && progressValue == 0 && text==null)
                _statusProgressBar.Visible = false;
            else
                _statusProgressBar.Visible = true;

            if (text == null)
                _statusTextBar.Text = "Ready";
            else
                _statusTextBar.Text = text;

            _statusStrip.Invalidate();
            _statusStrip.Update();
        }

        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            // If no any child form, hide tabControl 
            if (this.ActiveMdiChild == null)
            {
                if (_MDITabControl.TabCount==0)
                    _MDITabControl.Visible = false;
            }
            else
            {
                // If child form is new and no has tabPage, create new tabPage 
                if (this.ActiveMdiChild.Tag == null)
                {
                    // Add a tabPage to tabControl with child form caption
                    AddMdiChildTab(this.ActiveMdiChild);

                    if (MdiChildren.Length > 1 && _MDITabControl.Visible == false)
                        return;

                    // Child form always maximized
                    this.ActiveMdiChild.WindowState = FormWindowState.Maximized;

                    _MDITabControl.SelectedTab = this.ActiveMdiChild.Tag as TabPage;
                }
                else
                {
                    if (_MDITabControl.Visible == false)
                        return;

                    TabPage tp = this.ActiveMdiChild.Tag as TabPage;
                    if (tp != null)
                    {
                        // Child form always maximized
                        this.ActiveMdiChild.WindowState = FormWindowState.Maximized;

                        _MDITabControl.SelectedTab = tp;
                    }
                }

                if (!_MDITabControl.Visible)
                    _MDITabControl.Visible = true;
            }
        }

        void AddMdiChildTab(Form mdiChild)
        {
            TabPage tp = new TabPage(mdiChild.Text);
            tp.Tag = mdiChild;
            tp.Parent = _MDITabControl;
            //AddOwnedForm(mdiChild);
            mdiChild.Tag = tp;
            mdiChild.FormClosed += new FormClosedEventHandler(ActiveMdiChild_FormClosed);
            mdiChild.SizeChanged += new EventHandler(ActiveMdiChild_SizeChanged);
            mdiChild.Shown += new EventHandler(ActiveMdiChild_Shown);
        }

        void ActiveMdiChild_Shown(object sender, EventArgs e)
        {
            // Fix the icon when starting MDI child in maximized state
            if ((sender as Form).WindowState == FormWindowState.Maximized)
            {
                ActivateMdiChild(null);
                ActivateMdiChild((sender as Form));
            }
        }

        void ActiveMdiChild_SizeChanged(object sender, EventArgs e)
        {
            // Disable tab-mode if the active MDI child changes WindowState
            if (this.ActiveMdiChild == sender && this.ActiveMdiChild.WindowState != FormWindowState.Maximized)
            {
                // Check if we are about to open / close a window
                if (MdiChildren.Length == _MDITabControl.TabCount)
                {
                    if (_MDITabControl.SelectedTab == null || this.ActiveMdiChild == _MDITabControl.SelectedTab.Tag)
                    {
                        _MDITabControl.Visible = false;
                    }
                }
            }
        }

        private void ActiveMdiChild_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((sender as Form).Tag as TabPage).Dispose();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "Open Log File";
            fileDialog.Filter = "Text Files|*.txt|Log Files|*.log|All Files|*.*";
            if (fileDialog.ShowDialog() != DialogResult.OK)
                return;

            OpenFileSelection(fileDialog.FileNames);
        }

        private void OpenFileSelection(string[] filenames)
        {
            if (_defaultTailConfig == null)
            {
                // Attempt to load default session configuration from these locations
                // 1. SnakeTail.xml in application directory
                // 2. SnakeTail.xml in current user roaming app directory
                // 3. SnakeTail.xml in current user local app directory
                // 4. SnakeTail.xml in common app directory
                string appPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\SnakeTail.xml";
                string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SnakeTail\\SnakeTail.xml";
                string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SnakeTail\\SnakeTail.xml";
                string commonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SnakeTail\\SnakeTail.xml";
                TailConfig tailConfig = null;
                if (File.Exists(appPath))
                    tailConfig = LoadSessionFile(appPath);
                else
                if (File.Exists(roamingPath))
                    tailConfig = LoadSessionFile(roamingPath);
                else
                if (File.Exists(localPath))
                    tailConfig = LoadSessionFile(localPath);
                else
                if (File.Exists(commonPath))
                    tailConfig = LoadSessionFile(commonPath);

                if (tailConfig != null && tailConfig.TailFiles.Count > 0)
                {
                    _defaultTailConfig = tailConfig.TailFiles[0];
                    _defaultTailConfig.Title = null;
                }
                else
                {
                    _defaultTailConfig = new TailFileConfig();
                }
            }

            foreach (string filename in filenames)
            {
                TailForm mdiForm = new TailForm();
                TailFileConfig tailConfig = _defaultTailConfig;
                tailConfig.FilePath = filename;
                mdiForm.LoadConfig(tailConfig, "");
                mdiForm.MdiParent = this;
                mdiForm.Show();
                Application.DoEvents();
            }
        }

        private void openEventLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenEventLogDialog openEventLogDlg = new OpenEventLogDialog();
            if (openEventLogDlg.ShowDialog() != DialogResult.OK)
                return;

            EventLogForm mdiForm = new EventLogForm();
            mdiForm.MdiParent = this;
            mdiForm.LoadFile(openEventLogDlg.EventLogFile);
            if (!mdiForm.IsDisposed)
                mdiForm.Show();
        }

        private void _MDITabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((_MDITabControl.SelectedTab != null) && (_MDITabControl.SelectedTab.Tag != null))
            {
                // Minize flicker when switching between tabs, by changing to minimized state first
                if ((_MDITabControl.SelectedTab.Tag as Form).WindowState != FormWindowState.Maximized)
                    (_MDITabControl.SelectedTab.Tag as Form).WindowState = FormWindowState.Minimized;
                (_MDITabControl.SelectedTab.Tag as Form).Select();
            }
        }

        private void _MDITabControl_MouseClick(object sender, MouseEventArgs e)
        {
            var tabControl = sender as TabControl;
            TabPage tabPageCurrent = null;
            if (e.Button == MouseButtons.Middle)
            {
                for (var i = 0; i < tabControl.TabCount; i++)
                {
                    if (!tabControl.GetTabRect(i).Contains(e.Location))
                        continue;
                    tabPageCurrent = tabControl.TabPages[i];
                    break;
                }
                if (tabPageCurrent != null)
                    (tabPageCurrent.Tag as Form).Close();
            }
        }

        private void cascadeWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void tileWindowsHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void tileWindowsVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void minimizeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form activeChild = ActiveMdiChild;
            foreach (Form childForm in MdiChildren)
            {
                if (childForm.WindowState != FormWindowState.Minimized)
                    childForm.WindowState = FormWindowState.Minimized;
            }
            if (activeChild != null && activeChild != ActiveMdiChild)
                activeChild.Select();
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _MDITabControl.Visible = false;
            FormCollection forms = Application.OpenForms;
            for (int i = forms.Count - 1; i >= 0; i--)
            {
                TailForm tailForm = forms[i] as TailForm;
                if (tailForm != null)
                    tailForm.Close();
                else
                {
                    EventLogForm eventLogForm = forms[i] as EventLogForm;
                    if (eventLogForm != null)
                        eventLogForm.Close();
                }
            }
            if (SearchForm.Instance.Visible)
                SearchForm.Instance.Close();
        }

        private void enableTabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_MDITabControl.Visible)
            {
                _MDITabControl.Visible = false;
            }
            else
            if (this.ActiveMdiChild != null)
            {
                this.ActiveMdiChild.WindowState = FormWindowState.Maximized;
                _MDITabControl.Visible = true;
                _MDITabControl.SelectedTab = this.ActiveMdiChild.Tag as TabPage;
            }
        }

        private void saveSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveSession(saveFileDialog.FileName);
            }
        }

        private void loadSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadSession(openFileDialog.FileName);
            }
        }

        private void SaveSession(string filepath)
        {
            TailConfig tailConfig = new TailConfig();
            if (_MDITabControl.Visible)
                tailConfig.SelectedTab = _MDITabControl.SelectedIndex;
            else
                tailConfig.SelectedTab = -1;
            tailConfig.WindowSize = Size;
            tailConfig.WindowPosition = DesktopLocation;
            tailConfig.MinimizedToTray = _trayIcon.Visible;

            foreach (Form childForm in Application.OpenForms)
            {
                TailForm tailForm = childForm as TailForm;
                if (tailForm != null)
                {
                    TailFileConfig tailFile = new TailFileConfig();
                    tailForm.SaveConfig(tailFile);
                    tailConfig.TailFiles.Add(tailFile);
                }
                EventLogForm eventlogForm = childForm as EventLogForm;
                if (eventlogForm != null)
                {
                    TailFileConfig tailFile = new TailFileConfig();
                    eventlogForm.SaveConfig(tailFile);
                    tailConfig.TailFiles.Add(tailFile);
                }
            }

            XmlSerializer serializer = new XmlSerializer(typeof(TailConfig));
            using (XmlTextWriter writer = new XmlTextWriter(filepath, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");
                serializer.Serialize(writer, tailConfig, xmlnsEmpty);
            }
        }

        private TailConfig LoadSessionFile(string filepath)
        {
            TailConfig tailConfig = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TailConfig));
                using (XmlTextReader reader = new XmlTextReader(filepath))
                {
                    filepath = new Uri(reader.BaseURI).LocalPath;
                    tailConfig = serializer.Deserialize(reader) as TailConfig;
                }
                return tailConfig;
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    errorMsg += "\n" + ex.Message;
                }
                MessageBox.Show("Failed to open session xml file, please ensure it is valid file:\n\n   " + filepath + "\n\n" + errorMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void LoadSession(string filepath)
        {
            TailConfig tailConfig = LoadSessionFile(filepath);
            if (tailConfig != null)
            {
                if (!tailConfig.MinimizedToTray)
                {
                    Size = tailConfig.WindowSize;
                    DesktopLocation = tailConfig.WindowPosition;
                }

                List<string> eventLogFiles = EventLogForm.GetEventLogFiles();

                Application.DoEvents();

                foreach (TailFileConfig tailFile in tailConfig.TailFiles)
                {
                    int index = eventLogFiles.FindIndex(delegate(string arrItem) { return arrItem.Equals(tailFile.FilePath); });
                    if (index >= 0)
                    {
                        EventLogForm mdiForm = new EventLogForm();
                        mdiForm.Text = tailFile.Title;
                        if (!tailFile.Modeless)
                        {
                            mdiForm.MdiParent = this;
                            mdiForm.ShowInTaskbar = false;
                            AddMdiChildTab(mdiForm);
                            mdiForm.LoadConfig(tailFile);
                            if (mdiForm.IsDisposed)
                                continue;
                        }
                        mdiForm.Show();

                        if (tailConfig.SelectedTab == -1 || tailFile.Modeless)
                        {
                            if (tailFile.WindowState != FormWindowState.Maximized)
                            {
                                mdiForm.DesktopLocation = tailFile.WindowPosition;
                                mdiForm.Size = tailFile.WindowSize;
                            }
                            if (mdiForm.WindowState != tailFile.WindowState)
                                mdiForm.WindowState = tailFile.WindowState;
                        }

                        if (tailFile.Modeless)
                            mdiForm.LoadConfig(tailFile);
                    }
                    else
                    {
                        TailForm mdiForm = new TailForm();
                        mdiForm.Text = tailFile.Title;
                        if (!tailFile.Modeless)
                        {
                            mdiForm.MdiParent = this;
                            mdiForm.ShowInTaskbar = false;
                            AddMdiChildTab(mdiForm);
                            mdiForm.LoadConfig(tailFile, Path.GetDirectoryName(filepath));
                        }
                        mdiForm.Show();

                        if (tailConfig.SelectedTab == -1 || tailFile.Modeless)
                        {
                            if (tailFile.WindowState != FormWindowState.Maximized)
                            {
                                mdiForm.DesktopLocation = tailFile.WindowPosition;
                                mdiForm.Size = tailFile.WindowSize;
                            }
                            if (mdiForm.WindowState != tailFile.WindowState)
                                mdiForm.WindowState = tailFile.WindowState;
                        }

                        if (tailFile.Modeless)
                            mdiForm.LoadConfig(tailFile, Path.GetDirectoryName(filepath));
                    }
                    Application.DoEvents();
                }

                if (tailConfig.SelectedTab != -1)
                {
                    foreach (Form childForm in MdiChildren)
                        childForm.WindowState = FormWindowState.Minimized;

                    _MDITabControl.SelectedIndex = tailConfig.SelectedTab;
                    _MDITabControl.Visible = true;
                    (_MDITabControl.SelectedTab.Tag as Form).WindowState = FormWindowState.Maximized;
                }

                if (tailConfig.MinimizedToTray)
                {
                    _trayIcon.Visible = true;
                    WindowState = FormWindowState.Minimized;
                    Visible = false;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void windowToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem menuItem = e.ClickedItem as ToolStripMenuItem;
            if (menuItem != null && menuItem.IsMdiWindowListEntry)
            {
                // If a minimized window is chosen from the list, then it is restored to normal state
                this.windowToolStripMenuItem.DropDownItemClicked -= windowToolStripMenuItem_DropDownItemClicked;
                e.ClickedItem.PerformClick();
                if (ActiveMdiChild != null && ActiveMdiChild.WindowState == FormWindowState.Minimized)
                    ActiveMdiChild.WindowState = FormWindowState.Normal;
                this.windowToolStripMenuItem.DropDownItemClicked += windowToolStripMenuItem_DropDownItemClicked;
            }
        }

        private void minimizeToTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_trayIcon.Visible)
            {
                _trayIcon.Visible = true;
                WindowState = FormWindowState.Minimized;
                Visible = false;
                minimizeToTrayToolStripMenuItem.Checked = true;
                _trayIcon.ShowBalloonTip(3, "Minimized to tray", "Double click the system tray icon to restore window", ToolTipIcon.Info);
            }
            else
            {
                Visible = true;
                WindowState = FormWindowState.Normal;
                _trayIcon.Visible = false;
                minimizeToTrayToolStripMenuItem.Checked = false;
            }
        }

        private void _trayIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
         }

        private void windowToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            enableTabsToolStripMenuItem.Checked = _MDITabControl.Visible;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (HourGlass hourglass = new HourGlass(this))
                using (WebClient client = new WebClient())
                {
                    string value = client.DownloadString("http://snakenest.com/snaketail.pad.xml");
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(value);
                    XmlNode appVerNode = xmlDoc.SelectSingleNode("/XML_DIZ_INFO/Program_Info/Program_Version");
                    if (appVerNode != null)
                    {
                        Version appVer = new Version(appVerNode.InnerText);
                        if (appVer > System.Reflection.Assembly.GetExecutingAssembly().GetName().Version)
                        {
                            DialogResult res = MessageBox.Show("New version " + appVer.ToString() + " available\n\nCheck homepage for changelog and download?", "New update available", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (res == DialogResult.OK)
                            {
                                System.Diagnostics.Process.Start("http://snakenest.com/snaketail/");
                                return;
                            }
                        }
                    }
                }
                MessageBox.Show("Using latest version", "Check for updates", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ThreadExceptionDialog dlg = new ThreadExceptionDialog(ex);
                dlg.Text = "Error checking for new updates";
                dlg.ShowDialog();
            }
        }

        private void _trayIconContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // We steal the items from the main menu (we restore them when closing again)
            ToolStripItem[] items = new ToolStripItem[fileToolStripMenuItem.DropDownItems.Count];
            fileToolStripMenuItem.DropDownItems.CopyTo(items, 0);
            _trayIconContextMenuStrip.Items.Clear();            // Clear the dummy item
            _trayIconContextMenuStrip.Items.AddRange(items);
            minimizeToTrayToolStripMenuItem.Checked = true;
            minimizeToTrayToolStripMenuItem.Font = new Font(minimizeToTrayToolStripMenuItem.Font, FontStyle.Bold);
        }

        private void _trayIconContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            // Restore the items back to the main menu when closing
            ToolStripItem[] items = new ToolStripItem[_trayIconContextMenuStrip.Items.Count];
            _trayIconContextMenuStrip.Items.CopyTo(items, 0);
            fileToolStripMenuItem.DropDownItems.AddRange(items);
            _trayIconContextMenuStrip.Items.Clear();
            _trayIconContextMenuStrip.Items.Add(new ToolStripSeparator());  // Dummy item so menu is shown the next time
            minimizeToTrayToolStripMenuItem.Checked = false;
            minimizeToTrayToolStripMenuItem.Font = new Font(minimizeToTrayToolStripMenuItem.Font, FontStyle.Regular);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array array = e.Data.GetData(DataFormats.FileDrop) as Array;
                if (array == null)
                    return;

                // Extract strings from array
                List<string> filenames = new List<string>();
                foreach(object filename in array)
                {
                    filenames.Add(filename.ToString());
                }

                // Call OpenFile asynchronously.
                // Explorer instance from which file is dropped is not responding
                // all the time when DragDrop handler is active, so we need to return
                // immidiately (especially if OpenFile shows MessageBox).
                this.BeginInvoke(new Action<string[]>(OpenFileSelection), new object[] { filenames.ToArray() });
                this.Activate();        // in the case Explorer overlaps this form
            }
            catch (Exception)
            {
                // don't show MessageBox here - Explorer is waiting !
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (_trayIcon.Visible && WindowState == FormWindowState.Minimized)
                Visible = false;
        }
    }
}
