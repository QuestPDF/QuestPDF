using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace QuestPDF.Previewer
{
    internal class PreviewerPageControl : Control
    {
        public static readonly StyledProperty<RenderedPageInfo?> PageProperty =
            AvaloniaProperty.Register<PreviewerPageControl, RenderedPageInfo?>(nameof(Page));

        public RenderedPageInfo? Page
        {
            get => GetValue(PageProperty);
            set => SetValue(PageProperty, value);
        }

        public PreviewerPageControl()
        {
            PageProperty.Changed.Take(1).Subscribe(p =>
            {
                var size = p.NewValue.Value?.Size ?? Infrastructure.Size.Zero;
                Width = size.Width;
                Height = size.Height;
            });

            ClipToBounds = true;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            //context.DrawRectangle(Brushes.Red, null, new Rect(0, 0, Bounds.Width, Bounds.Height));

            var picture = Page?.Picture;
            if (picture != null)
                context.Custom(new SkCustomDrawOperation(
                    new Rect(0, 0, Bounds.Width, Bounds.Height),
                    c => c.DrawPicture(picture)));
        }
    }
}
