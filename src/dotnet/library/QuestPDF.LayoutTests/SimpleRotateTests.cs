using QuestPDF.Helpers;

namespace QuestPDF.LayoutTests;

public class SimpleRotateTests
{
    private void DrawTestSubject(IContainer container)
    {
        container
            .Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.AutoItem().Mock("a").SolidBlock(100, 100);
                    row.AutoItem().Mock("b").SolidBlock(100, 100);
                    row.AutoItem().Mock("c").SolidBlock(100, 100);
                });
                
                column.Item().Row(row =>
                {
                    row.AutoItem().Mock("d").SolidBlock(100, 100);
                    row.AutoItem().Mock("e").SolidBlock(100, 100);
                    row.AutoItem().Mock("f").SolidBlock(100, 100);
                });
            });
    }
    
    #region Single Page
    
    [Test]
    public void NoRotation()
    {
        LayoutTest
            .HavingSpaceOfSize(500, 500)
            .ForContent(content =>
            {
                content.Shrink().Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(300, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(100, 0).Size(100, 100);
                        page.Mock("c").Position(200, 0).Size(100, 100);
                        page.Mock("d").Position(0, 100).Size(100, 100);
                        page.Mock("e").Position(100, 100).Size(100, 100);
                        page.Mock("f").Position(200, 100).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void OneRotation()
    {
        LayoutTest
            .HavingSpaceOfSize(500, 500)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .RotateRight() // <-
                    .Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 300)
                    .Content(page =>
                    {
                        page.Mock("a").Position(200, 0).Size(100, 100);
                        page.Mock("b").Position(200, 100).Size(100, 100);
                        page.Mock("c").Position(200, 200).Size(100, 100);
                        page.Mock("d").Position(100, 0).Size(100, 100);
                        page.Mock("e").Position(100, 100).Size(100, 100);
                        page.Mock("f").Position(100, 200).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void TwoRotations()
    {
        LayoutTest
            .HavingSpaceOfSize(500, 500)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .RotateRight() // <-
                    .RotateRight()
                    .Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(300, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(300, 200).Size(100, 100);
                        page.Mock("b").Position(200, 200).Size(100, 100);
                        page.Mock("c").Position(100, 200).Size(100, 100);
                        page.Mock("d").Position(300, 100).Size(100, 100);
                        page.Mock("e").Position(200, 100).Size(100, 100);
                        page.Mock("f").Position(100, 100).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void ThreeRotation()
    {
        LayoutTest
            .HavingSpaceOfSize(500, 500)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .RotateRight() // <-
                    .RotateRight()
                    .RotateRight()
                    .Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 300)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 300).Size(100, 100);
                        page.Mock("b").Position(0, 200).Size(100, 100);
                        page.Mock("c").Position(0, 100).Size(100, 100);
                        page.Mock("d").Position(100, 300).Size(100, 100);
                        page.Mock("e").Position(100, 200).Size(100, 100);
                        page.Mock("f").Position(100, 100).Size(100, 100);
                    });
            });
    }
    
    #endregion
    
    #region Paging
    
    [Test]
    public void NoRotationWithPaging()
    {
        LayoutTest
            .HavingSpaceOfSize(500, 150)
            .ForContent(content =>
            {
                content.Shrink().Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(300, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(100, 0).Size(100, 100);
                        page.Mock("c").Position(200, 0).Size(100, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(300, 100)
                    .Content(page =>
                    {
                        page.Mock("d").Position(0, 0).Size(100, 100);
                        page.Mock("e").Position(100, 0).Size(100, 100);
                        page.Mock("f").Position(200, 0).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void OneRotationWithPaging()
    {
        LayoutTest
            .HavingSpaceOfSize(150, 500)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .RotateRight() // <-
                    .Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 300)
                    .Content(page =>
                    {
                        page.Mock("a").Position(100, 0).Size(100, 100);
                        page.Mock("b").Position(100, 100).Size(100, 100);
                        page.Mock("c").Position(100, 200).Size(100, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(100, 300)
                    .Content(page =>
                    {
                        page.Mock("d").Position(100, 0).Size(100, 100);
                        page.Mock("e").Position(100, 100).Size(100, 100);
                        page.Mock("f").Position(100, 200).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void TwoRotationsWithPaging()
    {
        LayoutTest
            .HavingSpaceOfSize(500, 150)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .RotateRight() // <-
                    .RotateRight()
                    .Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(300, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(300, 100).Size(100, 100);
                        page.Mock("b").Position(200, 100).Size(100, 100);
                        page.Mock("c").Position(100, 100).Size(100, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(300, 100)
                    .Content(page =>
                    {
                        page.Mock("d").Position(300, 100).Size(100, 100);
                        page.Mock("e").Position(200, 100).Size(100, 100);
                        page.Mock("f").Position(100, 100).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void ThreeRotationWithPaging()
    {
        LayoutTest
            .HavingSpaceOfSize(150, 500)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .RotateRight() // <-
                    .RotateRight()
                    .RotateRight()
                    .Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 300)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 300).Size(100, 100);
                        page.Mock("b").Position(0, 200).Size(100, 100);
                        page.Mock("c").Position(0, 100).Size(100, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(100, 300)
                    .Content(page =>
                    {
                        page.Mock("d").Position(0, 300).Size(100, 100);
                        page.Mock("e").Position(0, 200).Size(100, 100);
                        page.Mock("f").Position(0, 100).Size(100, 100);
                    });
            });
    }
    
    #endregion
}