using System.IO;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

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
                    page.Padding(25).Stack(stack =>
                    {
                        stack.Spacing(25);
                        stack.Item().Image(File.ReadAllBytes("logo.png"));
                        stack.Item().Image("logo.png");
                    });
                });
        }
        
        [Test]
        public void Exception()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A5)
                .ProducePdf()
                .ShowResults()
                .Render(page => page.Image("non_existent.png"));
        }
    }
}