using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using QuestPDF.Interop.Generators;

const string projectPath = "/Users/marcinziabek/RiderProjects/QuestPDF2/Source/QuestPDF/QuestPDF.csproj";

MSBuildLocator.RegisterDefaults();
using var workspace = MSBuildWorkspace.Create();
    
// Load your project
var project = await workspace.OpenProjectAsync(projectPath);
var compilation = await project.GetCompilationAsync();

PublicApiGenerator.GenerateSource(compilation!);