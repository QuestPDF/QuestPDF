﻿using System;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing
{
    internal class SkiaDocumentCanvasBase : SkiaCanvasBase, IDisposable
    {
        private SkDocument? Document { get; }

        protected SkiaDocumentCanvasBase(SkDocument document)
        {
            Document = document;
        }

        ~SkiaDocumentCanvasBase()
        {
            Dispose();
        }

        public void Dispose()
        {
            Canvas?.Dispose();
            Document?.Dispose();
            GC.SuppressFinalize(this);
        }
        
        public override void BeginDocument()
        {
            
        }

        public override void EndDocument()
        {
            Canvas?.Dispose();
            
            Document.Close();
            Document.Dispose();
        }

        public override void BeginPage(Size size)
        {
            Canvas = Document.BeginPage(size.Width, size.Height);
            
            base.BeginPage(size);
        }

        public override void EndPage()
        {
            base.EndPage();
            
            Document.EndPage();
            Canvas.Dispose();
        }
    }
}