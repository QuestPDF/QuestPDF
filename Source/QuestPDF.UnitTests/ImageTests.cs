using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
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
        public void Measure_TakesMinimalSpaceRegardlessOfSize()
        {
            TestPlan
                .For(x => new ImageElement
                {
                    DocumentImage = GenerateDocumentImage(400, 300)
                })
                .MeasureElement(new Size(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(0, 0));
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
            var image = GenerateDocumentImage(60, 20);
            
            TestPlan
                .For(x =>
                {
                    var container = new Container();
                    container.Image(image);
                    return container;
                })
                .MeasureElement(new Size(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(300, 100));
        }
        
        [Test]
        public void ImageObject_ThrowsEncodingException_WhenImageDataIsIncorrect()
        {
            Func<Infrastructure.Image> action = () => Infrastructure.Image.FromBinaryData(new byte[] { 1, 2, 3 });
            Assert.That(action, Throws.Exception.TypeOf<DocumentComposeException>().With.Message.EqualTo("Cannot decode the provided image."));
        }
        
        [Test]
        public void ImageObject_ThrowsEncodingException_WhenStreamIsIncorrect()
        {
            Func<Infrastructure.Image> action = () =>
            {
                using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
                return Infrastructure.Image.FromStream(stream);
            };

            Assert.That(action, Throws.Exception.TypeOf<DocumentComposeException>().With.Message.EqualTo("Cannot decode the provided image."));
        }
        
        [Test]
        public void ImageObject_ThrowsFileNotFoundException_FileIsNotFound()
        {
            Func<Infrastructure.Image> action = () => Infrastructure.Image.FromFile("non-existing-file.jpg");
            Assert.That(action, Throws.Exception.TypeOf<DocumentComposeException>().With.Message.EqualTo("Cannot load provided image, file not found: non-existing-file.jpg"));
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

            using var sharedImage = DocumentImage.FromBinaryData(photo);
            
            var documentWithSingleImageUsedMultipleTimesSize = GetDocumentSize(container =>
            {
                container.Column(column =>
                {
                    foreach (var i in Enumerable.Range(0, 10))
                        column.Item().Image(sharedImage);
                });
            });

            var documentWithMultipleImagesSizeRatio = (documentWithMultipleImagesSize / (float)documentWithSingleImageSize);
            Assert.That(documentWithMultipleImagesSizeRatio, Is.InRange(9.9f, 10));
            
            var documentWithSingleImageUsedMultipleTimesSizeRatio = (documentWithSingleImageUsedMultipleTimesSize / (float)documentWithSingleImageSize);
            Assert.That(documentWithSingleImageUsedMultipleTimesSizeRatio, Is.InRange(1f, 1.05f));
        }
        
        [Test]
        public void ImageCompressionHasImpactOnDocumentSize()
        {
            var photo = File.ReadAllBytes("Resources/photo.jpg");

            var veryLowCompressionSize = GetDocumentSize(container => container.Image(photo).WithCompressionQuality(ImageCompressionQuality.VeryLow));
            var bestCompressionSize = GetDocumentSize(container => container.Image(photo).WithCompressionQuality(ImageCompressionQuality.Best));

            var compressionSizeRatio = (bestCompressionSize / (float)veryLowCompressionSize);
            Assert.That(compressionSizeRatio, Is.GreaterThan(10));
        }
        
        [Test]
        public void TargetDpiHasImpactOnDocumentSize()
        {
            var photo = File.ReadAllBytes("Resources/photo.jpg");
            
            var lowDpiSize = GetDocumentSize(container => container.Image(photo).WithRasterDpi(12));
            var highDpiSize = GetDocumentSize(container => container.Image(photo).WithRasterDpi(144));

            var dpiSizeRatio = (highDpiSize / (float)lowDpiSize);
            Assert.That(dpiSizeRatio, Is.GreaterThan(40));
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

        static DocumentImage GenerateDocumentImage(int width, int height)
        {
            var image = Placeholders.Image(width, height);
            var result = DocumentImage.FromBinaryData(image);
            result.IsShared = false;
            return result;
        }
    }
}