using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    internal class PreviewerWindow : FluentWindow
    {
        private readonly PreviewerControl _previewHost;

        public static readonly StyledProperty<IDocument?> DocumentProperty =
            AvaloniaProperty.Register<PreviewerWindow, IDocument?>(nameof(Document));

        public IDocument? Document
        {
            get => GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }

        public PreviewerWindow()
        {
            InitializeComponent();

            _previewHost = this.FindControl<PreviewerControl>("PreviewerSurface");

            this.FindControl<Button>("GeneratePdf")
                .Click += (_, __) => _ = PreviewerUtils.SavePdfWithDialog(Document, this);

            DocumentProperty.Changed.Subscribe(v => _previewHost.Document = v.NewValue.Value);
            HotReloadManager.Register(InvalidatePreview);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InvalidatePreview()
        {
            _previewHost.InvalidateDocument();
        }
    }
}
