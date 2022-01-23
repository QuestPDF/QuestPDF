using System;
using System.IO;

namespace QuestPDF.Helpers
{
    /// <summary>
    /// SkiaSharp calls the Position property when generating target document file.
    /// If the output stream does not support the Position property, the NullReferenceException is thrown.
    /// This wrapper fixes this issue by providing cached Position value (always the end of the stream / current length).
    /// Example stream affected: HttpContext.Response.Body
    /// </summary>
    internal class WriteOnlyStream : Stream
    {
        private readonly Stream InnerStream;
        private long StreamLength { get; set; }

        public WriteOnlyStream(Stream stream)
        {            
            if (!stream.CanWrite)
                throw new NotSupportedException("Stream cannot be written");

            InnerStream = stream;
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => StreamLength;

        public override long Position
        { 
            get => StreamLength; 
            set => throw new NotImplementedException(); 
        }

        public override void Flush() => InnerStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) 
            => throw new NotImplementedException();

        public override long Seek(long offset, SeekOrigin origin) 
            => throw new NotImplementedException();

        public override void SetLength(long value) 
            => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            InnerStream.Write(buffer, offset, count);
            StreamLength += count;
        }
    }
}
