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
    partial class SearchForm
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
            this._searchTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._downRadioBtn = new System.Windows.Forms.RadioButton();
            this._upRadioBtn = new System.Windows.Forms.RadioButton();
            this._findNextBtn = new System.Windows.Forms.Button();
            this._cancelBtn = new System.Windows.Forms.Button();
            this._matchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _searchTextBox
            // 
            this._searchTextBox.Location = new System.Drawing.Point(72, 11);
            this._searchTextBox.Name = "_searchTextBox";
            this._searchTextBox.Size = new System.Drawing.Size(223, 20);
            this._searchTextBox.TabIndex = 1;
            this._searchTextBox.TextChanged += new System.EventHandler(this._searchTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find what:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._downRadioBtn);
            this.groupBox1.Controls.Add(this._upRadioBtn);
            this.groupBox1.Location = new System.Drawing.Point(179, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(115, 44);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Direction";
            // 
            // _downRadioBtn
            // 
            this._downRadioBtn.AutoSize = true;
            this._downRadioBtn.Location = new System.Drawing.Point(51, 19);
            this._downRadioBtn.Name = "_downRadioBtn";
            this._downRadioBtn.Size = new System.Drawing.Size(53, 17);
            this._downRadioBtn.TabIndex = 4;
            this._downRadioBtn.Text = "Down";
            this._downRadioBtn.UseVisualStyleBackColor = true;
            // 
            // _upRadioBtn
            // 
            this._upRadioBtn.AutoSize = true;
            this._upRadioBtn.Checked = true;
            this._upRadioBtn.Location = new System.Drawing.Point(6, 19);
            this._upRadioBtn.Name = "_upRadioBtn";
            this._upRadioBtn.Size = new System.Drawing.Size(39, 17);
            this._upRadioBtn.TabIndex = 4;
            this._upRadioBtn.TabStop = true;
            this._upRadioBtn.Text = "Up";
            this._upRadioBtn.UseVisualStyleBackColor = true;
            // 
            // _findNextBtn
            // 
            this._findNextBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._findNextBtn.Location = new System.Drawing.Point(308, 10);
            this._findNextBtn.Name = "_findNextBtn";
            this._findNextBtn.Size = new System.Drawing.Size(75, 23);
            this._findNextBtn.TabIndex = 4;
            this._findNextBtn.Text = "Find Next";
            this._findNextBtn.UseVisualStyleBackColor = true;
            this._findNextBtn.Click += new System.EventHandler(this._findNextBtn_Click);
            // 
            // _cancelBtn
            // 
            this._cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelBtn.Location = new System.Drawing.Point(308, 39);
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.Size = new System.Drawing.Size(75, 23);
            this._cancelBtn.TabIndex = 5;
            this._cancelBtn.Text = "Cancel";
            this._cancelBtn.UseVisualStyleBackColor = true;
            this._cancelBtn.Click += new System.EventHandler(this._cancelBtn_Click);
            // 
            // _matchCaseCheckBox
            // 
            this._matchCaseCheckBox.AutoSize = true;
            this._matchCaseCheckBox.Location = new System.Drawing.Point(15, 58);
            this._matchCaseCheckBox.Name = "_matchCaseCheckBox";
            this._matchCaseCheckBox.Size = new System.Drawing.Size(83, 17);
            this._matchCaseCheckBox.TabIndex = 2;
            this._matchCaseCheckBox.Text = "Match Case";
            this._matchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // SearchForm
            // 
            this.AcceptButton = this._findNextBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelBtn;
            this.ClientSize = new System.Drawing.Size(396, 92);
            this.Controls.Add(this._matchCaseCheckBox);
            this.Controls.Add(this._cancelBtn);
            this.Controls.Add(this._findNextBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._searchTextBox);
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Find";
            this.Load += new System.EventHandler(this.SearchForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SearchForm_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _searchTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton _downRadioBtn;
        private System.Windows.Forms.RadioButton _upRadioBtn;
        private System.Windows.Forms.Button _findNextBtn;
        private System.Windows.Forms.Button _cancelBtn;
        private System.Windows.Forms.CheckBox _matchCaseCheckBox;
    }
}