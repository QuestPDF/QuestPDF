using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Hyperlink : ContainerElement, IContentDirectionAware, ISemanticAware
    {
        public SemanticTreeManager? SemanticTreeManager { get; set; }
        
        public ContentDirection ContentDirection { get; set; }
        public string Url { get; set; } = "https://www.questpdf.com";
        public string? Description { get; set; }
        
        internal override void Draw(Size availableSpace)
        {
            if (SemanticTreeManager?.IsCurrentContentArtifact() ?? false)
            {
                base.Draw(availableSpace);
                return;
            }
            
            var targetSize = base.Measure(availableSpace);

            if (targetSize.Type is SpacePlanType.Empty or SpacePlanType.Wrap)
                return;
            
            var horizontalOffset = ContentDirection == ContentDirection.LeftToRight
                ? Position.Zero
                : new Position(availableSpace.Width - targetSize.Width, 0);

            Canvas.Translate(horizontalOffset);
            Canvas.DrawHyperlink(availableSpace, Url, Description);
            Canvas.Translate(horizontalOffset.Reverse());
            
            base.Draw(availableSpace);
        }

        internal override string? GetCompanionHint() => Url;
        
        internal override IEnumerable<KeyValuePair<string, string>>? GetCompanionProperties()
        {
            yield return new("Url", Url);
        }
    }
}