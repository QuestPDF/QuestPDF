using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    internal class PreviewerView : UserControl
    {
        private readonly PreviewerControl _previewHost;

        public static readonly StyledProperty<IDocument?> DocumentProperty =
            AvaloniaProperty.Register<PreviewerControl, IDocument?>(nameof(Document));

        public IDocument? Document
        {
            get => GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }

        public PreviewerView()
        {
            InitializeComponent();

            _previewHost = this.FindControl<PreviewerControl>("PreviewerSurface");

            DocumentProperty
              .Changed
              .Subscribe(v => _previewHost.Document = v.NewValue.Value);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void InvalidatePreview()
        {
            _previewHost.InvalidateDocument();
        }
    }
}
