using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class RotateTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<Rotate>();

        [Test]
        public void Draw()
        {
            TestPlan
                .For(x => new Rotate
                {
                    Child = x.CreateChild(),
                    Angle = 123
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasRotate(123)
                .ExpectChildDraw(new Size(400, 300))
                .ExpectCanvasRotate(-123)
                .CheckDrawResult();
        } 
    }
}