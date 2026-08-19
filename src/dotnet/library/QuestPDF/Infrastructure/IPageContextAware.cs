namespace QuestPDF.Infrastructure
{
    internal interface IPageContextAware
    {
        IPageContext PageContext { get; set; }
    }
}
