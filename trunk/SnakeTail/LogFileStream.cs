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
using System.IO;
using System.Text;

namespace SnakeTail
{
    class LogFileStream : IDisposable
    {
        string _filePath = "";
        string _filePathAbsolute = "";
        Encoding _fileEncoding = Encoding.Default;
        FileStream _fileStream = null;
        StreamReader _fileReader = null;
        ThreadPoolQueue _threadPool = new ThreadPoolQueue();
        DateTime _lastFileCheck = DateTime.Now;
        int _lastLineNumber = 0;
        string _lastFileCheckError = "";
        long _lastFileCheckLength = 0;
        TimeSpan _fileCheckFrequency = TimeSpan.FromSeconds(10);
        bool _fileCheckPattern = false;

        public event EventHandler FileReloadedEvent;

        public LogFileStream(string configPath, string filePath, Encoding fileEncoding, int fileCheckFrequency, bool fileCheckPattern)
        {
            _fileEncoding = fileEncoding;
            _filePath = filePath;
            _filePathAbsolute = Path.Combine(configPath, _filePath);
            if (fileCheckFrequency > 0)
                _fileCheckFrequency = TimeSpan.FromSeconds(fileCheckFrequency);
            _fileCheckPattern = fileCheckPattern;
            LoadFile(_filePathAbsolute, _fileEncoding, _fileCheckPattern);
        }

        public void Reset()
        {
            FileReloadedEvent = null;
        }

        public void CheckLogFile(bool forceReload)
        {
            _lastFileCheck = DateTime.Now;

            try
            {
                // Refreshes the directory of the file, to ensure that we see the latest changes
                // If the directory is on a network share, then this can be a long blocking operation
                _threadPool.CheckResult();
                DirectoryInfo dirInfo = null;
                try
                {
                    dirInfo = new DirectoryInfo(Path.GetDirectoryName(_filePathAbsolute));
                }
                catch (System.ArgumentException ex)
                {
                    // Any problems with the path should also be detected with the synchronous LoadFile-check
                    System.Diagnostics.Debug.WriteLine("Failed to refresh directory path: " + ex.Message);
                }
                catch (System.Security.SecurityException ex)
                {
                    // Any problems with the path should also be detected with the synchronous LoadFile-check
                    System.Diagnostics.Debug.WriteLine("Failed to access directory path: " + ex.Message);
                }
                catch (System.IO.IOException ex)
                {
                    // Any problems with the path should also be detected with the synchronous LoadFile-check
                    System.Diagnostics.Debug.WriteLine("Failed to read directory path: " + ex.Message);
                }
                if (dirInfo != null)
                    _threadPool.ExecuteRequest(RefreshDirectoryInfo, dirInfo);
            }
            catch (ApplicationException ex)
            {
                // Any problems with the path should also be detected with the synchronous LoadFile-check
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            if (_fileStream == null || forceReload)
            {
                LoadFile(_filePathAbsolute, _fileEncoding, _fileCheckPattern);
                if (_fileStream != null || forceReload)
                {
                    if (FileReloadedEvent != null)
                        FileReloadedEvent(this, null);
                }
            }
            else
            {
                string configPath = Path.GetDirectoryName(_filePathAbsolute);
                bool fileChanged = false;
                long fileCheckLength = 0;

                using (LogFileStream testLogFile = new LogFileStream(configPath, _filePathAbsolute, _fileEncoding, _fileCheckFrequency.Seconds, _fileCheckPattern))
                {
                    fileCheckLength = testLogFile.Length;
                    string name = testLogFile._fileStream != null ? testLogFile._fileStream.Name : null;

                    if (fileCheckLength < Length)
                        fileChanged = true;
                    else if (Position > fileCheckLength)
                        fileChanged = true;
                    else if (_fileStream.Name != name)
                        fileChanged = true;
                    else if (_lastFileCheckLength <= fileCheckLength && _lastFileCheckLength > Length)
                        fileChanged = true;
                }

                if (fileChanged)
                {
                    // The file have been renamed / deleted (reload new file)
                    LoadFile(_filePathAbsolute, _fileEncoding, _fileCheckPattern);
                    if (FileReloadedEvent != null)
                        FileReloadedEvent(this, null);
                }
                _lastFileCheckLength = fileCheckLength;
            }
        }

        static void RefreshDirectoryInfo(object state)
        {
            try
            {
                DirectoryInfo directoryInfo = state as DirectoryInfo;
                if (directoryInfo != null)
                    directoryInfo.Refresh();
            }
            catch (System.IO.IOException ex)
            {
                throw new ApplicationException("Failed to refresh directory path: " + ex.Message, ex);
            }
        }

        public bool FileAtStart
        {
            get { return Length > 0 && _lastLineNumber == 0; }
        }

        public long Length
        {
            get { return _fileStream != null ? _fileStream.Length : 0; }
        }

        public long Position
        {
            get { return _fileStream != null ? _fileStream.Position : 0; }
        }

        public string Name
        {
            get { return _fileStream != null ? _fileStream.Name : null; }
        }

        public Encoding FileEncoding
        {
            get { return _fileEncoding; }
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public string FilePathAbsolute
        {
            get { return _filePathAbsolute; }
        }

        public int FileCheckInterval
        {
            get { return _fileCheckFrequency.Seconds; }
        }

        public bool FileCheckPattern
        {
            get { return _fileCheckPattern; }
        }

        public bool ValidLineCount(int lineCount)
        {
            if (_fileStream != null && _lastLineNumber == lineCount)
                return true;
            else
                return false;
        }

        static string FindFileUsingPattern(string filePathAbsolute)
        {
            // Consider using FileSystemWatcher
            string filename = Path.GetFileName(filePathAbsolute);
            string directory = Path.GetDirectoryName(filePathAbsolute);
            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] files = dir.GetFiles(filename);
            FileInfo lastestFile = null;
            foreach (FileInfo file in files)
            {
                if (lastestFile == null || lastestFile.LastWriteTime < file.LastWriteTime)
                    lastestFile = file;
            }
            if (lastestFile != null)
                return lastestFile.FullName;
            else
                return null;
        }

        public void Dispose()
        {
            FileReloadedEvent = null;

            _threadPool.Dispose();

            CloseFile(false);
        }

        void CloseFile(bool publishEvent)
        {
            _lastFileCheckError = "";
            _lastLineNumber = 0;

            bool closedFile = false;
            if (_fileReader != null)
            {
                _fileReader.Dispose();
                _fileReader = null;
                closedFile = true;
            }
            if (_fileStream != null)
            {
                _fileStream.Dispose();
                _fileStream = null;
                closedFile = true;
            }

            if (publishEvent && closedFile)
            {
                if (FileReloadedEvent != null)
                    FileReloadedEvent(this, null);
            }
        }

        bool LoadFile(string filepath, Encoding fileEncoding, bool fileCheckPattern)
        {
            CloseFile(false);

            if (String.IsNullOrEmpty(filepath))
            {
                _lastFileCheckError = "No file path";
                return false;
            }
            else
            if (fileCheckPattern)
            {
                try
                {
                    filepath = FindFileUsingPattern(filepath);
                    if (filepath == null)
                    {
                        _lastFileCheckError = "No files matching pattern";
                        return false;
                    }
                }
                catch (ArgumentException ex)
                {
                    _lastFileCheckError = "Invalid file matching pattern path - " + ex.Message;
                    return false;
                }
                catch (DirectoryNotFoundException)
                {
                    _lastFileCheckError = "Directory not found";
                    return false;
                }
                catch (System.Security.SecurityException)
                {
                    _lastFileCheckError = "No permission to list files using matching pattern";
                    return false;
                }
                catch (IOException ex)
                {
                    _lastFileCheckError = ex.Message;
                    return false;
                }
            }

            try
            {
                _fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 65536, FileOptions.SequentialScan);
            }
            catch (ArgumentException ex)
            {
                _lastFileCheckError = "Invalid argument for opening file - " + ex.Message;
                return false;
            }
            catch (System.Security.SecurityException)
            {
                _lastFileCheckError = "No permission to open the file";
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                _lastFileCheckError = "Access to the file is denied";
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                _lastFileCheckError = "Directory not found";
                return false;
            }
            catch (FileNotFoundException)
            {
                _lastFileCheckError = "File not found";
                return false;
            }
            catch (IOException ex)
            {
                _lastFileCheckError = ex.Message;
                return false;
            }

            _fileReader = new StreamReader(_fileStream, fileEncoding, true, 65536);

            try
            {
                if (!_fileReader.EndOfStream)
                    _lastFileCheckError = "";
            }
            catch (System.Security.SecurityException)
            {
                CloseFile(true);
                _lastFileCheckError = "No permission to read the file";
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                CloseFile(true);
                _lastFileCheckError = "Read access to the file is denied";
                return false;
            }
            catch (System.IO.IOException ex)
            {
                CloseFile(true);
                _lastFileCheckError = ex.Message;
                return false;
            }

            _lastLineNumber = 0;
            return true;
        }

