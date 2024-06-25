using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal class WrapChild : Element
{
    internal override SpacePlan Measure(Size availableSpace)
    {
        return SpacePlan.Wrap("This element is used only for testing purposes and does not fit on any space by design.");
    }

    internal override void Draw(Size availableSpace)
    {
        
    }
}