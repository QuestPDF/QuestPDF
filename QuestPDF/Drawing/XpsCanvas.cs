using System.IO;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class XpsCanvas : SkiaDocumentCanvasBase
    {
        public XpsCanvas(Stream stream, DocumentMetadata documentMetadata) 
            : base(SKDocument.CreateXps(stream, documentMetadata.RasterDpi))
        {
            
        }
    }
}