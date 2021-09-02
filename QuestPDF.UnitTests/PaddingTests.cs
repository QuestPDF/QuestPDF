using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class PaddingTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new Padding().MeasureWithoutChild();
        
        [Test]
        public void Draw_ShouldHandleNullChild() => new Padding().DrawWithoutChild();
        
        private Padding GetPadding(TestPlan plan)
        {
            return new Padding()
            {
                Top = 10,
                Right = 20,
                Bottom = 30,
                Left = 40,
                
                Child = plan.CreateChild()
            };
        }

        [Test]
        public void Measure_General_EnoughSpace()
        {
            TestPlan
                .For(GetPadding)
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(340, 260), SpacePlan.FullRender(140, 60))
                .CheckMeasureResult(SpacePlan.FullRender(200, 100));
        } 
        
        [Test]
        public void Measure_NotEnoughWidth()
        {
            TestPlan
                .For(GetPadding)
                .MeasureElement(new Size(50, 300))
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_NotEnoughHeight()
        {
            TestPlan
                .For(GetPadding)
                .MeasureElement(new Size(20, 300))
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_AcceptsPartialRender()
        {
            TestPlan
                .For(GetPadding)
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(340, 260), SpacePlan.PartialRender(40, 160))
                .CheckMeasureResult(SpacePlan.PartialRender(100, 200));
        }
        
        [Test]
        public void Measure_AcceptsWrap()
        {
            TestPlan
                .For(GetPadding)
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(340, 260), SpacePlan.Wrap())
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Draw_General()
        {
            TestPlan
                .For(GetPadding)
                .DrawElement(new Size(400, 300))
                .ExpectCanvasTranslate(new Position(40, 10))
                .ExpectChildDraw(new Size(340, 260))
                .ExpectCanvasTranslate(new Position(-40, -10))
                .CheckDrawResult();
        }
    }
}