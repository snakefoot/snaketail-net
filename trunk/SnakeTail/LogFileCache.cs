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
using System.Windows.Forms;

namespace SnakeTail
{
    class LogFileCache
    {
        public int FirstIndex { get; set; }
        public List<ListViewItem> Items { get; set; }

        ListViewItem _lastCacheMissItem = null;
        int _lastCacheMissIndex = 0;

        public event EventHandler LoadingFileEvent;
        public event EventHandler FillCacheEvent;

        public LogFileCache(int cacheSize)
        {
            Items = new List<ListViewItem>();
            for (int i = 0; i < cacheSize; ++i)
                Items.Add(null);
            FirstIndex = 0;
        }

        public void SetLastCacheMiss(int index, ListViewItem item)
        {
            // Hvis vi bliver spurgt til elementer lige efter sidste element, saa flyt cache fremad
            if (FirstIndex + Items.Count == index)
            {
                PrepareCache(FirstIndex + 1, index, true);
                Items[index - FirstIndex] = item;
            }
            else
            {
                _lastCacheMissItem = item;
                _lastCacheMissIndex = index;
            }
        }

        public ListViewItem LookupCache(int index)
        {
            if (_lastCacheMissItem != null && _lastCacheMissIndex == index)
                return _lastCacheMissItem;

            if (index >= FirstIndex && index < FirstIndex + Items.Count)
            {
                if (Items[index - FirstIndex] != null)
                    return Items[index - FirstIndex];
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        public int FillCacheEndOfFile(LogFileStream logFileStream, int lineCount)
        {
            if (lineCount == 0 && !logFileStream.CloseToEnd(Items.Count))
            {
                do
                {
                    if (logFileStream.ReadLine(lineCount + 1) != null)
                        lineCount++;

                    // We have read a good part of the file
                    if (lineCount % Items.Count == 0)
                    {
                        if (LoadingFileEvent != null)
                            LoadingFileEvent(logFileStream, null);
                    }
                }
                while (!logFileStream.CloseToEnd(Items.Count));

                // We are almost finished with loading the file
                if (LoadingFileEvent != null)
                    LoadingFileEvent(null, null);
            }

            int lastCacheIndex = 0;
            do
            {
                string line = logFileStream.ReadLine(lineCount + 1);
                if (line == null)
                {
                    if (logFileStream.FileAtStart)
                        return 0;
                    return lineCount;
                }

                lineCount++;
                if (lastCacheIndex == Items.Count - 1)
                    PrepareCache(lineCount + Items.Count / 2, lineCount + Items.Count / 2, true);
                else
                    PrepareCache(lineCount - 1, lineCount - 1, true);
                Items[lineCount - FirstIndex - 1] = new ListViewItem(line);
                Items[lineCount - FirstIndex - 1].SubItems.Add("");
                lastCacheIndex = FillCache(logFileStream, FirstIndex + Items.Count);
                System.Diagnostics.Debug.Assert(lastCacheIndex != -1);
                lineCount = FirstIndex + lastCacheIndex + 1;
            }
            while (lastCacheIndex == Items.Count - 1);

            return lineCount;
        }

        public int FillCache(LogFileStream logFileStream, int endindex)
        {
            if (endindex > FirstIndex + Items.Count)
                return -1;

            int lastItem = Items.Count - 1;

            for (int i = 0; i < Items.Count; i++)
            {
                if (FirstIndex + i > endindex)
                {
                    lastItem = i - 1;
                    break;
                }

                if (Items[i] != null)
                    continue;   // Already cached

                if (i == 0)
                {
                    // We are filling the cache
                    if (FillCacheEvent != null)
                        FillCacheEvent(this, null);
                }

                string line = logFileStream.ReadLine(FirstIndex + i + 1);
                if (line == null)
                {
                    lastItem = i - 1;
                    break;
                }

                Items[i] = new ListViewItem(line);
                Items[i].SubItems.Add("");
            }

            // We are done filling the cache
            if (FillCacheEvent != null)
                FillCacheEvent(null, null);

            if (lastItem != Items.Count - 1)
                return lastItem;
            else
                return Items.Count - 1;
        }

        // The startindex is the important part
        public bool PrepareCache(int startindex, int endindex, bool mustBeFilled)
        {
            if (startindex >= FirstIndex && endindex < FirstIndex + Items.Count)
            {
                if (!mustBeFilled || Items[0] != null)
                {
                    if (LookupCache(endindex) == null)
                        return false;   // Cache needs to be filled
                    else
                        return true;    // Cache is ready
                }
            }

            if (startindex >= FirstIndex)
            {
                // Reading forward, we attempt to keep most of the cache
                int maxRemoveCount = startindex - FirstIndex;
                int minRemoveCount = Math.Max(0, maxRemoveCount - Items.Count + (endindex - startindex) + 1);
                if (minRemoveCount >= Items.Count)
                {
                    for (int i = 0; i < Items.Count; ++i)
                        Items[i] = null;

                    FirstIndex = startindex;
                }
                else
                    if (minRemoveCount > 0)
                    {
                        for (int i = 0; i < minRemoveCount; ++i)
                        {
                            Items.RemoveAt(0);
                            Items.Add(null);
                        }
                        FirstIndex += minRemoveCount;
                        while (mustBeFilled && Items[0] == null && FirstIndex < startindex)
                        {
                            Items.RemoveAt(0);
                            Items.Add(null);
                            FirstIndex++;
                        }
                    }
            }
            else
            {
                // Reading backward, we attempt to move further back as restart is expensive
                int firstIndex = Math.Max(0, startindex - Items.Count / 2);
                if (firstIndex + Items.Count < endindex)
                    firstIndex = Math.Min(startindex, endindex - Items.Count + 1);

                int removeCount = FirstIndex - firstIndex;
                if (removeCount >= Items.Count)
                {
                    for (int i = 0; i < Items.Count; ++i)
                        Items[i] = null;
                }
                else
                    if (removeCount > 0)
                    {
                        // Need to move elements FirstLine - StartIndex forward
                        for (int i = 0; i < removeCount; ++i)
                        {
                            Items.RemoveAt(Items.Count - 1);
                            Items.Insert(0, null);
                        }
                    }
                FirstIndex = firstIndex;
            }

            return false;// Cache needs to be filled
        }
    };
}
