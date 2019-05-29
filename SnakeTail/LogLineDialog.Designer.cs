namespace SnakeTail
{
    partial class LogLineDialog
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
            this.lineText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lineText
            // 
            this.lineText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lineText.Location = new System.Drawing.Point(0, 0);
            this.lineText.Multiline = true;
            this.lineText.Name = "lineText";
            this.lineText.ReadOnly = true;
            this.lineText.Size = new System.Drawing.Size(808, 60);
            this.lineText.TabIndex = 0;
            // 
            // LogLineDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 60);
            this.Controls.Add(this.lineText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogLineDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox lineText;
    }
}