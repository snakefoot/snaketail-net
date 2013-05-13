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
        public readonly ExternalToolConfig ToolConfig;
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
            ToolConfig = toolConfig;
            _fileParameters = fileParameters;
        }

        public void Execute()
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.Arguments = Environment.ExpandEnvironmentVariables(ToolConfig.Arguments);
            foreach (KeyValuePair<string, string> parameter in _fileParameters)
                startInfo.Arguments = startInfo.Arguments.Replace(parameter.Key, parameter.Value);
            startInfo.FileName = Environment.ExpandEnvironmentVariables(ToolConfig.Command);
            foreach (KeyValuePair<string, string> parameter in _fileParameters)
                startInfo.FileName = startInfo.FileName.Replace(parameter.Key, parameter.Value);
            startInfo.WorkingDirectory = Environment.ExpandEnvironmentVariables(ToolConfig.InitialDirectory);
            foreach (KeyValuePair<string, string> parameter in _fileParameters)
                startInfo.WorkingDirectory = startInfo.WorkingDirectory.Replace(parameter.Key, parameter.Value);
            if (ToolConfig.HideWindow)
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            if (ToolConfig.RunAsAdmin)
            {
                if (!IsAdministrator)
                    startInfo.Verb = "runas";
            }
            System.Diagnostics.Process.Start(startInfo);
        }

        private static bool IsAdministrator
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
