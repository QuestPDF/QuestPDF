// Updates the documentation from QuestPDF.DocumentationExamples:
// 1. Code marked with `#region docs:<name>` is exported to <docs>/snippets/<name>.cs,
//    so documentation pages can include it with `<<< @/snippets/<name>.cs`.
//    `#region docs:<name> container` starts the snippet with `container`, for chains that begin with a method call.
// 2. The tests are run, and every image on a page that includes snippets is replaced with the freshly generated one.
// Usage: dotnet run export-documentation-snippets.cs -- <path to the 'docs' folder of QuestPDF-Documentation>

using System.Diagnostics;
using System.Text.RegularExpressions;

if (args.Length != 1 || !Directory.Exists(args[0]))
{
    Console.Error.WriteLine("Usage: dotnet run export-documentation-snippets.cs -- <path to the 'docs' folder of QuestPDF-Documentation>");
    return 1;
}

var scriptDirectory = (string)AppContext.GetData("EntryPointFileDirectoryPath")!;
var examplesDirectory = Path.GetFullPath(Path.Combine(scriptDirectory, "..", "QuestPDF.DocumentationExamples"));
var docsDirectory = Path.GetFullPath(args[0]);
var snippetsDirectory = Path.Combine(docsDirectory, "snippets");
var publicDirectory = Path.Combine(docsDirectory, "public");

var errors = new List<string>();
var snippets = new Dictionary<string, string>();
var outputFiles = new Dictionary<string, string>();
var outputFile = new Regex(@"Generate(?:Images|Svg)\(\s*(?:\w+\s*=>\s*)?\$?""(?<name>[^""]+)""");

foreach (var file in Directory.EnumerateFiles(examplesDirectory, "*.cs", SearchOption.AllDirectories).Order())
{
    if (IsBuildOutput(file))
        continue;

    foreach (var snippet in FindSnippets(file))
    {
        if (!snippets.TryAdd(snippet.Name, snippet.Code))
            errors.Add($"Snippet '{snippet.Name}' is defined more than once (again in {Relative(examplesDirectory, file)}).");
    }

    foreach (Match match in outputFile.Matches(File.ReadAllText(file)))
    {
        var name = match.Groups["name"].Value;

        if (!outputFiles.TryAdd(name, Relative(examplesDirectory, file)))
            errors.Add($"Two examples write the same file '{name}' ({outputFiles[name]} and {Relative(examplesDirectory, file)}), so the result depends on the test order.");
    }
}

if (errors.Count == 0)
{
    if (Directory.Exists(snippetsDirectory))
        Directory.Delete(snippetsDirectory, recursive: true);

    foreach (var (name, code) in snippets)
    {
        var path = SnippetPath(name);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, code);
    }

    Console.WriteLine($"Exported {snippets.Count} snippet(s) to {snippetsDirectory}");

    var pagesWithSnippets = CheckDocumentationReferences();
    UpdateImages(pagesWithSnippets);
}

foreach (var error in errors)
    Console.Error.WriteLine($"error: {error}");

return errors.Count == 0 ? 0 : 1;

IEnumerable<(string Name, string Code)> FindSnippets(string file)
{
    var lines = File.ReadAllLines(file);
    var anySnippetStart = new Regex(@"^\s*#region\s+docs:");
    var snippetStart = new Regex(@"^\s*#region\s+docs:(?<name>[a-z0-9\-/]+)(\s+(?<receiver>[A-Za-z_]\w*))?\s*$");
    var regionStart = new Regex(@"^\s*#region\b");
    var regionEnd = new Regex(@"^\s*#endregion\b");

    for (var i = 0; i < lines.Length; i++)
    {
        if (!anySnippetStart.IsMatch(lines[i]))
            continue;

        var match = snippetStart.Match(lines[i]);

        if (!match.Success)
        {
            errors.Add($"{Relative(examplesDirectory, file)}:{i + 1}: expected '#region docs:<name>' or '#region docs:<name> <receiver>', with a lowercase name made of letters, digits, '-' and '/'.");
            continue;
        }

        var name = match.Groups["name"].Value;
        var receiver = match.Groups["receiver"].Value;
        var body = new List<string>();
        var depth = 1;
        var end = i + 1;

        for (; end < lines.Length; end++)
        {
            if (regionStart.IsMatch(lines[end]))
                depth++;
            else if (regionEnd.IsMatch(lines[end]))
                depth--;
            else
                body.Add(lines[end]);

            if (depth == 0)
                break;
        }

        if (depth != 0)
        {
            errors.Add($"Snippet '{name}' in {Relative(examplesDirectory, file)} has no matching #endregion.");
            yield break;
        }

        var code = Dedent(body).ToList();

        if (code.Count == 0)
        {
            errors.Add($"Snippet '{name}' in {Relative(examplesDirectory, file)} is empty.");
            continue;
        }

        if (receiver != "")
        {
            if (!code[0].StartsWith('.'))
            {
                errors.Add($"Snippet '{name}' in {Relative(examplesDirectory, file)} names the receiver '{receiver}', but its first line doesn't start with a method call.");
                continue;
            }

            code = code.Select(line => line == "" ? "" : "    " + line).Prepend(receiver).ToList();
        }

        yield return (name, string.Join("\n", code) + "\n");
    }
}

