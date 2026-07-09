using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class DefaultTextStyle : ContainerElement
    {
        public TextStyle TextStyle { get; set; } = TextStyle.Default;
    }
}