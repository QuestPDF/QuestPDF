namespace QuestPDF.Infrastructure
{
    public enum ImageResizeStrategy
    {
        // TODO: add comments
        Always,
        ScaleOnlyToSmallerResolution,
        ScaleOnlyToSignificantlySmallerResolution,
        Never
    }
}