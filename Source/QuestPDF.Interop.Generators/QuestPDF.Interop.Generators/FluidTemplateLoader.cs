using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Fluid;

namespace QuestPDF.Interop.Generators;

internal static class FluidTemplateLoader
{
    private static readonly ConcurrentDictionary<string, IFluidTemplate> TemplateCache = new();
    private static readonly FluidParser Parser;
    private static readonly TemplateOptions Options;

    static FluidTemplateLoader()
    {
        Options = new TemplateOptions();

        // Configure Fluid to allow accessing all object properties
        Options.MemberAccessStrategy = new UnsafeMemberAccessStrategy();
        Options.MemberAccessStrategy.IgnoreCasing = true;

        FluidFilters.RegisterFilters(Options);
        Parser = new FluidParser();
    }

    public static IFluidTemplate LoadTemplate(string templateName)
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