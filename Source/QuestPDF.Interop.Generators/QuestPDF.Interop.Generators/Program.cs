using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using QuestPDF.Interop.Generators;

const string projectPath = "/Users/marcinziabek/RiderProjects/QuestPDF2/Source/QuestPDF/QuestPDF.csproj";

MSBuildLocator.RegisterDefaults();
using var workspace = MSBuildWorkspace.Create();
    
var project = await workspace.OpenProjectAsync(projectPath);
var compilation = await project.GetCompilationAsync();

PublicApiGenerator.GenerateSource(compilation!);
await TestOutputStabilityAgainstExpected();

async Task TestOutputStabilityAgainstExpected()
{
    var filesInGeneratedFolder = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "generated"), "QuestPDF.Interop.g.*");

    foreach (var actualPath in filesInGeneratedFolder)
    {
        var expectedPath = Path.Combine(AppContext.BaseDirectory, "Expected", Path.GetFileName(actualPath));
    
        var actual = await File.ReadAllTextAsync(actualPath);
        var expected = await File.ReadAllTextAsync(expectedPath);
    
        actual = string.Join("\n", actual.Split("\n").Skip(1));
        expected = string.Join("\n", expected.Split("\n").Skip(1));
    
        var identical = string.Equals(expected, actual, StringComparison.Ordinal);
        var status = identical ? "OK" : "ERROR";
    
        Console.ForegroundColor = identical ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"{Path.GetFileName(actualPath)}: {status}");
    }
}
