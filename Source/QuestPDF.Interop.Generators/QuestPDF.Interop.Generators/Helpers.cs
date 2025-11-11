using System.Text.RegularExpressions;

namespace QuestPDF.Interop.Generators;

public static class Helpers
{
    private static readonly Regex SnakeCaseRegex = new("(?<!^)([A-Z])", RegexOptions.Compiled);
    
    public static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return SnakeCaseRegex.Replace(input, "_$1").ToLowerInvariant();
    }
}