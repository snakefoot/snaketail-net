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

namespace SnakeTail
{
    class ExternalTool
    {
        ExternalToolConfig _toolConfig;
        Dictionary<string, string> _fileParameters;

        public static readonly List<string> ParamList = new List<string> {
            "$(FilePath)",
            "$(FileDirectory)",
            "$(FileName)",
            "$(LineText)",
            "$(LineNumber)",
            "$(ProgramDirectory)",
            "$(ServiceName)",
            "$(SessionDirectory)",
            "$(SessionFileName)",
            "$(SessionName)",
            "$(SessionPath)",
            "$(ViewName)"};

        public ExternalTool(ExternalToolConfig toolConfig, Dictionary<string, string> fileParameters)
        {
            _toolConfig = toolConfig;
            _fileParameters = fileParameters;
        }

        public void Execute()
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.Arguments = _toolConfig.Arguments;
            foreach (KeyValuePair<string, string> paramter in _fileParameters)
                startInfo.Arguments = startInfo.Arguments.Replace(paramter.Key, paramter.Value);
            startInfo.FileName = _toolConfig.Command;
            foreach (KeyValuePair<string, string> paramter in _fileParameters)
                startInfo.FileName = startInfo.FileName.Replace(paramter.Key, paramter.Value);
            startInfo.WorkingDirectory = _toolConfig.InitialDirectory;
            foreach (KeyValuePair<string, string> paramter in _fileParameters)
                startInfo.WorkingDirectory = startInfo.WorkingDirectory.Replace(paramter.Key, paramter.Value);
            if (_toolConfig.HideWindow)
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            if (_toolConfig.RunAsAdmin)
            {
                if (!IsAdministrator)
                    startInfo.Verb = "runas";
            }
            System.Diagnostics.Process.Start(startInfo);
        }

        private bool IsAdministrator
        {
            get
            {
                System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();
                System.Security.Principal.WindowsPrincipal wp = new System.Security.Principal.WindowsPrincipal(wi);

                return wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }
    }
}
