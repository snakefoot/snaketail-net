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
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventLogForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._switchWindowModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._addFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._resetFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._filterActiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._eventImageList = new System.Windows.Forms.ImageList(this.components);
            this._eventMessageText = new System.Windows.Forms.RichTextBox();
            this._filterEventLogTimer = new System.Windows.Forms.Timer(this.components);
            this._eventListView = new SnakeTail.EventLogListView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this._contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
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
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._eventMessageText);
            this.splitContainer1.Size = new System.Drawing.Size(602, 318);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 2;
            // 
            // _contextMenuStrip
            // 
            this._contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._switchWindowModeToolStripMenuItem,
            this._addFilterToolStripMenuItem,
            this._resetFilterToolStripMenuItem,
            this._filterActiveToolStripMenuItem});
            this._contextMenuStrip.Name = "_contextMenuStrip";
            this._contextMenuStrip.Size = new System.Drawing.Size(189, 92);
            this._contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._contextMenuStrip_Opening);
            // 
            // _switchWindowModeToolStripMenuItem
            // 
            this._switchWindowModeToolStripMenuItem.Name = "_switchWindowModeToolStripMenuItem";
            this._switchWindowModeToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this._switchWindowModeToolStripMenuItem.Text = "Switch window mode";
            this._switchWindowModeToolStripMenuItem.Click += new System.EventHandler(this.switchWindowModeToolStripMenuItem_Click);
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
            // _filterActiveToolStripMenuItem
            // 
            this._filterActiveToolStripMenuItem.Name = "_filterActiveToolStripMenuItem";
            this._filterActiveToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this._filterActiveToolStripMenuItem.Text = "Filter Mode";
            this._filterActiveToolStripMenuItem.Click += new System.EventHandler(this._filterActiveToolStripMenuItem_Click);
            // 
            // _eventImageList
            // 
            this._eventImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_eventImageList.ImageStream")));
            this._eventImageList.TransparentColor = System.Drawing.Color.Transparent;
            this._eventImageList.Images.SetKeyName(0, "Error.png");
            this._eventImageList.Images.SetKeyName(1, "Warning.png");
            this._eventImageList.Images.SetKeyName(2, "Info.png");
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
            this._eventListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this._eventListView_RetrieveVirtualItem);
            this._eventListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this._eventListView_ItemSelectionChanged);
            // 
            // EventLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 318);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EventLogForm";
            this.Text = "EventLog";
            this.Load += new System.EventHandler(this.EventLogForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EventLogForm_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this._contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private EventLogListView _eventListView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox _eventMessageText;
        private System.Windows.Forms.ImageList _eventImageList;
        private System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _switchWindowModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _addFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _filterActiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _resetFilterToolStripMenuItem;
        private System.Windows.Forms.Timer _filterEventLogTimer;
    }
}