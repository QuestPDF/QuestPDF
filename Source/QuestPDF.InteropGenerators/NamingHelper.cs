namespace QuestPDF.InteropGenerators;

/// <summary>
/// Helper class for naming convention conversions
/// </summary>
public static class NamingHelper
{
    /// <summary>
    /// Converts PascalCase to SCREAMING_SNAKE_CASE
    /// </summary>
    /// <param name="pascalCase">String in PascalCase format</param>
    /// <returns>String in SCREAMING_SNAKE_CASE format</returns>
    public static string ToSnakeCase(string pascalCase)
    {
        if (string.IsNullOrEmpty(pascalCase))
            return pascalCase;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToUpperInvariant(pascalCase[0]));

        for (int i = 1; i < pascalCase.Length; i++)
        {
            if (char.IsUpper(pascalCase[i]))
            {
                result.Append('_');
                result.Append(pascalCase[i]);
            }
            else
            {
                result.Append(char.ToUpperInvariant(pascalCase[i]));
            }
        }

        return result.ToString();
    }
}