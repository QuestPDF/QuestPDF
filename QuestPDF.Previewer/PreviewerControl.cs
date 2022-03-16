using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    internal class PreviewerControl : Control
    {
        public static readonly StyledProperty<IDocument?> DocumentProperty =
          AvaloniaProperty.Register<PreviewerControl, IDocument?>(nameof(Document));

        public IDocument? Document
        {
            get => GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }

        public static readonly StyledProperty<bool> IsGeneratingDocumentProperty =
          AvaloniaProperty.Register<PreviewerControl, bool>(nameof(IsGeneratingDocument));

        public bool IsGeneratingDocument
        {
            get => GetValue(IsGeneratingDocumentProperty);
            set => SetValue(IsGeneratingDocumentProperty, value);
        }

        public float PageSpacing { get; set; } = 20;

        public PreviewerControl()
        {
            DocumentProperty
              .Changed
              .Subscribe(_ => InvalidateVisual());
        }

        public override void Render(DrawingContext context)
        {
            if (Document == null)
                return;

            try
            {
                IsGeneratingDocument = true;
                Render(context, Document);
            }
            finally
            {
                IsGeneratingDocument = false;
            }
        }

        private void Render(DrawingContext context, IDocument document)
        {
            //TODO optimize, currently we generate the document twice.
            // First generation for calculating the sizes
            // Second generation for the actual rendering.
            var docSizeTracker = new SizeTrackingCanvas();
            try
            {
                DocumentGenerator.RenderDocument(docSizeTracker, document);
            }
            catch (Exception ex)
            {
                DrawException(context, ex);
                return;
            }

            Width = docSizeTracker.PageSizes.Max(p => p.Width);
            Height = docSizeTracker.PageSizes.Sum(p => p.Height) + ((docSizeTracker.PageSizes.Count - 1) * PageSpacing);

            context.Custom(new DocumentRenderOperation(document, new Rect(0, 0, Width, Height), PageSpacing));
        }

        private void DrawException(DrawingContext context, Exception ex)
        {
            var exceptionMsg = string.Join("\n", ex.GetType(), ex.Message);
            var fmtText = new FormattedText($"Exception occured:\n{exceptionMsg}", Typeface.Default, 25, TextAlignment.Left, TextWrapping.Wrap, new Avalonia.Size(Parent.Bounds.Width / 2, Parent.Bounds.Height / 2));
            var center = new Point((Parent.Bounds.Width - fmtText.Bounds.Width) / 2, (Parent.Bounds.Height - fmtText.Bounds.Height) / 2);
            context.DrawText(Brushes.Black, center, fmtText);
        }
    }
}
