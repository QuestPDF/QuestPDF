using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NUnit.Framework;
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
        private bool ApplyCaching { get; set; }
        private bool ApplyDebugging { get; set; }
        private RenderingTestResult ResultType { get; set; } = RenderingTestResult.Images;

        private static readonly bool ShowingResultsEnabled = (Environment.GetEnvironmentVariable("TEST_SHOW_RESULTS") ?? "true") == "true";
        
        private RenderingTest()
        {
            Console.WriteLine("Showing results: " + ShowingResultsEnabled);
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
        
        public RenderingTest MaxPages(int value)
        {
            MaxPagesThreshold = value;
            return this;
        }
        
        public RenderingTest EnableCaching(bool value = true)
        {
            ApplyCaching = value;
            return this;
        }
        
        public RenderingTest EnableDebugging(bool value = true)
        {
            ApplyDebugging = value;
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

        public void RenderDocument(Action<IDocumentContainer> content)
        {
            var document = new SimpleDocument(content, ApplyCaching, ApplyDebugging);
            Render(document);
        }
        
        public void Render(IDocument document)
        {
            if (ResultType == RenderingTestResult.Images)
            {
                var generationSettings = new ImageGenerationSettings { RasterDpi = 144 };
                
                Func<int, string> fileNameSchema = i => $"{FileNamePrefix}-${i}.png";
                document.GenerateImages(index => fileNameSchema(index), generationSettings);

                if (ShowResult && ShowingResultsEnabled)
                {
                    var firstImagePath = fileNameSchema(0);
                    OpenFileUsingDefaultProgram(firstImagePath);
                }
            }

            if (ResultType == RenderingTestResult.Pdf)
            {
                if (ShowResult && ShowingResultsEnabled)
                    document.GeneratePdfAndShow();

                else
                    document.GeneratePdf();
            }
        }
        
        static void OpenFileUsingDefaultProgram(string filePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                }
            };

            process.Start();
            process.WaitForExit();
        }
    }
}