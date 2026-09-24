using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.UnitTests;

public class DocumentGenerationFailureTests
{
    [TestCase(0, TestName = "ExceptionThrownByOutputStreamIsPropagated(AtFirstWrite)")]
    [TestCase(20_000, TestName = "ExceptionThrownByOutputStreamIsPropagated(DuringGeneration)")]
    public void ExceptionThrownByOutputStreamIsPropagated(int failAfterBytes)
    {
        var document = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Content().Column(column =>
                {
                    column.Spacing(50);
                    
                    foreach (var _ in Enumerable.Range(0, 1_000))
                        column.Item().Width(10).Height(10).Background(Placeholders.BackgroundColor());
                });
            });
        });
        
        var exception = Assert.Throws<IOException>(() => document.GeneratePdf(new FailingStream(failAfterBytes)));
        Assert.That(exception.Message, Is.EqualTo(FailingStream.ErrorMessage));
    }

    private sealed class FailingStream(int failAfterBytes) : Stream
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
}
