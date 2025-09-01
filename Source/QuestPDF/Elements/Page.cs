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

        public Color BackgroundColor { get; set; } = Colors.Transparent;
        
        public Element Background { get; set; } = Empty.Instance;
        public Element Foreground { get; set; } = Empty.Instance;
        
        public Element Header { get; set; } = Empty.Instance;
        public Element Content { get; set; } = Empty.Instance;
        public Element Footer { get; set; } = Empty.Instance;

        public void Compose(IContainer container)
        {
            container
                .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Page.ToString())
                .ContentDirection(ContentDirection)
                .DefaultTextStyle(DefaultTextStyle.DisableFontFeature(FontFeatures.StandardLigatures))
                .Layers(layers =>
                {
                    layers.Layer()
                        .ZIndex(int.MinValue)
                        .ArtifactBackground()
                        .Background(BackgroundColor);
                    
                    layers
                        .Layer()
                        .ArtifactBackground()
                        .Repeat()
                        .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Background.ToString())
                        .Element(Background);
                    
                    layers
                        .PrimaryLayer()
                        
                        .MinWidth(MinSize.Width)
                        .MinHeight(MinSize.Height)
                        .MaxWidth(MaxSize.Width)
                        .MaxHeight(MaxSize.Height)
                        .EnforceSizeWhenEmpty()
                        
                        .PaddingLeft(MarginLeft)
                        .PaddingRight(MarginRight)
                        .PaddingTop(MarginTop)
                        .PaddingBottom(MarginBottom)
                
                        .Decoration(decoration =>
                        {
                            decoration
                                .Before()
                                .ArtifactPaginationHeader()
                                .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Header.ToString())
                                .Element(Header);

                            decoration
                                .Content()
                                .NonTrackingElement(x => IsClose(MinSize.Width, MaxSize.Width) ? x.ExtendHorizontal() : x)
                                .NonTrackingElement(x => IsClose(MinSize.Height, MaxSize.Height) ? x.ExtendVertical() : x)
                                .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Content.ToString())
                                .Element(Content);

                            decoration
                                .After()
                                .ArtifactPaginationFooter()
                                .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Footer.ToString())
                                .Element(Footer);
                        });
                    
                    layers
                        .Layer()
                        .ArtifactPaginationWatermark()
                        .Repeat()
                        .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Foreground.ToString())
                        .Element(Foreground);
                });

            bool IsClose(float x, float y)
            {
                return Math.Abs(x - y) < Size.Epsilon;
            }
        }
    }
}