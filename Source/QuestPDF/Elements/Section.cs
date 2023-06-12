using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Section : ContainerElement, IStateResettable
    {
        public int DocumentId { get; set; }
        public string SectionName { get; set; }
        private bool IsRendered { get; set; }
        
        public void ResetState()
        {
            IsRendered = false;
        }
        
        internal override void Draw(Size availableSpace)
        {
            var targetName = GetTargetName(DocumentId, SectionName);
            
            if (!IsRendered)
            {
                Canvas.DrawSection(targetName);
                IsRendered = true;
            }
            
            PageContext.SetSectionPage(targetName);
            base.Draw(availableSpace);
        }
        
        internal static string GetTargetName(int documentId, string locationName)
        {
            return $"{documentId} | {locationName}";
        }
    }
}