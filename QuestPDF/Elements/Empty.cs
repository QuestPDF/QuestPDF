using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Empty : Element
    {
        internal static Empty Instance { get; } = new Empty();
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            return new FullRender(Size.Zero);
        }

        internal override void Draw(Size availableSpace)
        {
            
        }
    }
}