#:package Microsoft.CodeAnalysis.CSharp@5.6.0

using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

if (args.Length < 2)
{
    Console.WriteLine("Usage: dotnet run export-documentation-examples.cs -- <examples project folder> <docs folder> [--images]");
    return 1;
}

var examplesFolder = Path.GetFullPath(args[0]);
var docsFolder = Path.GetFullPath(args[1]);
var snippetsFolder = Path.Combine(docsFolder, "snippets");
var updateImages = args.Contains("--images");

var snippets = Directory
    .EnumerateFiles(examplesFolder, "*.cs", SearchOption.AllDirectories)
    .Where(path => !IsBuildOutput(path))
    .SelectMany(FindSnippets)
    .ToList();

var duplicates = snippets
    .GroupBy(snippet => snippet.Name)
    .Where(group => group.Count() > 1)
    .ToList();

foreach (var duplicate in duplicates)
{
    var sources = string.Join(", ", duplicate.Select(snippet => snippet.Source));
    Console.WriteLine($"error: snippet '{duplicate.Key}' is defined more than once: {sources}");
}

if (duplicates.Any())
    return 1;

foreach (var snippet in snippets)
{
    var path = Path.Combine(snippetsFolder, snippet.Name + ".cs");
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    File.WriteAllText(path, snippet.Code + "\n");
}

Console.WriteLine($"Exported {snippets.Count} snippets to {snippetsFolder}");

if (updateImages)
    UpdateImages();

return 0;

IEnumerable<Snippet> FindSnippets(string path)
{
    var tests = CSharpSyntaxTree
        .ParseText(File.ReadAllText(path))
        .GetRoot()
        .DescendantNodes()
        .OfType<MethodDeclarationSyntax>()
        .Where(IsTest);

    foreach (var test in tests)
    {
        var source = $"{Path.GetFileName(path)}:{test.Identifier.Text}";
        var markedSnippets = FindMarkedSnippets(test, source).ToList();

        if (markedSnippets.Any())
        {
            foreach (var snippet in markedSnippets)
                yield return snippet;

            continue;
        }

        var pageContentSnippet = FindPageContentSnippet(test, source);

        if (pageContentSnippet is null)
        {
            Console.WriteLine($"warning: {source} has neither a '// docs-export' comment nor a single page.Content() call; skipped");
            continue;
        }

        yield return pageContentSnippet;
    }
}

IEnumerable<Snippet> FindMarkedSnippets(MethodDeclarationSyntax test, string source)
{
    foreach (var statement in test.DescendantNodes().OfType<StatementSyntax>())
    {
        var exportName = FindExportName(statement);

        if (exportName is null)
            continue;

        var name = exportName == "" ? DefaultName(test) : $"{GroupName(test)}/{exportName}";
        var code = Dedent(statement.ToString(), Column(statement));

        yield return new Snippet(name, source, code);
    }
}

Snippet? FindPageContentSnippet(MethodDeclarationSyntax test, string source)
{
    var pageContents = test
        .DescendantNodes()
        .OfType<InvocationExpressionSyntax>()
        .Where(IsPageContent)
        .ToList();

    if (pageContents.Count != 1)
        return null;

    var pageContent = pageContents.Single();
    var statement = pageContent.Ancestors().OfType<StatementSyntax>().First();
    var chain = test.SyntaxTree.GetText().ToString(TextSpanBetween(pageContent, statement));

    return new Snippet(DefaultName(test), source, "container" + Dedent(chain, Column(statement)));
}

void UpdateImages()
{
    var testsStarted = DateTime.UtcNow;
    var test = Process.Start("dotnet", ["test", examplesFolder]);
    test.WaitForExit();

    if (test.ExitCode != 0)
    {
        Console.WriteLine("error: the examples failed; no images were updated");
        return;
    }

    var docsImages = Directory
        .EnumerateFiles(Path.Combine(docsFolder, "public"), "*", SearchOption.AllDirectories)
        .ToLookup(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);

    var generatedImages = Directory
        .EnumerateFiles(Path.Combine(examplesFolder, "bin"), "*.*", SearchOption.AllDirectories)
        .Where(path => IsImage(path) && File.GetLastWriteTimeUtc(path) >= testsStarted);

    var updated = 0;

    foreach (var generatedImage in generatedImages)
    {
        foreach (var docsImage in docsImages[Path.GetFileName(generatedImage)])
        {
            if (File.ReadAllBytes(docsImage).SequenceEqual(File.ReadAllBytes(generatedImage)))
                continue;

            File.Copy(generatedImage, docsImage, overwrite: true);
            Console.WriteLine($"Updated {Path.GetRelativePath(docsFolder, docsImage)}");
            updated++;
        }
    }

    Console.WriteLine($"Updated {updated} images");
}

static bool IsTest(MethodDeclarationSyntax method)
{
    return method.AttributeLists
        .SelectMany(list => list.Attributes)
        .Any(attribute => attribute.Name.ToString() == "Test");
}

static bool IsPageContent(InvocationExpressionSyntax invocation)
{
    return invocation.ArgumentList.Arguments.Count == 0
        && invocation.Expression is MemberAccessExpressionSyntax
        {
            Name.Identifier.Text: "Content",
            Expression: IdentifierNameSyntax { Identifier.Text: "page" }
        };
}

static string? FindExportName(StatementSyntax statement)
{
    var marker = statement
        .GetLeadingTrivia()
        .Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
        .Select(trivia => Regex.Match(trivia.ToString(), @"^//\s*docs-export(?::\s*(?<name>[\w/-]+))?\s*$"))
        .FirstOrDefault(match => match.Success);

    return marker?.Groups["name"].Value;
}

static string GroupName(MethodDeclarationSyntax test)
{
    var className = test.Ancestors().OfType<ClassDeclarationSyntax>().First().Identifier.Text;
    return ToKebabCase(Regex.Replace(className, "Examples?$", ""));
}

static string DefaultName(MethodDeclarationSyntax test)
{
    return $"{GroupName(test)}/{ToKebabCase(test.Identifier.Text)}";
}

static TextSpan TextSpanBetween(SyntaxNode start, SyntaxNode end)
{
    return TextSpan.FromBounds(start.Span.End, end.Span.End);
}

static int Column(SyntaxNode node)
{
    return node.GetLocation().GetLineSpan().StartLinePosition.Character;
}

static string Dedent(string code, int column)
{
    var lines = code
        .Replace("\r\n", "\n")
        .Split('\n')
        .Select(line => RemoveIndentation(line, column));

    return string.Join("\n", lines).TrimEnd();
}

static string RemoveIndentation(string line, int column)
{
    var hasIndentation = line.Length >= column && string.IsNullOrWhiteSpace(line[..column]);
    return hasIndentation ? line[column..] : line.TrimStart();
}

static string ToKebabCase(string name)
{
    return Regex.Replace(name, "(?<=[a-z0-9])(?=[A-Z])", "-").ToLowerInvariant();
}

static bool IsBuildOutput(string path)
{
    var separator = Path.DirectorySeparatorChar;
    return path.Contains($"{separator}bin{separator}") || path.Contains($"{separator}obj{separator}");
}

static bool IsImage(string path)
{
    return Path.GetExtension(path).ToLowerInvariant() is ".webp" or ".png" or ".jpg" or ".jpeg" or ".svg";
}

record Snippet(string Name, string Source, string Code);
