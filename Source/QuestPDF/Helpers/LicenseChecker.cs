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
            "[QuestPDF] The library is running in Evaluation Mode. " +
            "This mode is fully functional and intended only for product evaluation and internal testing. " +
            "Commercial and production use requires an appropriate license. " +
            "For licensing details and pricing, please visit: https://www.questpdf.com/license";

        try
        {
            if (TraceHasListeners())
                Trace.TraceWarning(warningMessage);
            
            else
                Console.WriteLine($"\n{warningMessage}\n");
        }
        catch
        {
            
        }
    }

    private static bool TraceHasListeners()
    {
        if (Trace.Listeners.Count == 0)
            return false;
        
        if (Trace.Listeners.Count == 1 && Trace.Listeners[0] is DefaultTraceListener)
            return false;
        
        return true;
    }
    
    private static void ThrowExceptionWithWelcomeMessage()
    {
        const string newParagraph = "\n\n";

        var exceptionMessage = 
            $"{newParagraph}{newParagraph}Thank you for choosing QuestPDF 👋{newParagraph}" +
            $"Before you continue, please take a moment to configure your license. This step helps ensure correct license compliance.{newParagraph}" +
            $"QuestPDF requires a Commercial License for production use by organizations with more than $1M USD in annual gross revenue. " +
            $"Individuals, non-profits, open-source projects, and smaller companies qualify for the free Community license.{newParagraph}" +
            $"If you are not the decision-maker for software purchases, please share the licensing and pricing details with your team lead or manager: https://www.questpdf.com/license {newParagraph}" +
            $"Available license options:\n" +
            $"- Community: free,\n" +
            $"- Evaluation: intended solely for evaluation before choosing an appropriate license; not suitable for production use,\n" +
            $"- Professional: paid, for teams up to 10 developers with dedicated support,\n" +
            $"- Enterprise: paid, for unlimited developers with prioritized dedicated support.{newParagraph}" +
            $"Set the license once at application startup. By doing so, you confirm that the selected tier matches your usage:\n" +
            $"> QuestPDF.Settings.License = LicenseType.Evaluation; // or Community / Professional / Enterprise{newParagraph}" +
            $"No license key or activation is required — we trust you to select the correct option. " +
            $"By choosing the right license, you help ensure QuestPDF remains sustainable and continuously improving for everyone. {newParagraph}" +
            $"We wish you a great experience! 🚀{newParagraph}{newParagraph}";

        throw new Exception(exceptionMessage)
        {
            HelpLink = "https://www.questpdf.com/license"
        };
    }
}