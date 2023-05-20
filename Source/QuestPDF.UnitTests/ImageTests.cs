using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;
using SkiaSharp;
using ImageElement = QuestPDF.Elements.Image;
using DocumentImage = QuestPDF.Infrastructure.Image;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ImageTests
    {
        [Test]
        public void Measure_TakesAvailableSpaceRegardlessOfSize()
        {
            TestPlan
                .For(x => new ImageElement
                {
                    DocumentImage = GenerateDocumentImage(400, 300)
                })
                .MeasureElement(new Size(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(300, 200));
        }
        
        [Test]
        public void Draw_TakesAvailableSpaceRegardlessOfSize()
        {
            TestPlan
                .For(x => new ImageElement
                {
                    CompressionQuality = ImageCompressionQuality.High,
                    TargetDpi = DocumentSettings.DefaultRasterDpi,
                    DocumentImage = GenerateDocumentImage(400, 300)
                })
                .DrawElement(new Size(300, 200))
                .ExpectCanvasDrawImage(new Position(0, 0), new Size(300, 200))
                .CheckDrawResult();
        }
        
        [Test]
        public void Fluent_RecognizesImageProportions()
        {
            var image = GenerateDocumentImage(600, 200);
            
            TestPlan
                .For(x =>
                {
                    var container = new Container();
                    container.Image(image);
                    return container;
                })
                .MeasureElement(new Size(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(300, 100));;
        }
        
        [Test]
        public void UsingSharedImageShouldNotDrasticallyIncreaseDocumentSize()
        {
            var photo = File.ReadAllBytes("Resources/photo.jpg");

            var documentWithSingleImageSize = GetDocumentSize(container =>
            {
                container.Image(photo);
            });

            var documentWithMultipleImagesSize = GetDocumentSize(container =>
            {
                container.Column(column =>
                {
                    foreach (var i in Enumerable.Range(0, 10))
                        column.Item().Image(photo);
                });
            });

            var documentWithSingleImageUsedMultipleTimesSize = GetDocumentSize(container =>
            {
                container.Column(column =>
                {
                    var sharedImage = DocumentImage.FromBinaryData(photo);
                    
                    foreach (var i in Enumerable.Range(0, 10))
                        column.Item().Image(sharedImage);
                });
            });

            (documentWithMultipleImagesSize / (float)documentWithSingleImageSize).Should().BeInRange(9.9f, 10);
            (documentWithSingleImageUsedMultipleTimesSize / (float)documentWithSingleImageSize).Should().BeInRange(1f, 1.05f);
        }
        
        [Test]
        public void ImageCompressionHasImpactOnDocumentSize()
        {
            var photo = File.ReadAllBytes("Resources/photo.jpg");

            var veryLowCompressionSize = GetDocumentSize(container => container.Image(photo).WithCompressionQuality(ImageCompressionQuality.VeryLow));
            var bestCompressionSize = GetDocumentSize(container => container.Image(photo).WithCompressionQuality(ImageCompressionQuality.Best));

            (bestCompressionSize / (float)veryLowCompressionSize).Should().BeGreaterThan(25);
        }
        
        [Test]
        public void TargetDpiHasImpactOnDocumentSize()
        {
            var photo = File.ReadAllBytes("Resources/photo.jpg");
            
            var lowDpiSize = GetDocumentSize(container => container.Image(photo).WithRasterDpi(12));
            var highDpiSize = GetDocumentSize(container => container.Image(photo).WithRasterDpi(144));

            (highDpiSize / (float)lowDpiSize).Should().BeGreaterThan(40);
        }

        private static int GetDocumentSize(Action<IContainer> container)
        {
            return Document
                .Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Content().Element(container);
                    });
                })
                .GeneratePdf()
                .Length;
        }

        DocumentImage GenerateDocumentImage(int width, int height)
        {
            var image = Placeholders.Image(width, height);
            return DocumentImage.FromBinaryData(image);
        }
    }
}