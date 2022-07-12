using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using Avalonia;
using ReactiveUI;
using Unit = System.Reactive.Unit;
using Avalonia.Threading;

namespace QuestPDF.Previewer
{
    internal class PreviewerWindowViewModel : ReactiveObject
    {
        private ObservableCollection<PreviewPage> _pages = new();
        public ObservableCollection<PreviewPage> Pages
        {
            get => _pages;
            set => this.RaiseAndSetIfChanged(ref _pages, value);
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
            ShowDocumentationCommand = ReactiveCommand.Create(() => OpenLink("https://www.questpdf.com/documentation/api-reference.html"));
            SponsorProjectCommand = ReactiveCommand.Create(() => OpenLink("https://github.com/sponsors/QuestPDF"));

            LoadItems();
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
        
        private void HandleUpdatePreview(ICollection<PreviewPage> pages)
        {
            var oldPages = Pages;
            
            Pages.Clear();
            Pages = new ObservableCollection<PreviewPage>(pages);
            
            foreach (var page in oldPages)
                page.Picture.Dispose();
        }
        
        public ObservableCollection<InspectionElement> Items { get; set; }
        
        private InspectionElement _selectedItem;
        public InspectionElement SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }
        
        public void LoadItems()
        {
            Items = new ObservableCollection<InspectionElement>();

            var text = File.ReadAllText("hierarchy.json");
            var hierarchy = JsonSerializer.Deserialize<InspectionElement>(text);

            Items.Add(hierarchy);
        }
    }

    internal class InspectionElementLocation
    {
        public int PageNumber { get; set; }
        public float Top { get; set; }
        public float Left { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }

    internal class Metadata
    {
        public string Label { get; set; }
        public string Value { get; set; }

        public Metadata(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }
    
    internal class InspectionElement
    {
        public string Element { get; set; }
        public List<InspectionElementLocation> Location { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public List<InspectionElement> Children { get; set; }
        public Thickness Margin { get; set; }
        
        public string FontColor => Element == "DebugPointer" ? "#FFF" : "#AFFF";
        public string Text => Element == "DebugPointer" ? Properties.First(x => x.Key == "Target").Value : Element;

        public IList<Metadata> Metadata => ListMetadata().ToList();

        public IEnumerable<Metadata> ListMetadata()
        {
            yield return new Metadata("Element name", Element);
            yield return new Metadata("Position left", Location[0].Left.ToString("N2"));
            yield return new Metadata("Position top", Location[0].Top.ToString("N2"));
            yield return new Metadata("Width", Location[0].Width.ToString("N2"));
            yield return new Metadata("Height", Location[0].Height.ToString("N2"));

            foreach (var property in Properties)
                yield return new Metadata(property.Key, property.Value);
        }
    }
}
