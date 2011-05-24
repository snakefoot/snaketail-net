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
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace SnakeTail
{
    public class TailConfig
    {
        List<TailFileConfig> _tailFiles = new List<TailFileConfig>();

        public List<TailFileConfig> TailFiles { get { return _tailFiles; } set { _tailFiles = value; } }
        public int SelectedTab { get; set; }
        public System.Drawing.Size WindowSize { get; set; }
        public System.Drawing.Point WindowPosition { get; set; }
        public bool MinimizedToTray { get; set; }
    }

    public class TailKeywordConfig
    {
        public string Keyword { get; set; }
        public bool MatchCaseSensitive { get; set; }
        public bool MatchRegularExpression { get; set; }
        public bool LogHitCounter { get; set; }
        public string TextColor { get; set; }   // ColorTranslator
        public string BackColor { get; set; }   // ColorTranslator

        internal Color? FormTextColor
        {
            get
            {
                if (TextColor != null)
                    return ColorTranslator.FromHtml(TextColor);
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    TextColor = ColorTranslator.ToHtml(value.Value);
                else
                    TextColor = null;
            }
        }

        internal Color? FormBackColor
        {
            get
            {
                if (BackColor != null)
                    return ColorTranslator.FromHtml(BackColor);
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    BackColor = ColorTranslator.ToHtml(value.Value);
                else
                    BackColor = null;
            }
        }

        internal System.Text.RegularExpressions.Regex KeywordRegex { get; set; }
    }

    public class TailFileConfig
    {
        public string FilePath { get; set; }
        public string FileEncoding { get; set; }
        public int FileCacheSize { get; set; }
        public int FileCheckInterval { get; set; }
        public int FileChangeCheckInterval { get; set; }
        public bool FileCheckPattern { get; set; }
        public string TextColor { get; set; }   // ColorTranslator
        public string BackColor { get; set; }   // ColorTranslator
        public string Font { get; set; }        // TypeConverter
        public bool Modeless { get; set; }
        public string Title { get; set; }
        public System.Windows.Forms.FormWindowState WindowState { get; set; }
        public System.Drawing.Size WindowSize { get; set; }
        public System.Drawing.Point WindowPosition { get; set; }
        public string ServiceName { get; set; }
        public string IconFile { get; set; }
        public bool ColumnFilterActive { get; set; }
        [XmlArray("ColumnFilters")]
        [XmlArrayItem("Filters")]
        public List<List<string>> ColumnFilters { get; set; }
        [XmlArray("KeywordHighlight")]
        [XmlArrayItem("Keyword")]
        public List<TailKeywordConfig> KeywordHighlight { get; set; }

        internal Font FormFont
        {
            get
            {
                if (Font != null)
                {
                    TypeConverter fontConverter = TypeDescriptor.GetConverter(typeof(Font));
                    return (Font)fontConverter.ConvertFromString(Font);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                TypeConverter fontConverter = TypeDescriptor.GetConverter(typeof(Font));
                Font = fontConverter.ConvertToString(value);
            }
        }

        internal Encoding EnumFileEncoding
        {
            get
            {
                if (FileEncoding == Encoding.UTF8.ToString())
                    return Encoding.UTF8;
                else
                if (FileEncoding == Encoding.ASCII.ToString())
                    return Encoding.ASCII;
                else
                if (FileEncoding == Encoding.Unicode.ToString())
                    return Encoding.Unicode;
                else
                    return Encoding.Default;
            }
            set
            {
                FileEncoding = value.ToString();
            }
        }

        internal Color? FormTextColor
        {
            get
            {
                if (TextColor != null)
                    return ColorTranslator.FromHtml(TextColor);
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    TextColor = ColorTranslator.ToHtml(value.Value);
                else
                    TextColor = null;
            }
        }

        internal Color? FormBackColor
        {
            get
            {
                if (BackColor != null)
                    return ColorTranslator.FromHtml(BackColor);
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    BackColor = ColorTranslator.ToHtml(value.Value);
                else
                    BackColor = null;
            }
        }
   }
}
