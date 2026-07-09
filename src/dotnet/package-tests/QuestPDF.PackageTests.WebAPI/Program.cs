using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using QuestPDF.PackageTests.Shared;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok("ready"));

app.MapGet("/generate", () =>
{
    const string resultFile = "TestOutput.zip";

    File.Delete(resultFile);
    TestRunner.Run();
    ZipFile.CreateFromDirectory(TestRunner.OutputFolder, resultFile);
    
    return Results.File(Path.GetFullPath(resultFile));
});

app.Run();