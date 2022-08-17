using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Empty : Element
    {
        internal static Empty Instance { get; } = new Empty();
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return availableSpace.IsNegative() 
                ? SpacePlan.Wrap() 
                : SpacePlan.FullRender(0, 0);
        }

        internal override void Draw(Size availableSpace)
        {
            
        }
    }
}