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
    partial class TailConfigApplyAllForm
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
            this._checkBoxColors = new System.Windows.Forms.CheckBox();
            this._checkBoxFont = new System.Windows.Forms.CheckBox();
            this._checkboxKeywords = new System.Windows.Forms.CheckBox();
            this._buttonOk = new System.Windows.Forms.Button();
            this._buttonCancel = new System.Windows.Forms.Button();
            this._checkboxTools = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _checkBoxColors
            // 
            this._checkBoxColors.AutoSize = true;
            this._checkBoxColors.Location = new System.Drawing.Point(12, 12);
            this._checkBoxColors.Name = "_checkBoxColors";
            this._checkBoxColors.Size = new System.Drawing.Size(81, 17);
            this._checkBoxColors.TabIndex = 0;
            this._checkBoxColors.Text = "View Colors";
            this._checkBoxColors.UseVisualStyleBackColor = true;
            // 
            // _checkBoxFont
            // 
            this._checkBoxFont.AutoSize = true;
            this._checkBoxFont.Location = new System.Drawing.Point(12, 35);
            this._checkBoxFont.Name = "_checkBoxFont";
            this._checkBoxFont.Size = new System.Drawing.Size(73, 17);
            this._checkBoxFont.TabIndex = 1;
            this._checkBoxFont.Text = "View Font";
            this._checkBoxFont.UseVisualStyleBackColor = true;
            // 
            // _checkboxKeywords
            // 
            this._checkboxKeywords.AutoSize = true;
            this._checkboxKeywords.Location = new System.Drawing.Point(12, 58);
            this._checkboxKeywords.Name = "_checkboxKeywords";
            this._checkboxKeywords.Size = new System.Drawing.Size(111, 17);
            this._checkboxKeywords.TabIndex = 2;
            this._checkboxKeywords.Text = "Keyword Highlight";
            this._checkboxKeywords.UseVisualStyleBackColor = true;
            // 
            // _buttonOk
            // 
            this._buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._buttonOk.Location = new System.Drawing.Point(75, 107);
            this._buttonOk.Name = "_buttonOk";
            this._buttonOk.Size = new System.Drawing.Size(75, 23);
            this._buttonOk.TabIndex = 4;
            this._buttonOk.Text = "OK";
            this._buttonOk.UseVisualStyleBackColor = true;
            // 
            // _buttonCancel
            // 
            this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._buttonCancel.Location = new System.Drawing.Point(165, 107);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(75, 23);
            this._buttonCancel.TabIndex = 5;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            // 
            // _checkboxTools
            // 
            this._checkboxTools.AutoSize = true;
            this._checkboxTools.Location = new System.Drawing.Point(12, 81);
            this._checkboxTools.Name = "_checkboxTools";
            this._checkboxTools.Size = new System.Drawing.Size(93, 17);
            this._checkboxTools.TabIndex = 3;
            this._checkboxTools.Text = "External Tools";
            this._checkboxTools.UseVisualStyleBackColor = true;
            // 
            // TailConfigApplyAllForm
            // 
            this.AcceptButton = this._buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._buttonCancel;
            this.ClientSize = new System.Drawing.Size(252, 142);
            this.Controls.Add(this._checkboxTools);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonOk);
            this.Controls.Add(this._checkboxKeywords);
            this.Controls.Add(this._checkBoxFont);
            this.Controls.Add(this._checkBoxColors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TailConfigApplyAllForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Apply Options to All Views";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox _checkBoxColors;
        public System.Windows.Forms.CheckBox _checkBoxFont;
        public System.Windows.Forms.CheckBox _checkboxKeywords;
        private System.Windows.Forms.Button _buttonOk;
        private System.Windows.Forms.Button _buttonCancel;
        public System.Windows.Forms.CheckBox _checkboxTools;
    }
}