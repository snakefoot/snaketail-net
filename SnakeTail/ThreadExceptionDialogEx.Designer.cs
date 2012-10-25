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
    partial class ThreadExceptionDialogEx
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
            this._detailsBtn = new System.Windows.Forms.Button();
            this._abortBtn = new System.Windows.Forms.Button();
            this._resumeBtn = new System.Windows.Forms.Button();
            this._reportText = new System.Windows.Forms.TextBox();
            this._pictureBox = new System.Windows.Forms.PictureBox();
            this._sendReportBtn = new System.Windows.Forms.Button();
            this._reportListBox = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _detailsBtn
            // 
            this._detailsBtn.Location = new System.Drawing.Point(11, 110);
            this._detailsBtn.Name = "_detailsBtn";
            this._detailsBtn.Size = new System.Drawing.Size(75, 23);
            this._detailsBtn.TabIndex = 4;
            this._detailsBtn.Text = "&Details....";
            this._detailsBtn.UseVisualStyleBackColor = true;
            this._detailsBtn.Click += new System.EventHandler(this._detailsBtn_Click);
            // 
            // _abortBtn
            // 
            this._abortBtn.Location = new System.Drawing.Point(297, 110);
            this._abortBtn.Name = "_abortBtn";
            this._abortBtn.Size = new System.Drawing.Size(75, 23);
            this._abortBtn.TabIndex = 2;
            this._abortBtn.Text = "&Quit";
            this._abortBtn.UseVisualStyleBackColor = true;
            this._abortBtn.Click += new System.EventHandler(this._abortBtn_Click);
            // 
            // _resumeBtn
            // 
            this._resumeBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._resumeBtn.Location = new System.Drawing.Point(216, 110);
            this._resumeBtn.Name = "_resumeBtn";
            this._resumeBtn.Size = new System.Drawing.Size(75, 23);
            this._resumeBtn.TabIndex = 1;
            this._resumeBtn.Text = "&Ignore";
            this._resumeBtn.UseVisualStyleBackColor = true;
            // 
            // _reportText
            // 
            this._reportText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._reportText.Location = new System.Drawing.Point(67, 12);
            this._reportText.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this._reportText.Multiline = true;
            this._reportText.Name = "_reportText";
            this._reportText.ReadOnly = true;
            this._reportText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._reportText.Size = new System.Drawing.Size(305, 92);
            this._reportText.TabIndex = 24;
            this._reportText.TabStop = false;
            this._reportText.Text = "Description";
            // 
            // _pictureBox
            // 
            this._pictureBox.Location = new System.Drawing.Point(12, 12);
            this._pictureBox.Name = "_pictureBox";
            this._pictureBox.Size = new System.Drawing.Size(46, 44);
            this._pictureBox.TabIndex = 25;
            this._pictureBox.TabStop = false;
            // 
            // _sendReportBtn
            // 
            this._sendReportBtn.Location = new System.Drawing.Point(92, 110);
            this._sendReportBtn.Name = "_sendReportBtn";
            this._sendReportBtn.Size = new System.Drawing.Size(75, 23);
            this._sendReportBtn.TabIndex = 0;
            this._sendReportBtn.Text = "Send Report";
            this._sendReportBtn.UseVisualStyleBackColor = true;
            this._sendReportBtn.Click += new System.EventHandler(this._sendReportBtn_Click);
            // 
            // _reportListBox
            // 
            this._reportListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._reportListBox.FormattingEnabled = true;
            this._reportListBox.Location = new System.Drawing.Point(12, 138);
            this._reportListBox.Name = "_reportListBox";
            this._reportListBox.Size = new System.Drawing.Size(360, 4);
            this._reportListBox.TabIndex = 5;
            this._reportListBox.Visible = false;
            this._reportListBox.DoubleClick += new System.EventHandler(this._reportListBox_DoubleClick);
            this._reportListBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._reportListBox_KeyPress);
            this._reportListBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._reportListBox_PreviewKeyDown);
            // 
            // ThreadExceptionDialogEx
            // 
            this.AcceptButton = this._sendReportBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._resumeBtn;
            this.ClientSize = new System.Drawing.Size(384, 145);
            this.Controls.Add(this._reportListBox);
            this.Controls.Add(this._sendReportBtn);
            this.Controls.Add(this._pictureBox);
            this.Controls.Add(this._reportText);
            this.Controls.Add(this._resumeBtn);
            this.Controls.Add(this._abortBtn);
            this.Controls.Add(this._detailsBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ThreadExceptionDialogEx";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreadExceptionDialogEx";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ThreadExceptionDialogEx_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _detailsBtn;
        private System.Windows.Forms.Button _abortBtn;
        private System.Windows.Forms.Button _resumeBtn;
        private System.Windows.Forms.TextBox _reportText;
        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.Button _sendReportBtn;
        private System.Windows.Forms.ListBox _reportListBox;
    }
}