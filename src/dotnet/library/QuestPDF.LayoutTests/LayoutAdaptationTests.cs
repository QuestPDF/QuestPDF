using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests;

/// <summary>
/// Verifies that a configured constraint which the available space can never satisfy is lowered
/// to the nearest value that it can, instead of failing the entire layout.
/// </summary>
/// <remarks>
/// <para>The adaptation is limited to final axes. The width offered to an element is always the largest it can ever
/// receive: no later page is wider, and neither is any other column of the same layout. A horizontal shortfall is
/// therefore always final and is resolved in place.</para>
/// <para>The height is final only for an element offered the entire height of a fixed page, or of a box that
/// is fixed in height. Below other content the height is flowing: moving the content to the next page may still
/// resolve the shortfall, so the configuration is preserved and the content moves. The tests at the bottom of this
/// file pin that distinction, together with the pagination hints that request such a move: they give way where
/// no page can hold the content, instead of failing the layout or costing an empty page.</para>
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

    #region The vertical axis: flowing heights move, final heights adapt

    [Test]
    public void MinHeightGreaterThanTheRemainingSpace_MovesToTheNextPage()
    {
        // A whole empty page satisfies the constraint, so the content moves instead of adapting.
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().Column(column =>
                {
                    column.Item().Mock("first").SolidBlock(100, 60);
                    column.Item().MinHeight(80).Mock("block").SolidBlock(20, 5);
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
                        page.Mock("block").Position(0, 0).Size(100, 80);
                    });
            });
    }

    [Test]
    public void MinHeightGreaterThanAnyPage_IsClampedToThePageHeight()
    {
        // The content moves to the next page, where it is offered the entire height. No page is taller,
        // so the minimum can never be satisfied and is lowered to the page height instead of failing the layout.
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().Column(column =>
                {
                    column.Item().Mock("first").SolidBlock(100, 60);
                    column.Item().MinHeight(250).Mock("block").SolidBlock(20, 5);
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
                        page.Mock("block").Position(0, 0).Size(100, 100);
                    });
            });
    }

    [Test]
    public void MinHeightGreaterThanAFixedBox_IsClampedToTheBox()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().MinHeight(250).Mock("content").SolidBlock(20, 5);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(20, 100)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(20, 100);
                    });
            });
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

    [Test]
    public void AspectRatioTooTallForAnyPage_FallsBackToTheLargestFittingArea()
    {
        // The FitWidth option would require 100 / 0.5 = 200 points of height, and no page is taller than 100.
        // The largest area of the same ratio that fits the page is 50 x 100.
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().Column(column =>
                {
                    column.Item().Mock("first").SolidBlock(100, 60);
                    column.Item().AspectRatio(0.5f, AspectRatioOption.FitWidth).Mock("image").SolidBlock(5, 5);
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
                        page.Mock("image").Position(0, 0).Size(50, 100);
                    });
            });
    }

    [Test]
    public void AspectRatioTooTallForItsBox_FallsBackToTheLargestFittingArea()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().AspectRatio(0.5f, AspectRatioOption.FitWidth).Mock("image").SolidBlock(5, 5);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 100)
                    .Content(page =>
                    {
                        page.Mock("image").Position(0, 0).Size(50, 100);
                    });
            });
    }

    #endregion

    #region Pagination hints give way where no page can hold the content

    [Test]
    public void ShowEntireAroundContentThatFitsAFreshPage_MovesItToTheNextPage()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().Column(column =>
                {
                    column.Item().Mock("first").SolidBlock(100, 60);
                    column.Item().ShowEntire().Mock("block").SolidBlock(100, 80);
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
                        page.Mock("block").Position(0, 0).Size(100, 80);
                    });
            });
    }

    [Test]
    public void ShowEntireAroundContentTallerThanAnyPage_LetsItFlow()
    {
        // The content moves once, to a page that offers it the entire height. That page cannot hold it entirely
        // and no other page is taller, so the content is split there instead of failing the layout.
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().Column(column =>
                {
                    column.Item().Mock("first").SolidBlock(100, 60);
                    column.Item().ShowEntire().Mock("block").ContinuousBlock(100, 250);
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
                        page.Mock("block").Position(0, 0).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 50);
                    });
            });
    }

    [Test]
    public void ShowEntireInsideAFixedBox_LetsTheContentFlow()
    {
        // The box is fixed in height, so the content is offered its final height on the very first page.
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().ShowEntire().Mock("block").ContinuousBlock(100, 250);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 50)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 50);
                    });
            });
    }

    [Test]
    public void EnsureSpaceBelowOtherContent_MovesToTheNextPage()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().Column(column =>
                {
                    column.Item().Mock("first").SolidBlock(100, 60);
                    column.Item().EnsureSpace(80).Mock("block").ContinuousBlock(100, 150);
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
                        page.Mock("block").Position(0, 0).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 50);
                    });
            });
    }

    [Test]
    public void EnsureSpaceOnAFreshPage_DoesNotCostAnEmptyPage()
    {
        // The element is offered the entire height of the page right away. No page offers more,
        // so it renders immediately instead of leaving the page empty and trying again on the next one.
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().Column(column => column.Item().EnsureSpace(500).Mock("block").ContinuousBlock(100, 250));
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 50);
                    });
            });
    }

    [Test]
    public void PreventPageBreakAroundContentThatFitsAFreshPage_MovesItToTheNextPage()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().Column(column =>
                {
                    column.Item().Mock("first").SolidBlock(100, 60);
                    column.Item().PreventPageBreak().Mock("block").SolidBlock(100, 80);
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
                        page.Mock("block").Position(0, 0).Size(100, 80);
                    });
            });
    }

    [Test]
    public void PreventPageBreakOnAFreshPage_DoesNotCostAnEmptyPage()
    {
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().Column(column => column.Item().PreventPageBreak().Mock("block").ContinuousBlock(100, 250));
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(100, 50);
                    });
            });
    }

    #endregion

    #region Balanced multi-column layouts

    [Test]
    public void BalancedMultiColumn_HonorsTheHintsWhereMovingToTheNextColumnHelps()
    {
        // The balanced height is found with candidate offers, at which the hint moves the block to the next column.
        // The content is drawn with the same offer, so it lands in the same columns.
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().MultiColumn(multiColumn =>
                {
                    multiColumn.BalanceHeight();

                    multiColumn.Content().Column(column =>
                    {
                        column.Item().Mock("a").SolidBlock(50, 30);
                        column.Item().ShowEntire().Mock("b").SolidBlock(50, 50);
                    });
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 30);
                        page.Mock("b").Position(50, 0).Size(50, 50);
                    });
            });
    }

    [Test]
    public void BalancedMultiColumn_SplitsContentTallerThanAnyColumn()
    {
        // No column is taller than the page, so the block is split across columns instead of failing the layout.
        LayoutTest
            .ForPage(page =>
            {
                page.MinSize(new PageSize(100, 100));
                page.MaxSize(new PageSize(100, 100));

                page.Content().MultiColumn(multiColumn =>
                {
                    multiColumn.BalanceHeight();
                    multiColumn.Content().ShowEntire().Mock("block").ContinuousBlock(50, 150);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("block").Position(0, 0).Size(50, 100);
                        page.Mock("block").Position(50, 0).Size(50, 50);
                    });
            });
    }

    #endregion
}
