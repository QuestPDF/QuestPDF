using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
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
                .CheckMeasureResult(new PartialRender(400, 300))
                
                .DrawElement(new Size(400, 300))
                .CheckDrawResult()
                
                .MeasureElement(new Size(500, 400))
                .CheckMeasureResult(new FullRender(Size.Zero));
        }
    }
}