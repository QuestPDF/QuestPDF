using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine
{
    internal sealed class ElementMock : Element
    {
        public string Id { get; set; }
        public Func<LayoutSpace, SpacePlan> MeasureFunc { get; set; }
        public Action<LayoutSpace> DrawFunc { get; set; }

        internal override SpacePlan Measure(LayoutSpace availableSpace) => MeasureFunc(availableSpace);
        internal override void Draw(LayoutSpace availableSpace) => DrawFunc(availableSpace);
    }
}