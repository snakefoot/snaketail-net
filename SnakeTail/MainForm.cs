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
            fileDialog.Title = "Open Log File";
            fileDialog.Filter = "Text Files|*.txt|Log Files|*.log|All Files|*.*";
            if (fileDialog.ShowDialog() != DialogResult.OK)
                return;

            TailForm mdiForm = new TailForm();
            mdiForm.LoadFile(fileDialog.FileName);
            mdiForm.MdiParent = this;
            mdiForm.Show();
        }

        private void openEventLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenEventLogDialog openEventLogDlg = new OpenEventLogDialog();
            if (openEventLogDlg.ShowDialog() != DialogResult.OK)
                return;

            EventLogForm mdiForm = new EventLogForm();
            mdiForm.MdiParent = this;
            mdiForm.LoadFile(openEventLogDlg.EventLogFile);
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

        private void LoadSession(string filepath)
        {
            TailConfig tailConfig = null;
            XmlSerializer serializer = new XmlSerializer(typeof(TailConfig));
            using (XmlTextReader reader = new XmlTextReader(filepath))
            {
                filepath = new Uri(reader.BaseURI).LocalPath;
                tailConfig = serializer.Deserialize(reader) as TailConfig;
            }
            if (tailConfig != null)
            {
                if (tailConfig.MinimizedToTray)
                {
                    _trayIcon.Visible = true;
                    ShowInTaskbar = false;
                    Visible = false;
                }
                else
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
            _trayIcon.Visible = true;
            WindowState = FormWindowState.Minimized;
            Hide();
        }

        private void _trayIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            _trayIcon.Visible = false;
        }
    }
}
