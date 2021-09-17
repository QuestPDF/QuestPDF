using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class TranslateTests
    {
        [Test]
        public void Draw()
        {
            TestPlan
                .For(x => new Translate
                {
                    Child = x.CreateChild(),
                    TranslateX = 50,
                    TranslateY = 75
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasTranslate(50, 75)
                .ExpectChildDraw(new Size(400, 300))
                .ExpectCanvasTranslate(-50, -75)
                .CheckDrawResult();
        }
    }
}