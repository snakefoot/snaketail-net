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
    public partial class KeywordConfigForm : Form
    {
        public TailKeywordConfig TailKeywordConfig { get; private set; }

        public KeywordConfigForm(TailKeywordConfig keywordConfig)
        {
            InitializeComponent();
            if (keywordConfig != null)
                TailKeywordConfig = keywordConfig;
            else
            {
                TailKeywordConfig = new TailKeywordConfig();
                TailKeywordConfig.FormBackColor = Color.Red;
                TailKeywordConfig.FormTextColor = Color.White;
            }
        }

        private void KeywordConfigForm_Load(object sender, EventArgs e)
        {
            if (TailKeywordConfig.FormBackColor.HasValue)
                _keywordEdt.BackColor = TailKeywordConfig.FormBackColor.Value;
            if (TailKeywordConfig.FormTextColor.HasValue)
                _keywordEdt.ForeColor = TailKeywordConfig.FormTextColor.Value;
            _keywordEdt.Text = TailKeywordConfig.Keyword;
            _matchCaseChk.Checked = TailKeywordConfig.MatchCaseSensitive;
            _matchRegExChk.Checked = TailKeywordConfig.MatchRegularExpression;
            _logHitChk.Checked = TailKeywordConfig.LogHitCounter;
        }

        private void _textColorBtn_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.Color = _keywordEdt.ForeColor;
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                _keywordEdt.ForeColor = colorDlg.Color;
            }
        }

        private void _backColorBtn_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.Color = _keywordEdt.BackColor;
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                _keywordEdt.BackColor = colorDlg.Color;
            }
        }

        private void _okBtn_Click(object sender, EventArgs e)
        {
            TailKeywordConfig.Keyword = _keywordEdt.Text;
            TailKeywordConfig.MatchCaseSensitive = _matchCaseChk.Checked;
            TailKeywordConfig.MatchRegularExpression = _matchRegExChk.Checked;
            if (TailKeywordConfig.MatchRegularExpression)
            {
                try
                {
                    System.Text.RegularExpressions.Regex testParsing = null;
                    if (TailKeywordConfig.MatchCaseSensitive)
                        testParsing = new System.Text.RegularExpressions.Regex(TailKeywordConfig.Keyword);
                    else
                        testParsing = new System.Text.RegularExpressions.Regex(TailKeywordConfig.Keyword, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }
                catch (System.ArgumentException exception)
                {
                    MessageBox.Show(String.Format("Failed to parse regular expression:\n\n{0} ({1})", exception.Message, exception.GetType().ToString()), "Invalid regular expression", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            TailKeywordConfig.LogHitCounter = _logHitChk.Checked;
			if (TailKeywordConfig.LogHitCounter)
			{
				TailKeywordConfig.FormBackColor = null;
				TailKeywordConfig.FormTextColor = null;
			}
			else
			{
	            TailKeywordConfig.FormBackColor = _keywordEdt.BackColor;
    	        TailKeywordConfig.FormTextColor = _keywordEdt.ForeColor;
			}

            DialogResult = DialogResult.OK;
        }
    }
}
