using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Threading;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Previewer
{
    public record RenderedPageInfo(SKPicture Picture, Size Size);

    internal class DocumentRenderer : INotifyPropertyChanged
    {
        public float PageSpacing { get; set; }
        public Size Bounds { get; private set; }
        public IDocument? Document { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<RenderedPageInfo> _pages = new();
        public ObservableCollection<RenderedPageInfo> Pages
        {
            get => _pages;
            set
            {
                if (_pages != value)
                {
                    _pages = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pages)));
                }
            }
        }

        private bool _isRendering;
        public bool IsRendering
        {
            get => _isRendering;
            private set
            {
                if (_isRendering != value)
                {
                    _isRendering = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRendering)));
                }
            }
        }

        public void UpdateDocument(IDocument? document)
        {
            Document = document;
            if (document != null)
            {
                try
                {
                    IsRendering = true;
                    RenderDocument(document);
                }
                catch (Exception ex)
                {
                    RenderDocument(CreateExceptionDocument(ex));
                }
                finally
                {
                    IsRendering = false;
                }
            }
        }

        private void RenderDocument(IDocument document)
        {
            var canvas = new PreviewerCanvas();

            DocumentGenerator.RenderDocument(canvas, new SizeTrackingCanvas(), document, s =>
            {
                var width = s.PageSizes.Max(p => p.Width);
                var height = s.PageSizes.Sum(p => p.Height) + ((s.PageSizes.Count - 1) * PageSpacing);
                Bounds = new Size(width, height);
            });

            foreach (var pages in Pages)
                pages?.Picture?.Dispose();
            Dispatcher.UIThread.Post(() =>
            {
                Pages.Clear();
                Pages = new ObservableCollection<RenderedPageInfo>(canvas.Pictures);
            });
        }

        private static IDocument CreateExceptionDocument(Exception exception)
        {
            return Fluent.Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Inch);
                    page.PageColor(Colors.Red.Lighten4);
                    page.DefaultTextStyle(x => x.FontSize(16));

                    page.Header()
                        .BorderBottom(2)
                        .BorderColor(Colors.Red.Medium)
                        .PaddingBottom(5)
                        .Text("Ooops! Something went wrong...").FontSize(28).FontColor(Colors.Red.Medium).Bold();

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        var currentException = exception;

                        while (currentException != null)
                        {
                            column.Item().Text(exception.GetType().Name).FontSize(20).SemiBold();
                            column.Item().Text(exception.Message).FontSize(14);
                            column.Item().PaddingTop(10).Text(exception.StackTrace).FontSize(10).Light();

                            currentException = currentException.InnerException;

                            if (currentException != null)
                                column.Item().PaddingVertical(15).LineHorizontal(2).LineColor(Colors.Red.Medium);
                        }
                    });
                });
            });
        }
    }
}
