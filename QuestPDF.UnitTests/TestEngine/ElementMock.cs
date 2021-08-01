using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine
{
    internal class ElementMock : Element
    {
        public string Id { get; set; }
        public Func<Size, ISpacePlan> MeasureFunc { get; set; }
        public Action<Size> DrawFunc { get; set; }

        internal override ISpacePlan Measure(Size availableSpace) => MeasureFunc(availableSpace);
        internal override void Draw(Size availableSpace) => DrawFunc(availableSpace);
    }
}