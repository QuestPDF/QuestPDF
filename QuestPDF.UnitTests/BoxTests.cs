using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class BoxTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<Box>();
        
        [Test]
        public void Draw_Wrap()
        {
            TestPlan
                .For(x => new Box
                {
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.Wrap())
                .CheckDrawResult();
        }
        
        [Test]
        public void Measure_PartialRender()
        {
            TestPlan
                .For(x => new Box
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.PartialRender(200, 100))
                .ExpectChildDraw(new Size(200, 100))
                .CheckDrawResult();
        }
        
        [Test]
        public void Measure_FullRender()
        {
            TestPlan
                .For(x => new Box
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(500, 400))
                .ExpectChildMeasure(expectedInput: new Size(500, 400), returns: SpacePlan.FullRender(300, 200))
                .ExpectChildDraw(new Size(300, 200))
                .CheckDrawResult();
        }
    }
}