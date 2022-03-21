using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    internal class PreviewerWindow : Window
    {
        public DocumentRenderer DocumentRenderer { get; } = new();

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
            //
            // this.FindControl<Button>("GeneratePdf")
            //     .Click += (_, _) => _ = PreviewerUtils.SavePdfWithDialog(Document, this);

            DocumentProperty.Changed.Subscribe(v => Task.Run(() => DocumentRenderer.UpdateDocument(v.NewValue.Value)));
            HotReloadManager.UpdateApplicationRequested += (_, _) => InvalidatePreview();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InvalidatePreview()
        {
            Dispatcher.UIThread.Post(() =>
            {
                var document = Document;
                _ = Task.Run(() => DocumentRenderer.UpdateDocument(document));
            });
        }
    }
}
