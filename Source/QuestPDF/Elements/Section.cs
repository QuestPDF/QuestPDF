using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Section : ContainerElement, IStateful, IPageContextAware
    {
        public IPageContext PageContext { get; set; }
        
        public string SectionName { get; set; }
        private bool IsRendered { get; set; }

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
    
        object IStateful.CloneState()
        {
            return IsRendered;
        }

        void IStateful.SetState(object state)
        {
            IsRendered = (bool) state;
        }

        void IStateful.ResetState(bool hardReset)
        {
            IsRendered = false;
        }
    
        #endregion
    }
}