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
using System.Text;
using System.Windows.Forms;

namespace SnakeTail
{
    public partial class ExternalToolConfigForm : Form
    {
        public ExternalToolConfig ExternalToolConfig { get; private set; }

        public ExternalToolConfigForm(ExternalToolConfig extenalToolConfig)
        {
            InitializeComponent();
            if (extenalToolConfig != null)
                ExternalToolConfig = extenalToolConfig;
            else
            {
                ExternalToolConfig = new ExternalToolConfig();
                ExternalToolConfig.Name = "New Tool";
            }

            List<string> parameterNames = new List<string>();
            foreach (ExternalTool.ParameterName parameterName in Enum.GetValues(typeof(ExternalTool.ParameterName)))
                parameterNames.Add(ExternalTool.GetParameterSymbol(parameterName));

            _paramBindingSource.DataSource = parameterNames;

            _argParamCmb.DataSource = _paramBindingSource;
            _initDirParamCmb.DataSource = _paramBindingSource;
        }

        private void ExternalToolConfigForm_Load(object sender, EventArgs e)
        {
            _nameEdt.Text = ExternalToolConfig.Name;
            _cmdEdt.Text = ExternalToolConfig.Command;
            _argsEdt.Text = ExternalToolConfig.Arguments;
            _shortcutEdt.Text = ExternalToolConfig.ShortcutKey;
            _initDirEdt.Text = ExternalToolConfig.InitialDirectory;
            _runAdminChk.Checked = ExternalToolConfig.RunAsAdmin;
            _hideWindowChk.Checked = ExternalToolConfig.HideWindow;
            _initDirParamCmb.SelectedIndex = -1;
            _argParamCmb.SelectedIndex = -1;
        }

        private void _okBtn_Click(object sender, EventArgs e)
        {
            ExternalToolConfig.Name = _nameEdt.Text;
            ExternalToolConfig.Command = _cmdEdt.Text;
            ExternalToolConfig.Arguments = _argsEdt.Text;
            ExternalToolConfig.ShortcutKey = _shortcutEdt.Text;
            ExternalToolConfig.InitialDirectory = _initDirEdt.Text;
            ExternalToolConfig.RunAsAdmin = _runAdminChk.Checked;
            ExternalToolConfig.HideWindow = _hideWindowChk.Checked;
        }

        private void _argParamCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_argParamCmb.SelectedItem != null)
            {
                _argsEdt.Text += _argParamCmb.SelectedItem as string;
                _argParamCmb.SelectedIndex = -1;
            }
        }

        private void _initDirParamCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initDirParamCmb.SelectedItem != null)
            {
                _initDirEdt.Text += _initDirParamCmb.SelectedItem as string;
                _initDirParamCmb.SelectedIndex = -1;
            }
        }

        private void _browseCmdBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "Find Tool Location";
            if (!String.IsNullOrEmpty(_cmdEdt.Text))
                fileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(_cmdEdt.Text);
            fileDialog.Filter = "Executable Files|*.exe|All Files|*.*";
            if (fileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            _cmdEdt.Text = fileDialog.FileName;
        }

        private void _shortcutEdt_KeyDown(object sender, KeyEventArgs e)
        {
            // Ignore initial Shift + Control + Alt keys
            if (e.KeyCode == Keys.ShiftKey)
                e.Handled = true;
            else
            if (e.KeyCode == Keys.ControlKey)
                e.Handled = true;
            else
            if (e.KeyCode == Keys.Menu)
                e.Handled = true;
            else
            {
                e.Handled = true;
                ExternalToolConfig tmp = new SnakeTail.ExternalToolConfig();
                tmp.ShortcutKeyEnum = Control.ModifierKeys | e.KeyCode;
                _shortcutEdt.Text = tmp.ShortcutKey;
                _shortcutEdt.Select(0, 0);
                try
                {
                    ToolStripMenuItem tempItem = new ToolStripMenuItem();
                    tempItem.ShortcutKeys = tmp.ShortcutKeyEnum.Value;
                }
                catch
                {
                    MessageBox.Show(this, string.Format("'{0}' cannot be used as shortcut", tmp.ShortcutKey), this.Text + " - Invalid shortcut", MessageBoxButtons.OK);
                    _shortcutEdt.Text = string.Empty;
                }
            }
        }

        private void _shortcutEdt_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
