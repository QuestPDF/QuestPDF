using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class XpsCanvas : SkiaDocumentCanvasBase
    {
        public XpsCanvas(Stream stream, DocumentMetadata documentMetadata) 
            : base(CreateXps(stream, documentMetadata))
        {
            
        }
        
        private static SKDocument CreateXps(Stream stream, DocumentMetadata documentMetadata)
        {
            try
            {
                return SKDocument.CreateXps(stream, documentMetadata.RasterDpi);
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("XPS", exception);
            }
        }
    }
}