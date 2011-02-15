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

        public LogFileStream(string configPath, string filePath, Encoding fileEncoding)
        {
            _filePath = filePath;
            _filePathAbsolute = Path.Combine(configPath, _filePath);
            _fileEncoding = fileEncoding;

            LoadFile(_filePathAbsolute, _fileEncoding);
        }

        public void CheckLogFile()
        {
            _lastFileCheck = DateTime.Now;

            if (_fileStream == null)
            {
                LoadFile(_filePathAbsolute, _fileEncoding);
            }
            else
            {
                string configPath = Path.GetDirectoryName(_filePathAbsolute);
                LogFileStream testLogFile = new LogFileStream(configPath, _filePathAbsolute, _fileEncoding);
                long checkLength = testLogFile.Length;
                testLogFile.LoadFile(null, _fileEncoding);  // Release the file handle

                if (checkLength < Length)
                {
                    // The file have been renamed / deleted (reload new file)
                    LoadFile(_filePathAbsolute, _fileEncoding);
                }
            }
        }

        public string LogHitText { get; set; }
        public int LogHitTextCount { get; set; }

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

        public bool ValidLineCount(int lineCount)
        {
            if (_fileStream != null && _lastLineNumber == lineCount)
                return true;
            else
                if (_fileStream == null && lineCount == 1)
                    return true;    // File not found message is ok
                else
                    return false;
        }

        bool LoadFile(string filepath, Encoding fileEncoding)
        {
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
            if (filepath == null)
                return false;

            try
            {
                _fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 65536, FileOptions.SequentialScan);
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            catch (FileNotFoundException)
            {
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
                    return "Cannot open file: " + _filePathAbsolute;
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
                if (DateTime.Now.Subtract(_lastFileCheck).Seconds >= 10)
                    CheckLogFile();
                return null;
            }

            string line = null;
            for (int i = 0; i < lineNumber && !_fileReader.EndOfStream; ++i)
            {
                line = _fileReader.ReadLine();
                if (line == null)
                    return null;

                if (LogHitText != null && line.IndexOf(LogHitText) != -1)
                    LogHitTextCount++;

                _lastLineNumber++;
            }

            _lastFileCheck = DateTime.Now;
            return line;
        }
    }
}
