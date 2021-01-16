using System;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample.Layouts
{
    public class ImageTemplate : IComponent
    {
        private Func<Size, byte[]> Source { get; }
        private float AspectRatio { get; }

        public ImageTemplate(byte[] source, float aspectRatio = 1.333333f) : this(_ => source, aspectRatio)
        {
            
        }
        
        public ImageTemplate(Func<Size, byte[]> source, float aspectRatio = 1.333333f)
        {
            Source = source;
            AspectRatio = aspectRatio;
        }

        public void Compose(IContainer container)
        {
            container
                .AspectRatio(AspectRatio)
                .Background("#EEEEEE")
                .Image(Source(Size.Zero));
        }
    }
}