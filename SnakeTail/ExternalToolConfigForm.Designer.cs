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
    partial class ExternalToolConfigForm
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            this._nameEdt = new System.Windows.Forms.TextBox();
            this._cmdEdt = new System.Windows.Forms.TextBox();
            this._browseCmdBtn = new System.Windows.Forms.Button();
            this._argParamCmb = new System.Windows.Forms.ComboBox();
            this._shortcutEdt = new System.Windows.Forms.TextBox();
            this._cancelBtn = new System.Windows.Forms.Button();
            this._okBtn = new System.Windows.Forms.Button();
            this._runAdminChk = new System.Windows.Forms.CheckBox();
            this._hideWindowChk = new System.Windows.Forms.CheckBox();
            this._initDirEdt = new System.Windows.Forms.TextBox();
            this._argsEdt = new System.Windows.Forms.TextBox();
            this._initDirParamCmb = new System.Windows.Forms.ComboBox();
            this._paramBindingSource = new System.Windows.Forms.BindingSource(this.components);
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._paramBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(35, 13);
            label1.TabIndex = 1;
            label1.Text = "Name";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 35);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(54, 13);
            label2.TabIndex = 2;
            label2.Text = "Command";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 62);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(57, 13);
            label3.TabIndex = 6;
            label3.Text = "Arguments";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(6, 115);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(97, 13);
            label4.TabIndex = 9;
            label4.Text = "Press Shortcut Key";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(6, 88);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(76, 13);
            label5.TabIndex = 14;
            label5.Text = "Initial Directory";
            // 
            // _nameEdt
            // 
            this._nameEdt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._nameEdt.Location = new System.Drawing.Point(109, 6);
            this._nameEdt.Name = "_nameEdt";
            this._nameEdt.Size = new System.Drawing.Size(199, 20);
            this._nameEdt.TabIndex = 0;
            // 
            // _cmdEdt
            // 
            this._cmdEdt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cmdEdt.Location = new System.Drawing.Point(109, 32);
            this._cmdEdt.Name = "_cmdEdt";
            this._cmdEdt.Size = new System.Drawing.Size(199, 20);
            this._cmdEdt.TabIndex = 1;
            // 
            // _browseCmdBtn
            // 
            this._browseCmdBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._browseCmdBtn.Location = new System.Drawing.Point(314, 30);
            this._browseCmdBtn.Name = "_browseCmdBtn";
            this._browseCmdBtn.Size = new System.Drawing.Size(75, 23);
            this._browseCmdBtn.TabIndex = 2;
            this._browseCmdBtn.Text = "Browse...";
            this._browseCmdBtn.UseVisualStyleBackColor = true;
            this._browseCmdBtn.Click += new System.EventHandler(this._browseCmdBtn_Click);
            // 
            // _argParamCmb
            // 
            this._argParamCmb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._argParamCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._argParamCmb.DropDownWidth = 100;
            this._argParamCmb.FormattingEnabled = true;
            this._argParamCmb.Location = new System.Drawing.Point(314, 58);
            this._argParamCmb.Name = "_argParamCmb";
            this._argParamCmb.Size = new System.Drawing.Size(20, 21);
            this._argParamCmb.TabIndex = 4;
            this._argParamCmb.SelectedIndexChanged += new System.EventHandler(this._argParamCmb_SelectedIndexChanged);
            // 
            // _shortcutEdt
            // 
            this._shortcutEdt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._shortcutEdt.Location = new System.Drawing.Point(109, 111);
            this._shortcutEdt.Name = "_shortcutEdt";
            this._shortcutEdt.Size = new System.Drawing.Size(130, 20);
            this._shortcutEdt.TabIndex = 7;
            this._shortcutEdt.KeyDown += new System.Windows.Forms.KeyEventHandler(this._shortcutEdt_KeyDown);
            // 
            // _cancelBtn
            // 
            this._cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelBtn.Location = new System.Drawing.Point(314, 182);
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.Size = new System.Drawing.Size(75, 23);
            this._cancelBtn.TabIndex = 11;
            this._cancelBtn.Text = "Cancel";
            this._cancelBtn.UseVisualStyleBackColor = true;
            // 
            // _okBtn
            // 
            this._okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okBtn.Location = new System.Drawing.Point(233, 182);
            this._okBtn.Name = "_okBtn";
            this._okBtn.Size = new System.Drawing.Size(75, 23);
            this._okBtn.TabIndex = 10;
            this._okBtn.Text = "OK";
            this._okBtn.UseVisualStyleBackColor = true;
            this._okBtn.Click += new System.EventHandler(this._okBtn_Click);
            // 
            // _runAdminChk
            // 
            this._runAdminChk.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._runAdminChk.Location = new System.Drawing.Point(4, 134);
            this._runAdminChk.Name = "_runAdminChk";
            this._runAdminChk.Size = new System.Drawing.Size(120, 24);
            this._runAdminChk.TabIndex = 8;
            this._runAdminChk.Text = "Admin Rights";
            this._runAdminChk.UseVisualStyleBackColor = true;
            // 
            // _hideWindowChk
            // 
            this._hideWindowChk.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._hideWindowChk.Location = new System.Drawing.Point(4, 155);
            this._hideWindowChk.Name = "_hideWindowChk";
            this._hideWindowChk.Size = new System.Drawing.Size(120, 24);
            this._hideWindowChk.TabIndex = 9;
            this._hideWindowChk.Text = "Hide Window";
            this._hideWindowChk.UseVisualStyleBackColor = true;
            // 
            // _initDirEdt
            // 
            this._initDirEdt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._initDirEdt.Location = new System.Drawing.Point(109, 85);
            this._initDirEdt.Name = "_initDirEdt";
            this._initDirEdt.Size = new System.Drawing.Size(199, 20);
            this._initDirEdt.TabIndex = 5;
            // 
            // _argsEdt
            // 
            this._argsEdt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._argsEdt.Location = new System.Drawing.Point(109, 59);
            this._argsEdt.Name = "_argsEdt";
            this._argsEdt.Size = new System.Drawing.Size(199, 20);
            this._argsEdt.TabIndex = 3;
            // 
            // _initDirParamCmb
            // 
            this._initDirParamCmb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._initDirParamCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._initDirParamCmb.DropDownWidth = 100;
            this._initDirParamCmb.FormattingEnabled = true;
            this._initDirParamCmb.Location = new System.Drawing.Point(314, 84);
            this._initDirParamCmb.Name = "_initDirParamCmb";
            this._initDirParamCmb.Size = new System.Drawing.Size(20, 21);
            this._initDirParamCmb.TabIndex = 6;
            this._initDirParamCmb.SelectedIndexChanged += new System.EventHandler(this._initDirParamCmb_SelectedIndexChanged);
            // 
            // _paramBindingSource
            // 
            this._paramBindingSource.AllowNew = false;
            // 
            // ExternalToolConfigForm
            // 
            this.AcceptButton = this._okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 213);
            this.Controls.Add(this._initDirParamCmb);
            this.Controls.Add(this._argsEdt);
            this.Controls.Add(this._initDirEdt);
            this.Controls.Add(label5);
            this.Controls.Add(this._hideWindowChk);
            this.Controls.Add(this._runAdminChk);
            this.Controls.Add(this._okBtn);
            this.Controls.Add(this._cancelBtn);
            this.Controls.Add(label4);
            this.Controls.Add(this._shortcutEdt);
            this.Controls.Add(this._argParamCmb);
            this.Controls.Add(label3);
            this.Controls.Add(this._browseCmdBtn);
            this.Controls.Add(this._cmdEdt);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Controls.Add(this._nameEdt);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(4000, 1800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 36);
            this.Name = "ExternalToolConfigForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure External Tool";
            this.Load += new System.EventHandler(this.ExternalToolConfigForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._paramBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _nameEdt;
        private System.Windows.Forms.TextBox _cmdEdt;
        private System.Windows.Forms.Button _browseCmdBtn;
        private System.Windows.Forms.ComboBox _argParamCmb;
        private System.Windows.Forms.TextBox _shortcutEdt;
        private System.Windows.Forms.Button _cancelBtn;
        private System.Windows.Forms.Button _okBtn;
        private System.Windows.Forms.CheckBox _runAdminChk;
        private System.Windows.Forms.CheckBox _hideWindowChk;
        private System.Windows.Forms.TextBox _initDirEdt;
        private System.Windows.Forms.TextBox _argsEdt;
        private System.Windows.Forms.ComboBox _initDirParamCmb;
        private System.Windows.Forms.BindingSource _paramBindingSource;
    }
}