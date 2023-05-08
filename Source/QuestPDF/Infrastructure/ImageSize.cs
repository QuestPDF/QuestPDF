namespace QuestPDF.Infrastructure
{
    public struct ImageSize
    {
        public readonly int Width;
        public readonly int Height;

        public ImageSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}