using System.Collections.Generic;
using NUnit.Framework;
using QuestPDF.Drawing;
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
                .ExpectChildMeasure(MainLayer, new Size(800, 600), SpacePlan.Wrap("Mock"))
                .CheckMeasureResult(SpacePlan.Wrap("The content of the primary layer does not fit (even partially) the available space."));
        }

        [Test]
        public void Measure_PartialRender()
        {
            TestPlan
                .For(GetLayers)
                .MeasureElement(new Size(800, 600))
                .ExpectChildMeasure(MainLayer, new Size(800, 600), SpacePlan.PartialRender(700, 500))
                .CheckMeasureResult(SpacePlan.PartialRender(700, 500));
        }
        
        [Test]
        public void Measure_FullRender()
        {
            TestPlan
                .For(GetLayers)
                .MeasureElement(new Size(800, 600))
                .ExpectChildMeasure(MainLayer, new Size(800, 600), SpacePlan.FullRender(500, 400))
                .CheckMeasureResult(SpacePlan.FullRender(500, 400));
        }
        
        #endregion
        
        #region draw
        
        [Test]
        public void Draw_Simple()
        {
            TestPlan
                .For(GetLayers)
                .MeasureElement(new Size(800, 600))
                
                .ExpectChildMeasure(BackgroundLayer, new Size(800, 600), SpacePlan.FullRender(100, 200))
                .ExpectChildMeasure(MainLayer, new Size(800, 600), SpacePlan.PartialRender(200, 300))
                .ExpectChildMeasure(ForegroundLayer, new Size(800, 600), SpacePlan.FullRender(300, 400))
                
                
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
                
                .ExpectChildMeasure(BackgroundLayer, new Size(800, 600), SpacePlan.PartialRender(100, 200))
                .ExpectChildMeasure(MainLayer, new Size(800, 600), SpacePlan.PartialRender(200, 300))
                .ExpectChildMeasure(ForegroundLayer, new Size(800, 600), SpacePlan.Wrap("Mock"))
                
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
                
                .ExpectChildMeasure(BackgroundLayer, new Size(800, 600), SpacePlan.Wrap("Mock"))
                .ExpectChildMeasure(MainLayer, new Size(800, 600), SpacePlan.PartialRender(200, 300))
                .ExpectChildMeasure(ForegroundLayer, new Size(800, 600), SpacePlan.PartialRender(300, 400))
                
                .ExpectChildDraw(MainLayer, new Size(800, 600))
                .ExpectChildDraw(ForegroundLayer, new Size(800, 600))
                
                .CheckDrawResult();
        }
        
        #endregion
    }
}