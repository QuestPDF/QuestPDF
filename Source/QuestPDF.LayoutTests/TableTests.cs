namespace QuestPDF.LayoutTests;

public class TableTests
{
    [Test]
    public void RowSpan_CornerCase1()
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
    public void RowSpan_CornerCase2()
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
    public void RowSpan_CornerCase3()
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
}