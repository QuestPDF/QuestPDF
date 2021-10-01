using System.Collections.Generic;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class LayersTests
    {
        private const string BackgroundLayer = "background";
        private const string MainLayer = "main";
        private const string ForegroundLayer = "foreground";
        
        private static Layers GetLayers(TestPlan x)
        {
            return new Layers
            {
                Children = new List<Layer>
                {
                    new Layer
                    {
                        Child = x.CreateChild(BackgroundLayer)
                    },
                    new Layer
                    {
                        Child = x.CreateChild(MainLayer),
                        IsPrimary = true
                    },
                    new Layer
                    {
                        Child = x.CreateChild(ForegroundLayer)
                    }
                }
            };
        }
        
        #region measure
        
        [Test]
        public void Measure_Wrap()
        {
            TestPlan
                .For(GetLayers)
                .MeasureElement(new Size(800, 600))
                .ExpectChildMeasure(MainLayer, new Size(800, 600), new Wrap())
                .CheckMeasureResult(new Wrap());
        }

        [Test]
        public void Measure_PartialRender()
        {
            TestPlan
                .For(GetLayers)
                .MeasureElement(new Size(800, 600))
                .ExpectChildMeasure(MainLayer, new Size(800, 600), new PartialRender(700, 500))
                .CheckMeasureResult(new PartialRender(700, 500));
        }
        
        [Test]
        public void Measure_FullRender()
        {
            TestPlan
                .For(GetLayers)
                .MeasureElement(new Size(800, 600))
                .ExpectChildMeasure(MainLayer, new Size(800, 600), new FullRender(500, 400))
                .CheckMeasureResult(new FullRender(500, 400));
        }
        
        #endregion
        
        #region draw
        
        [Test]
        public void Draw_Simple()
        {
            TestPlan
                .For(GetLayers)
                .MeasureElement(new Size(800, 600))
                
                .ExpectChildMeasure(BackgroundLayer, new Size(800, 600), new FullRender(100, 200))
                .ExpectChildMeasure(MainLayer, new Size(800, 600), new PartialRender(200, 300))
                .ExpectChildMeasure(ForegroundLayer, new Size(800, 600), new FullRender(300, 400))
                
                
                .ExpectChildDraw(BackgroundLayer, new Size(800, 600))
                .ExpectChildDraw(MainLayer, new Size(800, 600))
                .ExpectChildDraw(ForegroundLayer, new Size(800, 600))
                
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_WhenSecondaryLayerReturnsWrap_SkipThatLayer_1()
        {
            TestPlan
                .For(GetLayers)
                .MeasureElement(new Size(800, 600))
                
                .ExpectChildMeasure(BackgroundLayer, new Size(800, 600), new PartialRender(100, 200))
                .ExpectChildMeasure(MainLayer, new Size(800, 600), new PartialRender(200, 300))
                .ExpectChildMeasure(ForegroundLayer, new Size(800, 600), new Wrap())
                
                .ExpectChildDraw(BackgroundLayer, new Size(800, 600))
                .ExpectChildDraw(MainLayer, new Size(800, 600))
                
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_WhenSecondaryLayerReturnsWrap_SkipThatLayer_2()
        {
            TestPlan
                .For(GetLayers)
                .MeasureElement(new Size(800, 600))
                
                .ExpectChildMeasure(BackgroundLayer, new Size(800, 600), new Wrap())
                .ExpectChildMeasure(MainLayer, new Size(800, 600), new PartialRender(200, 300))
                .ExpectChildMeasure(ForegroundLayer, new Size(800, 600), new PartialRender(300, 400))
                
                .ExpectChildDraw(MainLayer, new Size(800, 600))
                .ExpectChildDraw(ForegroundLayer, new Size(800, 600))
                
                .CheckDrawResult();
        }
        
        #endregion
    }
}