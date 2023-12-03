using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine
{
    internal sealed class ElementMock : Element
    {
        public string Id { get; set; }
        public Func<Size, SpacePlan> MeasureFunc { get; set; }
        public Action<Size> DrawFunc { get; set; }

        internal override SpacePlan Measure(Size availableSpace) => MeasureFunc(availableSpace);
        internal override void Draw(Size availableSpace) => DrawFunc(availableSpace);
    }
}