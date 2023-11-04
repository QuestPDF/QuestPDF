using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.LayoutTests.TestEngine;

namespace QuestPDF.LayoutTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        return;
        
        LayoutTest
            .HavingSpaceOfSize(200, 400)
            .WithContent(content =>
            {
                content.Column(column =>
                {
                    column.Spacing(25);

                    column.Item().Mock("a").Size(150, 200);
                    column.Item().Mock("b").Size(150, 150);
                    column.Item().Mock("c").Size(150, 100);
                    column.Item().Mock("d").Size(150, 150);
                    column.Item().Mock("e").Size(150, 300);
                    column.Item().Mock("f").Size(150, 150);
                    column.Item().Mock("g").Size(150, 100);
                    column.Item().Mock("h").Size(150, 500);
                });
            })
            .ExpectedDrawResult(document =>
            {
                document
                    .Page()
                    .TakenAreaSize(400, 300)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(250, 200);
                        page.Mock("b").Position(150, 50).Size(50, 150);
                        page.Mock("c").Position(200, 100).Size(100, 50);
                    });
                
                document
                    .Page()
                    .TakenAreaSize(400, 300)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(150, 100);
                        page.Mock("b").Position(250, 150).Size(50, 150);
                        page.Mock("c").Position(300, 200).Size(100, 50);
                    });
            });
            //.CompareVisually();
    }
    
    [Test]
    public void Test2()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .WithContent(content =>
            {
                content.Column(column =>
                {
                    column.Spacing(25);

                    column.Item().Mock("a").Size(150, 150);
                    column.Item().Mock("b").Size(125, 100);
                });
            })
            .ExpectedDrawResult(document =>
            {
                document
                    .Page()
                    .TakenAreaSize(150, 200)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(150, 150);
                        page.Mock("b").Position(0, 175).Size(125, 25);
                    });
                
                document
                    .Page()
                    .TakenAreaSize(125, 75)
                    .Content(page =>
                    {
                        page.Mock("b").Position(0, 0).Size(125, 75);
                    });
            })
            .Validate();
    }
    
    [Test]
    public void Test3()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .WithContent(content =>
            {
                content.Layers(layers =>
                {
                    layers.Layer().Mock("a").Size(100, 150);
                    layers.PrimaryLayer().Mock("b").Size(150, 100);
                });
            })
            .ExpectedDrawResult(document =>
            {
                document
                    .Page()
                    .TakenAreaSize(150, 100)
                    .Content(page =>
                    {
                        page.Mock("b").Position(0, 0).Size(150, 100);
                        page.Mock("a").Position(0, 0).Size(100, 150);
                        
                    });
                
                document.ExpectInfiniteLayoutException();
            })
           // .CompareVisually();
           .Validate();
    }
}