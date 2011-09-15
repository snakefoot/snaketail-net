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
using System.Text;
using System.Windows.Forms;

namespace SnakeTail
{
    public interface ITailForm
    {
        Form TailWindow { get; }

        bool SearchForText(string searchText, bool matchCase, bool searchForward, bool keywordHighlights);

        void SaveConfig(TailFileConfig config);

        void LoadConfig(TailFileConfig config, string configPath);
    }
}
