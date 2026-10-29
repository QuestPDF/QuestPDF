using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Section : ContainerElement, IStateful, IPageContextAware
    {
        public IPageContext PageContext { get; set; }
        
        public string SectionName { get; set; }

        internal override void Draw(Size availableSpace)
        {
            var measurement = base.Measure(availableSpace);
            
            // postponed content (e.g. moved to the next page by EnsureSpace or PreventPageBreak) shows nothing on this page,
            // so the section starts on the page where its content appears
            if (measurement.Type == SpacePlanType.PartialRender && measurement.Height <= Size.Epsilon)
            {
                base.Draw(availableSpace);
                return;
            }

            // content drawn completely on earlier pages shows nothing on this page, so the section does not extend here,
            // e.g. when a row keeps drawing its finished items until the tallest one ends
            if (measurement.Type == SpacePlanType.Empty && IsRendered)
            {
                base.Draw(availableSpace);
                return;
            }

            if (!IsRendered)
            {
                var targetName = PageContext.GetDocumentLocationName(SectionName);
                Canvas.DrawSection(targetName);
                IsRendered = true;
            }
            
            PageContext.SetSectionPage(SectionName);
            base.Draw(availableSpace);
        }
        
        #region IStateful
        
        private bool IsRendered { get; set; }

        public void ResetState(bool hardReset = false)
        {
            if (hardReset)
                IsRendered = false;
        }

        public object GetState() => IsRendered;
        public void SetState(object state) => IsRendered = (bool) state;
    
        #endregion
        
        internal override string? GetCompanionHint() => SectionName;
        internal override string? GetCompanionSearchableContent() => SectionName;
        
        internal override IEnumerable<KeyValuePair<string, string>>? GetCompanionProperties()
        {
            yield return new("SectionName", SectionName);
        }
    }
}