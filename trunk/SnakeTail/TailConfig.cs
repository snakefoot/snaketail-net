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

    public class TailFileConfig
    {
        public string FilePath { get; set; }
        public string FileEncoding { get; set; }
        public int FileCacheSize { get; set; }
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
        public string LogHitText { get; set; }
        public bool ColumnFilterActive { get; set; }
        [XmlArray("ColumnFilters")]
        [XmlArrayItem("Filters")]
        public List<List<string>> ColumnFilters { get; set; }
   }
}
