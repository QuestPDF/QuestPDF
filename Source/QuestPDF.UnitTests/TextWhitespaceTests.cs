using System;
using NUnit.Framework;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

public class TextWhitespaceTests
{
    private static byte[] GeneratePdf(string text)
    {
        return Document
            .Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Content().Text(text);
                });
            })
            .GeneratePdf();
    }

    [Test]
    public void WordFollowedByNewLineAndLongWhitespaceLine_IsRendered()
    {
        // A word followed by a newline and a series of spaces that exceeds the available
        // width used to throw a DocumentLayoutException, because the whitespace-only line
        // reported a width larger than the available space (issue #1436).
        var text = ".\n" + new string(' ', 5000);

        Assert.DoesNotThrow(() => GeneratePdf(text));
    }

    [Test]
    public void WhitespaceLineBetweenWords_IsRendered()
    {
        var text = ".\n" + new string(' ', 5000) + "\n.";

        Assert.DoesNotThrow(() => GeneratePdf(text));
    }

    [Test]
    public void LongWhitespaceLineFollowedByWord_IsRendered()
    {
        var text = new string(' ', 5000) + "\n.";

        Assert.DoesNotThrow(() => GeneratePdf(text));
    }

    [Test]
    public void WordFollowedByLongWhitespaceOnSameLine_IsRendered()
    {
        var text = "." + new string(' ', 5000);

        Assert.DoesNotThrow(() => GeneratePdf(text));
    }

    [Test]
    public void OnlyWhitespace_IsRendered()
    {
        var text = new string(' ', 5000);

        Assert.DoesNotThrow(() => GeneratePdf(text));
    }

    [Test]
    public void WordFollowedByNewLineAndLongTabLine_IsRendered()
    {
        var text = ".\n" + new string('\t', 2000);

        Assert.DoesNotThrow(() => GeneratePdf(text));
    }

    [Test]
    public void SingleCharacterWiderThanAvailableSpace_StillThrows()
    {
        // A single character that genuinely cannot fit within the available width must still
        // be reported as a layout error - it is not collapsible whitespace.
        Assert.Throws<DocumentLayoutException>(() =>
        {
            Document
                .Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(20, 200);
                        page.Margin(0);
                        page.DefaultTextStyle(style => style.FontSize(200));
                        page.Content().Text("W");
                    });
                })
                .GeneratePdf();
        });
    }
}
