using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Threading;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace QuestPDF.Previewer
{
    internal sealed class PreviewerWindowViewModel : ReactiveObject
    {
        private ObservableCollection<DocumentSnapshot.PageSnapshot> _pages = new();
        public ObservableCollection<DocumentSnapshot.PageSnapshot> Pages
        {
            get => _pages;
            set => this.RaiseAndSetIfChanged(ref _pages, value);
        }
        
        private bool _documentContentHasLayoutOverflowIssues;
        public bool DocumentContentHasLayoutOverflowIssues
        {
            get => _documentContentHasLayoutOverflowIssues;
            set => this.RaiseAndSetIfChanged(ref _documentContentHasLayoutOverflowIssues, value);
        }
        
        private float _currentScroll;
        public float CurrentScroll
        {
            get => _currentScroll;
            set => this.RaiseAndSetIfChanged(ref _currentScroll, value);
        }

        private float _scrollViewportSize;
        public float ScrollViewportSize
        {
            get => _scrollViewportSize;
            set
            {
                this.RaiseAndSetIfChanged(ref _scrollViewportSize, value);
                VerticalScrollbarVisible = value < 1;
            }
        }

        private bool _verticalScrollbarVisible;
        public bool VerticalScrollbarVisible
        {
            get => _verticalScrollbarVisible;
            private set => Dispatcher.UIThread.Post(() => this.RaiseAndSetIfChanged(ref _verticalScrollbarVisible, value));
        }

        public ReactiveCommand<Unit, Unit> ShowPdfCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowDocumentationCommand { get; }
        public ReactiveCommand<Unit, Unit> SponsorProjectCommand { get; }

        public PreviewerWindowViewModel()
        {
            CommunicationService.Instance.OnDocumentRefreshed += HandleUpdatePreview;
            
            ShowPdfCommand = ReactiveCommand.Create(ShowPdf);
            ShowDocumentationCommand = ReactiveCommand.Create(() => OpenLink("https://www.questpdf.com/api-reference/index.html"));
            SponsorProjectCommand = ReactiveCommand.Create(() => OpenLink("https://github.com/sponsors/QuestPDF"));
        }

        private void ShowPdf()
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.pdf");
            Helpers.GeneratePdfFromDocumentSnapshots(filePath, Pages);

            OpenLink(filePath);
        }
        
        private void OpenLink(string path)
        {
            using var openBrowserProcess = new Process
            {
                StartInfo = new()
                {
                    UseShellExecute = true,
                    FileName = path
                }
            };

            openBrowserProcess.Start();
        }
        
        private void HandleUpdatePreview(DocumentSnapshot documentSnapshot)
        {
            var oldPages = Pages;
            
            Pages.Clear();
            Pages = new ObservableCollection<DocumentSnapshot.PageSnapshot>(documentSnapshot.Pages);
            DocumentContentHasLayoutOverflowIssues = documentSnapshot.DocumentContentHasLayoutOverflowIssues;
            
            foreach (var page in oldPages)
                page.Picture.Dispose();
        }
    }
}
