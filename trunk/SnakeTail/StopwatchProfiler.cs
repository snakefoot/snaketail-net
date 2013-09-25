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
using System.Diagnostics;
using System.Text;

namespace SnakeTail
{
    public struct StopwatchProfiler : IDisposable
    {
        Stopwatch _stopWatch;
        string _identifier;

        class Profile
        {
            public Profile(TimeSpan elapsed)
            {
                Elapsed = elapsed;
                Count = 1;
            }
            public TimeSpan Elapsed { get; set; }
            public int Count { get; set; }
        }

        static Dictionary<string, Profile> _profiles = new Dictionary<string, Profile>();

        public StopwatchProfiler(string identifier)
        {
            _identifier = identifier;
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }

        public static void ClearReports()
        {
            _profiles.Clear();
        }

        public void Dispose()
        {
            _stopWatch.Stop();
            if (_profiles.ContainsKey(_identifier))
            {
                _profiles[_identifier].Elapsed += _stopWatch.Elapsed;
                _profiles[_identifier].Count += 1;
            }
            else
            {
                _profiles.Add(_identifier, new Profile(_stopWatch.Elapsed));
            }
        }

        static public string GetReport()
        {
            StringBuilder result = new StringBuilder();
            foreach (KeyValuePair<string, Profile> profile in _profiles)
            {
                result.AppendLine(profile.Key + " - " + profile.Value.Elapsed.ToString() + " (" + profile.Value.Count.ToString() + ")");
            }
            return result.ToString();
        }
    }
}
