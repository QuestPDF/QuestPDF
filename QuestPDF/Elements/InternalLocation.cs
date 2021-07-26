using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class InternalLocation : ContainerElement, IStateResettable
    {
        public string LocationName { get; set; }
        private bool IsRendered { get; set; }
        
        public void ResetState()
        {
            IsRendered = false;
        }
        
        internal override void Draw(Size availableSpace)
        {
            if (!IsRendered)
            {
                PageContext.SetLocationPage(LocationName);
                
                Canvas.DrawLocation(LocationName);
                IsRendered = true;
            }
            
            Child?.Draw(availableSpace);
        }
    }
}