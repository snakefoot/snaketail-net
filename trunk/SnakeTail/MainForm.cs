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

        public string CurrenTailConfig { get { return _currenTailConfig != null ? _currenTailConfig : ""; } }

        private TailFileConfig _defaultTailConfig = null;
        private string _currenTailConfig = null;

        private string _mruRegKey = "SOFTWARE\\SnakeNest.com\\SnakeTail\\MRU";
        private JWC.MruStripMenu _mruMenu;

        public MainForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.SnakeIcon;
            _trayIcon.Icon = Properties.Resources.SnakeIcon; 
            _instance = this;

            _MDITabControl.ImageList = new ImageList();
            _MDITabControl.ImageList.ImageSize = new System.Drawing.Size(16, 16);
            _MDITabControl.ImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            _MDITabControl.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            _MDITabControl.ImageList.Images.Add(new Bitmap(Properties.Resources.GreenBulletIcon.ToBitmap()));
            _MDITabControl.ImageList.Images.Add(new Bitmap(Properties.Resources.YellowBulletIcon.ToBitmap()));

            bool loadFromRegistry = false;
            try
            {
                Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(_mruRegKey);
                if (regKey != null)
                    loadFromRegistry = true;
            }
            catch
            {
            }

            saveRecentFilesToRegistryToolStripMenuItem.Checked = loadFromRegistry;
            _mruMenu = new JWC.MruStripMenuInline(recentFilesToolStripMenuItem, recentFile1ToolStripMenuItem, new JWC.MruStripMenu.ClickedHandler(OnMruFile), _mruRegKey, loadFromRegistry, 10);
        }

        private void UpdateTitle()
        {
            Text = Application.ProductName;
            if (_currenTailConfig != null)
                Text += " - " + Path.GetFileNameWithoutExtension(_currenTailConfig);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                int filesOpened = 0;
                for (int i = 1; i < args.Length; ++i)
                {
                    if (args[1].EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (LoadSession(args[1]))
                            ++filesOpened;
                    }
                    else
                    {
                        filesOpened += OpenFileSelection(new string[] { args[i] });
                    }
                    if (filesOpened == 0 && i >= 2)
                        break;  // Stop attempting to open all arguements if the first two fails
                }
            }
        }

        public void SetStatusBar(string text)
        {
            SetStatusBar(text, 0, 0);
        }

        public void SetStatusBar(string text, int progressValue, int progressMax)
        {
            _statusProgressBar.Maximum = progressMax;
            _statusProgressBar.Value = progressValue;
            if (progressMax == 0 && progressValue == 0)
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
            fileDialog.Filter = "Default Filter|*.txt;*.text;*.log*;*.xlog|Log Files|*.log*;*.xlog|Text Files|*.txt;*.text|All Files|*.*";
            if (fileDialog.ShowDialog() != DialogResult.OK)
                return;

            OpenFileSelection(fileDialog.FileNames);
        }

        private void OnMruFile(int number, String filename)
        {
            bool openedFile = false;
            if (filename.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
                openedFile = LoadSession(filename);
            else
                openedFile = OpenFileSelection(new string[] { filename }) == 1;

            if (!openedFile)
            {
                MessageBox.Show("The file '" + filename + "'cannot be opened and will be removed from the Recent list(s)", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _mruMenu.RemoveFile(number);
            }
        }

        private string GetDefaultConfigPath()
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
            if (File.Exists(appPath))
                return appPath;
            else if (File.Exists(roamingPath))
                return roamingPath;
            else if (File.Exists(localPath))
                return localPath;
            else if (File.Exists(commonPath))
                return commonPath;
            else
                return string.Empty;
        }

        private int OpenFileSelection(string[] filenames)
        {
            if (_defaultTailConfig == null)
            {
                TailConfig tailConfig = null;
                string defaultPath = GetDefaultConfigPath();
                if (!string.IsNullOrEmpty(defaultPath))
                    tailConfig = LoadSessionFile(defaultPath);

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

            int filesOpened = 0;
            foreach (string filename in filenames)
            {
                string configPath = "";
                try
                {
                    if (string.IsNullOrEmpty(Path.GetDirectoryName(filename)))
                        configPath = Directory.GetCurrentDirectory();
                }
                catch
                {
                }

                TailForm mdiForm = new TailForm();
                TailFileConfig tailConfig = _defaultTailConfig;
                tailConfig.FilePath = filename;
                mdiForm.LoadConfig(tailConfig, configPath);
                if (mdiForm.IsDisposed)
                    continue;
                
                _mruMenu.AddFile(filename);

                mdiForm.MdiParent = this;
                mdiForm.Show();
                ++filesOpened;
                Application.DoEvents();
            }
            return filesOpened;
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
                SuspendLayout();
                (_MDITabControl.SelectedTab.Tag as Form).SuspendLayout();
                Form activeMdiChild = this.ActiveMdiChild;
                if (activeMdiChild != null)
                    activeMdiChild.SuspendLayout();
                // Minimize flicker when switching between tabs, by changing to minimized state first
                if ((_MDITabControl.SelectedTab.Tag as Form).WindowState != FormWindowState.Maximized)
                    (_MDITabControl.SelectedTab.Tag as Form).WindowState = FormWindowState.Minimized;
                (_MDITabControl.SelectedTab.Tag as Form).Select();
                if (activeMdiChild != null && !activeMdiChild.IsDisposed)
                    activeMdiChild.ResumeLayout();
                (_MDITabControl.SelectedTab.Tag as Form).ResumeLayout();
                ResumeLayout();
                (_MDITabControl.SelectedTab.Tag as Form).Refresh();
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
                ITailForm tailForm = forms[i] as ITailForm;
                if (tailForm != null)
                    tailForm.TailWindow.Close();
            }
            if (SearchForm.Instance.Visible)
                SearchForm.Instance.Close();
            _currenTailConfig = null;
            UpdateTitle();
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
            if (!String.IsNullOrEmpty(_currenTailConfig))
            {
                saveFileDialog.FileName = Path.GetFileName(_currenTailConfig);
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(_currenTailConfig);
            }
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

            List<Form> childForms = new List<Form>();

            // We first loop through the tabpages to store in proper TabPage order
            foreach (TabPage tagPage in _MDITabControl.TabPages)
            {
                Form tailForm = tagPage.Tag as Form;
                if (tailForm != null)
                    childForms.Add(tailForm);
            }

            // Then we loop through all forms (includes free floating)
            foreach (Form childForm in Application.OpenForms)
            {
                if (childForms.IndexOf(childForm) == -1)
                    childForms.Add(childForm);
            }

            // Save all forms and store in proper order
            foreach (Form childForm in childForms)
            {
                ITailForm tailForm = childForm as ITailForm;
                if (tailForm != null)
                {
                    TailFileConfig tailFile = new TailFileConfig();
                    tailForm.SaveConfig(tailFile);
                    tailConfig.TailFiles.Add(tailFile);
                }
            }

            SaveConfig(tailConfig, filepath);

            if (String.IsNullOrEmpty(_currenTailConfig))
                _mruMenu.AddFile(filepath);
            else if (_currenTailConfig != filepath)
                _mruMenu.RenameFile(_currenTailConfig, filepath);

            _currenTailConfig = filepath;

            UpdateTitle();
        }

        public void SaveConfig(TailConfig tailConfig, string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                string defaultPath = GetDefaultConfigPath();
                if (string.IsNullOrEmpty(defaultPath))
                {
                    defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SnakeTail\\";
                    if (!Directory.Exists(defaultPath))
                        Directory.CreateDirectory(defaultPath);

                    defaultPath += "SnakeTail.xml";
                }

                filepath = defaultPath;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(TailConfig));
            using (XmlTextWriter writer = new XmlTextWriter(filepath, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");
                serializer.Serialize(writer, tailConfig, xmlnsEmpty);
            }

            _defaultTailConfig = null;  // Force reload incase we saved a new default config
        }

        private TailConfig LoadSessionFile(string filepath)
        {
            TailConfig tailConfig = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TailConfig));
                using (XmlTextReader reader = new XmlTextReader(filepath))
                {
                    _currenTailConfig = new Uri(reader.BaseURI).LocalPath;
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

        private bool LoadSession(string filepath)
        {
            TailConfig tailConfig = LoadSessionFile(filepath);
            if (tailConfig == null)
                return false;

            _mruMenu.AddFile(filepath);

            if (!tailConfig.MinimizedToTray)
            {
                Size = tailConfig.WindowSize;
                DesktopLocation = tailConfig.WindowPosition;
            }

            UpdateTitle();

            List<string> eventLogFiles = EventLogForm.GetEventLogFiles();

            Application.DoEvents();

            foreach (TailFileConfig tailFile in tailConfig.TailFiles)
            {
                Form mdiForm = null;

                int index = eventLogFiles.FindIndex(delegate(string arrItem) { return arrItem.Equals(tailFile.FilePath); });
                if (index >= 0)
                    mdiForm = new EventLogForm();
                else
                    mdiForm = new TailForm();

                if (mdiForm != null)
                {
                    ITailForm tailForm = mdiForm as ITailForm;
                    string tailConfigPath = Path.GetDirectoryName(filepath);

                    mdiForm.Text = tailFile.Title;
                    if (!tailFile.Modeless)
                    {
                        mdiForm.MdiParent = this;
                        mdiForm.ShowInTaskbar = false;
                        AddMdiChildTab(mdiForm);
                        if (tailForm != null)
                            tailForm.LoadConfig(tailFile, tailConfigPath);
                        if (mdiForm.IsDisposed)
                        {
                            _MDITabControl.TabPages.Remove(mdiForm.Tag as TabPage);
                            continue;
                        }
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
                    {
                        if (tailForm != null)
                            tailForm.LoadConfig(tailFile, tailConfigPath);
                    }
                }
                Application.DoEvents();
            }

            if (tailConfig.SelectedTab != -1 && _MDITabControl.TabPages.Count > 0)
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

            return true;
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

        private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopMost = !TopMost;
            alwaysOnTopToolStripMenuItem.Checked = TopMost;
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
                using(new HourGlass(this))
                {
                    CheckForUpdates updateChecker = new CheckForUpdates();
                    updateChecker.PadUrl = Program.PadUrl;
                    updateChecker.PromptAlways = true;
                    updateChecker.SendReport(null);
                }
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

                this.Activate();        // in the case Explorer overlaps this form

                // Call OpenFile asynchronously.
                // Explorer instance from which file is dropped is not responding
                // all the time when DragDrop handler is active, so we need to return
                // immidiately (especially if OpenFile shows MessageBox).
                System.Threading.ThreadPool.QueueUserWorkItem(worker_DoWork, filenames.ToArray());
            }
            catch (Exception ex)
            {
                // don't show MessageBox here - Explorer is waiting !
                System.Diagnostics.Debug.WriteLine("Drag Drop Failed: " + ex.Message);
            }
        }

        void worker_DoWork(object param)
        {
            // Discovered a strange problem where the Windows Explorer would lock, eventhough I deferred the actual DragDrop operation using BeginInvoke().
            // The solution was to create a thread, that slept for 100 ms and then invoked the wanted method. If I removed the sleep from the new thread,
            // then Windows Explorer would lock again. Very strange indeed. 
            System.Threading.Thread.Sleep(100);
            this.BeginInvoke(new Action<string[]>(delegate(string[] filenames) { OpenFileSelection(filenames); }), new object[] { param });
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (_trayIcon.Visible && WindowState == FormWindowState.Minimized)
                Visible = false;
        }

        private void _MDITabControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _MDITabControl.AllowDrop = true;
                _MDITabControl.DoDragDrop(_MDITabControl.SelectedTab, DragDropEffects.All);
                _MDITabControl.AllowDrop = false;
            }
        }

        private void _MDITabControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TabPage)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void _MDITabControl_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = _MDITabControl.PointToClient(new Point(e.X, e.Y));
            for(int i = 0; i < _MDITabControl.TabPages.Count; ++i)
            {
                if (_MDITabControl.GetTabRect(i).Contains(clientPoint))
                {
                    if (_MDITabControl.TabPages[i] == _MDITabControl.SelectedTab)
                        break;  // No change

                    TabPage tabPage = _MDITabControl.SelectedTab;
                    _MDITabControl.TabPages.Remove(tabPage);
                    _MDITabControl.TabPages.Insert(i, tabPage);
                    _MDITabControl.SelectedIndex = i;
                    break;
                }
            }
        }

        private void clearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mruMenu.RemoveAll();
        }

        private void saveRecentFilesToRegistryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveRecentFilesToRegistryToolStripMenuItem.Checked)
            {
                try
                {
                    Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser;
                    regKey.DeleteSubKey(_mruRegKey, false);
                    saveRecentFilesToRegistryToolStripMenuItem.Checked = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to remove list of recently used files from registry.\n\n" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                saveRecentFilesToRegistryToolStripMenuItem.Checked = true;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _instance = null;
                if (saveRecentFilesToRegistryToolStripMenuItem.Checked)
                    _mruMenu.SaveToRegistry();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to save list of recently used files to registry.\n\n" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
