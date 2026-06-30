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
            $"{newParagraph}{newParagraph}Welcome to QuestPDF, and thank you for choosing it 👋{newParagraph}" +
            $"Before you generate your first document, please configure the license. It is a single line of code at application startup, and it confirms which license tier applies to your usage:{newParagraph}" +
            $"> QuestPDF.Settings.License = LicenseType.Evaluation; // or Community / Professional / Enterprise{newParagraph}" +
            $"QuestPDF requires a commercial license for production use by organizations with more than $1M USD in annual gross revenue. " +
            $"Individuals, non-profits, open-source projects, and smaller companies qualify for the free Community license.{newParagraph}" +
            $"Available tiers:\n" +
            $"- Community: free,\n" +
            $"- Professional: paid, for teams of up to 10 developers, with dedicated support,\n" +
            $"- Enterprise: paid, for unlimited developers, with prioritized dedicated support,\n" +
            $"- Evaluation: for assessing the library before selecting a license; not permitted in production.{newParagraph}" +
            $"QuestPDF License and Pricing: https://www.questpdf.com/pricing{newParagraph}" +
            $"If you are not the decision-maker for software purchases, please forward the license and pricing pages to your team lead or manager.{newParagraph}" +
            $"No license key or activation is required — the setting is a self-declaration, and we trust you to choose correctly. " +
            $"By selecting the appropriate tier, you help keep QuestPDF sustainable and continuously improving for everyone.{newParagraph}" +
            $"We wish you a great experience! 🚀{newParagraph}{newParagraph}";

        throw new Exception(exceptionMessage)
        {
            HelpLink = "https://www.questpdf.com/license"
        };
    }
}