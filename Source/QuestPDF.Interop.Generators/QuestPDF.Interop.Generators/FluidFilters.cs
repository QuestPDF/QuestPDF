using System.Text;
using Fluid;
using Fluid.Values;

namespace QuestPDF.Interop.Generators;

public static class FluidFilters
{
    public static void RegisterSnakeCaseFilter(TemplateOptions options)
    {
        options.Filters.AddFilter("snake_case", (input, arguments, context) =>
        {
            var stringValue = input.ToStringValue();
            return new StringValue(ToSnakeCase(stringValue));
        });
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var result = new StringBuilder();
        result.Append(char.ToLowerInvariant(value[0]));

        for (int i = 1; i < value.Length; i++)
        {
            var currentChar = value[i];

            if (char.IsUpper(currentChar))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(currentChar));
            }
            else
            {
                result.Append(currentChar);
            }
        }

        return result.ToString();
    }
}