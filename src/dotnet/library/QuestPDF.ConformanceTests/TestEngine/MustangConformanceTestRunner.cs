using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.TestEngine;

public static class MustangConformanceTestRunner
{
    public class ValidationResult
    {
        public bool IsDocumentValid => !FailedRules.Any();
        public ICollection<string> FailedRules { get; set; } = [];
        
        public string GetErrorMessage()
        {
            var errorMessage = new StringBuilder();

            foreach (var failedRule in FailedRules)
            {
                errorMessage.AppendLine($"🟥\tError");
                errorMessage.AppendLine($"\t{failedRule}");
                errorMessage.AppendLine();
            }

            return errorMessage.ToString();
        }
    }

    public static void TestConformance(string filePath)
    {
        var result = RunMustang(filePath);

        if (!result.IsDocumentValid)
        {
            Console.WriteLine(result.GetErrorMessage());
            Assert.Fail();
        }
    }

    private static ValidationResult RunMustang(string pdfFilePath)
    {
        if (!File.Exists(pdfFilePath))
            throw new FileNotFoundException($"PDF file not found: {pdfFilePath}");

        var mustangExecutablePath = Environment.GetEnvironmentVariable("MUSTANG_EXECUTABLE_PATH");
        
        if (string.IsNullOrEmpty(mustangExecutablePath))
            throw new Exception("The location path of the Mustang executable is not set. Set the MUSTANG_EXECUTABLE_PATH environment variable to the path of the Mustang executable.");
        
        var arguments = $"-jar {mustangExecutablePath} --action validate --source {pdfFilePath}";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return new ValidationResult()
        {
            FailedRules = XDocument
                .Parse(output)
                .Descendants("error")
                .Select(x => x.Value)
                .ToList()
        };
    }
}