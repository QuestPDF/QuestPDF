using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests;

// the tests change the global caching setting
[TestFixture, NonParallelizable]
public class HyperlinkTests
{
    private const string Url = "https://example.com/questpdf";

    // pages are 100x100
    // the tests mirror SectionLinkTests, both link elements follow the same rules

    // the document is rendered twice: with caching, the second pass replays the drawing operations recorded in the first one,
    // without caching, the elements draw themselves again, including the EnsureSpace and PreventPageBreak elements inside the link

    [Test]
    public void LinkCoversContentAndOpensUrl()
    {
        var links = GetLinks(page =>
        {
            // the alignment offers the link exactly the space of its content
            page.Content().Padding(10).AlignLeft().AlignTop().Hyperlink(Url).Width(40).Height(20);
        });

        // PDF coordinates start at the bottom-left corner of the page
        Assert.That(links, Is.EqualTo(new[] { new[] { new LinkAnnotation(10, 70, 50, 90, Url, null) } }));
    }

    [Test]
    public void LinkCoversEntireSpaceOfferedToIt()
    {
        var links = GetLinks(page =>
        {
            // the content is centered, the link covers the entire width offered to it, which contains the content wherever it is placed
            page.Content().AlignTop().Hyperlink(Url).AlignCenter().Width(20).Height(20);
        });

        Assert.That(links, Is.EqualTo(new[] { new[] { new LinkAnnotation(0, 80, 100, 100, Url, null) } }));
    }

    [Test]
    public void LinkCoversSameSpaceInRightToLeftDirection()
    {
        var links = GetLinks(page =>
        {
            // the content is placed at the right edge
            page.Content().AlignTop().ContentFromRightToLeft().Hyperlink(Url).Width(20).Height(20);
        });

        Assert.That(links, Is.EqualTo(new[] { new[] { new LinkAnnotation(0, 80, 100, 100, Url, null) } }));
    }

