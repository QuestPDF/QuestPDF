using System;
using System.IO;

namespace QuestPDF.Helpers
{
    internal class WriteStreamWrapper : Stream
    {
        private readonly Stream _innerStream;

        private long _length;

        public WriteStreamWrapper(Stream stream)
        {            
            if (!stream.CanWrite)
            {
                throw new NotSupportedException("Stream cannot be written");
            }

            _innerStream = stream;
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position { 
            get => _length; 
            set => throw new NotImplementedException(); 
        }

        public override void Flush() 
            => _innerStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) 
            => throw new NotImplementedException();

        public override long Seek(long offset, SeekOrigin origin) 
            => throw new NotImplementedException();

        public override void SetLength(long value) 
            => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
            _length += count;
        }
    }
}
