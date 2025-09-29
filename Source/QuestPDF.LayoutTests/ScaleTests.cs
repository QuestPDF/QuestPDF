using QuestPDF.Helpers;

namespace QuestPDF.LayoutTests;

public class ScaleTests
{
    private void DrawTestSubject(IContainer container)
    {
        container
            .Inlined(inlined =>
            {
                inlined.Item().Mock("a").SolidBlock(100, 100);
                inlined.Item().Mock("b").SolidBlock(100, 100);
                inlined.Item().Mock("c").SolidBlock(100, 100);
                inlined.Item().Mock("d").SolidBlock(100, 100);
                inlined.Item().Mock("e").SolidBlock(100, 100);
                inlined.Item().Mock("f").SolidBlock(100, 100);
            });
    }
    
    [Test]
    public void DefaultScale()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(600, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(100, 0).Size(100, 100);
                        page.Mock("c").Position(200, 0).Size(100, 100);
                        page.Mock("d").Position(300, 0).Size(100, 100);
                        page.Mock("e").Position(400, 0).Size(100, 100);
                        page.Mock("f").Position(500, 0).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void PositiveScale05()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.Scale(0.5f).Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(300, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(50, 0).Size(100, 100);
                        page.Mock("c").Position(100, 0).Size(100, 100);
                        page.Mock("d").Position(150, 0).Size(100, 100);
                        page.Mock("e").Position(200, 0).Size(100, 100);
                        page.Mock("f").Position(250, 0).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void PositiveScale15()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.Scale(1.5f).Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(750, 300)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(150, 0).Size(100, 100);
                        page.Mock("c").Position(300, 0).Size(100, 100);
                        page.Mock("d").Position(450, 0).Size(100, 100);
                        page.Mock("e").Position(600, 0).Size(100, 100);
                        page.Mock("f").Position(0, 150).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void PositiveScale25()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.Scale(2.5f).Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(750, 500)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(250, 0).Size(100, 100);
                        page.Mock("c").Position(500, 0).Size(100, 100);
                        page.Mock("d").Position(0, 250).Size(100, 100);
                        page.Mock("e").Position(250, 250).Size(100, 100);
                        page.Mock("f").Position(500, 250).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void PositiveScaleTwoPages()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 250)
            .ForContent(content =>
            {
                content.Scale(2).Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(800, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(200, 0).Size(100, 100);
                        page.Mock("c").Position(400, 0).Size(100, 100);
                        page.Mock("d").Position(600, 0).Size(100, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(400, 200)
                    .Content(page =>
                    {
                        page.Mock("e").Position(0, 0).Size(100, 100);
                        page.Mock("f").Position(200, 0).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void ScaleVertical()
    {
        LayoutTest
            .HavingSpaceOfSize(450, 800)
            .ForContent(content =>
            {
                content.ScaleVertical(2f).Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(400, 400)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(100, 0).Size(100, 100);
                        page.Mock("c").Position(200, 0).Size(100, 100);
                        page.Mock("d").Position(300, 0).Size(100, 100);
                        page.Mock("e").Position(0, 200).Size(100, 100);
                        page.Mock("f").Position(100, 200).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void ScaleVerticalNegative()
    {
        LayoutTest
            .HavingSpaceOfSize(400, 500)
            .ForContent(content =>
            {
                content.ScaleVertical(-1.5f).Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(400, 300)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 500).Size(100, 100);
                        page.Mock("b").Position(100, 500).Size(100, 100);
                        page.Mock("c").Position(200, 500).Size(100, 100);
                        page.Mock("d").Position(300, 500).Size(100, 100);
                        page.Mock("e").Position(0, 350).Size(100, 100);
                        page.Mock("f").Position(100, 350).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void ScaleHorizontal()
    {
        LayoutTest
            .HavingSpaceOfSize(450, 800)
            .ForContent(content =>
            {
                content.ScaleHorizontal(2f).Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(400, 300)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(200, 0).Size(100, 100);
                        page.Mock("c").Position(0, 100).Size(100, 100);
                        page.Mock("d").Position(200, 100).Size(100, 100);
                        page.Mock("e").Position(0, 200).Size(100, 100);
                        page.Mock("f").Position(200, 200).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void ScaleHorizontalNegative()
    {
        LayoutTest
            .HavingSpaceOfSize(700, 400)
            .ForContent(content =>
            {
                content.ScaleHorizontal(-1.5f).Element(DrawTestSubject);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(600, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(700, 0).Size(100, 100);
                        page.Mock("b").Position(550, 0).Size(100, 100);
                        page.Mock("c").Position(400, 0).Size(100, 100);
                        page.Mock("d").Position(250, 0).Size(100, 100);
                        page.Mock("e").Position(700, 100).Size(100, 100);
                        page.Mock("f").Position(550, 100).Size(100, 100);
                    });
            });
    }
    
    [Test]
    public void WrapHorizontal()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Scale(3).Width(50).Height(10);
            })
            .ExpectLayoutException("The available horizontal space is less than the minimum width.");
    }
    
    [Test]
    public void WrapVertical()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Scale(3).Width(10).Height(50);
            })
            .ExpectLayoutException("The available vertical space is less than the minimum height.");
    }
}