using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Page : IComponent
    {
        public ContentDirection ContentDirection { get; set; }
        public TextStyle DefaultTextStyle { get; set; } = TextStyle.Default;
        
        public Size MinSize { get; set; } = PageSizes.A4;
        public Size MaxSize { get; set; } = PageSizes.A4;

        public float MarginLeft { get; set; }
        public float MarginRight { get; set; }
        public float MarginTop { get; set; }
        public float MarginBottom { get; set; }

        public Color BackgroundColor { get; set; } = Colors.White;
        
        public Element Background { get; set; } = Empty.Instance;
        public Element Foreground { get; set; } = Empty.Instance;
        
        public Element Header { get; set; } = Empty.Instance;
        public Element Content { get; set; } = Empty.Instance;
        public Element Footer { get; set; } = Empty.Instance;

        public void Compose(IContainer container)
        {
            container
                .ContentDirection(ContentDirection)
                .Background(BackgroundColor)
                .DefaultTextStyle(DefaultTextStyle)
                .Layers(layers =>
                {
                    layers
                        .Layer()
                        .DebugPointer(DebugPointerType.LayoutStructure, "Page background layer")
                        .Repeat()
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
                
                        .Decoration(decoration =>
                        {
                            decoration
                                .Before()
                                .DebugPointer(DebugPointerType.LayoutStructure, "Page header")
                                .Element(Header);

                            decoration
                                .Content()
                                .NonTrackingElement(x => IsClose(MinSize.Width, MaxSize.Width) ? x.ExtendHorizontal() : x)
                                .NonTrackingElement(x => IsClose(MinSize.Height, MaxSize.Height) ? x.ExtendVertical() : x)
                                .DebugPointer(DebugPointerType.LayoutStructure, "Page content")
                                .Element(Content);

                            decoration
                                .After()
                                .DebugPointer(DebugPointerType.LayoutStructure, "Page footer")
                                .Element(Footer);
                        });
                    
                    layers
                        .Layer()
                        .DebugPointer(DebugPointerType.LayoutStructure, "Page foreground layer")
                        .Repeat()
                        .Element(Foreground);
                });

            bool IsClose(float x, float y)
            {
                return Math.Abs(x - y) < Size.Epsilon;
            }
        }
    }
}