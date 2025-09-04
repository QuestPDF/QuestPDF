using NUnit.Framework;

namespace QuestPDF.LayoutTests
{
    public class ColumnTests
    {
        [Test]
        public void Typical()
        {
            LayoutTest
            .HavingSpaceOfSize(100, 140)
            .WithContent(content => content.Shrink().Column(column =>
            {
                column.Spacing(10);
                column.Item().Mock("a").Size(50, 30);
                column.Item().Mock("b").Size(40, 20);
                column.Item().Mock("c").Size(70, 40);
                column.Item().Mock("d").Size(60, 60);
            }))
            .ExpectedDrawResult(document =>
            {
                document.Page().RequiredAreaSize(70, 140).Content(page =>
                {
                    page.Mock("a").Position(0, 0).Size(70, 30);
                    page.Mock("b").Position(0, 40).Size(70, 20);
                    page.Mock("c").Position(0, 70).Size(70, 40);
                    page.Mock("d").Position(0, 120).Size(70, 20);
                });

                document.Page().RequiredAreaSize(60, 40).Content(page =>
                {
                    page.Mock("d").Position(0, 0).Size(60, 40);
                });
            });
        }

        [Test]
        public void SingleItem()
        {
            LayoutTest
            .HavingSpaceOfSize(100, 100)
            .WithContent(content => content.Shrink().Column(column =>
            {
                column.Spacing(10);
                column.Item().Mock("a").Size(50, 30);
            }))
            .ExpectedDrawResult(document =>
            {
                document.Page().RequiredAreaSize(50, 30).Content(page =>
                {
                    page.Mock("a").Position(0, 0).Size(50, 30);
                });
            });
        }

        [Test]
        public void ZeroHeightItemDoesNotConsumeSpacing()
        {
            LayoutTest
            .HavingSpaceOfSize(100, 100)
            .WithContent(content => content.Shrink().Column(column =>
            {
                column.Spacing(10);
                column.Item().Mock("a").Size(50, 30);
                column.Item().Mock("b").Size(50, 0);
                column.Item().Mock("c").Size(70, 20);
            }))
            .ExpectedDrawResult(document =>
            {
                document.Page().RequiredAreaSize(70, 60).Content(page =>
                {
                    page.Mock("a").Position(0, 0).Size(70, 30);
                    page.Mock("b").Position(0, 30).Size(70, 0);
                    page.Mock("c").Position(0, 40).Size(70, 20);
                });
            });
        }

        [Test]
        public void NoSpacing()
        {
            LayoutTest
            .HavingSpaceOfSize(100, 100)
            .WithContent(content => content.Shrink().Column(column =>
            {
                column.Spacing(0);
                column.Item().Mock("a").Size(50, 30);
                column.Item().Mock("b").Size(40, 20);
            }))
            .ExpectedDrawResult(document =>
            {
                document.Page().RequiredAreaSize(50, 50).Content(page =>
                {
                    page.Mock("a").Position(0, 0).Size(50, 30);
                    page.Mock("b").Position(0, 30).Size(50, 20);
                });
            });
        }

        [Test]
        public void PartialRenderItem()
        {
            LayoutTest
            .HavingSpaceOfSize(100, 80)
            .WithContent(content => content.Shrink().Column(column =>
            {
                column.Spacing(5);
                column.Item().Mock("a").Size(50, 40);
                column.Item().Mock("b").Size(60, 50);
            }))
            .ExpectedDrawResult(document =>
            {
                document.Page().RequiredAreaSize(60, 80).Content(page =>
                {
                    page.Mock("a").Position(0, 0).Size(60, 40);
                    page.Mock("b").Position(0, 45).Size(60, 35);
                });

                document.Page().RequiredAreaSize(60, 15).Content(page =>
                {
                    page.Mock("b").Position(0, 0).Size(60, 15);
                });
            });
        }

        // New test for wide spanning items that exceed column width
        [Test]
        public void ItemSpanningMultipleColumns()
        {
            // Setup larger space to test wide items
            LayoutTest
            .HavingSpaceOfSize(200, 150)
            .WithContent(content => content.Shrink().Column(column =>
            {
                column.Spacing(10);

                // Regular item
                column.Item().Mock("normal").Size(50, 30);

                // Wide spanning item (180px width - wider than normal column)
                column.Item().Mock("wide-item").Size(180, 40);

                // Following regular items
                column.Item().Mock("item1").Size(60, 25);
                column.Item().Mock("item2").Size(70, 35);
            }))
            .ExpectedDrawResult(document =>
            {
                // First page should accommodate the wide item
                document.Page()
                .RequiredAreaSize(180, 150) // Expanded to fit wide item
                .Content(page =>
                {
                    // All items expand to match widest element (180px)
                    page.Mock("normal").Position(0, 0).Size(180, 30);
                    page.Mock("wide-item").Position(0, 40).Size(180, 40); // +10 spacing
                    page.Mock("item1").Position(0, 90).Size(180, 25);    // +10 spacing
                    page.Mock("item2").Position(0, 125).Size(180, 25);  // Only 25 of 35 fits
                });

                // Second page contains remaining part of last item
                document.Page()
                .RequiredAreaSize(70, 10) // Original width (70) for remaining portion
                .Content(page =>
                {
                    page.Mock("item2").Position(0, 0).Size(70, 10);
                });
            });
        }
    }
}
