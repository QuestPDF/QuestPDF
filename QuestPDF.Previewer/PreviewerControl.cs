using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
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

        public static readonly StyledProperty<double> PageSpacingProperty =
            AvaloniaProperty.Register<PreviewerControl, double>(nameof(PageSpacing), 20);

        public double PageSpacing
        {
            get => GetValue(PageSpacingProperty);
            set => SetValue(PageSpacingProperty, value);
        }

        private readonly DocumentRenderer _renderer = new();

        public PreviewerControl()
        {
            ClipToBounds = true;

            _renderer.PageSpacing = (float)PageSpacing;
            DocumentProperty.Changed.Subscribe(_ => _renderer.UpdateDocument(Document));
            PageSpacingProperty.Changed.Subscribe(f => _renderer.PageSpacing = (float)f.NewValue.Value);
        }

        public override void Render(DrawingContext context)
        {
            IsGeneratingDocument = _renderer.IsRendering;
            if (_renderer.IsRendering)
                return;

            if (_renderer.RenderException != null)
            {
                context.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, Bounds.Width, Bounds.Height));
                DrawException(context, _renderer.RenderException);
                return;
            }

            Width = _renderer.Bounds.Width;
            Height = _renderer.Bounds.Height;

            if (_renderer.Picture == null)
                return;

            context.Custom(new SkCustomDrawOperation(
                new Rect(0, 0, Bounds.Width, Bounds.Height),
                c => c.DrawPicture(_renderer.Picture)));
        }

        internal void InvalidateDocument()
        {
            Dispatcher.UIThread.Post(() =>
            {
                _renderer.UpdateDocument(Document);
                InvalidateVisual();
            });
        }

        private void DrawException(DrawingContext context, Exception ex)
        {
            var parentBounds = Parent?.Bounds ?? Bounds;

            var exceptionMsg = string.Join("\n", ex.GetType(), ex.Message);

            var fmtText = new FormattedText($"Exception occured:\n{exceptionMsg}", 
                Typeface.Default, 25, TextAlignment.Left, TextWrapping.Wrap, 
                new Avalonia.Size(parentBounds.Width / 2, parentBounds.Height / 2));

            var center = new Point((parentBounds.Width - fmtText.Bounds.Width) / 2, (parentBounds.Height - fmtText.Bounds.Height) / 2);
            context.DrawText(Brushes.Black, center, fmtText);
        }
    }
}
