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
    class LogFileStream
    {
        string _filePath = "";
        string _filePathAbsolute = "";
        Encoding _fileEncoding = Encoding.Default;
        FileStream _fileStream = null;
        StreamReader _fileReader = null;
        DateTime _lastFileCheck = DateTime.Now;
        int _lastLineNumber = 0;
        string _lastFileError;
        TimeSpan _fileCheckFrequency = TimeSpan.FromSeconds(10);
        bool _fileCheckPattern = false;

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

        public void CheckLogFile()
        {
            _lastFileCheck = DateTime.Now;

            if (_fileStream == null)
            {
                LoadFile(_filePathAbsolute, _fileEncoding, _fileCheckPattern);
            }
            else
            {
                string configPath = Path.GetDirectoryName(_filePathAbsolute);
                LogFileStream testLogFile = new LogFileStream(configPath, _filePathAbsolute, _fileEncoding, _fileCheckFrequency.Seconds, _fileCheckPattern);
                long checkLength = testLogFile.Length;
                string name = testLogFile._fileStream!=null ? testLogFile._fileStream.Name : null;
                testLogFile.LoadFile(null, _fileEncoding, _fileCheckPattern);  // Release the file handle

                if (checkLength < Length || _fileStream.Name != name)
                {
                    // The file have been renamed / deleted (reload new file)
                    LoadFile(_filePathAbsolute, _fileEncoding, _fileCheckPattern);
                }
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

        bool LoadFile(string filepath, Encoding fileEncoding, bool fileCheckPattern)
        {
            _lastFileError = "";

            if (fileCheckPattern)
            {
                string filename = Path.GetFileName(_filePathAbsolute);
                string directory = Path.GetDirectoryName(_filePathAbsolute);
                DirectoryInfo dir = new DirectoryInfo(directory);
                FileInfo[] files = dir.GetFiles(filename);
                FileInfo lastestFile = null;
                foreach (FileInfo file in files)
                {
                    if (lastestFile == null || lastestFile.LastWriteTime < file.LastWriteTime)
                        lastestFile = file;
                }
                if (lastestFile != null)
                    filepath = lastestFile.FullName;
                else
                {
                    _lastFileError = "No files matching pattern";
                    return false;
                }
            }

            if (_fileStream != null)
            {
                _fileStream.Dispose();
                _fileStream = null;
            }
            if (_fileReader != null)
            {
                _fileReader.Dispose();
                _fileReader = null;
            }

            _lastLineNumber = 0;
            if (String.IsNullOrEmpty(filepath))
            {
                _lastFileError = "No file path";
                return false;
            }

            try
            {
                _fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 65536, FileOptions.SequentialScan);
            }
            catch (ArgumentException ex)
            {
                _lastFileError = "Invalid argument for opening file - " + ex.Message;
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                _lastFileError = "Unauthorized Access";
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                _lastFileError = "Directory not found";
                return false;
            }
            catch (FileNotFoundException)
            {
                _lastFileError = "File not found";
                return false;
            }
            catch (IOException ex)
            {
                _lastFileError = ex.Message;
                return false;
            }
            _fileReader = new StreamReader(_fileStream, fileEncoding, true, 65536);
            _lastLineNumber = 0;
            return true;
        }

        public bool CloseToEnd(int lineCount)
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

        public string ReadLine(int lineNumber)
        {
            if (_fileReader == null || _fileStream == null)
            {
                // Check if file is available (once a second)
                if (_lastFileCheck != DateTime.Now)
                    CheckLogFile();

                if (lineNumber == 1)
                    return "Cannot open file: " + _filePathAbsolute + (String.IsNullOrEmpty(_lastFileError) ? "" : " (" + _lastFileError + ")");
                else
                    return null;
            }

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
                // Check if file has been renamed (once every 10 seconds)
                if (DateTime.Now.Subtract(_lastFileCheck) >= _fileCheckFrequency)
                    CheckLogFile();
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
    }
}
