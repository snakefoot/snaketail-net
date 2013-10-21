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
            this._keywordEdt = new System.Windows.Forms.TextBox();
            this._matchCaseChk = new System.Windows.Forms.CheckBox();
            this._matchRegExChk = new System.Windows.Forms.CheckBox();
            this._textColorBtn = new System.Windows.Forms.Button();
            this._backColorBtn = new System.Windows.Forms.Button();
            this._cancelBtn = new System.Windows.Forms.Button();
            this._okBtn = new System.Windows.Forms.Button();
            this._logHitChk = new System.Windows.Forms.CheckBox();
            this._textColoringChk = new System.Windows.Forms.CheckBox();
            this._externalToolCmb = new System.Windows.Forms.ComboBox();
            this._alertHighlightChk = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._launchToolChk = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
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
            // _keywordEdt
            // 
            this._keywordEdt.Location = new System.Drawing.Point(63, 6);
            this._keywordEdt.Name = "_keywordEdt";
            this._keywordEdt.Size = new System.Drawing.Size(292, 20);
            this._keywordEdt.TabIndex = 1;
            // 
            // _matchCaseChk
            // 
            this._matchCaseChk.Location = new System.Drawing.Point(63, 32);
            this._matchCaseChk.Name = "_matchCaseChk";
            this._matchCaseChk.Size = new System.Drawing.Size(133, 16);
            this._matchCaseChk.TabIndex = 2;
            this._matchCaseChk.Text = "Match Case Sensitive";
            this._matchCaseChk.UseVisualStyleBackColor = true;
            // 
            // _matchRegExChk
            // 
            this._matchRegExChk.Location = new System.Drawing.Point(202, 32);
            this._matchRegExChk.Name = "_matchRegExChk";
            this._matchRegExChk.Size = new System.Drawing.Size(153, 16);
            this._matchRegExChk.TabIndex = 4;
            this._matchRegExChk.Text = "Match Regular Expression";
            this._matchRegExChk.UseVisualStyleBackColor = true;
            // 
            // _textColorBtn
            // 
            this._textColorBtn.Location = new System.Drawing.Point(165, 19);
            this._textColorBtn.Name = "_textColorBtn";
            this._textColorBtn.Size = new System.Drawing.Size(125, 23);
            this._textColorBtn.TabIndex = 5;
            this._textColorBtn.Text = "Text Color...";
            this._textColorBtn.UseVisualStyleBackColor = true;
            this._textColorBtn.Click += new System.EventHandler(this._textColorBtn_Click);
            // 
            // _backColorBtn
            // 
            this._backColorBtn.Location = new System.Drawing.Point(295, 19);
            this._backColorBtn.Name = "_backColorBtn";
            this._backColorBtn.Size = new System.Drawing.Size(125, 23);
            this._backColorBtn.TabIndex = 6;
            this._backColorBtn.Text = "Background Color...";
            this._backColorBtn.UseVisualStyleBackColor = true;
            this._backColorBtn.Click += new System.EventHandler(this._backColorBtn_Click);
            // 
            // _cancelBtn
            // 
            this._cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelBtn.Location = new System.Drawing.Point(367, 192);
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.Size = new System.Drawing.Size(75, 23);
            this._cancelBtn.TabIndex = 9;
            this._cancelBtn.Text = "Cancel";
            this._cancelBtn.UseVisualStyleBackColor = true;
            // 
            // _okBtn
            // 
            this._okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okBtn.Location = new System.Drawing.Point(286, 192);
            this._okBtn.Name = "_okBtn";
            this._okBtn.Size = new System.Drawing.Size(75, 23);
            this._okBtn.TabIndex = 10;
            this._okBtn.Text = "OK";
            this._okBtn.UseVisualStyleBackColor = true;
            this._okBtn.Click += new System.EventHandler(this._okBtn_Click);
            // 
            // _logHitChk
            // 
            this._logHitChk.Location = new System.Drawing.Point(6, 46);
            this._logHitChk.Name = "_logHitChk";
            this._logHitChk.Size = new System.Drawing.Size(153, 24);
            this._logHitChk.TabIndex = 11;
            this._logHitChk.Text = "Window Title Counter";
            this._logHitChk.UseVisualStyleBackColor = true;
            // 
            // _textColoringChk
            // 
            this._textColoringChk.Location = new System.Drawing.Point(6, 19);
            this._textColoringChk.Name = "_textColoringChk";
            this._textColoringChk.Size = new System.Drawing.Size(153, 21);
            this._textColoringChk.TabIndex = 12;
            this._textColoringChk.Text = "Line Coloring";
            this._textColoringChk.UseVisualStyleBackColor = true;
            this._textColoringChk.CheckedChanged += new System.EventHandler(this._noHighlightChk_CheckedChanged);
            // 
            // _externalToolCmb
            // 
            this._externalToolCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._externalToolCmb.FormattingEnabled = true;
            this._externalToolCmb.Location = new System.Drawing.Point(165, 74);
            this._externalToolCmb.Name = "_externalToolCmb";
            this._externalToolCmb.Size = new System.Drawing.Size(125, 21);
            this._externalToolCmb.TabIndex = 14;
            // 
            // _alertHighlightChk
            // 
            this._alertHighlightChk.Location = new System.Drawing.Point(6, 99);
            this._alertHighlightChk.Name = "_alertHighlightChk";
            this._alertHighlightChk.Size = new System.Drawing.Size(153, 21);
            this._alertHighlightChk.TabIndex = 15;
            this._alertHighlightChk.Text = "Alert Highlight";
            this._alertHighlightChk.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._launchToolChk);
            this.groupBox1.Controls.Add(this._alertHighlightChk);
            this.groupBox1.Controls.Add(this._textColoringChk);
            this.groupBox1.Controls.Add(this._logHitChk);
            this.groupBox1.Controls.Add(this._externalToolCmb);
            this.groupBox1.Controls.Add(this._textColorBtn);
            this.groupBox1.Controls.Add(this._backColorBtn);
            this.groupBox1.Location = new System.Drawing.Point(12, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(431, 127);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Keyword Match Actions";
            // 
            // _launchToolChk
            // 
            this._launchToolChk.AutoSize = true;
            this._launchToolChk.Location = new System.Drawing.Point(6, 76);
            this._launchToolChk.Name = "_launchToolChk";
            this._launchToolChk.Size = new System.Drawing.Size(127, 17);
            this._launchToolChk.TabIndex = 17;
            this._launchToolChk.Text = "Launch External Tool";
            this._launchToolChk.UseVisualStyleBackColor = true;
            this._launchToolChk.CheckedChanged += new System.EventHandler(this._launchToolChk_CheckedChanged);
            // 
            // KeywordConfigForm
            // 
            this.AcceptButton = this._okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelBtn;
            this.ClientSize = new System.Drawing.Size(454, 227);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._okBtn);
            this.Controls.Add(this._cancelBtn);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.CheckBox _textColoringChk;
        private System.Windows.Forms.ComboBox _externalToolCmb;
        private System.Windows.Forms.CheckBox _alertHighlightChk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox _launchToolChk;
    }
}