namespace QuestPDF.Elements.Text.Items;

internal class TextBlockParagraphSpacing : ITextBlockItem
{
    public float Width { get; }
    public float Height { get; }

    public TextBlockParagraphSpacing(float width, float height)
    {
        Width = width;
        Height = height;
    }
}