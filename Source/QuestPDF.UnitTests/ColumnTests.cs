using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    // [TestFixture]
    // public class ColumnTests
    // {
    //     private Column CreateColumnWithTwoItems(TestPlan testPlan)
    //     {
    //         return new Column
    //         {
    //             Items =
    //             {
    //                 new ColumnItem
    //                 {
    //                     Child = testPlan.CreateChild("first")
    //                 },
    //                 new ColumnItem
    //                 {
    //                     Child = testPlan.CreateChild("second")
    //                 }
    //             }
    //         };
    //     }
    //     
    //     private Column CreateColumnWithTwoItemsWhereFirstIsFullyRendered(TestPlan testPlan)
    //     {
    //         var column = CreateColumnWithTwoItems(testPlan);
    //         column.Items.First().IsRendered = true;
    //         return column;
    //     }
    //     
    //     #region Measure
    //
    //     [Test]
    //     public void Measure_ReturnsWrap_WhenFirstChildWraps()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItems)
    //             .MeasureElement(new Size(400, 300))
    //             .ExpectChildMeasure("first", new Size(400, 300), SpacePlan.Wrap())
    //             .CheckMeasureResult(SpacePlan.Wrap());
    //     }
    //     
    //     [Test]
    //     public void Measure_ReturnsPartialRender_WhenFirstChildReturnsPartialRender()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItems)
    //             .MeasureElement(new Size(400, 300))
    //             .ExpectChildMeasure("first", new Size(400, 300), SpacePlan.PartialRender(300, 200))
    //             .CheckMeasureResult(SpacePlan.PartialRender(300, 200));
    //     }
    //     
    //     [Test]
    //     public void Measure_ReturnsPartialRender_WhenSecondChildWraps()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItems)
    //             .MeasureElement(new Size(400, 300))
    //             .ExpectChildMeasure("first", new Size(400, 300), SpacePlan.FullRender(200, 100))
    //             .ExpectChildMeasure("second", new Size(400, 200), SpacePlan.Wrap())
    //             .CheckMeasureResult(SpacePlan.PartialRender(200, 100));
    //     }
    //     
    //     [Test]
    //     public void Measure_ReturnsPartialRender_WhenSecondChildReturnsPartialRender()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItems)
    //             .MeasureElement(new Size(400, 300))
    //             .ExpectChildMeasure("first", new Size(400, 300), SpacePlan.FullRender(200, 100))
    //             .ExpectChildMeasure("second", new Size(400, 200), SpacePlan.PartialRender(300, 150))
    //             .CheckMeasureResult(SpacePlan.PartialRender(300, 250));
    //     }
    //     
    //     [Test]
    //     public void Measure_ReturnsFullRender_WhenSecondChildReturnsFullRender()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItems)
    //             .MeasureElement(new Size(400, 300))
    //             .ExpectChildMeasure("first", new Size(400, 300), SpacePlan.FullRender(200, 100))
    //             .ExpectChildMeasure("second", new Size(400, 200), SpacePlan.FullRender(100, 50))
    //             .CheckMeasureResult(SpacePlan.FullRender(200, 150));
    //     }
    //
    //     #endregion
    //     
    //     #region Draw
    //     
    //     [Test]
    //     public void Draw_WhenFirstChildWraps()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItems)
    //             .DrawElement(new Size(400, 300))
    //             .ExpectChildMeasure("first", new Size(400, 300), SpacePlan.Wrap())
    //             .CheckDrawResult();
    //     }
    //     
    //     [Test]
    //     public void Draw_WhenFirstChildPartiallyRenders()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItems)
    //             .DrawElement(new Size(400, 300))
    //             .ExpectChildMeasure("first", new Size(400, 300), SpacePlan.PartialRender(200, 100))
    //             .ExpectCanvasTranslate(0, 0)
    //             .ExpectChildDraw("first", new Size(400, 100))
    //             .ExpectCanvasTranslate(0, 0)
    //             .CheckDrawResult();
    //     }
    //     
    //     [Test]
    //     public void Draw_WhenFirstChildFullyRenders_AndSecondChildWraps()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItems)
    //             .DrawElement(new Size(400, 300))
    //             .ExpectChildMeasure("first", new Size(400, 300), SpacePlan.FullRender(200, 100))
    //             .ExpectChildMeasure("second", new Size(400, 200), SpacePlan.Wrap())
    //             .ExpectCanvasTranslate(0, 0)
    //             .ExpectChildDraw("first", new Size(400, 100))
    //             .ExpectCanvasTranslate(0, 0)
    //             .CheckDrawResult();
    //     }
    //     
    //     [Test]
    //     public void Draw_WhenFirstChildFullyRenders_AndSecondChildPartiallyRenders()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItems)
    //             .DrawElement(new Size(400, 300))
    //             .ExpectChildMeasure("first", new Size(400, 300), SpacePlan.FullRender(200, 100))
    //             .ExpectChildMeasure("second", new Size(400, 200), SpacePlan.PartialRender(250, 150))
    //             .ExpectCanvasTranslate(0, 0)
    //             .ExpectChildDraw("first", new Size(400, 100))
    //             .ExpectCanvasTranslate(0, 0)
    //             .ExpectCanvasTranslate(0, 100)
    //             .ExpectChildDraw("second", new Size(400, 150))
    //             .ExpectCanvasTranslate(0, -100)
    //             .CheckDrawResult();
    //     }
    //     
    //     [Test]
    //     public void Draw_WhenFirstChildFullyRenders_AndSecondChildFullyRenders()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItems)
    //             .DrawElement(new Size(400, 300))
    //             .ExpectChildMeasure("first", new Size(400, 300), SpacePlan.FullRender(200, 100))
    //             .ExpectChildMeasure("second", new Size(400, 200), SpacePlan.FullRender(250, 150))
    //             .ExpectCanvasTranslate(0, 0)
    //             .ExpectChildDraw("first", new Size(400, 100))
    //             .ExpectCanvasTranslate(0, 0)
    //             .ExpectCanvasTranslate(0, 100)
    //             .ExpectChildDraw("second", new Size(400, 150))
    //             .ExpectCanvasTranslate(0, -100)
    //             .CheckDrawResult();
    //     }
    //     
    //     [Test]
    //     public void Draw_UsesEmpty_WhenFirstChildIsRendered()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItemsWhereFirstIsFullyRendered)
    //             .DrawElement(new Size(400, 300))
    //             .ExpectChildMeasure("second", new Size(400, 300), SpacePlan.PartialRender(200, 300))
    //             .ExpectCanvasTranslate(0, 0)
    //             .ExpectChildDraw("second", new Size(400, 300))
    //             .ExpectCanvasTranslate(0, 0)
    //             .CheckState<Column>(x => x.Items.First().IsRendered)
    //             .CheckDrawResult();
    //     }
    //     
    //     [Test]
    //     public void Draw_TogglesFirstRenderedFlag_WhenSecondFullyRenders()
    //     {
    //         TestPlan
    //             .For(CreateColumnWithTwoItemsWhereFirstIsFullyRendered)
    //             .DrawElement(new Size(400, 300))
    //             .ExpectChildMeasure("second", new Size(400, 300), SpacePlan.FullRender(200, 300))
    //             .ExpectCanvasTranslate(0, 0)
    //             .ExpectChildDraw("second", new Size(400, 300))
    //             .ExpectCanvasTranslate(0, 0)
    //             .CheckDrawResult()
    //             .CheckState<Column>(x => !x.Items.First().IsRendered);
    //     }
    //     
    //     #endregion
    // }
}