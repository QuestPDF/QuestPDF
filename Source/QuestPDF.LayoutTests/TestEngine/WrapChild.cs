using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal class WrapChild : Element
{
    internal override SpacePlan Measure(Size availableSpace)
    {
        return SpacePlan.Wrap();
    }

    internal override void Draw(Size availableSpace)
    {
        
    }
}