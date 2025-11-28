using System;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent;

public sealed class MultiColumnDescriptor
{
    internal MultiColumn MultiColumn { get; } = new MultiColumn();

    internal MultiColumnDescriptor()
    {
        
    }
        
    /// <summary>
    /// Configures the horizontal spacing between adjacent columns in the layout.
    /// </summary>
    /// <remarks>
    /// This method affects the visual presentation of the column arrangement.
    /// Positive values increase separation, while negative values may cause overlap.
    /// </remarks>
    public void Spacing(float value, Unit unit = Unit.Point)
    {
        MultiColumn.Spacing = value.ToPoints(unit);
    }
    
    /// <summary>
    /// Defines the number of vertical columns in the layout.
    /// </summary>
    /// <remarks>
    /// This method determines the basic structure of the grid layout.
    /// Setting this value will redistribute the existing elements across the new column count.
    /// </remarks>
    public void Columns(int value = 2)
    {
        if (value < 2)
            throw new DocumentComposeException("The 'MultiColumn.Columns' value should be higher than 1.");
        
        MultiColumn.ColumnCount = value;
    }

    /// <summary>
    /// Controls the content distribution across columns to achieve balanced heights.
    /// </summary>
    /// <remarks>
    /// <para>When enabled: content flow is adjusted to equalize column heights; each column will have approximately the same height.</para>
    /// <para>When disabled: layout occupies the full vertical space, the last column may be shorter or empty, depending on content quantity.</para>
    /// </remarks>
    public void BalanceHeight(bool enable = true)
    {
        MultiColumn.BalanceHeight = enable;
    }

    /// <summary>
    /// Retrieves a container for the primary content to be distributed across multiple columns.
    /// </summary>
    /// <remarks>
    /// The returned container serves as the main content area for the multi-column layout.
    /// It supports all available layout elements and automatically divides its content among the defined columns.
    /// </remarks>
    public IContainer Content()
    {
        if (MultiColumn.Content is not Empty)
            throw new DocumentComposeException("The 'MultiColumn.Content' layer has already been defined. Please call this method only once.");
        
        var container = new Container();
        MultiColumn.Content = container;
        return container;
    }
    
    /// <summary>
    /// Retrieves a container for content positioned between columns in the layout.
    /// </summary>
    /// <remarks>
    /// The container's dimensions are determined by the height of the columns and the configured spacing.
    /// This container supports all available layout elements, allowing for flexible design of inter-column content.
    /// </remarks>
    public IContainer Spacer()
    {
        if (MultiColumn.Spacer is not Empty)
            throw new DocumentComposeException("The 'MultiColumn.Spacer' layer has already been defined. Please call this method only once.");
        
        var container = new RepeatContent();
        MultiColumn.Spacer = container;
        return container;
    }
}

public static class MultiColumnExtensions
{
    /// <summary>
    /// Creates a multi-column layout within the current container element.
    /// </summary>
    /// <remarks>
    /// A multi-column layout organizes content into vertical columns, similar to a newspaper or magazine layout.
    /// This approach allows for efficient use of horizontal space and can improve readability, especially
    /// for wide containers or screens. The content flows from one column to the next, and the number of
    /// columns can be adjusted based on the container's width or specific design requirements.
    /// </remarks>
    /// <param name="handler">The action to configure the column's content and behavior.</param>
    public static void MultiColumn(this IContainer element, Action<MultiColumnDescriptor> handler)
    {
        var descriptor = new MultiColumnDescriptor();
        handler(descriptor);

        element
            .Element(x => descriptor.MultiColumn.BalanceHeight ? x.ShrinkVertical() : x)
            .Element(descriptor.MultiColumn);
    }
}