using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements
{
    internal sealed class Page : IComponent
    {
        public ContentDirection ContentDirection { get; set; }
        public TextStyle DefaultTextStyle { get; set; } = TextStyle.Default;
        
        public Size? MinSize { get; set; }
        public Size? MaxSize { get; set; }

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
            SetDefaultPageSizeIfNotSpecified();
            
            container
                .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Page.ToString())
                .ContentDirection(ContentDirection)
                .DefaultTextStyle(DefaultTextStyle.DisableFontFeature(FontFeatures.StandardLigatures))
                .Layers(layers =>
                {
                    layers.Layer()
                        .ZIndex(int.MinValue)
                        .Background(BackgroundColor);
                    
                    layers
                        .Layer()
                        .Repeat()
                        .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Background.ToString())
                        .Artifact(SkSemanticNodeSpecialId.BackgroundArtifact)
                        .Element(Background);
                    
                    layers
                        .PrimaryLayer()
                        
                        .MinWidth(MinSize?.Width ?? 0)
                        .MinHeight(MinSize?.Height ?? 0)
                        .MaxWidth(MaxSize?.Width ?? Size.Max.Width)
                        .MaxHeight(MaxSize?.Height ?? Size.Max.Height)
                        .EnforceSizeWhenEmpty()
                        
                        .PaddingLeft(MarginLeft)
                        .PaddingRight(MarginRight)
                        .PaddingTop(MarginTop)
                        .PaddingBottom(MarginBottom)
                
                        .Decoration(decoration =>
                        {
                            decoration
                                .Before()
                                .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Header.ToString())
                                .Element(Header);

                            decoration
                                .Content()
                                .NonTrackingElement(x => (MinSize?.Width == MaxSize?.Width) ? x.ExtendHorizontal() : x)
                                .NonTrackingElement(x => (MinSize?.Height == MaxSize?.Height) ? x.ExtendVertical() : x)
                                .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Content.ToString())
                                .Element(Content);

                            decoration
                                .After()
                                .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Footer.ToString())
                                .Element(Footer);
                        });
                    
                    layers
                        .Layer()
                        .Repeat()
                        .Artifact(SkSemanticNodeSpecialId.PaginationWatermarkArtifact)
                        .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Foreground.ToString())
                        .Element(Foreground);
                });
        }

        private void SetDefaultPageSizeIfNotSpecified()
        {
            if (MinSize.HasValue || MaxSize.HasValue)
                return;

            MinSize = PageSizes.A4;
            MaxSize = PageSizes.A4;
        }
    }
}