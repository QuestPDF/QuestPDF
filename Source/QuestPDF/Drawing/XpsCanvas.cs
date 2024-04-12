using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing
{
    internal sealed class XpsCanvas : SkiaDocumentCanvasBase
    {
        public XpsCanvas(SkWriteStream stream, DocumentSettings documentSettings) : base(CreateXps(stream, documentSettings))
        {
            
        }
        
        private static SkDocument CreateXps(SkWriteStream stream, DocumentSettings documentSettings)
        {
            try
            {
                return SkXpsDocument.Create(stream, documentSettings.ImageRasterDpi);
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("XPS", exception);
            }
        }
    }
}