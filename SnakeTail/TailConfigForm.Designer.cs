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
    partial class TailConfigForm
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
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.ColumnHeader keywordColumnHeader;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.ColumnHeader toolNameColumnHeader;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.ColumnHeader keywordCaseColumnHeader;
            System.Windows.Forms.ColumnHeader keywordRegexColumnHeader;
            System.Windows.Forms.ColumnHeader keywordLogHitColumnHeader;
            System.Windows.Forms.ColumnHeader toolShortcutColumnHeader;
            this._tabControl = new System.Windows.Forms.TabControl();
            this._tabPageView = new System.Windows.Forms.TabPage();
            this._displayIconChk = new System.Windows.Forms.CheckBox();
            this._windowIconEdt = new System.Windows.Forms.TextBox();
            this._windowTitleEdt = new System.Windows.Forms.TextBox();
            this._backColorBtn = new System.Windows.Forms.Button();
            this._textColorBtn = new System.Windows.Forms.Button();
            this._tabPageFile = new System.Windows.Forms.TabPage();
            this._titleMatchFilenameChk = new System.Windows.Forms.CheckBox();
            this._browseBtn = new System.Windows.Forms.Button();
            this._fileChangeCheckIntervalEdt = new System.Windows.Forms.TextBox();
            this._fileCheckPatternChk = new System.Windows.Forms.CheckBox();
            this._fileReopenCheckIntervalEdt = new System.Windows.Forms.TextBox();
            this._windowServiceEdt = new System.Windows.Forms.TextBox();
            this._fileCacheSizeEdt = new System.Windows.Forms.TextBox();
            this._fileEncodingCmb = new System.Windows.Forms.ComboBox();
            this._filePathEdt = new System.Windows.Forms.TextBox();
            this._tabPageKeyWords = new System.Windows.Forms.TabPage();
            this._moveDownKeywordBtn = new System.Windows.Forms.Button();
            this._moveUpKeywordBtn = new System.Windows.Forms.Button();
            this._delWordBtn = new System.Windows.Forms.Button();
            this._edtWordBtn = new System.Windows.Forms.Button();
            this._addWordBtn = new System.Windows.Forms.Button();
            this._keywordListView = new System.Windows.Forms.ListView();
            this._tabPageExtTools = new System.Windows.Forms.TabPage();
            this._moveDownToolBtn = new System.Windows.Forms.Button();
            this._moveUpToolBtn = new System.Windows.Forms.Button();
            this._delToolBtn = new System.Windows.Forms.Button();
            this._editToolBtn = new System.Windows.Forms.Button();
            this._addToolBtn = new System.Windows.Forms.Button();
            this._extToolsListView = new System.Windows.Forms.ListView();
            this._acceptBtn = new System.Windows.Forms.Button();
            this._cancelBtn = new System.Windows.Forms.Button();
            this._applyAllBtn = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            keywordColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            label6 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            toolNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            label10 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            keywordCaseColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            keywordRegexColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            keywordLogHitColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            toolShortcutColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._tabControl.SuspendLayout();
            this._tabPageView.SuspendLayout();
            this._tabPageFile.SuspendLayout();
            this._tabPageKeyWords.SuspendLayout();
            this._tabPageExtTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(8, 46);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(70, 13);
            label2.TabIndex = 13;
            label2.Text = "Window Icon";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(69, 13);
            label1.TabIndex = 11;
            label1.Text = "Window Title";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(3, 9);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(48, 13);
            label3.TabIndex = 0;
            label3.Text = "File Path";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(3, 56);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(71, 13);
            label4.TabIndex = 2;
            label4.Text = "File Encoding";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(3, 84);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(80, 13);
            label5.TabIndex = 4;
            label5.Text = "File Cache Size";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(3, 165);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(90, 13);
            label7.TabIndex = 9;
            label7.Text = "Windows Service";
            // 
            // keywordColumnHeader
            // 
            keywordColumnHeader.Text = "Keyword";
            keywordColumnHeader.Width = 83;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(3, 113);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(136, 13);
            label6.TabIndex = 6;
            label6.Text = "File Reopen Check Interval";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(3, 139);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(135, 13);
            label8.TabIndex = 11;
            label8.Text = "File Change Check Interval";
            // 
            // toolNameColumnHeader
            // 
            toolNameColumnHeader.Text = "Name";
            toolNameColumnHeader.Width = 146;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(209, 113);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(29, 13);
            label10.TabIndex = 14;
            label10.Text = "secs";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(209, 139);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(37, 13);
            label9.TabIndex = 13;
            label9.Text = "msecs";
            // 
            // keywordCaseColumnHeader
            // 
            keywordCaseColumnHeader.Text = "Case Sensitive";
            keywordCaseColumnHeader.Width = 59;
            // 
            // keywordRegexColumnHeader
            // 
            keywordRegexColumnHeader.Text = "RegEx Match";
            keywordRegexColumnHeader.Width = 59;
            // 
            // keywordLogHitColumnHeader
            // 
            keywordLogHitColumnHeader.Text = "Log Hit";
            keywordLogHitColumnHeader.Width = 59;
            // 
            // toolShortcutColumnHeader
            // 
            toolShortcutColumnHeader.Text = "Shortcut Key";
            toolShortcutColumnHeader.Width = 89;
            // 
            // _tabControl
            // 
            this._tabControl.Controls.Add(this._tabPageView);
            this._tabControl.Controls.Add(this._tabPageFile);
            this._tabControl.Controls.Add(this._tabPageKeyWords);
            this._tabControl.Controls.Add(this._tabPageExtTools);
            this._tabControl.Location = new System.Drawing.Point(6, 6);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(367, 233);
            this._tabControl.TabIndex = 8;
            // 
            // _tabPageView
            // 
            this._tabPageView.Controls.Add(this._displayIconChk);
            this._tabPageView.Controls.Add(label2);
            this._tabPageView.Controls.Add(this._windowIconEdt);
            this._tabPageView.Controls.Add(label1);
            this._tabPageView.Controls.Add(this._windowTitleEdt);
            this._tabPageView.Controls.Add(this._backColorBtn);
            this._tabPageView.Controls.Add(this._textColorBtn);
            this._tabPageView.Location = new System.Drawing.Point(4, 22);
            this._tabPageView.Name = "_tabPageView";
            this._tabPageView.Padding = new System.Windows.Forms.Padding(3);
            this._tabPageView.Size = new System.Drawing.Size(359, 207);
            this._tabPageView.TabIndex = 0;
            this._tabPageView.Text = "View";
            this._tabPageView.UseVisualStyleBackColor = true;
            // 
            // _displayIconChk
            // 
            this._displayIconChk.AutoSize = true;
            this._displayIconChk.Location = new System.Drawing.Point(11, 136);
            this._displayIconChk.Name = "_displayIconChk";
            this._displayIconChk.Size = new System.Drawing.Size(172, 17);
            this._displayIconChk.TabIndex = 14;
            this._displayIconChk.Text = "Display tab icon on log change";
            this._displayIconChk.UseVisualStyleBackColor = true;
            // 
            // _windowIconEdt
            // 
            this._windowIconEdt.Location = new System.Drawing.Point(84, 43);
            this._windowIconEdt.Name = "_windowIconEdt";
            this._windowIconEdt.Size = new System.Drawing.Size(269, 20);
            this._windowIconEdt.TabIndex = 2;
            // 
            // _windowTitleEdt
            // 
            this._windowTitleEdt.Location = new System.Drawing.Point(84, 13);
            this._windowTitleEdt.Name = "_windowTitleEdt";
            this._windowTitleEdt.Size = new System.Drawing.Size(115, 20);
            this._windowTitleEdt.TabIndex = 1;
            // 
            // _backColorBtn
            // 
            this._backColorBtn.Location = new System.Drawing.Point(11, 98);
            this._backColorBtn.Name = "_backColorBtn";
            this._backColorBtn.Size = new System.Drawing.Size(125, 23);
            this._backColorBtn.TabIndex = 4;
            this._backColorBtn.Text = "Background Color";
            this._backColorBtn.UseVisualStyleBackColor = true;
            this._backColorBtn.Click += new System.EventHandler(this._backColorBtn_Click);
            // 
            // _textColorBtn
            // 
            this._textColorBtn.Location = new System.Drawing.Point(11, 69);
            this._textColorBtn.Name = "_textColorBtn";
            this._textColorBtn.Size = new System.Drawing.Size(125, 23);
            this._textColorBtn.TabIndex = 3;
            this._textColorBtn.Text = "Text Color and Font";
            this._textColorBtn.UseVisualStyleBackColor = true;
            this._textColorBtn.Click += new System.EventHandler(this._textColorBtn_Click);
            // 
            // _tabPageFile
            // 
            this._tabPageFile.Controls.Add(this._titleMatchFilenameChk);
            this._tabPageFile.Controls.Add(this._browseBtn);
            this._tabPageFile.Controls.Add(label10);
            this._tabPageFile.Controls.Add(label9);
            this._tabPageFile.Controls.Add(this._fileChangeCheckIntervalEdt);
            this._tabPageFile.Controls.Add(label8);
            this._tabPageFile.Controls.Add(this._fileCheckPatternChk);
            this._tabPageFile.Controls.Add(this._fileReopenCheckIntervalEdt);
            this._tabPageFile.Controls.Add(label6);
            this._tabPageFile.Controls.Add(label7);
            this._tabPageFile.Controls.Add(this._windowServiceEdt);
            this._tabPageFile.Controls.Add(this._fileCacheSizeEdt);
            this._tabPageFile.Controls.Add(label5);
            this._tabPageFile.Controls.Add(label4);
            this._tabPageFile.Controls.Add(this._fileEncodingCmb);
            this._tabPageFile.Controls.Add(label3);
            this._tabPageFile.Controls.Add(this._filePathEdt);
            this._tabPageFile.Location = new System.Drawing.Point(4, 22);
            this._tabPageFile.Name = "_tabPageFile";
            this._tabPageFile.Padding = new System.Windows.Forms.Padding(3);
            this._tabPageFile.Size = new System.Drawing.Size(359, 207);
            this._tabPageFile.TabIndex = 1;
            this._tabPageFile.Text = "Log File";
            this._tabPageFile.UseVisualStyleBackColor = true;
            // 
            // _titleMatchFilenameChk
            // 
            this._titleMatchFilenameChk.AutoSize = true;
            this._titleMatchFilenameChk.Location = new System.Drawing.Point(221, 30);
            this._titleMatchFilenameChk.Name = "_titleMatchFilenameChk";
            this._titleMatchFilenameChk.Size = new System.Drawing.Size(120, 17);
            this._titleMatchFilenameChk.TabIndex = 16;
            this._titleMatchFilenameChk.Text = "Title match filename";
            this._titleMatchFilenameChk.UseVisualStyleBackColor = true;
            // 
            // _browseBtn
            // 
            this._browseBtn.Location = new System.Drawing.Point(323, 6);
            this._browseBtn.Name = "_browseBtn";
            this._browseBtn.Size = new System.Drawing.Size(30, 20);
            this._browseBtn.TabIndex = 15;
            this._browseBtn.Text = "...";
            this._browseBtn.UseVisualStyleBackColor = true;
            this._browseBtn.Click += new System.EventHandler(this._browseBtn_Click);
            // 
            // _fileChangeCheckIntervalEdt
            // 
            this._fileChangeCheckIntervalEdt.Location = new System.Drawing.Point(144, 136);
            this._fileChangeCheckIntervalEdt.Name = "_fileChangeCheckIntervalEdt";
            this._fileChangeCheckIntervalEdt.Size = new System.Drawing.Size(59, 20);
            this._fileChangeCheckIntervalEdt.TabIndex = 12;
            // 
            // _fileCheckPatternChk
            // 
            this._fileCheckPatternChk.AutoSize = true;
            this._fileCheckPatternChk.Location = new System.Drawing.Point(103, 30);
            this._fileCheckPatternChk.Name = "_fileCheckPatternChk";
            this._fileCheckPatternChk.Size = new System.Drawing.Size(112, 17);
            this._fileCheckPatternChk.TabIndex = 8;
            this._fileCheckPatternChk.Text = "Regex in File Path";
            this._fileCheckPatternChk.UseVisualStyleBackColor = true;
            this._fileCheckPatternChk.CheckedChanged += new System.EventHandler(this._fileCheckPatternChk_CheckedChanged);
            // 
            // _fileReopenCheckIntervalEdt
            // 
            this._fileReopenCheckIntervalEdt.Location = new System.Drawing.Point(144, 110);
            this._fileReopenCheckIntervalEdt.Name = "_fileReopenCheckIntervalEdt";
            this._fileReopenCheckIntervalEdt.Size = new System.Drawing.Size(59, 20);
            this._fileReopenCheckIntervalEdt.TabIndex = 7;
            // 
            // _windowServiceEdt
            // 
            this._windowServiceEdt.Location = new System.Drawing.Point(103, 162);
            this._windowServiceEdt.Name = "_windowServiceEdt";
            this._windowServiceEdt.Size = new System.Drawing.Size(100, 20);
            this._windowServiceEdt.TabIndex = 10;
            // 
            // _fileCacheSizeEdt
            // 
            this._fileCacheSizeEdt.Location = new System.Drawing.Point(103, 81);
            this._fileCacheSizeEdt.Name = "_fileCacheSizeEdt";
            this._fileCacheSizeEdt.Size = new System.Drawing.Size(100, 20);
            this._fileCacheSizeEdt.TabIndex = 5;
            // 
            // _fileEncodingCmb
            // 
            this._fileEncodingCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._fileEncodingCmb.FormattingEnabled = true;
            this._fileEncodingCmb.Location = new System.Drawing.Point(103, 53);
            this._fileEncodingCmb.Name = "_fileEncodingCmb";
            this._fileEncodingCmb.Size = new System.Drawing.Size(250, 21);
            this._fileEncodingCmb.TabIndex = 3;
            // 
            // _filePathEdt
            // 
            this._filePathEdt.Location = new System.Drawing.Point(103, 6);
            this._filePathEdt.Name = "_filePathEdt";
            this._filePathEdt.Size = new System.Drawing.Size(214, 20);
            this._filePathEdt.TabIndex = 1;
            // 
            // _tabPageKeyWords
            // 
            this._tabPageKeyWords.Controls.Add(this._moveDownKeywordBtn);
            this._tabPageKeyWords.Controls.Add(this._moveUpKeywordBtn);
            this._tabPageKeyWords.Controls.Add(this._delWordBtn);
            this._tabPageKeyWords.Controls.Add(this._edtWordBtn);
            this._tabPageKeyWords.Controls.Add(this._addWordBtn);
            this._tabPageKeyWords.Controls.Add(this._keywordListView);
            this._tabPageKeyWords.Location = new System.Drawing.Point(4, 22);
            this._tabPageKeyWords.Name = "_tabPageKeyWords";
            this._tabPageKeyWords.Padding = new System.Windows.Forms.Padding(3);
            this._tabPageKeyWords.Size = new System.Drawing.Size(359, 207);
            this._tabPageKeyWords.TabIndex = 2;
            this._tabPageKeyWords.Text = "Keyword Highlight";
            this._tabPageKeyWords.UseVisualStyleBackColor = true;
            // 
            // _moveDownKeywordBtn
            // 
            this._moveDownKeywordBtn.Location = new System.Drawing.Point(276, 147);
            this._moveDownKeywordBtn.Name = "_moveDownKeywordBtn";
            this._moveDownKeywordBtn.Size = new System.Drawing.Size(75, 23);
            this._moveDownKeywordBtn.TabIndex = 11;
            this._moveDownKeywordBtn.Text = "Move Down";
            this._moveDownKeywordBtn.UseVisualStyleBackColor = true;
            this._moveDownKeywordBtn.Click += new System.EventHandler(this._moveDownKeywordBtn_Click);
            // 
            // _moveUpKeywordBtn
            // 
            this._moveUpKeywordBtn.Location = new System.Drawing.Point(276, 118);
            this._moveUpKeywordBtn.Name = "_moveUpKeywordBtn";
            this._moveUpKeywordBtn.Size = new System.Drawing.Size(75, 23);
            this._moveUpKeywordBtn.TabIndex = 10;
            this._moveUpKeywordBtn.Text = "Move Up";
            this._moveUpKeywordBtn.UseVisualStyleBackColor = true;
            this._moveUpKeywordBtn.Click += new System.EventHandler(this._moveUpKeywordBtn_Click);
            // 
            // _delWordBtn
            // 
            this._delWordBtn.Location = new System.Drawing.Point(276, 64);
            this._delWordBtn.Name = "_delWordBtn";
            this._delWordBtn.Size = new System.Drawing.Size(75, 23);
            this._delWordBtn.TabIndex = 4;
            this._delWordBtn.Text = "Remove";
            this._delWordBtn.UseVisualStyleBackColor = true;
            this._delWordBtn.Click += new System.EventHandler(this._delWordBtn_Click);
            // 
            // _edtWordBtn
            // 
            this._edtWordBtn.Location = new System.Drawing.Point(276, 35);
            this._edtWordBtn.Name = "_edtWordBtn";
            this._edtWordBtn.Size = new System.Drawing.Size(75, 23);
            this._edtWordBtn.TabIndex = 3;
            this._edtWordBtn.Text = "Edit...";
            this._edtWordBtn.UseVisualStyleBackColor = true;
            this._edtWordBtn.Click += new System.EventHandler(this._edtWordBtn_Click);
            // 
            // _addWordBtn
            // 
            this._addWordBtn.Location = new System.Drawing.Point(276, 6);
            this._addWordBtn.Name = "_addWordBtn";
            this._addWordBtn.Size = new System.Drawing.Size(75, 23);
            this._addWordBtn.TabIndex = 2;
            this._addWordBtn.Text = "Add...";
            this._addWordBtn.UseVisualStyleBackColor = true;
            this._addWordBtn.Click += new System.EventHandler(this._addWordBtn_Click);
            // 
            // _keywordListView
            // 
            this._keywordListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._keywordListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            keywordColumnHeader,
            keywordCaseColumnHeader,
            keywordRegexColumnHeader,
            keywordLogHitColumnHeader});
            this._keywordListView.FullRowSelect = true;
            this._keywordListView.HideSelection = false;
            this._keywordListView.Location = new System.Drawing.Point(6, 6);
            this._keywordListView.MultiSelect = false;
            this._keywordListView.Name = "_keywordListView";
            this._keywordListView.Size = new System.Drawing.Size(264, 195);
            this._keywordListView.TabIndex = 0;
            this._keywordListView.UseCompatibleStateImageBehavior = false;
            this._keywordListView.View = System.Windows.Forms.View.Details;
            this._keywordListView.DoubleClick += new System.EventHandler(this._edtWordBtn_Click);
            // 
            // _tabPageExtTools
            // 
            this._tabPageExtTools.Controls.Add(this._moveDownToolBtn);
            this._tabPageExtTools.Controls.Add(this._moveUpToolBtn);
            this._tabPageExtTools.Controls.Add(this._delToolBtn);
            this._tabPageExtTools.Controls.Add(this._editToolBtn);
            this._tabPageExtTools.Controls.Add(this._addToolBtn);
            this._tabPageExtTools.Controls.Add(this._extToolsListView);
            this._tabPageExtTools.Location = new System.Drawing.Point(4, 22);
            this._tabPageExtTools.Name = "_tabPageExtTools";
            this._tabPageExtTools.Padding = new System.Windows.Forms.Padding(3);
            this._tabPageExtTools.Size = new System.Drawing.Size(359, 207);
            this._tabPageExtTools.TabIndex = 3;
            this._tabPageExtTools.Text = "External Tools";
            this._tabPageExtTools.UseVisualStyleBackColor = true;
            // 
            // _moveDownToolBtn
            // 
            this._moveDownToolBtn.Location = new System.Drawing.Point(276, 147);
            this._moveDownToolBtn.Name = "_moveDownToolBtn";
            this._moveDownToolBtn.Size = new System.Drawing.Size(75, 23);
            this._moveDownToolBtn.TabIndex = 10;
            this._moveDownToolBtn.Text = "Move Down";
            this._moveDownToolBtn.UseVisualStyleBackColor = true;
            this._moveDownToolBtn.Click += new System.EventHandler(this._moveDownToolBtn_Click);
            // 
            // _moveUpToolBtn
            // 
            this._moveUpToolBtn.Location = new System.Drawing.Point(276, 118);
            this._moveUpToolBtn.Name = "_moveUpToolBtn";
            this._moveUpToolBtn.Size = new System.Drawing.Size(75, 23);
            this._moveUpToolBtn.TabIndex = 9;
            this._moveUpToolBtn.Text = "Move Up";
            this._moveUpToolBtn.UseVisualStyleBackColor = true;
            this._moveUpToolBtn.Click += new System.EventHandler(this._moveUpToolBtn_Click);
            // 
            // _delToolBtn
            // 
            this._delToolBtn.Location = new System.Drawing.Point(276, 64);
            this._delToolBtn.Name = "_delToolBtn";
            this._delToolBtn.Size = new System.Drawing.Size(75, 23);
            this._delToolBtn.TabIndex = 8;
            this._delToolBtn.Text = "Remove";
            this._delToolBtn.UseVisualStyleBackColor = true;
            this._delToolBtn.Click += new System.EventHandler(this._delToolBtn_Click);
            // 
            // _editToolBtn
            // 
            this._editToolBtn.Location = new System.Drawing.Point(276, 35);
            this._editToolBtn.Name = "_editToolBtn";
            this._editToolBtn.Size = new System.Drawing.Size(75, 23);
            this._editToolBtn.TabIndex = 7;
            this._editToolBtn.Text = "Edit...";
            this._editToolBtn.UseVisualStyleBackColor = true;
            this._editToolBtn.Click += new System.EventHandler(this._editToolBtn_Click);
            // 
            // _addToolBtn
            // 
            this._addToolBtn.Location = new System.Drawing.Point(276, 6);
            this._addToolBtn.Name = "_addToolBtn";
            this._addToolBtn.Size = new System.Drawing.Size(75, 23);
            this._addToolBtn.TabIndex = 6;
            this._addToolBtn.Text = "Add...";
            this._addToolBtn.UseVisualStyleBackColor = true;
            this._addToolBtn.Click += new System.EventHandler(this._addToolBtn_Click);
            // 
            // _extToolsListView
            // 
            this._extToolsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._extToolsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            toolNameColumnHeader,
            toolShortcutColumnHeader});
            this._extToolsListView.FullRowSelect = true;
            this._extToolsListView.HideSelection = false;
            this._extToolsListView.Location = new System.Drawing.Point(6, 6);
            this._extToolsListView.MultiSelect = false;
            this._extToolsListView.Name = "_extToolsListView";
            this._extToolsListView.Size = new System.Drawing.Size(264, 195);
            this._extToolsListView.TabIndex = 5;
            this._extToolsListView.UseCompatibleStateImageBehavior = false;
            this._extToolsListView.View = System.Windows.Forms.View.Details;
            this._extToolsListView.DoubleClick += new System.EventHandler(this._editToolBtn_Click);
            // 
            // _acceptBtn
            // 
            this._acceptBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._acceptBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._acceptBtn.Location = new System.Drawing.Point(198, 245);
            this._acceptBtn.Name = "_acceptBtn";
            this._acceptBtn.Size = new System.Drawing.Size(75, 23);
            this._acceptBtn.TabIndex = 14;
            this._acceptBtn.Text = "OK";
            this._acceptBtn.UseVisualStyleBackColor = true;
            this._acceptBtn.Click += new System.EventHandler(this._acceptBtn_Click);
            // 
            // _cancelBtn
            // 
            this._cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelBtn.Location = new System.Drawing.Point(290, 245);
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.Size = new System.Drawing.Size(75, 23);
            this._cancelBtn.TabIndex = 15;
            this._cancelBtn.Text = "Cancel";
            this._cancelBtn.UseVisualStyleBackColor = true;
            // 
            // _applyAllBtn
            // 
            this._applyAllBtn.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this._applyAllBtn.Location = new System.Drawing.Point(10, 245);
            this._applyAllBtn.Name = "_applyAllBtn";
            this._applyAllBtn.Size = new System.Drawing.Size(81, 23);
            this._applyAllBtn.TabIndex = 16;
            this._applyAllBtn.Text = "Apply to All";
            this._applyAllBtn.UseVisualStyleBackColor = true;
            // 
            // TailConfigForm
            // 
            this.AcceptButton = this._acceptBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelBtn;
            this.ClientSize = new System.Drawing.Size(377, 271);
            this.Controls.Add(this._applyAllBtn);
            this.Controls.Add(this._cancelBtn);
            this.Controls.Add(this._acceptBtn);
            this.Controls.Add(this._tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TailConfigForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure View";
            this.Load += new System.EventHandler(this.TailConfigForm_Load);
            this._tabControl.ResumeLayout(false);
            this._tabPageView.ResumeLayout(false);
            this._tabPageView.PerformLayout();
            this._tabPageFile.ResumeLayout(false);
            this._tabPageFile.PerformLayout();
            this._tabPageKeyWords.ResumeLayout(false);
            this._tabPageExtTools.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl _tabControl;
        private System.Windows.Forms.TabPage _tabPageView;
        private System.Windows.Forms.TextBox _windowIconEdt;
        private System.Windows.Forms.TextBox _windowTitleEdt;
        private System.Windows.Forms.Button _backColorBtn;
        private System.Windows.Forms.Button _textColorBtn;
        private System.Windows.Forms.TabPage _tabPageFile;
        private System.Windows.Forms.ComboBox _fileEncodingCmb;
        private System.Windows.Forms.TextBox _filePathEdt;
        private System.Windows.Forms.TextBox _fileCacheSizeEdt;
        private System.Windows.Forms.TextBox _windowServiceEdt;
        private System.Windows.Forms.Button _acceptBtn;
        private System.Windows.Forms.Button _cancelBtn;
        private System.Windows.Forms.TabPage _tabPageKeyWords;
        private System.Windows.Forms.ListView _keywordListView;
        private System.Windows.Forms.Button _edtWordBtn;
        private System.Windows.Forms.Button _addWordBtn;
        private System.Windows.Forms.Button _delWordBtn;
        private System.Windows.Forms.CheckBox _fileCheckPatternChk;
        private System.Windows.Forms.TextBox _fileReopenCheckIntervalEdt;
        private System.Windows.Forms.TextBox _fileChangeCheckIntervalEdt;
        private System.Windows.Forms.Button _browseBtn;
        private System.Windows.Forms.CheckBox _displayIconChk;
        private System.Windows.Forms.CheckBox _titleMatchFilenameChk;
        private System.Windows.Forms.TabPage _tabPageExtTools;
        private System.Windows.Forms.Button _delToolBtn;
        private System.Windows.Forms.Button _editToolBtn;
        private System.Windows.Forms.Button _addToolBtn;
        private System.Windows.Forms.ListView _extToolsListView;
        private System.Windows.Forms.Button _moveDownToolBtn;
        private System.Windows.Forms.Button _moveUpToolBtn;
        private System.Windows.Forms.Button _moveDownKeywordBtn;
        private System.Windows.Forms.Button _moveUpKeywordBtn;
        private System.Windows.Forms.Button _applyAllBtn;



    }
}