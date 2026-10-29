using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests;

// the tests change the global caching setting
[TestFixture, NonParallelizable]
public class SectionLinkTests
{
    // pages are 100x100
    // the tests mirror HyperlinkTests, both link elements follow the same rules

    // the document is rendered twice: with caching, the second pass replays the drawing operations recorded in the first one,
    // without caching, the elements draw themselves again, including the EnsureSpace and PreventPageBreak elements inside the link

    [Test]
    public void LinkCoversContentAndNavigatesToSection()
    {
        var pdfData = GenerateDocument(column =>
        {
            // the alignment offers the link exactly the space of its content
            column.Item().Padding(10).AlignLeft().AlignTop().SectionLink("chapter").Width(40).Height(20);
            column.Item().PageBreak();
            column.Item().Section("chapter").Height(20);
        });

        // PDF coordinates start at the bottom-left corner of the page
        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links, Is.EqualTo(new[] { new[] { new LinkAnnotation(10, 70, 50, 90, null, "chapter") }, Array.Empty<LinkAnnotation>() }));

        Assert.That(NamedDestination.GetPageNumber(pdfData, "chapter"), Is.EqualTo(2));
    }

    [Test]
    public void LinkCoversEntireSpaceOfferedToIt()
    {
        var pdfData = GenerateDocument(column =>
        {
            // the content is centered, the link covers the entire width offered to it, which contains the content wherever it is placed
            column.Item().SectionLink("chapter").AlignCenter().Width(20).Height(20);
            column.Item().Section("chapter");
        });

        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links, Is.EqualTo(new[] { new[] { new LinkAnnotation(0, 80, 100, 100, null, "chapter") } }));
    }

    [Test]
    public void LinkCoversSameSpaceInRightToLeftDirection()
    {
        var pdfData = GenerateDocument(column =>
        {
            // the content is placed at the right edge
            column.Item().ContentFromRightToLeft().SectionLink("chapter").Width(20).Height(20);
            column.Item().Section("chapter");
        });

        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links, Is.EqualTo(new[] { new[] { new LinkAnnotation(0, 80, 100, 100, null, "chapter") } }));
    }

    [Test]
    public void TextLinkNavigatesToSection()
    {
        // without tagging, the document has no semantic tree, so the link is created without a Link element
        var pdfData = GenerateDocument(column =>
        {
            column.Item().Text(text => text.SectionLink("Link", "chapter"));
            column.Item().Section("chapter");
        });

        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links.Single().Select(x => x.Destination), Is.EqualTo(new[] { "chapter" }));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ContentSplitAcrossPagesGetsLinkOnEachPage(bool enableCaching)
    {
        var pdfData = GenerateDocument(column =>
        {
            // only the first two items fit on the first page
            column.Item().SectionLink("chapter").Column(content =>
            {
                foreach (var _ in Enumerable.Range(0, 3))
                    content.Item().Height(40);
            });

            column.Item().Section("chapter");
        }, enableCaching);

        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links.Select(x => x.Count), Is.EqualTo(new[] { 1, 1 }));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void TextSplitAcrossPagesGetsEachLinkOnlyOnPageWithItsText(bool enableCaching)
    {
        var pdfData = GenerateDocument(column =>
        {
            // the paragraph has 12 lines, half of them fit on each page
            column.Item().Text(text =>
            {
                text.SectionLink("First", "first");
                text.Span(string.Concat(Enumerable.Repeat("\nText", 10)) + "\n");
                text.SectionLink("Last", "last");
            });

            column.Item().Section("first");
            column.Item().Section("last");
        }, enableCaching);

        var links = LinkAnnotation.ReadAll(pdfData);

        var destinations = links.Select(x => x.Select(link => link.Destination));
        Assert.That(destinations, Is.EqualTo(new[] { new[] { "first" }, new[] { "last" } }));

        // the text on the next page is shifted up by the height drawn so far, and so are its links
        Assert.That(links.SelectMany(x => x), Has.All.Matches<LinkAnnotation>(x => x.Bottom >= 0 && x.Top <= 100));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void PostponedContentGetsNoLinkAreaOnPreviousPage(bool enableCaching)
    {
        var pdfData = GenerateDocument(column =>
        {
            column.Item().Height(60);

            // the row is as tall as its first item (30), and offers that height to the link
            column.Item().Row(row =>
            {
                row.ConstantItem(50).Height(30);

                // only the first item (20) fits in the remaining space (40), less than the required 45
                row.RelativeItem().SectionLink("chapter").EnsureSpace(45).Column(content =>
                {
                    content.Item().Width(40).Height(20);
                    content.Item().Width(40).Height(30);
                });
            });

            column.Item().Section("chapter");
        }, enableCaching);

        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links.Select(x => x.Count), Is.EqualTo(new[] { 0, 1 }));
    }

    [Test]
    public void ContentThatDoesNotFitIsNotDrawn()
    {
        // parents do not draw content that does not fit, the link ignores such a call:
        // the mock canvas throws when a link is drawn, and the mock child fails the test when it is drawn
        TestPlan
            .For(x => new SectionLink
            {
                Child = x.CreateChild(),
                SectionName = "chapter"
            })
            .DrawElement(new Size(400, 300))
            .ExpectChildMeasure(new Size(400, 300), SpacePlan.Wrap("Mock"))
            .CheckDrawResult();
    }

    [Test]
    public void SkippedContentGetsNoLink()
    {
        var pdfData = GenerateDocument(column =>
        {
            column.Item().Row(row =>
            {
                // the row spans two pages, because of its first item, and draws its second item on both of them
                row.ConstantItem(50).Column(innerColumn =>
                {
                    innerColumn.Item().Height(100);
                    innerColumn.Item().Height(30);
                });

                // the content is skipped on the first page, and still receives the drawing call there, so it appears on the second page
                row.RelativeItem().SectionLink("chapter").SkipOnce().Width(40).Height(20);
            });

            column.Item().Section("chapter");
        });

        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links.Select(x => x.Count), Is.EqualTo(new[] { 0, 1 }));
    }

    [Test]
    public void SkippedContentIsDrawnAsWithoutLink()
    {
        // the skipped content still receives the drawing call, so the section inside it is registered, as it is without the link
        var pdfData = GenerateDocument(column =>
        {
            column.Item().SectionLink("chapter").Section("skipped").SkipOnce().Height(20);
            column.Item().Section("chapter");
        });

        Assert.That(NamedDestination.GetPageNumber(pdfData, "skipped"), Is.EqualTo(1));
    }

    [Test]
    public void ShowIfHidesLinkOnlyWhenPlacedBeforeIt()
    {
        var pdfData = GenerateDocument(column =>
        {
            column.Item().Row(row =>
            {
                // the row is as tall as its first item (30), and offers that height to the links
                row.ConstantItem(50).Height(30);

                // ShowIf hides only the content after it, so the link keeps its area, as a border or background would
                row.RelativeItem().SectionLink("kept").ShowIf(_ => false).Width(40).Height(20);
                row.RelativeItem().ShowIf(_ => false).SectionLink("hidden").Width(40).Height(20);
            });

            column.Item().Section("kept");
            column.Item().Section("hidden");
        });

        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links.Single().Select(x => x.Destination), Is.EqualTo(new[] { "kept" }));
    }

    [Test]
    public void ContentExcludedFromSemanticTreeGetsNoLink()
    {
        // in a tagged document, links are part of the semantic tree, so content marked as an artifact cannot have them
        var settings = new DocumentSettings { PDFUA_Conformance = PDFUA_Conformance.PDFUA_1 };

        var pdfData = GenerateDocument(column =>
        {
            column.Item().SectionLink("content").Text("Content");
            column.Item().SemanticIgnore().SectionLink("decoration").Text("Decoration");

            column.Item().Section("content");
            column.Item().Section("decoration");
        }, settings);

        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links.Single().Select(x => x.Destination), Is.EqualTo(new[] { "content" }));
    }

    [Test]
    public void TextLinkExcludedFromSemanticTreeGetsNoLink()
    {
        // the same rule applies to links inside text
        var settings = new DocumentSettings { PDFUA_Conformance = PDFUA_Conformance.PDFUA_1 };

        var pdfData = GenerateDocument(column =>
        {
            column.Item().Text(text => text.SectionLink("Content", "content"));
            column.Item().SemanticIgnore().Text(text => text.SectionLink("Decoration", "decoration"));

            column.Item().Section("content");
            column.Item().Section("decoration");
        }, settings);

        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links.Single().Select(x => x.Destination), Is.EqualTo(new[] { "content" }));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void LinksInRepeatedFooterAreCreatedOnlyOnFirstPage(bool enableCaching)
    {
        // in a tagged document, the footer is content on the first page only, and an artifact on the next ones, so it has no links there
        var settings = new DocumentSettings { PDFUA_Conformance = PDFUA_Conformance.PDFUA_1 };
        var cachingEnabled = QuestPDF.Settings.EnableCaching;
        byte[] pdfData;

        try
        {
            QuestPDF.Settings.EnableCaching = enableCaching;

            pdfData = GenerateDocument(page =>
            {
                page.Content().Column(column =>
                {
                    column.Item().Section("element").Height(50);
                    column.Item().Section("text").Height(50);
                });

                page.Footer().Column(column =>
                {
                    column.Item().SemanticLink("Element").SectionLink("element").Text("Element");
                    column.Item().SectionLink("element").SemanticLink("Reversed").Text("Reversed");
                    column.Item().Text(text => text.SectionLink("Text", "text"));
                });
            }, settings);
        }
        finally
        {
            QuestPDF.Settings.EnableCaching = cachingEnabled;
        }

        var links = LinkAnnotation.ReadAll(pdfData);
        Assert.That(links.Select(x => x.Count), Is.EqualTo(new[] { 3, 0 }));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void SectionNameCannotBeEmpty(string sectionName)
    {
        var exception = Assert.Throws<ArgumentException>(() => EmptyContainer.Create().SectionLink(sectionName));
        Assert.That(exception.Message, Does.StartWith("The section name cannot be null or whitespace."));
    }

    [Test]
    public void CompanionHints()
    {
        var sectionLink = new SectionLink { SectionName = "chapter" };

        Assert.That(sectionLink.GetCompanionHint(), Is.EqualTo("chapter"));
        Assert.That(sectionLink.GetCompanionSearchableContent(), Is.EqualTo("chapter"));
        Assert.That(sectionLink.GetCompanionProperties(), Is.EqualTo(new[] { new KeyValuePair<string, string>("SectionName", "chapter") }));
    }

    private static byte[] GenerateDocument(Action<ColumnDescriptor> content, bool enableCaching)
    {
        var cachingEnabled = QuestPDF.Settings.EnableCaching;

        try
        {
            QuestPDF.Settings.EnableCaching = enableCaching;
            return GenerateDocument(content);
        }
        finally
        {
            QuestPDF.Settings.EnableCaching = cachingEnabled;
        }
    }

    private static byte[] GenerateDocument(Action<ColumnDescriptor> content, DocumentSettings settings = null)
    {
        return GenerateDocument(page => page.Content().Column(content), settings);
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
