using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples.Engine
{
    public class ImageSizeAttribute : Attribute
    {
        private int Width { get; }
        private int Height { get; }

        public Size Size => new Size(Width, Height);
        
        public ImageSizeAttribute(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}