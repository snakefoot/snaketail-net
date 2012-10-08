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

namespace SnakeTail
{
    partial class EventLogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
            // base.Dispose() will close the window and cause resizing, must wait with stream dispose
            if (disposing && (_messageLookup != null))
            {
                _messageLookup.Dispose();
            }
            if (disposing && (_eventLog != null))
            {
                _eventLog.Dispose();
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventLogForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._eventListView = new SnakeTail.EventLogListView();
            this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._eventImageList = new System.Windows.Forms.ImageList(this.components);
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this._activeWindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._switchModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._configTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._addFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._resetFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._filterModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._eventMessageText = new System.Windows.Forms.RichTextBox();
            this._filterEventLogTimer = new System.Windows.Forms.Timer(this.components);
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this._contextMenuStrip.SuspendLayout();
            this._menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(57, 6);
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(185, 6);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(185, 6);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._eventListView);
            this.splitContainer1.Panel1.Controls.Add(this._menuStrip);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._eventMessageText);
            this.splitContainer1.Size = new System.Drawing.Size(602, 318);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 2;
            // 
            // _eventListView
            // 
            this._eventListView.ContextMenuStrip = this._contextMenuStrip;
            this._eventListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eventListView.FullRowSelect = true;
            this._eventListView.HideSelection = false;
            this._eventListView.Location = new System.Drawing.Point(0, 0);
            this._eventListView.Name = "_eventListView";
            this._eventListView.Size = new System.Drawing.Size(602, 239);
            this._eventListView.SmallImageList = this._eventImageList;
            this._eventListView.TabIndex = 1;
            this._eventListView.UseCompatibleStateImageBehavior = false;
            this._eventListView.View = System.Windows.Forms.View.Details;
            this._eventListView.VirtualMode = true;
            this._eventListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this._eventListView_ItemSelectionChanged);
            this._eventListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this._eventListView_RetrieveVirtualItem);
            // 
            // _contextMenuStrip
            // 
            this._contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripSeparator1});
            this._contextMenuStrip.Name = "_contextMenuStrip";
            this._contextMenuStrip.Size = new System.Drawing.Size(61, 10);
            this._contextMenuStrip.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this._contextMenuStrip_Closed);
            this._contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._contextMenuStrip_Opening);
            // 
            // _eventImageList
            // 
            this._eventImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_eventImageList.ImageStream")));
            this._eventImageList.TransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this._eventImageList.Images.SetKeyName(0, "Error.png");
            this._eventImageList.Images.SetKeyName(1, "Warning.png");
            this._eventImageList.Images.SetKeyName(2, "Info.png");
            // 
            // _menuStrip
            // 
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._activeWindowMenuItem});
            this._menuStrip.Location = new System.Drawing.Point(0, 0);
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.Size = new System.Drawing.Size(602, 24);
            this._menuStrip.TabIndex = 2;
            this._menuStrip.Text = "menuStrip1";
            this._menuStrip.Visible = false;
            // 
            // _activeWindowMenuItem
            // 
            this._activeWindowMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._copyToolStripMenuItem,
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem,
            toolStripSeparator2,
            this._switchModeToolStripMenuItem,
            this._configTextToolStripMenuItem,
            toolStripSeparator4,
            this._addFilterToolStripMenuItem,
            this._resetFilterToolStripMenuItem,
            this._filterModeToolStripMenuItem});
            this._activeWindowMenuItem.MergeAction = System.Windows.Forms.MergeAction.Replace;
            this._activeWindowMenuItem.MergeIndex = 1;
            this._activeWindowMenuItem.Name = "_activeWindowMenuItem";
            this._activeWindowMenuItem.Size = new System.Drawing.Size(39, 20);
            this._activeWindowMenuItem.Text = "Edit";
            this._activeWindowMenuItem.DropDownOpening += new System.EventHandler(this._contextMenuStrip_Opening);
            // 
            // _copyToolStripMenuItem
            // 
            this._copyToolStripMenuItem.Name = "_copyToolStripMenuItem";
            this._copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this._copyToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this._copyToolStripMenuItem.Text = "Copy";
            this._copyToolStripMenuItem.Click += new System.EventHandler(this._copyToolStripMenuItem_Click);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.findToolStripMenuItem.Text = "Find...";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.findNextToolStripMenuItem.Text = "Find Next";
            this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
            // 
            // _switchModeToolStripMenuItem
            // 
            this._switchModeToolStripMenuItem.Name = "_switchModeToolStripMenuItem";
            this._switchModeToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this._switchModeToolStripMenuItem.Text = "Switch window mode";
            this._switchModeToolStripMenuItem.Click += new System.EventHandler(this.switchWindowModeToolStripMenuItem_Click);
            // 
            // _configTextToolStripMenuItem
            // 
            this._configTextToolStripMenuItem.Name = "_configTextToolStripMenuItem";
            this._configTextToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this._configTextToolStripMenuItem.Text = "View Options...";
            this._configTextToolStripMenuItem.Click += new System.EventHandler(this._configTextToolStripMenuItem_Click);
            // 
            // _addFilterToolStripMenuItem
            // 
            this._addFilterToolStripMenuItem.Name = "_addFilterToolStripMenuItem";
            this._addFilterToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this._addFilterToolStripMenuItem.Text = "Add Filter";
            this._addFilterToolStripMenuItem.Click += new System.EventHandler(this._addFilterToolStripMenuItem_Click);
            // 
            // _resetFilterToolStripMenuItem
            // 
            this._resetFilterToolStripMenuItem.Name = "_resetFilterToolStripMenuItem";
            this._resetFilterToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this._resetFilterToolStripMenuItem.Text = "Reset Filter";
            this._resetFilterToolStripMenuItem.Click += new System.EventHandler(this.resetFilterToolStripMenuItem_Click);
            // 
            // _filterModeToolStripMenuItem
            // 
            this._filterModeToolStripMenuItem.Name = "_filterModeToolStripMenuItem";
            this._filterModeToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this._filterModeToolStripMenuItem.Text = "Filter Mode";
            this._filterModeToolStripMenuItem.Click += new System.EventHandler(this._filterActiveToolStripMenuItem_Click);
            // 
            // _eventMessageText
            // 
            this._eventMessageText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eventMessageText.Location = new System.Drawing.Point(0, 0);
            this._eventMessageText.Name = "_eventMessageText";
            this._eventMessageText.ReadOnly = true;
            this._eventMessageText.Size = new System.Drawing.Size(602, 75);
            this._eventMessageText.TabIndex = 0;
            this._eventMessageText.Text = "";
            // 
            // _filterEventLogTimer
            // 
            this._filterEventLogTimer.Tick += new System.EventHandler(this._filterEventLogTimer_Tick);
            // 
            // EventLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 318);
            this.Controls.Add(this.splitContainer1);
            this.MainMenuStrip = this._menuStrip;
            this.Name = "EventLogForm";
            this.Text = "EventLog";
            this.Activated += new System.EventHandler(this.EventLogForm_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EventLogForm_FormClosed);
            this.Load += new System.EventHandler(this.EventLogForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this._contextMenuStrip.ResumeLayout(false);
            this._menuStrip.ResumeLayout(false);
            this._menuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private EventLogListView _eventListView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox _eventMessageText;
        private System.Windows.Forms.ImageList _eventImageList;
        private System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
        private System.Windows.Forms.Timer _filterEventLogTimer;
        private System.Windows.Forms.MenuStrip _menuStrip;
        private System.Windows.Forms.ToolStripMenuItem _activeWindowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _switchModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _configTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _addFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _resetFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _filterModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
    }
}