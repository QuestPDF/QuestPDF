using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class DynamicComponentExtensions
    {
        public static void Dynamic<TDynamic>(this IContainer element) where TDynamic : IDynamicComponent, new()
        {
            element.Dynamic(new TDynamic());
        }

        public static void Dynamic(this IContainer element, IDynamicComponent dynamicElement)
        {
            element.Element(new DynamicHost(dynamicElement));
        }
    }
}