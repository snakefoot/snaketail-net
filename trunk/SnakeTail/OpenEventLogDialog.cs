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
    public partial class OpenEventLogDialog : Form
    {
        public OpenEventLogDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.SnakeIcon;

            List<string> logfiles = EventLogForm.GetEventLogFiles();
            foreach (string logfile in logfiles)
            {
                _listView.Items.Add(logfile);
                if (logfile == "Application")
                    _listView.SelectedIndices.Add(_listView.Items.Count-1);
            }
        }

        public string EventLogFile
        {
            get;
            private set;
        }

        private void _openBtn_Click(object sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count > 0)
                EventLogFile = _listView.SelectedItems[0].Text;
        }
    }
}
