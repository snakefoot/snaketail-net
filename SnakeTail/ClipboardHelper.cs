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
    sealed internal class ClipboardHelper
    {
        public static void CopyToClipboard(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            try
            {
                Clipboard.SetText(content);
            }
            catch (Exception firstException)
            {
                System.Diagnostics.Debug.WriteLine("Clipboard cannot be updated, maybe locked by another application: " + firstException.Message);
                try
                {
                    // Perform a last retry as recommended on stackoverflow
                    System.Threading.Thread.Sleep(100);
                    Clipboard.Clear();
                    System.Threading.Thread.Sleep(100);
                    Clipboard.SetDataObject(content, true, 0, 0);
                    return;
                }
                catch (Exception retryException)
                {
                    System.Diagnostics.Debug.WriteLine("Clipboard cannot be updated, maybe locked by another application: " + retryException.Message);
                }
                throw;
            }
        }
    }
}
