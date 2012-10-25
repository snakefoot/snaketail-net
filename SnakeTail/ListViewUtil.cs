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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SnakeTail
{
    public static class ListViewUtil
    {
        /// <summary>
        /// Performs <c>LVM_SETITEMCOUNT</c> on the given <c>ListView</c>.
        /// Also sets <c>LVSICF_NOINVALIDATEALL</c> and <c>LVSICF_NOSCROLL</c> flags
        /// to avoid expensive (and ugly) redrawing on frequent item additions.
        /// See http://msdn.microsoft.com/en-us/library/bb761188%28v=VS.85%29.aspx for more.
        /// </summary>
        public static void SetVirtualListSizeWithoutRefresh(ListView listView, int count)
        {
            SendMessage(listView.Handle,
                (int)ListViewMessages.LVM_SETITEMCOUNT,
                count,
                (int)(ListViewSetItemCountFlags.LVSICF_NOINVALIDATEALL |
                ListViewSetItemCountFlags.LVSICF_NOSCROLL));

            // The ListView.VirtualListSize property drives a private member
            // virtualListSize that is used in the implementation of
            // ListViewItemCollection (returned by ListView.Items) to validate
            // indices. If this is not updated, spurious ArgumentOutOfRangeExceptions
            // may be raised by functions and properties using the indexing
            // operator on ListView.Items, for instance FocusedItem.
            listViewVirtualListSizeField.SetValue(listView, count);
        }


        [Flags]
        private enum ListViewSetItemCountFlags
        {
            //#if (_WIN32_IE >= 0x0300)
            // these flags only apply to LVS_OWNERDATA listviews in report or list mode
            LVSICF_NOINVALIDATEALL = 0x00000001,
            LVSICF_NOSCROLL = 0x00000002,
            //#endif
        }

        private enum ListViewMessages
        {
            LVM_FIRST = 0x1000,      // ListView messages
            LVM_SETITEMCOUNT = (LVM_FIRST + 47),
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr handle, int messg, int wparam, int lparam);

        static ListViewUtil()
        {
            listViewVirtualListSizeField = typeof(ListView).GetField("virtualListSize", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            System.Diagnostics.Debug.Assert(listViewVirtualListSizeField != null, "System.Windows.Forms.ListView class no longer has a virtualListSize field.");
        }

        private static readonly System.Reflection.FieldInfo listViewVirtualListSizeField;
    }
}
