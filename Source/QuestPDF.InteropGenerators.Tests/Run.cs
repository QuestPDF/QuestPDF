using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

// TODO: point to your generator type:
using QuestPDF.InteropGenerators; // e.g., using QuestPDF.InteropGenerators;

internal static class Program
{
    // Adjust these values if needed:
    private static readonly string ProjectName  = "QuestPDF"; // target project in the solution
    private static readonly string OutputDir    = Path.GetFullPath("./_generated");
    private const string Configuration = "Debug"; // or "Release"
    // Optional: if you need a specific TFM, set: ["TargetFramework"] = "net8.0"

    public static async Task Main()
    {
        if (!MSBuildLocator.IsRegistered)
            MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create(new Dictionary<string, string>
        {
            ["Configuration"] = Configuration,
            ["TargetFramework"] = "net8.0"
        });

        workspace.WorkspaceFailed += (s, e) => Console.WriteLine($"[WorkspaceFailed] {e.Diagnostic.Kind}: {e.Diagnostic.Message}");

        var solutionPath = FindSolutionPath();
        Console.WriteLine($"Using solution: {solutionPath}");
        var solution = await workspace.OpenSolutionAsync(solutionPath);

        // Try by name first (only loaded projects are present)
        var project = solution.Projects.FirstOrDefault(p => p.Name == ProjectName);

        // Fallback: open the csproj directly (e.g., if the project failed to auto-load in the solution)
        if (project is null)
        {
            var solutionDir = Path.GetDirectoryName(solutionPath)!;
            var csprojPath = Path.Combine(solutionDir, ProjectName, $"{ProjectName}.csproj");
            if (File.Exists(csprojPath))
            {
                Console.WriteLine($"Project '{ProjectName}' not loaded from solution; opening directly: {csprojPath}");
                project = await workspace.OpenProjectAsync(csprojPath);
            }
        }

        if (project is null)
            throw new InvalidOperationException($"Project '{ProjectName}' not found or failed to load.");

        var compilation = (CSharpCompilation)(await project.GetCompilationAsync()
                         ?? throw new Exception("Compilation failed (null)."));

        // Create your incremental generator instance here:
        var generator = new PublicApiGenerator(); // <--- replace with your IIncrementalGenerator

        var parseOptions = (CSharpParseOptions)project.ParseOptions!;
        var additionalTexts = project.AdditionalDocuments
            .Select(d => (AdditionalText)new AdditionalTextDocumentAdapter(d));

        var driver = CSharpGeneratorDriver.Create(
            generators: new ISourceGenerator[] { generator.AsSourceGenerator() },
            additionalTexts: additionalTexts,
            parseOptions: parseOptions,
            optionsProvider: project.AnalyzerOptions.AnalyzerConfigOptionsProvider
        );

        var driver2 = driver.RunGenerators(compilation);
        var runResult = driver2.GetRunResult();

        Directory.CreateDirectory(OutputDir);

        foreach (var diag in runResult.Diagnostics)
            Console.WriteLine(diag.ToString());

        foreach (var result in runResult.Results)
        {
            Console.WriteLine($"Generator: {result.Generator.GetType().Name}");

            foreach (var gen in result.GeneratedSources)
            {
                var hintName = gen.HintName;
                var sourceText = gen.SourceText;
                var file = Path.Combine(OutputDir, Sanitize(hintName));
                await File.WriteAllTextAsync(file, sourceText.ToString());
                Console.WriteLine($"  Emitted: {file}");
            }

            foreach (var d in result.Diagnostics)
                Console.WriteLine($"  {d}");
        }

        Console.WriteLine("Done.");
    }

    private static string Sanitize(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }

    private static string FindSolutionPath()
    {
        // 1) Try upward search from current directory
        string? dir = Directory.GetCurrentDirectory();
        var tried = new List<string>();
        for (int i = 0; i < 10 && !string.IsNullOrEmpty(dir); i++)
        {
            var candidate = Path.Combine(dir, "QuestPDF.sln");
            tried.Add(candidate);
            if (File.Exists(candidate))
                return candidate;
            dir = Path.GetDirectoryName(dir);
        }

        // 2) Try from assembly base directory
        dir = AppContext.BaseDirectory;
        for (int i = 0; i < 10 && !string.IsNullOrEmpty(dir); i++)
        {
            var candidate = Path.Combine(dir, "QuestPDF.sln");
            tried.Add(candidate);
            if (File.Exists(candidate))
                return candidate;
            dir = Path.GetDirectoryName(dir);
        }

        // 3) Try known relative from this source file location (developer environment)
        // This file lives at: Source/QuestPDF.InteropGenerators.Tests/Run.cs
        // Solution resides at: Source/QuestPDF.sln
        var sourceRepoRootGuess = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, ".."));
        var candidate3 = Path.Combine(sourceRepoRootGuess, "QuestPDF.sln");
        tried.Add(candidate3);
        if (File.Exists(candidate3))
            return candidate3;

        throw new FileNotFoundException("Cannot locate 'QuestPDF.sln'. Tried:\n" + string.Join("\n", tried));
    }
}

// Tiny adapter so AdditionalFiles work exactly like in a real build.
internal sealed class AdditionalTextDocumentAdapter : AdditionalText
{
    private readonly TextDocument _doc;
    public AdditionalTextDocumentAdapter(TextDocument doc) => _doc = doc;

    public override string Path => _doc.FilePath ?? _doc.Name;

    public override Microsoft.CodeAnalysis.Text.SourceText GetText(
        System.Threading.CancellationToken cancellationToken = default) =>
        _doc.GetTextAsync(cancellationToken).GetAwaiter().GetResult();
}
