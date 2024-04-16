namespace QuestPDF.Infrastructure;

internal interface IPageContextAware
{
    public IPageContext PageContext { get; set; }
}