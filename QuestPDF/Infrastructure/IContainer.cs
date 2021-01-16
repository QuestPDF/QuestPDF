namespace QuestPDF.Infrastructure
{
    public interface IContainer
    {
        IElement? Child { get; set; }
    }
}