IEnumerable<string> Dedent(List<string> lines)
{
    while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0]))
        lines.RemoveAt(0);

    while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1]))
        lines.RemoveAt(lines.Count - 1);

    var indent = lines
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => line.Length - line.TrimStart().Length)
        .DefaultIfEmpty(0)
        .Min();

    return lines.Select(line => string.IsNullOrWhiteSpace(line) ? "" : line[indent..].TrimEnd());
}

List<string> CheckDocumentationReferences()
{
    var include = new Regex(@"^\s*<<<\s+(?<path>[^\s{#\[]+)", RegexOptions.Multiline);
    var usedSnippets = new HashSet<string>();
    var pagesWithSnippets = new List<string>();

    foreach (var page in Directory.EnumerateFiles(docsDirectory, "*.md", SearchOption.AllDirectories).Order())
    {
        if (IsBuildOutput(page))
            continue;

        var includesSnippet = false;

        foreach (Match match in include.Matches(File.ReadAllText(page)))
        {
            var target = match.Groups["path"].Value;

            var path = target.StartsWith("@/")
                ? Path.GetFullPath(Path.Combine(docsDirectory, target[2..]))
                : Path.GetFullPath(Path.Combine(Path.GetDirectoryName(page)!, target));

            if (!File.Exists(path))
            {
                errors.Add($"{Relative(docsDirectory, page)} includes '{target}', which does not exist.");
                continue;
            }

            usedSnippets.Add(path);
            includesSnippet |= path.StartsWith(snippetsDirectory + Path.DirectorySeparatorChar);
        }

        if (includesSnippet)
            pagesWithSnippets.Add(page);
    }

    foreach (var name in snippets.Keys.Where(name => !usedSnippets.Contains(SnippetPath(name))))
        Console.WriteLine($"warning: snippet '{name}' is not used by any documentation page.");

    return pagesWithSnippets;
}

void UpdateImages(List<string> pages)
{
    var testsStarted = DateTime.UtcNow;
    Console.WriteLine("Running the documentation examples to generate images...");

    var tests = Process.Start(new ProcessStartInfo("dotnet", ["test", examplesDirectory, "--nologo"])
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true
    })!;

    var errorOutput = tests.StandardError.ReadToEndAsync();
    var output = tests.StandardOutput.ReadToEnd();
    tests.WaitForExit();

    if (tests.ExitCode != 0)
    {
        Console.Error.WriteLine(output + errorOutput.Result);
        errors.Add("The documentation examples failed, so no images were updated.");
        return;
    }

    var generatedImages = Directory
        .EnumerateFiles(Path.Combine(examplesDirectory, "bin"), "*.*", SearchOption.AllDirectories)
        .Where(path => File.GetLastWriteTimeUtc(path) >= testsStarted)
        .GroupBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
        .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.OrdinalIgnoreCase);

    var image = new Regex(@"!\[[^\]]*\]\((?<path>/[^)\s]+\.(webp|png|jpg|jpeg|svg))\)", RegexOptions.IgnoreCase);
    var updated = 0;

    foreach (var page in pages)
    {
        foreach (Match match in image.Matches(File.ReadAllText(page)))
        {
            var target = match.Groups["path"].Value;
            var documentationImage = Path.GetFullPath(Path.Combine(publicDirectory, target.TrimStart('/')));
            var fileName = Path.GetFileName(documentationImage);

            if (!generatedImages.TryGetValue(fileName, out var candidates))
            {
                Console.WriteLine($"warning: {Relative(docsDirectory, page)} shows '{target}', which no test generates; left unchanged.");
                continue;
            }

            var contents = candidates.Select(File.ReadAllBytes).ToList();

            if (contents.Skip(1).Any(other => !other.AsSpan().SequenceEqual(contents[0])))
            {
                errors.Add($"Several tests generate different images named '{fileName}': {string.Join(", ", candidates.Select(c => Relative(examplesDirectory, c)))}.");
                continue;
            }

            if (File.Exists(documentationImage) && File.ReadAllBytes(documentationImage).AsSpan().SequenceEqual(contents[0]))
                continue;

            Directory.CreateDirectory(Path.GetDirectoryName(documentationImage)!);
            File.WriteAllBytes(documentationImage, contents[0]);
            Console.WriteLine($"Updated image {target}");
            updated++;
        }
    }

    Console.WriteLine($"Updated {updated} image(s) on {pages.Count} page(s) that include snippets.");
}

string SnippetPath(string name) => Path.GetFullPath(Path.Combine(snippetsDirectory, name + ".cs"));

string Relative(string root, string path) => Path.GetRelativePath(root, path).Replace('\\', '/');

bool IsBuildOutput(string path)
{
    var parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    return parts.Any(part => part is "bin" or "obj" or ".vitepress" or "node_modules");
}
