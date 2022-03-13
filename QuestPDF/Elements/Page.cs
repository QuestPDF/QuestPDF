using System;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Page : IComponent
    {
        public TextStyle DefaultTextStyle { get; set; } = new TextStyle();
        
        public Size MinSize { get; set; } = PageSizes.A4;
        public Size MaxSize { get; set; } = PageSizes.A4;

        public float MarginLeft { get; set; }
        public float MarginRight { get; set; }
        public float MarginTop { get; set; }
        public float MarginBottom { get; set; }

        public string BackgroundColor { get; set; } = Colors.Transparent;
        
        public Element Background { get; set; } = Empty.Instance;
        public Element Foreground { get; set; } = Empty.Instance;
        
        public Element Header { get; set; } = Empty.Instance;
        public Element Content { get; set; } = Empty.Instance;
        public Element Footer { get; set; } = Empty.Instance;

        public void Compose(IContainer container)
        {
            container
                .Background(BackgroundColor)
                .Layers(layers =>
                {
                    layers
                        .Layer()
                        .DebugPointer("Page background layer")
                        .Element(Background);
                    
                    layers
                        .PrimaryLayer()
                        .MinWidth(MinSize.Width)
                        .MinHeight(MinSize.Height)
                
                        .MaxWidth(MaxSize.Width)
                        .MaxHeight(MaxSize.Height)

                        .PaddingLeft(MarginLeft)
                        .PaddingRight(MarginRight)
                        .PaddingTop(MarginTop)
                        .PaddingBottom(MarginBottom)
                
                        .DefaultTextStyle(DefaultTextStyle)
                
                        .Decoration(decoration =>
                        {
                            decoration
                                .Before()
                                .DebugPointer("Page header")
                                .Element(Header);

                            decoration
                                .Content()
                                .Element(x => IsClose(MinSize.Width, MaxSize.Width) ? x.ExtendHorizontal() : x)
                                .Element(x => IsClose(MinSize.Height, MaxSize.Height) ? x.ExtendVertical() : x)
                                .DebugPointer("Page content")
                                .Element(Content);

                            decoration
                                .After()
                                .DebugPointer("Page footer")
                                .Element(Footer);
                        });
                    
                    layers
                        .Layer()
                        .DebugPointer("Page foreground layer")
                        .Element(Foreground);
                });

            bool IsClose(float x, float y)
            {
                return Math.Abs(x - y) < Size.Epsilon;
            }
        }
    }
}