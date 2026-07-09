using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class PageBreakTests
    {
        [Test]
        public void Measure()
        {
            TestPlan
                .For(x => new PageBreak())
                
                .MeasureElement(new Size(400, 300))
                .CheckMeasureResult(SpacePlan.PartialRender(Size.Zero))
                
                .DrawElement(new Size(400, 300))
                .CheckDrawResult()
                
                .MeasureElement(new Size(500, 400))
                .CheckMeasureResult(SpacePlan.Empty());
        }
    }
}