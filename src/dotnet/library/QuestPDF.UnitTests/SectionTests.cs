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
[NonParallelizable]
public class SectionTests
{
    // pages are 100x100, so after the first item (60) there are 40 points left

    // the document is rendered twice: with caching, the second pass replays the drawing operations recorded in the first one,
    // without caching, the elements draw themselves again, so their state must be reset between the passes

    [TestCase(true)]
    [TestCase(false)]
    public void ContentPostponedByEnsureSpaceStartsSectionOnNextPage(bool enableCaching)
    {
        var result = GenerateDocument(column =>
        {
            column.Item().Height(60);

            // only the first item (20) fits in the remaining space, less than the required 45
            column.Item().Section("x").EnsureSpace(45).Column(content =>
            {
                content.Item().Height(20);
                content.Item().Height(30);
            });
        }, enableCaching);

        Assert.That(result.PageCount, Is.EqualTo(2));
        Assert.That(result.SectionPages, Is.EqualTo((2, 2)));
        Assert.That(result.DestinationPage, Is.EqualTo(2));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ContentPostponedByPreventPageBreakStartsSectionOnNextPage(bool enableCaching)
    {
        var result = GenerateDocument(column =>
        {
            column.Item().Height(60);

            // the content (50) does not fit in the remaining space
            column.Item().Section("x").PreventPageBreak().Column(content =>
            {
                content.Item().Height(20);
                content.Item().Height(30);
            });
        }, enableCaching);

        Assert.That(result.PageCount, Is.EqualTo(2));
        Assert.That(result.SectionPages, Is.EqualTo((2, 2)));
        Assert.That(result.DestinationPage, Is.EqualTo(2));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ContentTallerThanPageIsPostponedOnlyOnce(bool enableCaching)
    {
        var result = GenerateDocument(column =>
        {
            // the content cannot fit on any page, PreventPageBreak postpones it once and then lets it span pages
            // TODO: known limitation, the first page stays blank, see ColumnTests.PostponedItemWithHorizontalPaddingInsideRow
            column.Item().Section("x").PreventPageBreak().Column(content =>
            {
                content.Item().Height(60);
                content.Item().Height(60);
            });
        }, enableCaching);

        Assert.That(result.PageCount, Is.EqualTo(3));
        Assert.That(result.SectionPages, Is.EqualTo((2, 3)));
        Assert.That(result.DestinationPage, Is.EqualTo(2));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SectionMarkerWithoutContentIsRegistered(bool enableCaching)
    {
        var result = GenerateDocument(column =>
        {
            column.Item().Height(60);
            column.Item().Section("x");
            column.Item().Height(30);
        }, enableCaching);

        Assert.That(result.PageCount, Is.EqualTo(1));
        Assert.That(result.SectionPages, Is.EqualTo((1, 1)));
        Assert.That(result.DestinationPage, Is.EqualTo(1));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SectionEndsOnPageWhereItsContentEnds(bool enableCaching)
    {
        var result = GenerateDocument(column =>
        {
            // the row keeps drawing the finished first item on every page until the second item ends
            column.Item().Row(row =>
            {
                row.RelativeItem().Section("x").Text("Section");

                row.RelativeItem().Column(content =>
                {
                    foreach (var _ in Enumerable.Range(0, 5))
                        content.Item().Height(50);
                });
            });
        }, enableCaching);

        Assert.That(result.PageCount, Is.EqualTo(3));
        Assert.That(result.SectionPages, Is.EqualTo((1, 1)));
        Assert.That(result.DestinationPage, Is.EqualTo(1));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SectionInsideMultiColumnIsRegistered(bool enableCaching)
    {
        var result = GenerateDocument(column =>
        {
            column.Item().MultiColumn(multiColumn =>
            {
                multiColumn.Columns(2);

                // six items of 40 points: two in each column of the first page, the last two on the second page
                multiColumn.Content().Section("x").Column(content =>
                {
                    foreach (var _ in Enumerable.Range(0, 6))
                        content.Item().Height(40);
                });
            });
        }, enableCaching);

        Assert.That(result.PageCount, Is.EqualTo(2));
        Assert.That(result.SectionPages, Is.EqualTo((1, 2)));
        Assert.That(result.DestinationPage, Is.EqualTo(1));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void SectionNameCannotBeEmpty(string sectionName)
    {
        var exception = Assert.Throws<ArgumentException>(() => EmptyContainer.Create().Section(sectionName));
        Assert.That(exception.Message, Does.StartWith("The section name cannot be null or whitespace."));
    }

    [Test]
    public void CompanionHints()
    {
        var section = new Section { SectionName = "chapter" };

        Assert.That(section.GetCompanionHint(), Is.EqualTo("chapter"));
        Assert.That(section.GetCompanionSearchableContent(), Is.EqualTo("chapter"));
        Assert.That(section.GetCompanionProperties(), Is.EqualTo(new[] { new KeyValuePair<string, string>("SectionName", "chapter") }));
    }

    private static (int PageCount, (int Start, int End)? SectionPages, int? DestinationPage) GenerateDocument(Action<ColumnDescriptor> content, bool enableCaching)
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

    private static (int PageCount, (int Start, int End)? SectionPages, int? DestinationPage) GenerateDocument(Action<ColumnDescriptor> content)
    {
        var observer = new SectionLocationObserver { SectionName = "x" };

        var document = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(100, 100);

                page.Content().Column(column =>
                {
                    content(column);
                    column.Item().Element(observer);
                });
            });
        });

        var pdfData = document.GeneratePdf();

        using var pdf = PdfInspector.Load(pdfData);
        var pageCount = pdf.Pages.Count();

        return (pageCount, observer.SectionPages, NamedDestination.GetPageNumber(pdfData, "x"));
    }

    // drawn at the end of the document, reads the pages registered for the section
    private sealed class SectionLocationObserver : Element, IPageContextAware
    {
        public IPageContext PageContext { get; set; }
        public string SectionName { get; init; }
        public (int Start, int End)? SectionPages { get; private set; }

        internal override SpacePlan Measure(Size availableSpace) => SpacePlan.FullRender(Size.Zero);

        internal override void Draw(Size availableSpace)
        {
            var location = PageContext.GetLocation(SectionName);
            SectionPages = location == null ? null : (location.PageStart, location.PageEnd);
        }
    }
}
