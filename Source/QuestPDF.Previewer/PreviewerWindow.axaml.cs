using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QuestPDF.Previewer
{
    internal partial class PreviewerWindow : Window
    {
        public PreviewerWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
