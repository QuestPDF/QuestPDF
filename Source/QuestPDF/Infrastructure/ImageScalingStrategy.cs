namespace QuestPDF.Infrastructure
{
    public enum ImageScalingStrategy
    {
        // TODO: add comments
        Always,
        ScaleOnlyToSmallerResolution,
        ScaleOnlyToSignificantlySmallerResolution,
        Never
    }
}