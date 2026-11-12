namespace QuestPDF.LayoutTests;

public class TableTests
{
    [Test]
    public void CellAfterRowSpanWrapsToNextPage()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 400)
            .ForContent(content =>
            {
                content
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                        });
                        
                        table.Cell()
                            .RowSpan(2)
                            .Mock("a")
                            .SolidBlock(100, 100);
                        
                        table.Cell()
                            .Mock("b")
                            .SolidBlock(100, 350);
                    });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(200, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(200, 350)
                    .Content(page =>
                    {
                        page.Mock("b").Position(0, 0).Size(200, 350);
                    });
            });
    }
    
    [Test]
    public void RowSpanStretchesToMatchTallerNeighborAcrossPages()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 400)
            .ForContent(content =>
            {
                content
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });
                        
                        table.Cell()
                            .Column(1)
                            .Row(1)
                            .RowSpan(3)
                            .Mock("a")
                            .SolidBlock(100, 100);
                        
                        table.Cell()
                            .Column(2)
                            .Row(2)
                            .Mock("b")
                            .ContinuousBlock(100, 600);
                    });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 400)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 400);
                        page.Mock("b").Position(100, 0).Size(100, 400);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(200, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 200);
                        page.Mock("b").Position(100, 0).Size(100, 200);
                    });
            });
    }
    
    [Test]
    public void EmptyRowSpanCoveringEntireTablePaginates()
    {
        // An empty cell whose RowSpan reaches the table's last row should not stop the
        // table from paginating its other cells; previously the table reported FullRender
        // after page 1 and silently dropped every row that hadn't fit.
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Empty spanning cell in column 1 — the trigger for the bug.
                        table.Cell().RowSpan(5).Mock("a");

                        // Five regular cells in column 2 that exceed a single page.
                        foreach (var i in Enumerable.Range(1, 5))
                            table.Cell().Mock($"line-{i}").SolidBlock(100, 100);
                    });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 200);
                        page.Mock("line-1").Position(100, 0).Size(100, 100);
                        page.Mock("line-2").Position(100, 100).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(200, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 200);
                        page.Mock("line-3").Position(100, 0).Size(100, 100);
                        page.Mock("line-4").Position(100, 100).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(200, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("line-5").Position(100, 0).Size(100, 100);
                    });
            });
    }

    [Test]
    public void EmptyRowSpanCoveringPartOfTablePaginates()
    {
        // Same bug shape as the test above, but the RowSpan ends before the table's last
        // row. Both the rows the span covers and the rows that follow it must paginate
        // correctly across pages instead of being dropped.
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Spanning cell covers rows 1-5 (not the full table).
                        table.Cell().Row(1).Column(1).RowSpan(5).Mock("a");

                        // Seven rows in column 2. Only the first five overlap the spanning cell.
                        foreach (var i in Enumerable.Range(1, 7))
                            table.Cell().Row((uint)i).Column(2).Mock($"line-{i}").SolidBlock(100, 100);
                    });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 200);
                        page.Mock("line-1").Position(100, 0).Size(100, 100);
                        page.Mock("line-2").Position(100, 100).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(200, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 200);
                        page.Mock("line-3").Position(100, 0).Size(100, 100);
                        page.Mock("line-4").Position(100, 100).Size(100, 100);
                    });

                // Spanning cell ends on row 5 together with line-5; line-6 fits below.
                document
                    .Page()
                    .RequiredAreaSize(200, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("line-5").Position(100, 0).Size(100, 100);
                        page.Mock("line-6").Position(100, 100).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(200, 100)
                    .Content(page =>
                    {
                        page.Mock("line-7").Position(100, 0).Size(100, 100);
                    });
            });
    }

    [Test]
    public void RowSpanWithPaginatedContentSpansMultiplePages()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 400)
            .ForContent(content =>
            {
                content
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                        });

                        table.Cell()
                            .Column(1)
                            .Row(1)
                            .RowSpan(3)
                            .Column(column =>
                            {
                                foreach (var i in Enumerable.Range(1, 5))
                                {
                                    column.Item().Width(200).Height(200).Mock($"{i}");
                                }
                            });
                    });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 400)
                    .Content(page =>
                    {
                        page.Mock("1").Position(0, 0).Size(200, 200);
                        page.Mock("2").Position(0, 200).Size(200, 200);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(200, 400)
                    .Content(page =>
                    {
                        page.Mock("3").Position(0, 0).Size(200, 200);
                        page.Mock("4").Position(0, 200).Size(200, 200);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(200, 200)
                    .Content(page =>
                    {
                        page.Mock("5").Position(0, 0).Size(200, 200);
                    });
            });
    }

    [Test]
    public void CellThatNeverFitsBelowFinishedRowSpanThrowsLayoutException()
    {
        // https://github.com/QuestPDF/QuestPDF/issues/1492
        // The spanning cell "a" finishes on the first page but still covers the second row.
        // The cell "c" does not fit on any page; previously the already rendered "a" kept the table
        // reporting a partial render, and the document generated empty pages forever.
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Row(1).Column(1).RowSpan(2).Mock("a").SolidBlock(100, 50);
                        table.Cell().Row(1).Column(2).Mock("b").SolidBlock(100, 50);
                        table.Cell().Row(2).Column(2).Mock("c").SolidBlock(100, 300);
                    });
            })
            .ExpectLayoutException("Insufficient space to render (even partially) a single row.");
    }

    [Test]
    public void FinishedRowSpanContinuesNextToPaginatedCell()
    {
        // Counterpart of the test above: the spanning cell "a" finishes on the first page as well,
        // but the cell "c" below "b" is rendered partially on every page, so the table must continue.
        // The spanning cell is drawn again on the second page, next to the rest of "c".
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Row(1).Column(1).RowSpan(2).Mock("a").SolidBlock(100, 50);
                        table.Cell().Row(1).Column(2).Mock("b").SolidBlock(100, 50);
                        table.Cell().Row(2).Column(2).Mock("c").ContinuousBlock(100, 350);
                    });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 200);
                        page.Mock("b").Position(100, 0).Size(100, 50);
                        page.Mock("c").Position(100, 50).Size(100, 150);
                    });

                document
                    .Page()
                    .RequiredAreaSize(200, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 200);
                        page.Mock("c").Position(100, 0).Size(100, 200);
                    });
            });
    }
}