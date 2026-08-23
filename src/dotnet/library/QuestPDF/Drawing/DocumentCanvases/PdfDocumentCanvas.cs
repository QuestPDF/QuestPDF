using System;
using System.Buffers;
using System.Diagnostics;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing.DocumentCanvases
{
    internal sealed class PdfDocumentCanvas : IDocumentCanvas, IDisposable
    {
        private SkWriteStream WriteStream { get; }
        private DocumentMetadata DocumentMetadata { get; }
        private DocumentSettings DocumentSettings { get; }
        private SkPdfTag? SemanticTag { get; set; }
        
        private SkDocument? Document { get; set; }
        private SkCanvas? CurrentPageCanvas { get; set; }
        private ProxyDrawingCanvas DrawingCanvas { get; } = new();
        
        public PdfDocumentCanvas(SkWriteStream stream, DocumentMetadata documentMetadata, DocumentSettings documentSettings)
        {
            WriteStream = stream;
            DocumentMetadata = documentMetadata;
            DocumentSettings = documentSettings;
        }

        private SkDocument CreatePdf()
        {
            // do not extract to another method, as it will cause the SkText objects
            // to be disposed before the SkPdfDocument is created
            using var title = new SkText(DocumentMetadata.Title);
            using var author = new SkText(DocumentMetadata.Author);
            using var subject = new SkText(DocumentMetadata.Subject);
            using var keywords = new SkText(DocumentMetadata.Keywords);
            using var creator = new SkText(DocumentMetadata.Creator);
            using var producer = new SkText(DocumentMetadata.Producer);
            using var language = new SkText(DocumentMetadata.Language);
            
            var internalMetadata = new SkPdfDocumentMetadata
            {
                Title = title,
                Author = author,
                Subject = subject,
                Keywords = keywords,
                Creator = creator,
                Producer = producer,
                Language = language,
                
                CreationDate = new SkDateTime(DocumentMetadata.CreationDate),
                ModificationDate = new SkDateTime(DocumentMetadata.ModifiedDate),
                
                PDFA_Conformance = GetPDFAConformanceLevel(DocumentSettings.PDFA_Conformance),
                PDFUA_Conformance = GetPDFUAConformanceLevel(DocumentSettings.PDFUA_Conformance),
                
                RasterDPI = DocumentSettings.ImageRasterDpi,
                CompressDocument = DocumentSettings.CompressDocument,
                
                SemanticNodeRoot = SemanticTag?.Instance ?? IntPtr.Zero
            };
            
            try
            {
                return SkPdfDocument.Create(WriteStream, internalMetadata);
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("PDF", exception);
            }
        }

        static Skia.PDFA_Conformance GetPDFAConformanceLevel(Infrastructure.PDFA_Conformance conformanceLevel)
        {
            return conformanceLevel switch
            {
                Infrastructure.PDFA_Conformance.None => Skia.PDFA_Conformance.None,
                // Infrastructure.PDFA_Conformance.PDFA_1A => Skia.PDFA_Conformance.PDFA_1A,
                // Infrastructure.PDFA_Conformance.PDFA_1B => Skia.PDFA_Conformance.PDFA_1B,
                Infrastructure.PDFA_Conformance.PDFA_2A => Skia.PDFA_Conformance.PDFA_2A,
                Infrastructure.PDFA_Conformance.PDFA_2B => Skia.PDFA_Conformance.PDFA_2B,
                Infrastructure.PDFA_Conformance.PDFA_2U => Skia.PDFA_Conformance.PDFA_2U,
                Infrastructure.PDFA_Conformance.PDFA_3A => Skia.PDFA_Conformance.PDFA_3A,
                Infrastructure.PDFA_Conformance.PDFA_3B => Skia.PDFA_Conformance.PDFA_3B,
                Infrastructure.PDFA_Conformance.PDFA_3U => Skia.PDFA_Conformance.PDFA_3U,
                _ => throw new ArgumentOutOfRangeException(nameof(conformanceLevel), conformanceLevel, "Unsupported PDF/A conformance level")
            };
        }
        
        static Skia.PDFUA_Conformance GetPDFUAConformanceLevel(Infrastructure.PDFUA_Conformance conformanceLevel)
        {
            return conformanceLevel switch
            {
                Infrastructure.PDFUA_Conformance.None => Skia.PDFUA_Conformance.None,
                Infrastructure.PDFUA_Conformance.PDFUA_1 => Skia.PDFUA_Conformance.PDFUA_1,
                _ => throw new ArgumentOutOfRangeException(nameof(conformanceLevel), conformanceLevel, "Unsupported PDF/UA conformance level")
            };
        }
        
        #region IDisposable
        
        ~PdfDocumentCanvas()
        {
            this.WarnThatFinalizerIsReached();
            Dispose();
        }
        
        public void Dispose()
        {
            Document?.Dispose();
            CurrentPageCanvas?.Dispose();
            DrawingCanvas?.Dispose();
            SemanticTag?.Dispose();
            
            // don't dispose WriteStream - its lifetime is managed externally
            
            GC.SuppressFinalize(this);
        }
        
        #endregion
        
        #region IDocumentCanvas
        
        public void SetSemanticTree(SemanticTreeNode? semanticTree)
        {
            SemanticTag?.Dispose();
            SemanticTag = null;

            if (semanticTree == null)
                return;

            // only the root element gets a managed wrapper: disposing it releases the entire native tree
            SemanticTag = new SkPdfTag(CreateNativeElement(semanticTree));

            static IntPtr CreateNativeElement(SemanticTreeNode node)
            {
                var element = SkPdfTag.CreateElement(node.NodeId, node.Type, node.Alt, node.Lang);
                HandleChildren();
                HandleAttributes();
                return element;

                void HandleChildren()
                {
                    var children = node.Children;
                    
                    if (children == null || children.Count == 0)
                        return;
                    
                    var childElements = ArrayPool<IntPtr>.Shared.Rent(children.Count);

                    for (var i = 0; i < children.Count; i++)
                        childElements[i] = CreateNativeElement(children[i]);

                    SkPdfTag.SetChildren(element, childElements, children.Count);
                    ArrayPool<IntPtr>.Shared.Return(childElements);
                }

                void HandleAttributes()
                {
                    var attributes = node.Attributes;
                    
                    if (attributes == null || attributes.Count == 0)
                        return;

                    for (var i = 0; i < attributes.Count; i++)
                    {
                        var attribute = attributes[i];
                        SkPdfTag.AddAttribute(element, attribute.Owner, attribute.Name, attribute.Value);
                    }
                }
            }
        }
        
        public void BeginDocument()
        {
            Document ??= CreatePdf();
        }

        public void EndDocument()
        {
            Document?.Close();
            Document?.Dispose();
        }

        public void BeginPage(Size size)
        {
            CurrentPageCanvas = Document?.BeginPage(size.Width, size.Height);
            
            DrawingCanvas.Target = new SkiaDrawingCanvas(size.Width, size.Height);
            DrawingCanvas.SetZIndex(0);
        }

        public void EndPage()
        {
            Debug.Assert(CurrentPageCanvas != null);
            
            using var documentPageSnapshot = DrawingCanvas.GetSnapshot();
            documentPageSnapshot.DrawOnSkCanvas(CurrentPageCanvas);
            
            CurrentPageCanvas.Save();
            CurrentPageCanvas.Dispose();
            CurrentPageCanvas = null;
            
            Document.EndPage();
        }
        
        public IDrawingCanvas GetDrawingCanvas()
        {
            return DrawingCanvas;
        }
        
        #endregion
    }
}