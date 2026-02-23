namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Provides access to all supported target language implementations.
/// </summary>
public static class LanguageProviderRegistry
{
    public static readonly ILanguageProvider Python = new PythonLanguageProvider();
    public static readonly ILanguageProvider TypeScript = new TypeScriptLanguageProvider();
    public static readonly ILanguageProvider Kotlin = new KotlinLanguageProvider();
}
