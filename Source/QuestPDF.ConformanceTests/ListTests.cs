using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class ListTests : ConformanceTestBase
{
    protected override Document GetDocumentUnderTest()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(60);

                    page.Content()
                        .PaddingVertical(30)
                        .SemanticSection()
                        .Column(column =>
                        {
                            column.Spacing(15);

                            column.Item()
                                .SemanticHeader1()
                                .Text("Conformance Test: Lists")
                                .FontSize(36)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item()
                                .SemanticList()
                                .Column(listColumn =>
                                {
                                    listColumn.Spacing(10);

                                    listColumn.Item()
                                        .SemanticListItem()
                                        .Row(row =>
                                        {
                                            row.ConstantItem(20).SemanticListLabel().Text("1.");

                                            row.RelativeItem()
                                                .SemanticListItemBody()
                                                .Column(bodyColumn =>
                                                {
                                                    bodyColumn.Spacing(8);

                                                    bodyColumn.Item().Text(Placeholders.Sentence());

                                                    bodyColumn.Item()
                                                        .SemanticList()
                                                        .Column(nestedColumn =>
                                                        {
                                                            nestedColumn.Spacing(10);

                                                            foreach (var i in Enumerable.Range(1, 4))
                                                            {
                                                                nestedColumn.Item()
                                                                    .SemanticListItem()
                                                                    .Row(nestedRow =>
                                                                    {
                                                                        nestedRow.ConstantItem(10)
                                                                            .SemanticListLabel()
                                                                            .Text("-");

                                                                        nestedRow.RelativeItem()
                                                                            .SemanticListItemBody()
                                                                            .Text(Placeholders.Sentence());
                                                                    });
                                                            }
                                                        });
                                                });
                                        });

                                    foreach (var i in Enumerable.Range(2, 5))
                                    {
                                        listColumn.Item()
                                            .SemanticListItem()
                                            .Row(row =>
                                            {
                                                row.ConstantItem(20)
                                                    .SemanticListLabel()
                                                    .Text($"{i}.");

                                                row.RelativeItem()
                                                    .SemanticListItemBody()
                                                    .Text(Placeholders.Sentence());
                                            });
                                    }
                                });
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("Sect", sect =>
            {
                sect.Child("H1", h1 => h1.Alt("Conformance Test: Lists"));

                sect.Child("L", list =>
                {
                    list.Child("LI", listItem =>
                    {
                        listItem.Child("Lbl");
                        listItem.Child("LBody", lBody =>
                        {
                            lBody.Child("P");

                            lBody.Child("L", nestedList =>
                            {
                                foreach (var i in Enumerable.Range(1, 4))
                                {
                                    nestedList.Child("LI", nestedItem =>
                                    {
                                        nestedItem.Child("Lbl");
                                        nestedItem.Child("LBody", listBody => listBody.Child("P"));
                                    });
                                }
                            });
                        });
                    });

                    foreach (var i in Enumerable.Range(2, 5))
                    {
                        list.Child("LI", listItem =>
                        {
                            listItem.Child("Lbl");
                            listItem.Child("LBody", listBody => listBody.Child("P"));
                        });
                    }
                });
            });
        });
    }
}