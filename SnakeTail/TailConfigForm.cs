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
    public partial class TailConfigForm : Form
    {
        public TailFileConfig TailFileConfig { get; private set; }
        bool _displayFileTab;

        public TailConfigForm(TailFileConfig tailFileConfig, bool displayFileTab)
        {
            InitializeComponent();
            TailFileConfig = tailFileConfig;
            _displayFileTab = displayFileTab;
        }

        private void UpdateKeywordListItem(TailKeywordConfig keyword, ref ListViewItem lvi)
        {
            lvi.Text = keyword.Keyword;
            lvi.UseItemStyleForSubItems = false;
            if (keyword.FormTextColor.HasValue)
                lvi.ForeColor = keyword.FormTextColor.Value;
            if (keyword.FormBackColor.HasValue)
                lvi.BackColor = keyword.FormBackColor.Value;
            lvi.SubItems.Add(keyword.MatchCaseSensitive ? "Yes" : "No");
            lvi.SubItems.Add(keyword.MatchRegularExpression ? "Yes" : "No");
            lvi.SubItems.Add(keyword.LogHitCounter ? "Yes" : "No");
            lvi.Tag = keyword;
        }

        private void TailConfigForm_Load(object sender, EventArgs e)
        {
            _windowTitleEdt.Text = TailFileConfig.Title;
            _windowIconEdt.Text = TailFileConfig.IconFile;

            if (_displayFileTab)
            {
                _filePathEdt.Text = TailFileConfig.FilePath;

                _fileEncodingCmb.Items.Add(Encoding.Default);
                _fileEncodingCmb.Items.Add(Encoding.UTF8);
                _fileEncodingCmb.Items.Add(Encoding.ASCII);
                _fileEncodingCmb.Items.Add(Encoding.Unicode);
                _fileEncodingCmb.SelectedItem = TailFileConfig.EnumFileEncoding;

                _fileCacheSizeEdt.Text = TailFileConfig.FileCacheSize.ToString();

                _fileCheckIntervalEdt.Text = TailFileConfig.FileCheckInterval.ToString();

                _fileCheckPatternChk.Checked = TailFileConfig.FileCheckPattern;

                _windowServiceEdt.Text = TailFileConfig.ServiceName;

                if (TailFileConfig.KeywordHighlight != null)
                {
                    foreach (TailKeywordConfig keyword in TailFileConfig.KeywordHighlight)
                    {
                        ListViewItem lvi = _keywordListView.Items.Add(new ListViewItem());
                        UpdateKeywordListItem(keyword, ref lvi);
                    }
                }
            }
            else
            {
                _tabControl.TabPages.Remove(_tabPageFile);
                _tabControl.TabPages.Remove(_tabPageKeyWords);
            }
        }

        private void _acceptBtn_Click(object sender, EventArgs e)
        {
            TailFileConfig.Title = _windowTitleEdt.Text;
            TailFileConfig.IconFile = _windowIconEdt.Text;

            if (_displayFileTab)
            {
                TailFileConfig.FilePath = _filePathEdt.Text;

                TailFileConfig.EnumFileEncoding = (Encoding)_fileEncodingCmb.SelectedItem;

                TailFileConfig.FileCacheSize = Int32.Parse(_fileCacheSizeEdt.Text);

                TailFileConfig.ServiceName = _windowServiceEdt.Text;

                TailFileConfig.FileCheckInterval = Int32.Parse(_fileCheckIntervalEdt.Text);

                TailFileConfig.FileCheckPattern = _fileCheckPatternChk.Checked;

                if (TailFileConfig.KeywordHighlight == null)
                    TailFileConfig.KeywordHighlight = new List<TailKeywordConfig>();
                else
                    TailFileConfig.KeywordHighlight.Clear();
                foreach (ListViewItem lvi in _keywordListView.Items)
                {
                    TailFileConfig.KeywordHighlight.Add(lvi.Tag as TailKeywordConfig);
                }
            }
        }

        private void _textColorBtn_Click(object sender, EventArgs e)
        {
            FontDialog fdlgText = new FontDialog();
            if (TailFileConfig.FormFont != null)
                fdlgText.Font = TailFileConfig.FormFont;
            if (TailFileConfig.FormTextColor != null)
                fdlgText.Color = TailFileConfig.FormTextColor.Value;
            fdlgText.ShowColor = true;
            if (fdlgText.ShowDialog() == DialogResult.OK)
            {
                TailFileConfig.FormFont = fdlgText.Font;
                TailFileConfig.FormTextColor = fdlgText.Color;
            }
        }

        private void _backColorBtn_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            if (TailFileConfig.FormBackColor != null)
                colorDlg.Color = TailFileConfig.FormBackColor.Value;
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                TailFileConfig.FormBackColor = colorDlg.Color;
            }
        }

        private void _addWordBtn_Click(object sender, EventArgs e)
        {
            KeywordConfigForm dlg = new KeywordConfigForm(null);
            if (dlg.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(dlg.TailKeywordConfig.Keyword))
            {
                ListViewItem lvi = _keywordListView.Items.Add(new ListViewItem());
                UpdateKeywordListItem(dlg.TailKeywordConfig, ref lvi);
            }
        }

        private void _edtWordBtn_Click(object sender, EventArgs e)
        {
            if (_keywordListView.SelectedItems.Count == 0)
                return;

            KeywordConfigForm dlg = new KeywordConfigForm(_keywordListView.SelectedItems[0].Tag as TailKeywordConfig);
            if (dlg.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(dlg.TailKeywordConfig.Keyword))
            {
                ListViewItem lvi = _keywordListView.SelectedItems[0];
                UpdateKeywordListItem(dlg.TailKeywordConfig, ref lvi);
            }
        }

        private void _delWordBtn_Click(object sender, EventArgs e)
        {
            if (_keywordListView.SelectedItems.Count == 0)
                return;

            _keywordListView.Items.Remove(_keywordListView.SelectedItems[0]);
        }
    }
}