        public bool CloseToEnd(int lineCount)
        {
            try
            {
                if (_fileReader == null)
                    return true;

                if (_fileReader.EndOfStream)
                    return true;

                if (_fileStream.Length <= _fileStream.Position + lineCount * 80)
                    return true;
                else
                    return false;
            }
            catch (System.IO.IOException)
            {
                CloseFile(true);
                return true;    // File is non-existing (empty)
            }
        }

        public string ReadLine(int lineNumber)
        {
            if (_fileReader == null || _fileStream == null)
            {
                // Check if file is available (once a second)
                if (_lastFileCheck != DateTime.Now)
                    CheckLogFile(true);

                if (lineNumber == 1)
                    return "Cannot open file: " + _filePathAbsolute + (String.IsNullOrEmpty(_lastFileCheckError) ? "" : " (" + _lastFileCheckError + ")");
                else
                    return null;
            }

            try
            {
                if (lineNumber <= _lastLineNumber)
                {
                    _fileStream.Seek(0, SeekOrigin.Begin);
                    _fileReader.DiscardBufferedData();
                    _lastLineNumber = 0;
                }
                else
                {
                    lineNumber -= _lastLineNumber;
                }

                if (_fileReader.EndOfStream)
                {
                    // Check if file has been renamed/truncated (once every 10 seconds)
                    if (DateTime.Now.Subtract(_lastFileCheck) >= _fileCheckFrequency)
                        CheckLogFile(false);
                    return null;
                }

                string line = null;
                for (int i = 0; i < lineNumber && !_fileReader.EndOfStream; ++i)
                {
                    line = _fileReader.ReadLine();
                    if (line == null)
                        return null;

                    _lastLineNumber++;
                }

                _lastFileCheck = DateTime.Now;
                return line;
            }
            catch (System.UnauthorizedAccessException ex)
            {
                CloseFile(true);
                if (lineNumber == 1)
                    return "Cannot read file: " + _filePathAbsolute + " (" + ex.Message + ")";
                return null;
            }
            catch (System.IO.IOException ex)
            {
                CloseFile(true);
                if (lineNumber == 1)
                    return "Cannot read file: " + _filePathAbsolute + " (" + ex.Message + ")";
                return null;
            }
        }
    }
}
