using System.Collections.Generic;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Registry for language providers. Provides centralized access to all supported
/// target language implementations.
/// </summary>
public static class LanguageProviderRegistry
{
    private static readonly Dictionary<string, ILanguageProvider> Providers = new()
    {
        ["Python"] = new PythonLanguageProvider(),
        ["TypeScript"] = new TypeScriptLanguageProvider(),
        ["Kotlin"] = new KotlinLanguageProvider()
    };

    /// <summary>
    /// Gets the Python language provider.
    /// </summary>
    public static ILanguageProvider Python => Providers["Python"];

    /// <summary>
    /// Gets the Java language provider.
    /// </summary>
    public static ILanguageProvider Java => Providers["Java"];

    /// <summary>
    /// Gets the TypeScript language provider.
    /// </summary>
    public static ILanguageProvider TypeScript => Providers["TypeScript"];

    /// <summary>
    /// Gets the Kotlin language provider.
    /// </summary>
    public static ILanguageProvider Kotlin => Providers["Kotlin"];

    /// <summary>
    /// Gets a language provider by name.
    /// </summary>
    public static ILanguageProvider GetProvider(string languageName)
    {
        return Providers.TryGetValue(languageName, out var provider)
            ? provider
            : throw new KeyNotFoundException($"No language provider found for '{languageName}'");
    }

    /// <summary>
    /// Gets all registered language providers.
    /// </summary>
    public static IEnumerable<ILanguageProvider> All => Providers.Values;
}
