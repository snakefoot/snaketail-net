using System.IO;
using System.IO.Compression;
using System.Text;

namespace SnakeTail
{
    // Must not declare instance fields: the base constructor calls the overridden methods before this constructor body runs
    class LogFileGZipStream : LogFileStream
    {
        public LogFileGZipStream(string configPath, string filePath, Encoding fileEncoding, int fileCheckFrequency, bool fileCheckPattern)
            : base(configPath, filePath, fileEncoding, fileCheckFrequency, fileCheckPattern)
        {
        }

        protected override StreamReader CreateFileReader(FileStream fileStream, Encoding fileEncoding)
        {
            // leaveOpen, so disposing the reader doesn't close the FileStream owned by the base class
            var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress, true);
            // StreamReader detects the BOM, which takes precedence over the specified encoding
            return new StreamReader(gzipStream, fileEncoding, true, 65536);
        }

        protected override void RewindFile(Encoding fileEncoding)
        {
            // GZipStream is not seekable, so restart decompression from the beginning of the file
            _fileStream.Seek(0, SeekOrigin.Begin);
            _fileReader.Dispose();
            _fileReader = CreateFileReader(_fileStream, fileEncoding);
        }
    }
}
