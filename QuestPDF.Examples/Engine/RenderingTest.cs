using System;
using System.Diagnostics;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples.Engine
{
    public class RenderingTest
    {
        private string FileNamePrefix = "test";
        private Size Size { get; set; }
        
        private RenderingTest()
        {
            
        }

        public static RenderingTest Create()
        {
            return new RenderingTest();
        }

        public RenderingTest FileName(string fileName)
        {
            FileNamePrefix = fileName;
            return this;
        }
        
        public RenderingTest PageSize(int width, int height)
        {
            Size = new Size(width, height);
            return this;
        }
        
        public void Render(Action<IContainer> content)
        {
            var container = new Container();
            content(container);
            
            Func<int, string> fileNameSchema = i => $"{FileNamePrefix}-${i}.png";

            var document = new SimpleDocument(container, Size);
            document.GenerateImages(fileNameSchema);

            Process.Start("explorer", fileNameSchema(0));
        }
    }
}