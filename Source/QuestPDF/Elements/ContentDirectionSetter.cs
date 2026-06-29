using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class ContentDirectionSetter : ContainerElement
    {
        public ContentDirection ContentDirection { get; set; }

        internal override string? GetCompanionHint() => ContentDirection.ToString();
    }
}