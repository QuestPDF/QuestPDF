using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class AspectRatioTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new AspectRatio().MeasureWithoutChild();
        
        [Test]
        public void Draw_ShouldHandleNullChild() => new AspectRatio().DrawWithoutChild();
        
        [Test]
        public void Measure_FitWidth_EnoughSpace_FullRender()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitArea,
                    Ratio = 2f
                })
                .MeasureElement(new Size(400, 201))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(100, 50))
                .CheckMeasureResult(SpacePlan.FullRender(400, 200));
        }
        
        [Test]
        public void Measure_FitWidth_EnoughSpace_PartialRender()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitArea,
                    Ratio = 2f
                })
                .MeasureElement(new Size(400, 201))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.PartialRender(100, 50))
                .CheckMeasureResult(SpacePlan.PartialRender(400, 200));
        }
        
        [Test]
        public void Measure_FitWidth_EnoughSpace_Wrap()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitArea,
                    Ratio = 2f
                })
                .MeasureElement(new Size(400, 201))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.Wrap())
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_FitWidth_EnoughSpace()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitWidth,
                    Ratio = 2f
                })
                .MeasureElement(new Size(400, 201))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(100, 50))
                .CheckMeasureResult(SpacePlan.FullRender(400, 200));
        }

        [Test]
        public void Measure_FitWidth_NotEnoughSpace()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitWidth,
                    Ratio = 2f
                })
                .MeasureElement(new Size(400, 199))
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_FitHeight_EnoughSpace()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitHeight,
                    Ratio = 2f
                })
                .MeasureElement(new Size(401, 200))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(100, 50))
                .CheckMeasureResult(SpacePlan.FullRender(400, 200));
        }
        
        [Test]
        public void Measure_FitHeight_NotEnoughSpace()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitHeight,
                    Ratio = 2f
                })
                .MeasureElement(new Size(399, 200))
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_FitArea_ToWidth()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitArea,
                    Ratio = 2f
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(100, 50))
                .CheckMeasureResult(SpacePlan.FullRender(400, 200));
        }
        
        [Test]
        public void Measure_FitArea_ToHeight()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitArea,
                    Ratio = 2f
                })
                .MeasureElement(new Size(500, 200))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(100, 50))
                .CheckMeasureResult(SpacePlan.FullRender(400, 200));
        }
        
        [Test]
        public void DrawChild_PerWidth()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitArea,
                    Ratio = 2f
                })
                .DrawElement(new Size(500, 200))
                .ExpectChildDraw(new Size(400, 200))
                .CheckDrawResult();
        }
        
        [Test]
        public void DrawChild_PerHeight()
        {
            TestPlan
                .For(x => new AspectRatio
                {
                    Child = x.CreateChild(),
                    Option = AspectRatioOption.FitArea,
                    Ratio = 2f
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildDraw(new Size(400, 200))
                .CheckDrawResult();
        } 
    }
}