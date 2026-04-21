namespace QuestPDF.LayoutTests;

public class SkipLastAndShowLastTests
{
    [Test]
    public void SkipLast_InBefore_HidesOnLastPage()
    {
        // Before (SkipLast): shown on pages 1-2, hidden on page 3 (last)
        // Content: 240px tall in a 100px page, with 80px available per page (before takes 20px)
        // Page 3: Before hidden → full 100px for content, but only 80px remains → fits

        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Decoration(decoration =>
                {
                    decoration.Before().SkipLast().Mock("before").Size(80, 20);
                    decoration.Content().Mock("content").ContinuousBlock(70, 240);
                });
            })
            .ExpectDrawResult(document =>
            {
                // Page 1: Before shown, content renders 80px (100 - 20 before)
                document
                    .Page()
                    .RequiredAreaSize(80, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(80, 20);
                        page.Mock("content").Position(0, 20).Size(80, 80);
                    });

                // Page 2: Same as page 1, another 80px rendered
                document
                    .Page()
                    .RequiredAreaSize(80, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(80, 20);
                        page.Mock("content").Position(0, 20).Size(80, 80);
                    });

                // Page 3 (last): Before hidden, remaining 80px fits in 100px space
                document
                    .Page()
                    .RequiredAreaSize(70, 80)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(70, 80);
                    });
            });
    }

    [Test]
    public void ShowLast_InAfter_ShowsOnLastPage()
    {
        // After (ShowLast): hidden on pages 1-2, shown on page 3 (last)
        // Content: 250px tall, full 100px per page on non-last pages

        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Decoration(decoration =>
                {
                    decoration.Content().Mock("content").ContinuousBlock(70, 250);
                    decoration.After().ShowLast().Mock("after").Size(80, 30);
                });
            })
            .ExpectDrawResult(document =>
            {
                // Page 1: After hidden, content gets full 100px
                document
                    .Page()
                    .RequiredAreaSize(70, 100)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(70, 100);
                    });

                // Page 2: After hidden, content gets full 100px
                document
                    .Page()
                    .RequiredAreaSize(70, 100)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(70, 100);
                    });

                // Page 3 (last): After shown, remaining 50px content + 30px after
                document
                    .Page()
                    .RequiredAreaSize(80, 80)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(80, 50);
                        page.Mock("after").Position(0, 50).Size(80, 30);
                    });
            });
    }

    [Test]
    public void ShowLast_SinglePage_ContentFitsOnOnePage()
    {
        // When content fits on a single page, it IS the last page
        // ShowLast should show, since the only page is also the last page

        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Decoration(decoration =>
                {
                    decoration.Content().Mock("content").Size(70, 40);
                    decoration.After().ShowLast().Mock("after").Size(80, 30);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(80, 70)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(80, 40);
                        page.Mock("after").Position(0, 40).Size(80, 30);
                    });
            });
    }

    [Test]
    public void SkipLast_SinglePage_ContentFitsOnOnePage()
    {
        // When content fits on a single page, it IS the last page
        // SkipLast should hide, since the only page is also the last page

        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Decoration(decoration =>
                {
                    decoration.Before().SkipLast().Mock("before").Size(80, 20);
                    decoration.Content().Mock("content").Size(70, 40);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(70, 40)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(70, 40);
                    });
            });
    }

    [Test]
    public void SkipLast_And_ShowLast_SwapOnLastPage()
    {
        // Before has two items: SkipLast (shown on non-last) and ShowLast (shown on last)
        // Both are 20px tall, so Before height stays constant → no oscillation

        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Decoration(decoration =>
                {
                    decoration.Before().Column(column =>
                    {
                        column.Item().SkipLast().Mock("before-regular").Size(60, 20);
                        column.Item().ShowLast().Mock("before-last").Size(60, 20);
                    });
                    decoration.Content().Mock("content").ContinuousBlock(50, 200);
                });
            })
            .ExpectDrawResult(document =>
            {
                // Page 1: "before-regular" shown (non-last page)
                document
                    .Page()
                    .RequiredAreaSize(60, 100)
                    .Content(page =>
                    {
                        page.Mock("before-regular").Position(0, 0).Size(60, 20);
                        page.Mock("content").Position(0, 20).Size(60, 80);
                    });

                // Page 2: "before-regular" shown (non-last page)
                document
                    .Page()
                    .RequiredAreaSize(60, 100)
                    .Content(page =>
                    {
                        page.Mock("before-regular").Position(0, 0).Size(60, 20);
                        page.Mock("content").Position(0, 20).Size(60, 80);
                    });

                // Page 3 (last): "before-last" shown instead of "before-regular"
                document
                    .Page()
                    .RequiredAreaSize(60, 60)
                    .Content(page =>
                    {
                        page.Mock("before-last").Position(0, 0).Size(60, 20);
                        page.Mock("content").Position(0, 20).Size(60, 40);
                    });
            });
    }

    [Test]
    public void ShowLast_InBefore_OscillationFallback()
    {
        // ShowLast in Before with a LARGE element (60px tall)
        // When shown on last page, it steals so much space that Content no longer fits
        // → Oscillation detected → Falls back to non-last-page behavior (ShowLast hidden)

        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Decoration(decoration =>
                {
                    decoration.Before().ShowLast().Mock("before").Size(60, 60);
                    decoration.Content().Mock("content").ContinuousBlock(50, 150);
                });
            })
            .ExpectDrawResult(document =>
            {
                // Page 1: ShowLast hidden (not last page), content gets full 100px
                document
                    .Page()
                    .RequiredAreaSize(50, 100)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(50, 100);
                    });

                // Page 2: Candidate last page, but ShowLast (60px) would leave only 40px for
                // remaining 50px of content → oscillation → fallback to non-last-page
                // ShowLast hidden, content renders remaining 50px
                document
                    .Page()
                    .RequiredAreaSize(50, 50)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(50, 50);
                    });
            });
    }

    [Test]
    public void SkipLast_InBefore_And_ShowLast_InAfter()
    {
        // SkipLast in Before + ShowLast in After: different decorations on last page

        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Decoration(decoration =>
                {
                    decoration.Before().SkipLast().Mock("before").Size(60, 20);
                    decoration.Content().Mock("content").ContinuousBlock(50, 200);
                    decoration.After().ShowLast().Mock("after").Size(60, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                // Page 1: Before shown (20px), After hidden, Content gets 80px
                document
                    .Page()
                    .RequiredAreaSize(60, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(60, 20);
                        page.Mock("content").Position(0, 20).Size(60, 80);
                    });

                // Page 2: Same as page 1
                document
                    .Page()
                    .RequiredAreaSize(60, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(60, 20);
                        page.Mock("content").Position(0, 20).Size(60, 80);
                    });

                // Page 3 (last): Before hidden, After shown (20px), remaining 40px content
                // Space = 100 - 0 (no before) - 20 (after) = 80, but only 40px remaining
                document
                    .Page()
                    .RequiredAreaSize(60, 60)
                    .Content(page =>
                    {
                        page.Mock("content").Position(0, 0).Size(60, 40);
                        page.Mock("after").Position(0, 40).Size(60, 20);
                    });
            });
    }

    [Test]
    public void ShowLast_CombinedWithShowOnce()
    {
        // ShowOnce().ShowLast() means: show only on the first AND last page
        // For multi-page content, first page ≠ last page, so this shows nothing
        // For single-page content, first = last, so it shows

        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Decoration(decoration =>
                {
                    decoration.Before().ShowOnce().ShowLast().Mock("before").Size(60, 20);
                    decoration.Content().Mock("content").Size(50, 30);
                });
            })
            .ExpectDrawResult(document =>
            {
                // Single page: ShowOnce allows (first render), ShowLast allows (last page) → shown
                document
                    .Page()
                    .RequiredAreaSize(60, 50)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(60, 20);
                        page.Mock("content").Position(0, 20).Size(60, 30);
                    });
            });
    }
}
