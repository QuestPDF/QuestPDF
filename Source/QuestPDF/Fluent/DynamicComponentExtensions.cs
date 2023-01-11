using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class DynamicComponentExtensions
    {
        public static void Dynamic<TState>(this IContainer element, IDynamicComponent<TState> dynamicElement) where TState : struct
        {
            var componentProxy = DynamicComponentProxy.CreateFrom(dynamicElement);
            element.Element(new DynamicHost(componentProxy));
        }
        
        public static void Element(this IContainer element, IDynamicElement child)
        {
            ElementExtensions.Element(element, child);
        }
    }
}