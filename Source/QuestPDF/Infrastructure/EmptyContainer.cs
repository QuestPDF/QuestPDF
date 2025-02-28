using QuestPDF.Elements;

namespace QuestPDF.Infrastructure;

public static class EmptyContainer
{
    /// <summary>
    /// Creates an empty IContainer instance.
    /// </summary>
    public static IContainer Create() => new Container();
}