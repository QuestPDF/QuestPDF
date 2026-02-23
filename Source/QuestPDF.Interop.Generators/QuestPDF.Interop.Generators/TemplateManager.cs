using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Fluid;
using Fluid.Values;

namespace QuestPDF.Interop.Generators;

internal static class TemplateManager
{
    private static readonly ConcurrentDictionary<string, IFluidTemplate> TemplateCache = new();
    private static readonly FluidParser Parser;
    private static readonly TemplateOptions Options;

    static TemplateManager()
    {
        Options = new TemplateOptions();

        Options.MemberAccessStrategy = new UnsafeMemberAccessStrategy();
        Options.MemberAccessStrategy.IgnoreCasing = true;

        // Naming convention filters
        Options.Filters.AddFilter("snakeCase", (input, _, _) => new StringValue(input.ToStringValue().ToSnakeCase()));
        Options.Filters.AddFilter("camelCase", (input, _, _) => new StringValue(ToCamelCase(input.ToStringValue())));
        Options.Filters.AddFilter("pascalCase", (input, _, _) => new StringValue(ToPascalCase(input.ToStringValue())));
        Options.Filters.AddFilter("screamingSnakeCase", (input, _, _) => new StringValue(input.ToStringValue().ToSnakeCase().ToUpperInvariant()));

        var parserOptions = new FluidParserOptions { AllowFunctions = true };
        
        Parser = new FluidParser(parserOptions);
    }

    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }

    private static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpperInvariant(input[0]) + input.Substring(1);
    }

    private static IFluidTemplate LoadTemplate(string templateName)
    {
        return TemplateCache.GetOrAdd(templateName, static name =>
        {
            var templateString = LoadTemplateString(name);

            if (!Parser.TryParse(templateString, out var template, out var error))
                throw new InvalidOperationException($"Failed to parse template '{name}': {error}");

            return template;
        });
    }

    public static string RenderTemplate(string templateName, object templateModel)
    {
        var template = LoadTemplate(templateName);
        var context = new TemplateContext(templateModel, Options);

        return template.Render(context);
    }

    /// <summary>
    /// Loads the raw content of a template file, or returns an empty string if it does not exist.
    /// </summary>
    public static string TryLoadRawTemplate(string templateName)
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"QuestPDF.Interop.Generators.Templates.{templateName}.liquid");

        if (stream == null)
            return string.Empty;

        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }

    private static string LoadTemplateString(string templateName)
    {
        var content = TryLoadRawTemplate(templateName);

        if (string.IsNullOrEmpty(content))
            throw new InvalidOperationException($"Template '{templateName}' not found.");

        return content;
    }
}