using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Scriban;

namespace QuestPDF.Interop.Generators;

internal static class ScribanTemplateLoader
{
    private static readonly ConcurrentDictionary<string, Scriban.Template> TemplateCache = new();
    
    public static Template LoadTemplate(string templateName)
    {
        return TemplateCache.GetOrAdd(templateName, static name =>
        {
            var templateString = LoadTemplateString(name);
            return Template.Parse(templateString);
        });
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