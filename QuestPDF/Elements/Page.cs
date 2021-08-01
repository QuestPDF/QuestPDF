using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Page : IComponent
    {
        public Size MinSize { get; set; } = PageSizes.A4;
        public Size MaxSize { get; set; } = PageSizes.A4;

        public float MarginLeft { get; set; }
        public float MarginRight { get; set; }
        public float MarginTop { get; set; }
        public float MarginBottom { get; set; }

        public Element Header { get; set; } = Empty.Instance;
        public Element Content { get; set; } = Empty.Instance;
        public Element Footer { get; set; } = Empty.Instance;

        public void Compose(IContainer container)
        {
            container

                .MinWidth(MinSize.Width)
                .MinHeight(MinSize.Height)
                
                .MaxWidth(MaxSize.Width)
                .MaxHeight(MaxSize.Height)
     
                .PaddingLeft(MarginLeft)
                .PaddingRight(MarginRight)
                .PaddingTop(MarginTop)
                .PaddingBottom(MarginBottom)
                
                .Decoration(decoration =>
                {
                    decoration.Header().Element(Header);
                    decoration.Content().Extend().Element(Content);
                    decoration.Footer().Element(Footer);
                });
        }
    }
}