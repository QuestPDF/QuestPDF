using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Empty : Element
    {
        internal static Empty Instance { get; } = new();
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return availableSpace.IsNegative() 
                ? SpacePlan.Wrap("The available space is negative.") 
                : SpacePlan.FullRender(0, 0);
        }

        internal override void Draw(Size availableSpace)
        {
            
        }
    }
}