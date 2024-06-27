using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent;

public static class MultiColumnExtensions
{
    /// <summary>
    /// Creates a multi-column layout within the current container element.
    /// </summary>
    public static IContainer MultiColumn(this IContainer element, int columnCount = 2, float spacing = 0)
    {
        return element.Element(new MultiColumn()
        {
            ColumnCount = columnCount,
            Spacing = spacing
        });
    }
}