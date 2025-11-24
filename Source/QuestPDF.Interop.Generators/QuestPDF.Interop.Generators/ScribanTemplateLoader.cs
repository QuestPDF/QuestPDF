using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Scriban;
using Scriban.Runtime;

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
    
    public static string RenderTemplate(string templateName, object templateModel)
    {
        var template = LoadTemplate(templateName);

        var context = new TemplateContext();
        var globals = new ScriptObject();

        globals.Import(typeof(ScribanFunctions));
        globals.Import(templateModel);
        
        context.PushGlobal(globals);
        
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