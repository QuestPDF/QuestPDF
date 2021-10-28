using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples.Engine
{
    public enum RenderingTestResult
    {
        Pdf,
        Images
    }
    
    public class RenderingTest
    {
        private string FileNamePrefix = "test";
        private Size Size { get; set; }
        private int? MaxPagesThreshold { get; set; }
        private bool ShowResult { get; set; }
        private RenderingTestResult ResultType { get; set; } = RenderingTestResult.Images;
        
        private RenderingTest()
        {
            
        }

        public static RenderingTest Create([CallerMemberName] string fileName = "test")
        {
            return new RenderingTest().FileName(fileName);
        }

        public RenderingTest FileName(string fileName)
        {
            FileNamePrefix = fileName;
            return this;
        }
        
        public RenderingTest PageSize(Size size)
        {
            Size = size;
            return this;
        }
        
        public RenderingTest PageSize(int width, int height)
        {
            return PageSize(new Size(width, height));
        }

        public RenderingTest ProducePdf()
        {
            ResultType = RenderingTestResult.Pdf;
            return this;
        }
        
        public RenderingTest ProduceImages()
        {
            ResultType = RenderingTestResult.Images;
            return this;
        }

        public RenderingTest ShowResults()
        {
            ShowResult = true;
            return this;
        }
        
        public void Render(Action<IContainer> content)
        {
            RenderDocument(container =>
            {
                container.Page(page =>
                {
                    page.Size(new PageSize(Size.Width, Size.Height));
                    page.Content().Container().Background(Colors.White).Element(content);
                });
            });
        }
        
        public void MaxPages(int value)
        {
            MaxPagesThreshold = value;
        }

        public void RenderDocument(Action<IDocumentContainer> content)
        {
            MaxPagesThreshold ??= ResultType == RenderingTestResult.Pdf ? 1000 : 10;
            var document = new SimpleDocument(content, MaxPagesThreshold.Value);

            Render(document);
        }
        
        private void Render(IDocument document)
        {
            if (ResultType == RenderingTestResult.Images)
            {
                Func<int, string> fileNameSchema = i => $"{FileNamePrefix}-${i}.png";
                document.GenerateImages(fileNameSchema);
                
                if (ShowResult)
                    Process.Start("explorer", fileNameSchema(0));
            }

            if (ResultType == RenderingTestResult.Pdf)
            {
                var fileName = $"{FileNamePrefix}.pdf";
                document.GeneratePdf(fileName);
                
                if (ShowResult)
                    Process.Start("explorer", fileName);
            }
        }
    }
}