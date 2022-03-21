using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Threading;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    internal class DocumentRenderer : INotifyPropertyChanged
    {
        public IDocument? Document { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<PreviewPage> _pages = new();
        public ObservableCollection<PreviewPage> Pages
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
            
            if (document == null) 
                return;
            
            try
            {
                IsRendering = true;
                RenderDocument(document);
            }
            catch (Exception exception)
            {
                var exceptionDocument = new ExceptionDocument(exception);
                RenderDocument(exceptionDocument);
            }
            finally
            {
                IsRendering = false;
            }
        }

        private void RenderDocument(IDocument document)
        {
            var canvas = new PreviewerCanvas();

            DocumentGenerator.RenderDocument(canvas, document);
            
            foreach (var pages in Pages)
                pages?.Picture?.Dispose();
            
            Dispatcher.UIThread.Post(() =>
            {
                Pages.Clear();
                Pages = new ObservableCollection<PreviewPage>(canvas.Pictures);
            });
        }
    }
}
