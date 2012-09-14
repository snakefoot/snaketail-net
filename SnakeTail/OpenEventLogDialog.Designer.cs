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
    partial class OpenEventLogDialog
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
            System.Windows.Forms.Button _openBtn;
            System.Windows.Forms.Button _cancelBtn;
            this._listView = new System.Windows.Forms.ListView();
            _openBtn = new System.Windows.Forms.Button();
            _cancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _openBtn
            // 
            _openBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            _openBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            _openBtn.Location = new System.Drawing.Point(191, 18);
            _openBtn.Name = "_openBtn";
            _openBtn.Size = new System.Drawing.Size(75, 23);
            _openBtn.TabIndex = 1;
            _openBtn.Text = "Open";
            _openBtn.UseVisualStyleBackColor = true;
            _openBtn.Click += new System.EventHandler(this._openBtn_Click);
            // 
            // _cancelBtn
            // 
            _cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            _cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _cancelBtn.Location = new System.Drawing.Point(191, 57);
            _cancelBtn.Name = "_cancelBtn";
            _cancelBtn.Size = new System.Drawing.Size(75, 23);
            _cancelBtn.TabIndex = 2;
            _cancelBtn.Text = "Cancel";
            _cancelBtn.UseVisualStyleBackColor = true;
            // 
            // _listView
            // 
            this._listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._listView.Location = new System.Drawing.Point(12, 12);
            this._listView.Name = "_listView";
            this._listView.Size = new System.Drawing.Size(164, 165);
            this._listView.TabIndex = 0;
            this._listView.UseCompatibleStateImageBehavior = false;
            this._listView.View = System.Windows.Forms.View.List;
            this._listView.DoubleClick += new System.EventHandler(this._listView_DoubleClick);
            // 
            // OpenEventLogDialog
            // 
            this.AcceptButton = _openBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = _cancelBtn;
            this.ClientSize = new System.Drawing.Size(278, 189);
            this.Controls.Add(this._listView);
            this.Controls.Add(_cancelBtn);
            this.Controls.Add(_openBtn);
            this.Name = "OpenEventLogDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open EventLog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView _listView;

    }
}