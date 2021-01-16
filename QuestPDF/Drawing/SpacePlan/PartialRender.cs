using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.SpacePlan
{
    internal class PartialRender : Size, ISpacePlan
    {
        public PartialRender(Size size) : this(size.Width, size.Height)
        {
            
        }
        
        public PartialRender(float width, float height) : base(width, height)
        {
            
        }
        
        public override string ToString() => $"PartialRender {base.ToString()}";
    }
}