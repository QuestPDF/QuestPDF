using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class FreeCanvas : ICanvas, IRenderingCanvas
    {
        #region IRenderingCanvas

        public void BeginDocument()
        {
            
        }

        public void EndDocument()
        {
            
        }

        public void BeginPage(Size size)
        {
            
        }

        public void EndPage()
        {
            
        }

        #endregion

        #region ICanvas

        public void Translate(Position vector)
        {
            
        }

        public void DrawRectangle(Position vector, Size size, string color)
        {
            
        }

        public void DrawText(string text, Position position, TextStyle style)
        {
            
        }

        public void DrawImage(SKImage image, Position position, Size size)
        {
            
        }

        public void DrawExternalLink(string url, Size size)
        {
           
        }

        public void DrawLocationLink(string locationName, Size size)
        {
            
        }

        public void DrawLocation(string locationName)
        {
            
        }

        #endregion
    }
}