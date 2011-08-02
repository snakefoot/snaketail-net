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
    class CPUMeter : IDisposable
    {
        CounterSample _prevSample;
        PerformanceCounter _cnt;

        /// Creates a per-process CPU meter instance tied to the current process.
        public CPUMeter()
        {
            String instancename = GetCurrentProcessInstanceName();
            _cnt = new PerformanceCounter("Process", "% Processor Time", instancename, true);
            ResetCounter();
        }

        /// Creates a per-process CPU meter instance tied to a specific process.
        public CPUMeter(int pid)
        {
            String instancename = GetProcessInstanceName(pid);
            _cnt = new PerformanceCounter("Process", "% Processor Time", instancename, true);
            ResetCounter();
        }

        /// Resets the internal counter. All subsequent calls to GetCpuUtilization() will 
        /// be relative to the point in time when you called ResetCounter(). This 
        /// method can be call as often as necessary to get a new baseline for 
        /// CPU utilization measurements.
        public void ResetCounter()
        {
            _prevSample = _cnt.NextSample();
        }

        /// Returns this process's CPU utilization since the last call to ResetCounter().
        public float GetCpuUtilization()
        {
            CounterSample curr = _cnt.NextSample();
            float cpuutil = CounterSample.Calculate(_prevSample, curr);
            _prevSample = curr;
            return cpuutil;
        }

        private static string GetCurrentProcessInstanceName()
        {
            using (Process proc = Process.GetCurrentProcess())
            {
                int pid = proc.Id;
                return GetProcessInstanceName(pid);
            }
        }

        private static string GetProcessInstanceName(int pid)
        {
            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");

            string[] instances = cat.GetInstanceNames();
            foreach (string instance in instances)
            {
                using (PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instance, true))
                {
                    int val = (int)cnt.RawValue;
                    if (val == pid)
                    {
                        return instance;
                    }
                }
            }
            throw new Exception("Could not find performance counter instance name for current process. This is truly strange ...");
        }

        public void Dispose()
        {
            if (_cnt != null) _cnt.Dispose();
        }
    }
}
