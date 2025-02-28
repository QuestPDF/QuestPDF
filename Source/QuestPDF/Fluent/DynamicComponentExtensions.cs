using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class DynamicComponentExtensions
    {
        /// <summary>
        /// Represents a dynamically generated section of the document.
        /// Components are page-aware, understand their positioning, can dynamically construct other content elements, and assess their dimensions, enabling complex layout creations.
        /// <a href="https://www.questpdf.com/concepts/code-patterns/dynamic-components.html">Learn more</a>
        /// </summary>
        /// <example>
        /// <para>
        /// Consider an invoice that presents all purchased items in a table format.
        /// Instead of just showing the final total price under the table, the requirement is to display the cumulative prices on each separate page.
        /// </para>
        /// <para>Using the dynamic component, you can manually assemble the table, count how many items are visible on each page, calculate the price sum for items visible on each page, and then render the result under each sub-table.</para>
        /// </example>
        public static void Dynamic(this IContainer element, IDynamicComponent dynamicElement)
        {
            var componentProxy = DynamicComponentProxy.CreateFrom(dynamicElement);
            element.Element(new DynamicHost(componentProxy));
        }

        /// <summary>
        /// Represents a section of the document dynamically created based on its inner state.
        /// Components are page-aware, understand their positioning, can dynamically construct other content elements, and assess their dimensions, enabling complex layout creations.
        /// <a href="https://www.questpdf.com/concepts/code-patterns/dynamic-components.html">Learn more</a>
        /// </summary>
        /// <example>
        /// <para>
        /// Consider an invoice that presents all purchased items in a table format.
        /// Instead of just showing the final total price under the table, the requirement is to display the cumulative prices on each separate page.
        /// </para>
        /// <para>Using the dynamic component, you can manually assemble the table, count how many items are visible on each page, calculate the price sum for items visible on each page, and then render the result under each sub-table.</para>
        /// </example>
        public static void Dynamic<TState>(this IContainer element, IDynamicComponent<TState> dynamicElement) where TState : struct
        {
            var componentProxy = DynamicComponentProxy.CreateFrom(dynamicElement);
            element.DebugPointer(DebugPointerType.Dynamic, dynamicElement.GetType().Name).Element(new DynamicHost(componentProxy));
        }
        
        /// <summary>
        /// Allows to inject the unattached content created by the <see cref="DynamicContext.CreateElement"/> method within the <see cref="DynamicComponentExtensions.Dynamic">Dynamic component</see>.
        /// </summary>
        public static void Element(this IContainer element, IDynamicElement child)
        {
            ElementExtensions.Element(element, child);
        }
    }
}