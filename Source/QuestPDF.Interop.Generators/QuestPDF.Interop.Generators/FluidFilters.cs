using System.Text;
using System.Text.Json;
using Fluid;
using Fluid.Values;

namespace QuestPDF.Interop.Generators;

public static class FluidFilters
{
    public static void RegisterFilters(TemplateOptions options)
    {
        options.Filters.AddFilter("snakeCase", (input, _, _) => new StringValue(input.ToStringValue().ToSnakeCase()));
    }
}