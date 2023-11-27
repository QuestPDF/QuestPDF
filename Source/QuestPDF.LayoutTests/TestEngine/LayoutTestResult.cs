using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal sealed class LayoutTestResult
{
    public Size PageSize { get; set; }
    
    public DocumentLayout ActualLayout { get; set; }
    public DocumentLayout ExpectedLayout { get; set; }

    public sealed class DocumentLayout
    {
        public ICollection<PageLayout> Pages { get; set; } = new List<PageLayout>();
        public bool GeneratesInfiniteLayout { get; set; }
    }
    
    public sealed class PageLayout
    {
        public Size RequiredArea { get; set; }
        public ICollection<MockLayoutPosition> Mocks { get; set; }
    }

    public sealed class MockLayoutPosition
    {
        public string MockId { get; set; }
        public Position Position { get; set; }
        public Size Size { get; set; }
    }
}

internal static class LayoutTestResultHelpers
{
    public static IEnumerable<(LayoutTestResult.MockLayoutPosition Below, LayoutTestResult.MockLayoutPosition Above)> GetOverlappingItems(this ICollection<LayoutTestResult.MockLayoutPosition> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            for (var j = i + 1; j < items.Count; j++)
            {
                var beforeChild = items.ElementAt(i);
                var afterChild = items.ElementAt(j);

                var beforeBoundingBox = BoundingBox.From(beforeChild.Position, beforeChild.Size);
                var afterBoundingBox = BoundingBox.From(afterChild.Position, afterChild.Size);

                var intersection = BoundingBoxExtensions.Intersection(beforeBoundingBox, afterBoundingBox);
                        
                if (intersection == null)
                    continue;

                yield return (beforeChild, afterChild);
            }
        }
    }
}