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
    partial class KeywordConfigForm
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            this._keywordEdt = new System.Windows.Forms.TextBox();
            this._matchCaseChk = new System.Windows.Forms.CheckBox();
            this._matchRegExChk = new System.Windows.Forms.CheckBox();
            this._textColorBtn = new System.Windows.Forms.Button();
            this._backColorBtn = new System.Windows.Forms.Button();
            this._cancelBtn = new System.Windows.Forms.Button();
            this._okBtn = new System.Windows.Forms.Button();
            this._logHitChk = new System.Windows.Forms.CheckBox();
            this._noHighlightChk = new System.Windows.Forms.CheckBox();
            this._externalToolCmb = new System.Windows.Forms.ComboBox();
            this._tabWarningIconChk = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 13);
            label1.TabIndex = 0;
            label1.Text = "Keyword";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(9, 171);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(112, 13);
            label2.TabIndex = 7;
            label2.Text = "Foreground Text Color";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(9, 200);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(92, 13);
            label3.TabIndex = 8;
            label3.Text = "Background Color";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(9, 119);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(108, 13);
            label4.TabIndex = 13;
            label4.Text = "Launch External Tool";
            // 
            // _keywordEdt
            // 
            this._keywordEdt.Location = new System.Drawing.Point(87, 6);
            this._keywordEdt.Name = "_keywordEdt";
            this._keywordEdt.Size = new System.Drawing.Size(233, 20);
            this._keywordEdt.TabIndex = 1;
            // 
            // _matchCaseChk
            // 
            this._matchCaseChk.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._matchCaseChk.Location = new System.Drawing.Point(9, 32);
            this._matchCaseChk.Name = "_matchCaseChk";
            this._matchCaseChk.Size = new System.Drawing.Size(153, 17);
            this._matchCaseChk.TabIndex = 2;
            this._matchCaseChk.Text = "Match Case Sensitive";
            this._matchCaseChk.UseVisualStyleBackColor = true;
            // 
            // _matchRegExChk
            // 
            this._matchRegExChk.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._matchRegExChk.Location = new System.Drawing.Point(9, 52);
            this._matchRegExChk.Name = "_matchRegExChk";
            this._matchRegExChk.Size = new System.Drawing.Size(153, 16);
            this._matchRegExChk.TabIndex = 4;
            this._matchRegExChk.Text = "Match Regular Expression";
            this._matchRegExChk.UseVisualStyleBackColor = true;
            // 
            // _textColorBtn
            // 
            this._textColorBtn.Location = new System.Drawing.Point(147, 166);
            this._textColorBtn.Name = "_textColorBtn";
            this._textColorBtn.Size = new System.Drawing.Size(75, 23);
            this._textColorBtn.TabIndex = 5;
            this._textColorBtn.Text = "Change...";
            this._textColorBtn.UseVisualStyleBackColor = true;
            this._textColorBtn.Click += new System.EventHandler(this._textColorBtn_Click);
            // 
            // _backColorBtn
            // 
            this._backColorBtn.Location = new System.Drawing.Point(147, 195);
            this._backColorBtn.Name = "_backColorBtn";
            this._backColorBtn.Size = new System.Drawing.Size(75, 23);
            this._backColorBtn.TabIndex = 6;
            this._backColorBtn.Text = "Change...";
            this._backColorBtn.UseVisualStyleBackColor = true;
            this._backColorBtn.Click += new System.EventHandler(this._backColorBtn_Click);
            // 
            // _cancelBtn
            // 
            this._cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelBtn.Location = new System.Drawing.Point(245, 235);
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.Size = new System.Drawing.Size(75, 23);
            this._cancelBtn.TabIndex = 9;
            this._cancelBtn.Text = "Cancel";
            this._cancelBtn.UseVisualStyleBackColor = true;
            // 
            // _okBtn
            // 
            this._okBtn.Location = new System.Drawing.Point(164, 235);
            this._okBtn.Name = "_okBtn";
            this._okBtn.Size = new System.Drawing.Size(75, 23);
            this._okBtn.TabIndex = 10;
            this._okBtn.Text = "OK";
            this._okBtn.UseVisualStyleBackColor = true;
            this._okBtn.Click += new System.EventHandler(this._okBtn_Click);
            // 
            // _logHitChk
            // 
            this._logHitChk.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._logHitChk.Location = new System.Drawing.Point(9, 69);
            this._logHitChk.Name = "_logHitChk";
            this._logHitChk.Size = new System.Drawing.Size(153, 24);
            this._logHitChk.TabIndex = 11;
            this._logHitChk.Text = "Log Hit Counter";
            this._logHitChk.UseVisualStyleBackColor = true;
            // 
            // _noHighlightChk
            // 
            this._noHighlightChk.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._noHighlightChk.Location = new System.Drawing.Point(9, 143);
            this._noHighlightChk.Name = "_noHighlightChk";
            this._noHighlightChk.Size = new System.Drawing.Size(153, 21);
            this._noHighlightChk.TabIndex = 12;
            this._noHighlightChk.Text = "No text highlight";
            this._noHighlightChk.UseVisualStyleBackColor = true;
            // 
            // _externalToolCmb
            // 
            this._externalToolCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._externalToolCmb.FormattingEnabled = true;
            this._externalToolCmb.Location = new System.Drawing.Point(147, 116);
            this._externalToolCmb.Name = "_externalToolCmb";
            this._externalToolCmb.Size = new System.Drawing.Size(121, 21);
            this._externalToolCmb.TabIndex = 14;
            // 
            // _tabWarningIconChk
            // 
            this._tabWarningIconChk.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._tabWarningIconChk.Location = new System.Drawing.Point(9, 92);
            this._tabWarningIconChk.Name = "_tabWarningIconChk";
            this._tabWarningIconChk.Size = new System.Drawing.Size(153, 21);
            this._tabWarningIconChk.TabIndex = 15;
            this._tabWarningIconChk.Text = "Display Tab Warning Icon";
            this._tabWarningIconChk.UseVisualStyleBackColor = true;
            // 
            // KeywordConfigForm
            // 
            this.AcceptButton = this._okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelBtn;
            this.ClientSize = new System.Drawing.Size(327, 264);
            this.Controls.Add(this._tabWarningIconChk);
            this.Controls.Add(this._externalToolCmb);
            this.Controls.Add(label4);
            this.Controls.Add(this._noHighlightChk);
            this.Controls.Add(this._logHitChk);
            this.Controls.Add(this._okBtn);
            this.Controls.Add(this._cancelBtn);
            this.Controls.Add(label3);
            this.Controls.Add(label2);
            this.Controls.Add(this._backColorBtn);
            this.Controls.Add(this._textColorBtn);
            this.Controls.Add(this._matchRegExChk);
            this.Controls.Add(this._matchCaseChk);
            this.Controls.Add(this._keywordEdt);
            this.Controls.Add(label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeywordConfigForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Keyword Highlight";
            this.Load += new System.EventHandler(this.KeywordConfigForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _keywordEdt;
        private System.Windows.Forms.CheckBox _matchCaseChk;
        private System.Windows.Forms.CheckBox _matchRegExChk;
        private System.Windows.Forms.Button _textColorBtn;
        private System.Windows.Forms.Button _backColorBtn;
        private System.Windows.Forms.Button _cancelBtn;
        private System.Windows.Forms.Button _okBtn;
        private System.Windows.Forms.CheckBox _logHitChk;
        private System.Windows.Forms.CheckBox _noHighlightChk;
        private System.Windows.Forms.ComboBox _externalToolCmb;
        private System.Windows.Forms.CheckBox _tabWarningIconChk;
    }
}