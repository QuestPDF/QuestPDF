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
        public void ScalingImageWithAlpha()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProducePdf()
                .ShowResults()
                .Render(page =>
                {
                    page.Padding(25).Layers(layers =>
                    {
                        layers.Layer().Image(Placeholders.Image);
                        layers.PrimaryLayer().Padding(25).Image("multilingual.png");
                    });
                });
        }

        [Test]
        public void DpiSetting()
        {
            RenderingTest
                .Create()
                .PageSize(400, 600)
                .ProduceImages()
                .ShowResults()
                .Render(page =>
                {
                    page.Padding(15).Column(column =>
                    {
                        column.Spacing(15);
                        
                        column.Item().Image("photo.jpg").WithRasterDpi(16).FitUnproportionally();
                        column.Item().Image("photo.jpg").WithRasterDpi(72);
                    });
                });
        }
        
        [Test]
        public void CompressionSetting()
        {
            RenderingTest
                .Create()
                .PageSize(400, 600)
                .ProduceImages()
                .ShowResults()
                .Render(page =>
                {
                    page.Padding(15).Column(column =>
                    {
                        column.Spacing(15);
                        
                        column.Item().Image("photo.jpg").WithCompressionQuality(ImageCompressionQuality.VeryLow).WithRasterDpi(72);
                        column.Item().Image("photo.jpg").WithCompressionQuality(ImageCompressionQuality.High).WithRasterDpi(72);
                    });
                });
        }
        
        [Test]
        public void ReusingImage_With()
        {
            RenderingTest
                .Create()
                .PageSize(400, 600)
                .ProduceImages()
                .ShowResults()
                .Render(page =>
                {
                    page.Padding(15).Column(column =>
                    {
                        column.Spacing(15);

                        var image = Image.FromFile("checkbox.png");
                        
                        foreach (var i in Enumerable.Range(0, 5))
                        {
                            column.Item().Row(row =>
                            {
                                row.AutoItem().Width(24).Image(image);
                                row.RelativeItem().PaddingLeft(8).AlignMiddle().Text(Placeholders.Label()).FontSize(16);
                            });
                        }
                    });
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
        public void ReusingTheSameImageFileShouldBePossible()
        {
            var fileName = Path.GetTempFileName() + ".jpg";
            
            try
            {
                var image = Placeholders.Image(300, 100);
                
                using var file = File.Create(fileName);
                file.Write(image);
                file.Dispose();
                
                RenderingTest
                    .Create()
                    .ProducePdf()
                    .PageSize(PageSizes.A4)
                    .ShowResults()
                    .Render(container =>
                    {
                        container
                            .Padding(20)
                            .Column(column =>
                            {
                                column.Spacing(20);
                                
                                column.Item().Image(fileName);
                                column.Item().Image(fileName);
                                column.Item().Image(fileName);
                            });
                    });
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}