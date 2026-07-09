using System;
using System.Diagnostics;
using QuestPDF.Infrastructure;

namespace QuestPDF.Helpers;

static class LicenseChecker
{
    private static bool IsLicenseValidated { get; set; } = false; 
    
    public static void ValidateLicense()
    {
        if (IsLicenseValidated)
            return;
        
        if (Settings.License == LicenseType.Evaluation)
            PrintLicenseEvaluationWarning();
        
        if (Settings.License is null)
            ThrowExceptionWithWelcomeMessage();
        
        IsLicenseValidated = true;
    }

    private static void PrintLicenseEvaluationWarning()
    {
        var warningMessage =
            "[QuestPDF] The QuestPDF library is running with the Evaluation license. " +
            "This mode is fully functional and intended only for evaluating the library before choosing a license. " +
            "It must not be used in production. " +
            "Pricing: https://www.questpdf.com/pricing | License terms: https://www.questpdf.com/license";

        Trace.TraceWarning(warningMessage);
    }
    
    private static void ThrowExceptionWithWelcomeMessage()
    {
        const string newParagraph = "\n\n";

        var exceptionMessage =
            $"Please configure the QuestPDF license by setting 'QuestPDF.Settings.License' at application startup.{newParagraph}{newParagraph}" +
            $"Welcome to QuestPDF, and thank you for choosing it 👋{newParagraph}" +
            $"Before you generate your first document, please declare which license tier applies to you. It is a single line of code at application startup:{newParagraph}" +
            $"> QuestPDF.Settings.License = LicenseType.Evaluation; // or Community / Professional / Enterprise{newParagraph}" +
            $"Available license tiers:\n" +
            $"- Evaluation: start here; take your time to assess the library before selecting a tier; not permitted in production,\n" +
            $"- Community: free; for individuals, non-profits, open-source projects, and organizations with annual gross revenue under $1M USD,\n" +
            $"- Professional: paid; covers your entire company, with unlimited developers,\n" +
            $"- Enterprise: paid; additionally covers subsidiaries and affiliated companies, includes prioritized support, and off-schedule software updates for critical issues.{newParagraph}" +
            $"Every tier includes the complete feature set; tiers differ only in usage rights and support.{newParagraph}" +
            $"Visit https://www.questpdf.com/pricing for a human-friendly overview of licensing and pricing.{newParagraph}" +
            $"Visit https://www.questpdf.com/license to access all legal documents.{newParagraph}" +
            $"No license key or activation is required: the setting is a self-declaration, and we simply trust you. By selecting the tier that matches your situation, you help keep QuestPDF sustainable and continuously improving for everyone.{newParagraph}" +
            $"Evaluating at work? You can start right away; that is exactly what the Evaluation tier is for, and the purchasing decision does not need to be yours. When your team is ready, simply forward the pricing page to your team lead or manager.{newParagraph}" +
            $"We wish you a great experience! 🚀{newParagraph}{newParagraph}";

        throw new Exception(exceptionMessage)
        {
            HelpLink = "https://www.questpdf.com/license"
        };
    }
}