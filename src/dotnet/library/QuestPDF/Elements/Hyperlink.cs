using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Hyperlink : ContainerElement, ISemanticAware
    {
        public SemanticTreeManager? SemanticTreeManager { get; set; }

        public string Url { get; set; } = "https://www.questpdf.com";
        public string? Description { get; set; }

        internal override void Draw(Size availableSpace)
        {
            var measurement = base.Measure(availableSpace);

            if (measurement.Type == SpacePlanType.Wrap)
                return;

            // the link covers the entire space offered to the element, as only that space is known to contain the content,
            // the content may be placed anywhere within it, e.g. with alignment or the right-to-left content direction
            if (ShouldDrawLink(measurement))
                Canvas.DrawHyperlink(availableSpace, Url, Description);

            // the content is drawn exactly as it would be without the link
            base.Draw(availableSpace);
        }

        private bool ShouldDrawLink(SpacePlan measurement)
        {
            // in a tagged document, links are part of the semantic tree, so content excluded from it (e.g. a repeated header) cannot have them
            if (SemanticTreeManager?.IsCurrentContentArtifact() ?? false)
                return false;

            // hidden or already drawn content shows nothing on this page
            if (measurement.Type == SpacePlanType.Empty)
                return false;

            // postponed content (e.g. moved to the next page by EnsureSpace or PreventPageBreak) shows nothing on this page,
            // even when the parent offers space (e.g. a row as tall as its other items)
            if (measurement.Type == SpacePlanType.PartialRender && measurement.Height <= Size.Epsilon)
                return false;

            return true;
        }

        internal override string? GetCompanionHint() => Url;

        internal override IEnumerable<KeyValuePair<string, string>>? GetCompanionProperties()
        {
            yield return new("Url", Url);
        }
    }
}
