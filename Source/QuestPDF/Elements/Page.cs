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

        public string BackgroundColor { get; set; } = Colors.White;
        
        public Element Background { get; set; } = Empty.Instance;
        public Element Foreground { get; set; } = Empty.Instance;
        
        public Element Header { get; set; } = Empty.Instance;
        public Element Content { get; set; } = Empty.Instance;
        public Element Footer { get; set; } = Empty.Instance;

        public void Compose(IContainer container)
        {
            container
                .InspectionPointer("Page Group", InspectorPointerType.PageStructure)
                .ContentDirection(ContentDirection)
                .Background(BackgroundColor)
                .DefaultTextStyle(DefaultTextStyle)
                .Layers(layers =>
                {
                    layers
                        .Layer()
                        .InspectionPointer("Page Background Layer", InspectorPointerType.PageStructure)
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
                                .InspectionPointer("Page Header", InspectorPointerType.PageStructure)
                                .Element(Header);

                            decoration
                                .Content()
                                .Element(x => IsClose(MinSize.Width, MaxSize.Width) ? x.ExtendHorizontal() : x)
                                .Element(x => IsClose(MinSize.Height, MaxSize.Height) ? x.ExtendVertical() : x)
                                .InspectionPointer("Page Content", InspectorPointerType.PageStructure)
                                .Element(Content);

                            decoration
                                .After()
                                .InspectionPointer("Page Footer", InspectorPointerType.PageStructure)
                                .Element(Footer);
                        });
                    
                    layers
                        .Layer()
                        .InspectionPointer("Page Foreground Layer", InspectorPointerType.PageStructure)
                        .Element(Foreground);
                });

            bool IsClose(float x, float y)
            {
                return Math.Abs(x - y) < Size.Epsilon;
            }
        }
    }
}