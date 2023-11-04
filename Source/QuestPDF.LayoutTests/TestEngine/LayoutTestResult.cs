using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal sealed class LayoutTestResult
{
    public Size PageSize { get; set; }
    
    public ICollection<PageLayoutSnapshot> ActualLayout { get; set; }
    public ICollection<PageLayoutSnapshot> ExpectedLayout { get; set; }

    public sealed class PageLayoutSnapshot
    {
        public Size RequiredArea { get; set; }
        public ICollection<MockLayoutPosition> MockPositions { get; set; }
    }

    public sealed class MockLayoutPosition
    {
        public string MockId { get; set; }
        public Position Position { get; set; }
        public Size Size { get; set; }
    }
}