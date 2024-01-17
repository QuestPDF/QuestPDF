using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    internal sealed class XpsCanvas : SkiaDocumentCanvasBase
    {
        public XpsCanvas(Stream stream, DocumentSettings documentSettings) 
            : base(CreateXps(stream, documentSettings))
        {
            
        }
        
        private static SKDocument CreateXps(Stream stream, DocumentSettings documentSettings)
        {
            try
            {
                return SKDocument.CreateXps(stream, documentSettings.ImageRasterDpi);
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("XPS", exception);
            }
        }
    }
}