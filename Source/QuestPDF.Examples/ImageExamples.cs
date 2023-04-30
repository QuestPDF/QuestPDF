using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class ImageExamples
    {
        [Test]
        public void LoadingImage()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A5)
                .ProducePdf()
                .ShowResults()
                .Render(page =>
                {
                    page.Padding(25).Column(column =>
                    {
                        column.Spacing(25);
                        
                        column.Item().Image("logo.png");

                        var binaryData = File.ReadAllBytes("logo.png");
                        column.Item().Image(binaryData);
                        
                        using var stream = new FileStream("logo.png", FileMode.Open);
                        column.Item().Image(stream);
                    });
                });
        }
        
        [Test]
        public void DynamicImage()
        {
            RenderingTest
                .Create()
                .PageSize(450, 350)
                .ProducePdf()
                .ShowResults()
                .Render(page =>
                {
                    page.Padding(25)
                        .Image(Placeholders.Image);
                });
        }
        
        [Test]
        public void Exception()
        {
            Assert.Throws<DocumentComposeException>(() =>
            {
                RenderingTest
                    .Create()
                    .PageSize(PageSizes.A2)
                    .ProducePdf()
                    .ShowResults()
                    .Render(page => page.Image("non_existent.png"));
            });
        }
        
        
        
        [Test]
        public void ImageResolutionScaling()
        {
            var image = Image.FromFile("large-image.jpg");
            
            Document
                .Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(210, 210);
                        page.Margin(50);
                        page.Content().Image(image);
                    });
                })
                .GeneratePdf($"test.pdf");
        }
    }
}