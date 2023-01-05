using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ShowOnceTest
    {
        [Test]
        public void Draw()
        {
            TestPlan
                .For(x => new ShowOnce()
                {
                    Child = x.CreateChild()
                })
                
                // Measure the element and return result
                .MeasureElement(new Size(300, 200))
                .ExpectChildMeasure("child", new Size(300, 200), SpacePlan.PartialRender(new Size(200, 200)))
                .CheckMeasureResult(SpacePlan.PartialRender(new Size(200, 200)))
                
                // Draw element partially
                .DrawElement(new Size(200, 200))
                .ExpectChildMeasure(new Size(200, 200), SpacePlan.PartialRender(new Size(200, 200)))
                .ExpectChildDraw(new Size(200, 200))
                .CheckDrawResult()
                
                // Element was not fully drawn
                // It should be measured again for rendering on next page
                .MeasureElement(new Size(800, 200))
                .ExpectChildMeasure(new Size(800, 200), SpacePlan.FullRender(new Size(400, 200)))
                .CheckMeasureResult(SpacePlan.FullRender(new Size(400, 200)))

                // Draw element on next page
                // Element was fully drawn at this point
                .DrawElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(new Size(400, 200)))
                .ExpectChildDraw(new Size(400, 200))
                .CheckDrawResult()
                
                // In the next attempt of measuring element, it should behave like empty parent.
                .MeasureElement(new Size(600, 200))
                .CheckMeasureResult(SpacePlan.FullRender(0, 0))
                
                // In the next attempt of measuring element, it should not draw its child
                .DrawElement(new Size(600, 200))
                .CheckDrawResult();
        }
    }
}