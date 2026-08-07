using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.Table;

/// <summary>
/// A container marked with the SemanticTable method must lead to a Table element
/// through single-child containers only. Otherwise, an exception is thrown.
/// </summary>
internal class SemanticTableDiscoveryTests
{
    [Test]
    public void MultiChildElementInterruptsTheRelationship()
    {
        var document = Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(25);

                    page.Content()
                        .SemanticTable()
                        .Column(column =>
                        {
                            column.Item().Text("Some text");
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns => columns.RelativeColumn());
                                table.Cell().Text("Cell");
                            });
                        });
                });
            });

        Assert.Throws<DocumentComposeException>(() => document.TestSemanticTree(null));
    }

    [Test]
    public void MissingTableThrowsException()
    {
        var document = Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(25);

                    page.Content()
                        .SemanticTable()
                        .Text("There is no table here");
                });
            });

        Assert.Throws<DocumentComposeException>(() => document.TestSemanticTree(null));
    }

    [Test]
    public void TableInsideLazyContentIsDiscovered()
    {
        var document = Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(25);

                    page.Content()
                        .Lazy(content =>
                        {
                            content
                                .SemanticTable()
                                .Padding(5)
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns => columns.RelativeColumn());
                                    table.Cell().Text("Cell");
                                });
                        });
                });
            });

        var expectedSemanticTree = ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("Table", table =>
                table.Child("TBody", tbody =>
                    tbody.Child("TR", tr =>
                        tr.Child("TD", td => td.Child("P")))));
        });

        document.TestSemanticTree(expectedSemanticTree);
    }

    [Test]
    public void NonSemanticDocumentsAreNotValidated()
    {
        var document = Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(25);

                    page.Content()
                        .SemanticTable()
                        .Text("There is no table here");
                });
            });

        // without accessibility conformance settings, semantic tags are ignored and no validation is performed
        Assert.DoesNotThrow(() =>
        {
            using var stream = new MemoryStream();
            document.WithSettings(DocumentSettings.Default).GeneratePdf(stream);
        });
    }
}
