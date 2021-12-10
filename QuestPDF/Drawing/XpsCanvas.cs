using System.IO;
using QuestPDF.Helpers;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class XpsCanvas : SkiaDocumentCanvasBase
    {
        public XpsCanvas(Stream stream, DocumentMetadata documentMetadata) 
            : base(SKDocument.CreateXps(new WriteStreamWrapper(stream), documentMetadata.RasterDpi))
        {
            
        }
    }
}