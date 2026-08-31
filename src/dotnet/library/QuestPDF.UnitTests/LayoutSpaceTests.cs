using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Elements.Table;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    /// <summary>
    /// Pins the rules by which containers derive the space of their children from their own,
    /// and the adaptations that leaf elements apply along a final axis.
    /// </summary>
    [TestFixture]
    public class LayoutSpaceTests
    {
        private const LayoutAxisMode Final = LayoutAxisMode.Final;
        private const LayoutAxisMode Flowing = LayoutAxisMode.Flowing;
        private const LayoutAxisMode Query = LayoutAxisMode.Query;

        private static LayoutSpace Space(float width, float height, LayoutAxisMode widthMode, LayoutAxisMode heightMode)
        {
            return new LayoutSpace(new Size(width, height), widthMode, heightMode);
        }

        private static readonly SpacePlan EmptinessProbe = SpacePlan.PartialRender(Size.Zero);

        #region Adaptation along a final axis

        [Test]
        public void AspectRatio_FitHeightTooWide_WidthNotFinal_ReportsShortfall()
        {
            // the element is asked what it needs, so it describes the configured option even though it does not fit
            TestPlan
                .For(x => new AspectRatio { Child = x.CreateChild(), Option = AspectRatioOption.FitHeight, Ratio = 2f })
                .MeasureElement(Space(399, 200, Query, Final))
                .ExpectChildMeasure(Size.Zero, EmptinessProbe)
                .CheckMeasureResult(SpacePlan.Wrap("To preserve the target aspect ratio, the content requires more horizontal space than available."));
        }

        [Test]
        public void AspectRatio_FitHeightTooWide_FinalWidth_FallsBackToTheLargestFittingArea()
        {
            // no later page is wider, so the option can never be satisfied and the largest area of the same ratio is used;
            // that area is derived from the final width, so the content is final along both axes
            TestPlan
                .For(x => new AspectRatio { Child = x.CreateChild(), Option = AspectRatioOption.FitHeight, Ratio = 2f })
                .MeasureElement(Space(399, 200, Final, Query))
                .ExpectChildMeasure(Size.Zero, EmptinessProbe)
                .ExpectChildMeasure(Space(399, 199.5f, Final, Final), SpacePlan.FullRender(399, 199.5f))
                .CheckMeasureResult(SpacePlan.FullRender(399, 199.5f));
        }

        [Test]
        public void AspectRatio_FitHeightThatFits_KeepsTheConfiguredOption()
        {
            TestPlan
                .For(x => new AspectRatio { Child = x.CreateChild(), Option = AspectRatioOption.FitHeight, Ratio = 2f })
                .MeasureElement(Space(401, 200, Final, Query))
                .ExpectChildMeasure(Size.Zero, EmptinessProbe)
                .ExpectChildMeasure(Space(400, 200, Query, Query), SpacePlan.FullRender(400, 200))
                .CheckMeasureResult(SpacePlan.FullRender(400, 200));
        }

        [Test]
        public void AspectRatio_FitWidthTooTall_IsNotAdapted()
        {
            // a vertical shortfall may still be resolved by moving the content to the next page
            TestPlan
                .For(x => new AspectRatio { Child = x.CreateChild(), Option = AspectRatioOption.FitWidth, Ratio = 2f })
                .MeasureElement(LayoutSpace.Final(new Size(400, 199)))
                .ExpectChildMeasure(Size.Zero, EmptinessProbe)
                .CheckMeasureResult(SpacePlan.Wrap("To preserve the target aspect ratio, the content requires more vertical space than available."));
        }

        [Test]
        public void Constrained_MinWidthTooLarge_WidthNotFinal_ReportsShortfall()
        {
            TestPlan
                .For(x => new Constrained { MinWidth = 100, Child = x.CreateChild() })
                .MeasureElement(Space(50, 400, Flowing, Final))
                .ExpectChildMeasure(Size.Zero, EmptinessProbe)
                .CheckMeasureResult(SpacePlan.Wrap("The available horizontal space is less than the minimum width."));
        }

        [Test]
        public void Constrained_MinWidthTooLarge_FinalWidth_IsClampedToTheAvailableSpace()
        {
            TestPlan
                .For(x => new Constrained { MinWidth = 100, Child = x.CreateChild() })
                .MeasureElement(Space(50, 400, Final, Query))
                .ExpectChildMeasure(Size.Zero, EmptinessProbe)
                .ExpectChildMeasure(Space(50, 400, Final, Query), SpacePlan.FullRender(20, 30))
                .CheckMeasureResult(SpacePlan.FullRender(50, 30));
        }

        [Test]
        public void Constrained_MinHeightTooLarge_IsNotAdapted()
        {
            TestPlan
                .For(x => new Constrained { MinHeight = 100, Child = x.CreateChild() })
                .MeasureElement(LayoutSpace.Final(new Size(400, 50)))
                .ExpectChildMeasure(Size.Zero, EmptinessProbe)
                .CheckMeasureResult(SpacePlan.Wrap("The available vertical space is less than the minimum height."));
        }

        [Test]
        public void Line_VerticalTooThick_WidthNotFinal_ReportsShortfall()
        {
            TestPlan
                .For(x => new Line { Type = LineType.Vertical, Thickness = 10 })
                .MeasureElement(LayoutSpace.Candidate(new Size(5, 100)))
                .CheckMeasureResult(SpacePlan.Wrap("The line thickness is greater than the available horizontal space."));
        }

        [Test]
        public void Line_VerticalTooThick_FinalWidth_IsNarrowed()
        {
            TestPlan
                .For(x => new Line { Type = LineType.Vertical, Thickness = 10 })
                .MeasureElement(Space(5, 100, Final, Query))
                .CheckMeasureResult(SpacePlan.FullRender(5, 0));
        }

        #endregion

        #region Propagation

        [Test]
        public void Constrained_ConfiguredMaximumThatBinds_MakesTheAxisFinal()
        {
            // the maximum applies on every page equally, so the content can never receive more than it
            TestPlan
                .For(x => new Constrained { MaxWidth = 100, Child = x.CreateChild() })
                .MeasureElement(LayoutSpace.Query(new Size(200, 400)))
                .ExpectChildMeasure(Size.Zero, EmptinessProbe)
                .ExpectChildMeasure(Space(100, 400, Final, Query), SpacePlan.FullRender(75, 400))
                .CheckMeasureResult(SpacePlan.FullRender(75, 400));
        }

        [Test]
        public void Constrained_ConfiguredMaximumThatDoesNotBind_InheritsTheMode()
        {
            // the environment is what limits the content here, and it says that this is only a question
            TestPlan
                .For(x => new Constrained { MaxWidth = 100, Child = x.CreateChild() })
                .MeasureElement(LayoutSpace.Query(new Size(50, 400)))
                .ExpectChildMeasure(Size.Zero, EmptinessProbe)
                .ExpectChildMeasure(Space(50, 400, Query, Query), SpacePlan.FullRender(50, 400))
                .CheckMeasureResult(SpacePlan.FullRender(50, 400));
        }

        [Test]
        public void AspectRatio_ContentTakesTheModeOfTheAxisTheAreaIsDerivedFrom()
        {
            // FitHeight derives the whole area from the height; when the height is final, so is the area
            TestPlan
                .For(x => new AspectRatio { Child = x.CreateChild(), Option = AspectRatioOption.FitHeight, Ratio = 2f })
                .MeasureElement(Space(401, 200, Query, Final))
                .ExpectChildMeasure(Size.Zero, EmptinessProbe)
                .ExpectChildMeasure(Space(400, 200, Final, Final), SpacePlan.FullRender(400, 200))
                .CheckMeasureResult(SpacePlan.FullRender(400, 200));
        }

        [Test]
        public void Column_OnlyTheItemOfferedTheEntireHeight_KeepsTheFinalHeight()
        {
            // the second item could still be moved to the next page and receive the entire height there
            TestPlan
                .For(x => new Column { Items = { x.CreateChild("a"), x.CreateChild("b") } })
                .MeasureElement(LayoutSpace.Final(new Size(100, 100)))
                .ExpectChildMeasure("a", Space(100, 100, Final, Final), SpacePlan.FullRender(100, 40))
                .ExpectChildMeasure("b", Space(100, 60, Final, Flowing), SpacePlan.FullRender(100, 20))
                .CheckMeasureResult(SpacePlan.FullRender(100, 60));
        }

        [Test]
        public void Column_InsideAColumn_NeverRestoresAFlowingHeight()
        {
            // an inner column placed below other content can move as a whole, so nothing inside it is at its final height
            TestPlan
                .For(x => new Column
                {
                    Items =
                    {
                        x.CreateChild("a"),
                        new Column { Items = { x.CreateChild("b"), x.CreateChild("c") } }
                    }
                })
                .MeasureElement(LayoutSpace.Final(new Size(100, 100)))
                .ExpectChildMeasure("a", Space(100, 100, Final, Final), SpacePlan.FullRender(100, 40))
                .ExpectChildMeasure("b", Space(100, 60, Final, Flowing), SpacePlan.FullRender(100, 10))
                .ExpectChildMeasure("c", Space(100, 50, Final, Flowing), SpacePlan.FullRender(100, 10))
                .CheckMeasureResult(SpacePlan.FullRender(100, 60));
        }

        [Test]
        public void Row_ConstantItemIsAlwaysFinal_RelativeItemInheritsTheMode()
        {
            TestPlan
                .For(x => new Row
                {
                    Items =
                    {
                        new RowItem { Type = RowItemType.Constant, Size = 30, Child = x.CreateChild("a") },
                        new RowItem { Type = RowItemType.Relative, Size = 1, Child = x.CreateChild("b") }
                    }
                })
                .MeasureElement(LayoutSpace.Query(new Size(100, 50)))
                .ExpectChildMeasure("a", Space(30, 50, Final, Query), SpacePlan.FullRender(30, 20))
                .ExpectChildMeasure("b", Space(70, 50, Query, Query), SpacePlan.FullRender(70, 20))
                // the row measures every item again with the height of the tallest one
                .ExpectChildMeasure("a", Space(30, 20, Final, Query), SpacePlan.FullRender(30, 20))
                .ExpectChildMeasure("b", Space(70, 20, Query, Query), SpacePlan.FullRender(70, 20))
                .CheckMeasureResult(SpacePlan.FullRender(100, 20));
        }

        [Test]
        public void Row_AutomaticItemIsAskedForItsNaturalWidth()
        {
            TestPlan
                .For(x => new Row { Items = { new RowItem { Type = RowItemType.Auto, Child = x.CreateChild("a") } } })
                .MeasureElement(LayoutSpace.Final(new Size(100, 50)))
                .ExpectChildMeasure("a", LayoutSpace.Query(Size.Max), SpacePlan.FullRender(30, 20))
                .ExpectChildMeasure("a", Space(30, 50, Final, Final), SpacePlan.FullRender(30, 20))
                .ExpectChildMeasure("a", Space(30, 20, Final, Final), SpacePlan.FullRender(30, 20))
                .CheckMeasureResult(SpacePlan.FullRender(30, 20));
        }

        [Test]
        public void Table_ConstantColumnsAreFinal_OnlyTheTopRowKeepsTheFinalHeight()
        {
            TestPlan
                .For(x =>
                {
                    var table = new Table
                    {
                        Columns = { new TableColumnDefinition(40, 0), new TableColumnDefinition(0, 1) },
                        Cells =
                        {
                            new TableCell { Row = 1, Column = 1, Child = x.CreateChild("a") },
                            new TableCell { Row = 1, Column = 2, Child = x.CreateChild("b") },
                            new TableCell { Row = 2, Column = 1, Child = x.CreateChild("c") },
                            new TableCell { Row = 2, Column = 2, Child = x.CreateChild("d") }
                        }
                    };

                    // a rendered table starts at the first row; the rendering loop does this for every stateful element
                    table.ResetState();
                    return table;
                })
                .MeasureElement(Space(100, 100, Query, Final))
                .ExpectChildMeasure("a", Space(40, 100, Final, Final), SpacePlan.FullRender(40, 30))
                .ExpectChildMeasure("b", Space(60, 100, Query, Final), SpacePlan.FullRender(60, 30))
                .ExpectChildMeasure("c", Space(40, 70, Final, Flowing), SpacePlan.FullRender(40, 20))
                .ExpectChildMeasure("d", Space(60, 70, Query, Flowing), SpacePlan.FullRender(60, 20))
                .CheckMeasureResult(SpacePlan.FullRender(100, 50));
        }

        [Test]
        public void Inlined_ItemsInheritTheWidthModeAndAreAskedForTheirHeight()
        {
            TestPlan
                .For(x => new Inlined { Elements = { x.CreateChild("a") } })
                .MeasureElement(LayoutSpace.Final(new Size(100, 100)))
                .ExpectChildMeasure("a", Space(100, Size.Max.Height, Final, Query), SpacePlan.FullRender(30, 10))
                .CheckMeasureResult(SpacePlan.FullRender(30, 10));
        }

        [Test]
        public void Padding_ForwardsTheModes()
        {
            TestPlan
                .For(x => new Padding { Left = 10, Top = 10, Right = 10, Bottom = 10, Child = x.CreateChild() })
                .MeasureElement(Space(100, 100, Final, Flowing))
                .ExpectChildMeasure(Space(80, 80, Final, Flowing), SpacePlan.FullRender(80, 20))
                .CheckMeasureResult(SpacePlan.FullRender(100, 40));
        }

        [Test]
        public void RotateLayout_QuarterTurnTransposesTheModes()
        {
            TestPlan
                .For(x => new RotateLayout { TurnCount = 1, Child = x.CreateChild() })
                .MeasureElement(Space(100, 200, Final, Query))
                .ExpectChildMeasure(Space(200, 100, Query, Final), SpacePlan.FullRender(200, 100))
                .CheckMeasureResult(SpacePlan.FullRender(100, 200));
        }

        [Test]
        public void Unconstrained_AsksForTheNaturalSize()
        {
            TestPlan
                .For(x => new Unconstrained { Child = x.CreateChild() })
                .MeasureElement(LayoutSpace.Final(new Size(100, 100)))
                .ExpectChildMeasure(LayoutSpace.Query(Size.Max), SpacePlan.FullRender(50, 20))
                .CheckMeasureResult(SpacePlan.FullRender(0, 0));
        }

        [Test]
        public void ScaleToFit_OffersCandidates()
        {
            // the scaled areas are candidates in a search, so the content must not adapt to any of them
            TestPlan
                .For(x => new ScaleToFit { Child = x.CreateChild() })
                .MeasureElement(LayoutSpace.Final(new Size(100, 100)))
                .ExpectChildMeasure(LayoutSpace.Candidate(new Size(100, 100)), SpacePlan.FullRender(50, 50))
                .ExpectChildMeasure(LayoutSpace.Candidate(new Size(100, 100)), SpacePlan.FullRender(50, 50))
                .CheckMeasureResult(SpacePlan.FullRender(50, 50));
        }

        #endregion
    }
}
