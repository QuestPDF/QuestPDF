using System.Diagnostics;
using System.Text;
using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.TestEngine;

public static class VeraPdfConformanceTestRunner
{
    public class ValidationResult
    {
        public bool IsDocumentValid => !FailedRules.Any();
        public ICollection<FailedRule> FailedRules { get; set; } = [];
    
        public class FailedRule
        {
            public string Profile { get; set; }
            public string Specification { get; set; }
            public string Clause { get; set; }
            public string Description { get; set; }
            public string ErrorMessage { get; set; }
            public string Context { get; set; }
        }

        public string GetErrorMessage()
        {
            if (!FailedRules.Any())
                return string.Empty;
        
            var errorMessage = new StringBuilder();
            
            foreach (var failedRule in FailedRules)
            {
                errorMessage.AppendLine($"🟥\t{failedRule.Profile}");
                errorMessage.AppendLine($"\t{failedRule.Specification}");
                errorMessage.AppendLine($"\t{failedRule.Clause}");
                errorMessage.AppendLine($"\t{failedRule.Description}");
                errorMessage.AppendLine();
                errorMessage.AppendLine($"\t{failedRule.ErrorMessage}");
                errorMessage.AppendLine();
                errorMessage.AppendLine($"\t{failedRule.Context}");
                errorMessage.AppendLine();
            }

            return errorMessage.ToString();
        }
    }
    
    public static void TestConformanceWithVeraPdf(this IDocument document)
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
        document.GeneratePdf(filePath);
        
        var result = RunVeraPDF(filePath);

        if (!result.IsDocumentValid)
        {
            Console.WriteLine(result.GetErrorMessage());
            Assert.Fail();
        }
        
        File.Delete(filePath);
    }
    
    private static ValidationResult RunVeraPDF(string pdfFilePath)
    {
        if (!File.Exists(pdfFilePath))
            throw new FileNotFoundException($"PDF file not found: {pdfFilePath}");
        
        var arguments = $"--format json \"{pdfFilePath}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "verapdf",
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

        var result = new ValidationResult();

        var profileResults = JsonDocument
            .Parse(output)
            .RootElement
            .GetProperty("report")
            .GetProperty("jobs")[0]
            .GetProperty("validationResult");
        
        foreach (var profileValidationResult in profileResults.EnumerateArray())
        {
            var failedRules = profileValidationResult
                .GetProperty("details")
                .GetProperty("ruleSummaries");

            foreach (var failedRule in failedRules.EnumerateArray())
            {
                foreach (var check in failedRule.GetProperty("checks").EnumerateArray())
                {
                    result.FailedRules.Add(new ValidationResult.FailedRule
                    {
                        Profile = profileValidationResult.GetProperty("profileName").GetString().Split(" ").First(),
                        Specification = failedRule.GetProperty("specification").GetString(),
                        Clause = failedRule.GetProperty("clause").GetString(),
                        Description = failedRule.GetProperty("description").GetString(),
                        ErrorMessage = check.GetProperty("errorMessage").GetString(),
                        Context = check.GetProperty("context").GetString()
                    });
                }
            }
        }

        return result;
    }
}