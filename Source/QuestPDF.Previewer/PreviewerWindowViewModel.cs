using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Threading;
using ReactiveUI;

namespace QuestPDF.Previewer
{
    internal sealed class PreviewerWindowViewModel : ReactiveObject
    {
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

        public PreviewerWindowViewModel()
        {
            CommunicationService.Instance.OnDocumentUpdated += x => DocumentContentHasLayoutOverflowIssues = x.DocumentContentHasLayoutOverflowIssues;
        }
        
        private static void OpenLink(string path)
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
    }
}
