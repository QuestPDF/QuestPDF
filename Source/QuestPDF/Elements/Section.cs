using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Section : ContainerElement, IStateful
    {
        public string SectionName { get; set; }

        internal override void Draw(Size availableSpace)
        {
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
    
        public void ResetState(bool hardReset = false) => IsRendered = false;
        public object GetState() => IsRendered;
        public void SetState(object state) => IsRendered = (bool) state;
    
        #endregion
    }
}