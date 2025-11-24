using Scriban.Runtime;

namespace QuestPDF.Interop.Generators;

public class ScribanFunctions
{
    public static string SnakeCase(string value)
    {
        return value.ToSnakeCase();
    }
}