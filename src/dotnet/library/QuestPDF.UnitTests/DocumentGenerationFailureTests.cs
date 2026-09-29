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
}