    [Test]
    public void TextLinkOpensUrl()
    {
        // without tagging, the document has no semantic tree, so the link is created without a Link element
        var links = GetLinks(page =>
        {
            page.Content().Text(text => text.Hyperlink("Link", Url));
        });

        Assert.That(links.Single().Select(x => x.Url), Is.EqualTo(new[] { Url }));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ContentSplitAcrossPagesGetsLinkOnEachPage(bool enableCaching)
    {
        var links = GetLinks(page =>
        {
            // only the first two items fit on the first page
            page.Content().Hyperlink(Url).Column(column =>
            {
                foreach (var _ in Enumerable.Range(0, 3))
                    column.Item().Height(40);
            });
        }, enableCaching);

        Assert.That(links.Select(x => x.Count), Is.EqualTo(new[] { 1, 1 }));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void TextSplitAcrossPagesGetsEachLinkOnlyOnPageWithItsText(bool enableCaching)
    {
        var links = GetLinks(page =>
        {
            // the paragraph has 12 lines, half of them fit on each page
            page.Content().Text(text =>
            {
                text.Hyperlink("First", "https://example.com/first");
                text.Span(string.Concat(Enumerable.Repeat("\nText", 10)) + "\n");
                text.Hyperlink("Last", "https://example.com/last");
            });
        }, enableCaching);

        var urls = links.Select(x => x.Select(link => link.Url));
        Assert.That(urls, Is.EqualTo(new[] { new[] { "https://example.com/first" }, new[] { "https://example.com/last" } }));

        // the text on the next page is shifted up by the height drawn so far, and so are its links
        Assert.That(links.SelectMany(x => x), Has.All.Matches<LinkAnnotation>(x => x.Bottom >= 0 && x.Top <= 100));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void PostponedContentGetsNoLinkAreaOnPreviousPage(bool enableCaching)
    {
        var links = GetLinks(page =>
        {
            page.Content().Column(column =>
            {
                column.Item().Height(60);

                // the row is as tall as its first item (30), and offers that height to the link
                column.Item().Row(row =>
                {
                    row.ConstantItem(50).Height(30);

                    // only the first item (20) fits in the remaining space (40), less than the required 45
                    row.RelativeItem().Hyperlink(Url).EnsureSpace(45).Column(content =>
                    {
                        content.Item().Height(20);
                        content.Item().Height(30);
                    });
                });
            });
        }, enableCaching);

        Assert.That(links.Select(x => x.Count), Is.EqualTo(new[] { 0, 1 }));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ContentTallerThanPageIsPostponedOnlyOnce(bool enableCaching)
    {
        var links = GetLinks(page =>
        {
            // the content cannot fit on any page, PreventPageBreak postpones it once and then lets it span pages
            // TODO: known limitation, the first page stays blank, see ColumnTests.PostponedItemWithHorizontalPaddingInsideRow
            page.Content().Hyperlink(Url).PreventPageBreak().Column(column =>
            {
                column.Item().Height(60);
                column.Item().Height(60);
            });
        }, enableCaching);

        Assert.That(links.Select(x => x.Count), Is.EqualTo(new[] { 0, 1, 1 }));
    }

    [Test]
    public void ContentThatDoesNotFitIsNotDrawn()
    {
        // parents do not draw content that does not fit, the link ignores such a call:
        // the mock canvas throws when a link is drawn, and the mock child fails the test when it is drawn
        TestPlan
            .For(x => new Hyperlink
            {
                Child = x.CreateChild(),
                Url = Url
            })
            .DrawElement(new Size(400, 300))
            .ExpectChildMeasure(new Size(400, 300), SpacePlan.Wrap("Mock"))
            .CheckDrawResult();
    }

    [Test]
    public void SkippedContentGetsNoLink()
    {
        var links = GetLinks(page =>
        {
            page.Content().Row(row =>
            {
                // the row spans two pages, because of its first item, and draws its second item on both of them
                row.ConstantItem(50).Column(column =>
                {
                    column.Item().Height(100);
                    column.Item().Height(30);
                });

                // the content is skipped on the first page, and still receives the drawing call there, so it appears on the second page
                row.RelativeItem().Hyperlink(Url).SkipOnce().Height(20);
            });
        });

        Assert.That(links.Select(x => x.Count), Is.EqualTo(new[] { 0, 1 }));
    }

    [Test]
    public void SkippedContentIsDrawnAsWithoutLink()
    {
        // the skipped content still receives the drawing call, so the section inside it is registered, as it is without the link
        var pdfData = GenerateDocument(page =>
        {
            page.Content().Hyperlink(Url).Section("skipped").SkipOnce().Height(20);
        });

        Assert.That(NamedDestination.GetPageNumber(pdfData, "skipped"), Is.EqualTo(1));
    }

    [Test]
    public void ShowIfHidesLinkOnlyWhenPlacedBeforeIt()
    {
        var links = GetLinks(page =>
        {
            page.Content().Row(row =>
            {
                // the row is as tall as its first item (30), and offers that height to the links
                row.ConstantItem(50).Height(30);

                // ShowIf hides only the content after it, so the link keeps its area, as a border or background would
                row.RelativeItem().Hyperlink("https://example.com/kept").ShowIf(_ => false).Height(20);
                row.RelativeItem().ShowIf(_ => false).Hyperlink("https://example.com/hidden").Height(20);
            });
        });

        Assert.That(links.Single().Select(x => x.Url), Is.EqualTo(new[] { "https://example.com/kept" }));
    }

    [Test]
    public void ContentExcludedFromSemanticTreeGetsNoLink()
    {
        // in a tagged document, links are part of the semantic tree, so content marked as an artifact cannot have them
        var settings = new DocumentSettings { PDFUA_Conformance = PDFUA_Conformance.PDFUA_1 };

        var links = GetLinks(page =>
        {
            page.Content().Column(column =>
            {
                column.Item().Hyperlink("https://example.com/content").Text("Content");
                column.Item().SemanticIgnore().Hyperlink("https://example.com/decoration").Text("Decoration");
            });
        }, settings);

        Assert.That(links.Single().Select(x => x.Url), Is.EqualTo(new[] { "https://example.com/content" }));
    }

    [Test]
    public void TextLinkExcludedFromSemanticTreeGetsNoLink()
    {
        // the same rule applies to links inside text
        var settings = new DocumentSettings { PDFUA_Conformance = PDFUA_Conformance.PDFUA_1 };

        var links = GetLinks(page =>
        {
            page.Content().Column(column =>
            {
                column.Item().Text(text => text.Hyperlink("Content", "https://example.com/content"));
                column.Item().SemanticIgnore().Text(text => text.Hyperlink("Decoration", "https://example.com/decoration"));
            });
        }, settings);

        Assert.That(links.Single().Select(x => x.Url), Is.EqualTo(new[] { "https://example.com/content" }));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void LinksInRepeatedFooterAreCreatedOnlyOnFirstPage(bool enableCaching)
    {
        // in a tagged document, the footer is content on the first page only, and an artifact on the next ones, so it has no links there
        var settings = new DocumentSettings { PDFUA_Conformance = PDFUA_Conformance.PDFUA_1 };

        var links = GetLinks(page =>
        {
            page.Content().Column(column =>
            {
                column.Item().Height(50);
                column.Item().Height(50);
            });

            page.Footer().Column(column =>
            {
                column.Item().SemanticLink("Element").Hyperlink("https://example.com/element").Text("Element");
                column.Item().Hyperlink("https://example.com/reversed").SemanticLink("Reversed").Text("Reversed");
                column.Item().Text(text => text.Hyperlink("Text", "https://example.com/text"));
            });
        }, enableCaching, settings);

        Assert.That(links.Select(x => x.Count), Is.EqualTo(new[] { 3, 0 }));
    }

    [Test]
    public void ContentExcludedFromSemanticTreeIsDrawnOnce()
    {
        // the content inside the link is drawn exactly as it would be without the link
        var withLink = CountRectangles(column => column.Item().SemanticIgnore().Hyperlink(Url).Width(40).Height(20).Background(Colors.Grey.Medium));
        var withoutLink = CountRectangles(column => column.Item().SemanticIgnore().Width(40).Height(20).Background(Colors.Grey.Medium));

        Assert.That(withoutLink, Is.Positive);
        Assert.That(withLink, Is.EqualTo(withoutLink));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void UrlCannotBeEmpty(string url)
    {
        var exception = Assert.Throws<ArgumentException>(() => EmptyContainer.Create().Hyperlink(url));
        Assert.That(exception.Message, Does.StartWith("The URL cannot be null or whitespace."));
    }

    [Test]
    public void CompanionHints()
    {
        var hyperlink = new Hyperlink { Url = Url };

        Assert.That(hyperlink.GetCompanionHint(), Is.EqualTo(Url));
        Assert.That(hyperlink.GetCompanionProperties(), Is.EqualTo(new[] { new KeyValuePair<string, string>("Url", Url) }));
    }

    private static int CountRectangles(Action<ColumnDescriptor> content)
    {
        var document = Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(100, 100);
                    page.Content().Column(content);
                });
            })
            .WithSettings(new DocumentSettings { PDFUA_Conformance = PDFUA_Conformance.PDFUA_1 });

        using var pdf = PdfInspector.Load(document.GeneratePdf());

        // the page content is either a single stream or an array of streams
        var contentReference = pdf.Pages.Single().GetProperty("/Contents");
        var contents = pdf.Resolve(contentReference);

        var streams = contents.ValueKind == JsonValueKind.Array
            ? contents.EnumerateArray().ToList()
            : [contentReference];

        return streams
            .SelectMany(stream => PdfInspector.GetContentStreamOperators(pdf.GetStreamText(stream)))
            .Count(x => x == "re");
    }

    private static List<List<LinkAnnotation>> GetLinks(Action<PageDescriptor> content, bool enableCaching, DocumentSettings settings = null)
    {
        var cachingEnabled = QuestPDF.Settings.EnableCaching;

        try
        {
            QuestPDF.Settings.EnableCaching = enableCaching;
            return GetLinks(content, settings);
        }
        finally
        {
            QuestPDF.Settings.EnableCaching = cachingEnabled;
        }
    }

    private static List<List<LinkAnnotation>> GetLinks(Action<PageDescriptor> content, DocumentSettings settings = null)
    {
        return LinkAnnotation.ReadAll(GenerateDocument(content, settings));
    }

    private static byte[] GenerateDocument(Action<PageDescriptor> content, DocumentSettings settings = null)
    {
        var document = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(100, 100);
                content(page);
            });
        });

        if (settings != null)
            document = document.WithSettings(settings);

        return document.GeneratePdf();
    }
}
