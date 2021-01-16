using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.SpacePlan
{
    internal class FullRender : Size, ISpacePlan
    {
        public FullRender(Size size) : this(size.Width, size.Height)
        {
            
        }
        
        public FullRender(float width, float height) : base(width, height)
        {
            
        }
        
        public override string ToString() => $"FullRender {base.ToString()}";
    }
}