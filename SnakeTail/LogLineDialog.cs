using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SnakeTail
{
    public partial class LogLineDialog : Form
    {
        public LogLineDialog()
        {
            InitializeComponent();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                Hide();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
