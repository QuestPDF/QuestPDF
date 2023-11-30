namespace QuestPDF.Infrastructure;

internal interface IStateful<T>
{
    public T State { get; set; }
}