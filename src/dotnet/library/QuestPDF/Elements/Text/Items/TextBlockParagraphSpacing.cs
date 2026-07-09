namespace QuestPDF.Elements.Text.Items;

internal sealed class TextBlockParagraphSpacing : ITextBlockItem
{
    public float Width { get; }
    public float Height { get; }

    public TextBlockParagraphSpacing(float width, float height)
    {
        Width = width;
        Height = height;
    }
}