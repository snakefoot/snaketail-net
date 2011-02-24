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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SnakeTail
{
    public partial class SearchForm : Form
    {
        private static SearchForm _instance = null;
        public static SearchForm Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SearchForm();
                    return _instance;
                }
                return _instance;
            }
        }

        private TailForm _activeTailForm = null;
        public TailForm ActiveTailForm
        {
            get
            {
                if (_activeTailForm != null)
                    return _activeTailForm;
                else
                    return MainForm.Instance.ActiveMdiChild as TailForm;
            }
            set
            {
                if (_activeTailForm != null)
                {
                    _activeTailForm.FormClosed -= _activeForm_FormClosed;
                }
                _activeTailForm = value;
                if (Visible)
                {
                    SetWindowPos(this.Handle, HWND_TOP, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);  // BringToFront without focus
                }
                if (_activeTailForm != null)
                    _activeTailForm.FormClosed += new FormClosedEventHandler(_activeForm_FormClosed);
            }
        }

        public void StartSearch(TailForm activeTailForm)
        {
            if (!Visible)
                Show(MainForm.Instance);
            ActiveTailForm = activeTailForm;
            BringToFront();
            _searchTextBox.SelectAll();
            _searchTextBox.Focus();
        }

        public void SearchAgain(TailForm activeTailForm, bool searchForward)
        {
            if (activeTailForm != null)
            {
                if (!Visible)
                    Show(MainForm.Instance);
                ActiveTailForm = activeTailForm;

                bool found = false;
                using (new HourGlass(this))
                {
                    using (new HourGlass(activeTailForm))
                    {
                        found = ActiveTailForm.SearchForText(_searchTextBox.Text, _matchCaseCheckBox.Checked, searchForward);
                    }
                }
                if (!found)
                    MessageBox.Show("Cannot find \"" + _searchTextBox.Text + "\"", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        void _activeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _activeTailForm = null;
            MainForm.Instance.Focus();
        }

        protected SearchForm()
        {
            InitializeComponent();
            _findNextBtn.Enabled = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        public static IntPtr HWND_TOP = (IntPtr)0;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowPos(IntPtr hWnd,
          IntPtr hWndInsertAfter,
          int x,
          int y,
          int cx,
          int cy,
          UInt32 uFlags);

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F3)
            {
                SearchAgain(ActiveTailForm, !_upRadioBtn.Checked);
                return true;
            }
            if (keyData == (Keys.Shift | Keys.F3))
            {
                SearchAgain(ActiveTailForm, _upRadioBtn.Checked);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void _cancelBtn_Click(object sender, EventArgs e)
        {
            MainForm.Instance.Focus();
            Hide();
        }

        private void _searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_searchTextBox.Text.Length == 0)
                _findNextBtn.Enabled = false;
            else
                _findNextBtn.Enabled = true;
        }

        private void _findNextBtn_Click(object sender, EventArgs e)
        {
            if (ActiveTailForm != null)
                SearchAgain(ActiveTailForm, !_upRadioBtn.Checked);
        }

        private void SearchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _instance = null;
        }
    }
}
