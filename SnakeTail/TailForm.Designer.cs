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
    partial class TailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TailForm));
            this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.switchToModelessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.configureFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureBackgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.startServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._tailTimer = new System.Windows.Forms.Timer(this.components);
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._statusTextBar = new System.Windows.Forms.ToolStripStatusLabel();
            this._statusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this._tailListView = new SnakeTail.LogFileListView();
            this.hiddenItem = new System.Windows.Forms.ColumnHeader();
            this.lineItem = new System.Windows.Forms.ColumnHeader();
            this._contextMenuStrip.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _contextMenuStrip
            // 
            this._contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.switchToModelessToolStripMenuItem,
            this.toolStripSeparator2,
            this.configureFontToolStripMenuItem,
            this.configureBackgroundColorToolStripMenuItem,
            this.toolStripSeparator1,
            this.startServiceToolStripMenuItem,
            this.stopServiceToolStripMenuItem,
            this.pauseServiceToolStripMenuItem,
            this.cToolStripMenuItem});
            this._contextMenuStrip.Name = "contextMenuStrip1";
            this._contextMenuStrip.Size = new System.Drawing.Size(204, 170);
            this._contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._contextMenuStrip_Opening);
            // 
            // switchToModelessToolStripMenuItem
            // 
            this.switchToModelessToolStripMenuItem.Name = "switchToModelessToolStripMenuItem";
            this.switchToModelessToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.switchToModelessToolStripMenuItem.Text = "Switch window mode";
            this.switchToModelessToolStripMenuItem.Click += new System.EventHandler(this.switchToModelessToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(200, 6);
            // 
            // configureFontToolStripMenuItem
            // 
            this.configureFontToolStripMenuItem.Name = "configureFontToolStripMenuItem";
            this.configureFontToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.configureFontToolStripMenuItem.Text = "Configure text...";
            this.configureFontToolStripMenuItem.Click += new System.EventHandler(this.configureFontToolStripMenuItem_Click);
            // 
            // configureBackgroundColorToolStripMenuItem
            // 
            this.configureBackgroundColorToolStripMenuItem.Name = "configureBackgroundColorToolStripMenuItem";
            this.configureBackgroundColorToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.configureBackgroundColorToolStripMenuItem.Text = "Configure background...";
            this.configureBackgroundColorToolStripMenuItem.Click += new System.EventHandler(this.configureBackgroundColorToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
            // 
            // startServiceToolStripMenuItem
            // 
            this.startServiceToolStripMenuItem.Name = "startServiceToolStripMenuItem";
            this.startServiceToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.startServiceToolStripMenuItem.Text = "Start Service...";
            this.startServiceToolStripMenuItem.Click += new System.EventHandler(this.startServiceToolStripMenuItem_Click);
            // 
            // stopServiceToolStripMenuItem
            // 
            this.stopServiceToolStripMenuItem.Name = "stopServiceToolStripMenuItem";
            this.stopServiceToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.stopServiceToolStripMenuItem.Text = "Stop Service...";
            this.stopServiceToolStripMenuItem.Click += new System.EventHandler(this.stopServiceToolStripMenuItem_Click);
            // 
            // pauseServiceToolStripMenuItem
            // 
            this.pauseServiceToolStripMenuItem.Name = "pauseServiceToolStripMenuItem";
            this.pauseServiceToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.pauseServiceToolStripMenuItem.Text = "Pause Service...";
            this.pauseServiceToolStripMenuItem.Click += new System.EventHandler(this.pauseServiceToolStripMenuItem_Click);
            // 
            // cToolStripMenuItem
            // 
            this.cToolStripMenuItem.Name = "cToolStripMenuItem";
            this.cToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.cToolStripMenuItem.Text = "Continue Service...";
            this.cToolStripMenuItem.Click += new System.EventHandler(this.cToolStripMenuItem_Click);
            // 
            // _tailTimer
            // 
            this._tailTimer.Interval = 50;
            this._tailTimer.Tick += new System.EventHandler(this._tailTimer_Tick);
            // 
            // _statusStrip
            // 
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusTextBar,
            this._statusProgressBar});
            this._statusStrip.Location = new System.Drawing.Point(0, 257);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.Size = new System.Drawing.Size(332, 22);
            this._statusStrip.TabIndex = 1;
            this._statusStrip.Text = "statusStrip1";
            this._statusStrip.Visible = false;
            // 
            // _statusTextBar
            // 
            this._statusTextBar.Name = "_statusTextBar";
            this._statusTextBar.Size = new System.Drawing.Size(215, 17);
            this._statusTextBar.Spring = true;
            this._statusTextBar.Text = "Ready";
            this._statusTextBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _statusProgressBar
            // 
            this._statusProgressBar.Name = "_statusProgressBar";
            this._statusProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // _tailListView
            // 
            this._tailListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hiddenItem,
            this.lineItem});
            this._tailListView.ContextMenuStrip = this._contextMenuStrip;
            this._tailListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tailListView.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tailListView.FullRowSelect = true;
            this._tailListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this._tailListView.HideSelection = false;
            this._tailListView.Location = new System.Drawing.Point(0, 0);
            this._tailListView.Margin = new System.Windows.Forms.Padding(0);
            this._tailListView.Name = "_tailListView";
            this._tailListView.OwnerDraw = true;
            this._tailListView.Size = new System.Drawing.Size(332, 279);
            this._tailListView.TabIndex = 0;
            this._tailListView.UseCompatibleStateImageBehavior = false;
            this._tailListView.View = System.Windows.Forms.View.Details;
            this._tailListView.VirtualMode = true;
            this._tailListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this._tailListView_DrawItem);
            this._tailListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this._tailListView_RetrieveVirtualItem);
            this._tailListView.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this._tailListView_CacheVirtualItems);
            this._tailListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this._tailListView_KeyDown);
            this._tailListView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this._tailListView_DrawSubItem);
            // 
            // hiddenItem
            // 
            this.hiddenItem.Width = 0;
            // 
            // TailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 279);
            this.Controls.Add(this._statusStrip);
            this.Controls.Add(this._tailListView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "TailForm";
            this.Text = "TailForm";
            this.Load += new System.EventHandler(this.TailForm_Load);
            this.Activated += new System.EventHandler(this.TailForm_Activated);
            this.Resize += new System.EventHandler(this.TailForm_Resize);
            this._contextMenuStrip.ResumeLayout(false);
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LogFileListView _tailListView;
        private System.Windows.Forms.Timer _tailTimer;
        private System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem switchToModelessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureBackgroundColorToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader hiddenItem;
        private System.Windows.Forms.ColumnHeader lineItem;
        private System.Windows.Forms.ToolStripMenuItem startServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem pauseServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cToolStripMenuItem;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripProgressBar _statusProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel _statusTextBar;
    }
}