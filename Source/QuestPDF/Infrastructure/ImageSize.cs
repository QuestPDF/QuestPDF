using System;

namespace QuestPDF.Infrastructure
{
    public readonly struct ImageSize
    {
        public readonly int Width;
        public readonly int Height;

        public ImageSize(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Image dimensions must be positive");
            
            Width = width;
            Height = height;
        }
    }
}