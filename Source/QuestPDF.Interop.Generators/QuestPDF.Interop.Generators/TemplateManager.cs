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
        Options.Filters.AddFilter("snakeCase", (input, _, _) => new StringValue(input.ToStringValue().ToSnakeCase()));

        Parser = new FluidParser();
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

    private static string LoadTemplateString(string templateName)
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"QuestPDF.Interop.Generators.Templates.{templateName}.liquid");

        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }
}