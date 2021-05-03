using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class WrapWhenLittleSpaceTests
    {
        [Test]
        public void Measure_ReturnsWrap_WhenNotEnoughSpace()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(new Size(400, 100))
                .CheckMeasureResult(new Wrap());
        }
        
        [Test]
        public void Measure_Continues_WhenEnoughSpace()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), new FullRender(300, 250))
                .CheckMeasureResult(new FullRender(300, 250));
        }
    }
}