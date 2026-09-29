using System;
using System.IO;

namespace QuestPDF.UnitTests;

/// <summary>
/// An output stream that accepts the given number of bytes, and then throws on every write.
/// </summary>
internal sealed class FailingStream(int failAfterBytes) : Stream
{
    public const string ErrorMessage = "[QuestPDF] Stream writing failed.";

    private long WrittenBytes { get; set; }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (WrittenBytes + count > failAfterBytes)
            throw new IOException(ErrorMessage);

        WrittenBytes += count;
    }

    public override void Write(ReadOnlySpan<byte> buffer) => Write(buffer.ToArray(), 0, buffer.Length);

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => WrittenBytes;
    public override long Position { get => WrittenBytes; set => throw new NotSupportedException(); }

    public override void Flush() { }
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}
