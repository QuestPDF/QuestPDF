using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests;

/// <summary>
/// Verifies that a configured constraint which the available space can never satisfy is lowered
/// to the nearest value that it can, instead of failing the entire layout.
/// </summary>
/// <remarks>
/// <para>The adaptation is limited to the horizontal axis. The width offered to an element is always
/// the largest it can ever receive: no later page is wider, and neither is any other column of the same
/// layout. A horizontal shortfall is therefore final and is resolved in place.</para>
/// <para>A vertical shortfall is left alone, because moving the content to the next page may still resolve it.
/// The tests at the bottom of this file pin that distinction.</para>
/// </remarks>
public class LayoutAdaptationTests
{
    #region AspectRatio

    [Test]
    public void AspectRatioTooWideForItsColumn_FallsBackToTheLargestFittingArea()
    {
        // The column is 100 points wide, so the FitHeight option would require 4 * 50 = 200 points.
        // The largest area of the same ratio that fits the column is 100 x 25.
        LayoutTest
            .HavingSpaceOfSize(300, 300)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(100).Height(50).AspectRatio(4f, AspectRatioOption.FitHeight).Mock("image").SolidBlock(5, 5);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 50)
                    .Content(page =>
                    {
                        page.Mock("image").Position(0, 0).Size(100, 25);
                    });
            });
    }

    [Test]
    public void AspectRatioTooWideForItsTableCell_FallsBackToTheLargestFittingArea()
    {
        LayoutTest
            .HavingSpaceOfSize(300, 300)
            .ForContent(content =>
            {
                content.Shrink().Table(table =>
                {
                    table.ColumnsDefinition(columns => columns.ConstantColumn(100));
                    table.Cell().Height(50).AspectRatio(4f, AspectRatioOption.FitHeight).Mock("image").SolidBlock(5, 5);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 50)
                    .Content(page =>
                    {
                        page.Mock("image").Position(0, 0).Size(100, 25);
                    });
            });
    }

    [Test]
    public void AspectRatioThatFitsHorizontally_KeepsTheConfiguredOption()
    {
        // The FitHeight option fits, so it is honored and no adaptation takes place.
        LayoutTest
            .HavingSpaceOfSize(300, 300)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.ConstantItem(250).Height(50).AspectRatio(4f, AspectRatioOption.FitHeight).Mock("image").SolidBlock(5, 5);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(250, 50)
                    .Content(page =>
                    {
                        page.Mock("image").Position(0, 0).Size(200, 50);
                    });
            });
    }

    [Test]
    public void AspectRatioFallbackIsAlignedCorrectlyInRightToLeftContent()
    {
        // The fallback area always spans the entire available width: the option is abandoned only when it
        // is wider than the space, which means the ratio is wider than the space, which makes FitArea resolve
        // to FitWidth. The right-to-left offset is therefore zero, exactly as in left-to-right content.
        LayoutTest
            .HavingSpaceOfSize(300, 300)
            .ForContent(content =>
            {
                content.Shrink().ContentFromRightToLeft().Row(row =>
                {
                    row.ConstantItem(100).Height(50).AspectRatio(4f, AspectRatioOption.FitHeight).Mock("image").SolidBlock(5, 5);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 50)
                    .Content(page =>
                    {
                        page.Mock("image").Position(0, 0).Size(100, 25);
                    });
            });
    }

    #endregion

    #region Constrained

    [Test]
    public void WidthGreaterThanTheAvailableSpace_IsClampedToTheAvailableSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Width(250).Mock("content").SolidBlock(5, 20);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 20)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(100, 20);
                    });
            });
    }

    [Test]
    public void MinWidthGreaterThanTheAvailableSpace_IsClampedToTheAvailableSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().MinWidth(250).Mock("content").SolidBlock(5, 20);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 20)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(100, 20);
                    });
            });
    }

    [Test]
    public void ClampedWidthIsDrawnExactlyAsItWasMeasured()
    {
        // A column draws its items with the full column width rather than with the measured one,
        // so the clamped value has to describe the same geometry in both operations.
        LayoutTest
            .HavingSpaceOfSize(100, 200)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().MinWidth(250).Mock("clamped").SolidBlock(5, 20);
                    column.Item().Mock("sibling").SolidBlock(5, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 40)
                    .Content(page =>
                    {
                        page.Mock("clamped").Position(0, 0).Size(100, 20);
                        page.Mock("sibling").Position(0, 20).Size(100, 20);
                    });
            });
    }

    [Test]
    public void MinWidthGreaterThanMaxWidth_IsStillReportedAsAnError()
    {
        // The two values contradict each other, so no amount of space could ever satisfy both.
        // There is no nearest feasible value to fall back to, therefore it stays an authoring error.
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().MinWidth(250).MaxWidth(50).SolidBlock(5, 20);
            })
            .ExpectLayoutException("The minimum width 250 is greater than the maximum width 50.");
    }

    #endregion

    #region A question about the natural size is not a constraint

    [Test]
    public void ScaleToFitScalesItsContentInsteadOfClampingIt()
    {
        // The areas ScaleToFit offers while searching for the right scale are candidates, not allocations.
        // The content has to describe itself as configured at every candidate, otherwise it would clamp
        // to the first one and never be scaled. The block is drawn at its configured size in scaled coordinates.
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.ScaleToFit().Width(500).Height(20).Mock("content").SolidBlock(5, 5);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(99.74609f, 3.9898438f)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(500, 20);
                    });
            });
    }

    [Test]
    public void UnconstrainedContentIsNotAdapted()
    {
        // Unconstrained asks its content for the natural size, so the configured width is honored in full.
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Unconstrained().Width(500).Height(20).Mock("content").SolidBlock(5, 5);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(0, 0)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(500, 20);
                    });
            });
    }

    #endregion

    #region The vertical axis is not adapted

    [Test]
    public void MinHeightGreaterThanTheAvailableSpace_IsStillReportedAsAnError()
    {
        // A taller page could satisfy the constraint, so the shortfall is not final and is reported.
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().MinHeight(250).SolidBlock(20, 5);
            })
            .ExpectLayoutException("The available vertical space is less than the minimum height.");
    }

    [Test]
    public void AspectRatioTooTallForTheRemainingSpace_MovesToTheNextPage()
    {
        // The content does not fit under the first item, but a whole empty page accommodates it.
        // Pagination resolves the shortfall, so the configured option is preserved instead of being adapted.
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().Column(column =>
                {
                    column.Item().Mock("first").SolidBlock(100, 60);
                    column.Item().AspectRatio(1f, AspectRatioOption.FitWidth).Mock("image").SolidBlock(5, 5);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("first").Position(0, 0).Size(100, 60);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("image").Position(0, 0).Size(100, 100);
                    });
            });
    }

    #endregion
}
