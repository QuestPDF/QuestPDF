using System.Text.RegularExpressions;

namespace QuestPDF.Interop.Generators;

internal static partial class NamingConventions
{
    [GeneratedRegex(@"([a-z0-9])([A-Z])")]
    private static partial Regex LowerToUpperRegex();

    [GeneratedRegex(@"([A-Z]+)([A-Z][a-z])")]
    private static partial Regex UpperSequenceRegex();

    [GeneratedRegex(@"([a-zA-Z])(\d)")]
    private static partial Regex AlphaToDigitRegex();

    [GeneratedRegex(@"(\d)([a-zA-Z])")]
    private static partial Regex DigitToAlphaRegex();

    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = LowerToUpperRegex().Replace(input, "$1_$2");
        result = UpperSequenceRegex().Replace(result, "$1_$2");
        result = AlphaToDigitRegex().Replace(result, "$1_$2");
        result = DigitToAlphaRegex().Replace(result, "$1_$2");

        return result.ToLowerInvariant();
    }

    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLowerInvariant(input[0]) + input[1..];
    }

    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpperInvariant(input[0]) + input[1..];
    }
}
