using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Examples.Engine
{
    [TestFixture]
    public class ExampleTestBase
    {
        private readonly Size DefaultImageSize = new Size(400, 300);
        private const string ResultPath = "Result";

        [SetUp]
        public void Setup()
        {
            if (Directory.Exists(ResultPath))
                Directory.Delete(ResultPath, true);
            
            Directory.CreateDirectory(ResultPath);
        }
        
        [Test]
        public void RunTest()
        {
            GetType()
                .GetMethods()
                .Where(IsExampleMethod)
                .ToList()
                .ForEach(HandleExample);
        }

        private bool IsExampleMethod(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return parameters.Length == 1 && parameters.First().ParameterType == typeof(IContainer);
        }
        
        private T GetAttribute<T>(MethodInfo methodInfo) where T : Attribute
        {
            return methodInfo.GetCustomAttributes().FirstOrDefault(y => y is T) as T;
        }
        
        private void HandleExample(MethodInfo methodInfo)
        {
            var size = GetAttribute<ImageSizeAttribute>(methodInfo)?.Size ?? DefaultImageSize;
            var fileName = GetAttribute<FileNameAttribute>(methodInfo)?.FileName ?? methodInfo.Name;
            var showResult = GetAttribute<ShowResultAttribute>(methodInfo) != null;
            var shouldIgnore = GetAttribute<IgnoreAttribute>(methodInfo) != null;

            if (shouldIgnore)
                return;
            
            var container = new Container();
            methodInfo.Invoke(this, new object[] {container});

            Func<int, string> fileNameSchema = i => $"{fileName.ToLower()}-${i}.png";

            try
            {
                var document = new SimpleDocument(container, size);
                document.GenerateImages(fileNameSchema);
            }
            catch (Exception e)
            {
                throw new Exception($"Cannot perform test ${fileName}", e);
            }

            if (showResult)
                Process.Start("explorer", fileNameSchema(0));
        }
        
        private byte[] RenderPage(Element element, Size size)
        {
            // scale the result so it is more readable
            const float scalingFactor = 2;
            
            var imageInfo = new SKImageInfo((int)(size.Width * scalingFactor), (int)(size.Height * scalingFactor));
            using var surface = SKSurface.Create(imageInfo);
            surface.Canvas.Scale(scalingFactor);

            var canvas = new Canvas(surface.Canvas);
            element?.Draw(canvas, size);

            surface.Canvas.Save();
            return surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).ToArray();
        }
    }
}