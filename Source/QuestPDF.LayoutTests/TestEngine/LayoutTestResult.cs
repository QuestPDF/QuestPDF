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