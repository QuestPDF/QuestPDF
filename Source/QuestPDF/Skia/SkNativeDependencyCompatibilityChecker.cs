using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal static class SkNativeDependencyCompatibilityChecker
{
    private static bool IsCompatibilityChecked = false;
        
    public static void Test()
    {
        const string exceptionBaseMessage = "The QuestPDF library has encountered an issue while loading one of its dependencies.";
        const string paragraph = "\n\n";
        
        if (IsCompatibilityChecked)
            return;
            
        // test with dotnet-based mechanism where native files are provided
        // in the "runtimes/{rid}/native" folder on Core, or by the targets file on .NET Framework
        var innerException = CheckIfExceptionIsThrownWhenLoadingNativeDependencies();

        if (innerException == null)
        {
            IsCompatibilityChecked = true;
            return;
        }

        if (!SkNativeDependencyProvider.IsCurrentPlatformSupported())
            ThrowCompatibilityException(innerException);
        
        // detect platform, copy appropriate native files and test compatibility again
        SkNativeDependencyProvider.EnsureNativeFileAvailability();
        
        innerException = CheckIfExceptionIsThrownWhenLoadingNativeDependencies();

        if (innerException == null)
        {
            IsCompatibilityChecked = true;
            return;
        }

        ThrowCompatibilityException(innerException);
        
        static void ThrowCompatibilityException(Exception innerException)
        {
            var supportedRuntimes = string.Join(", ", SkNativeDependencyProvider.SupportedPlatforms);
            var currentRuntime = SkNativeDependencyProvider.GetRuntimePlatform();
            
            var message = 
                $"{exceptionBaseMessage}{paragraph}" +
                "Your runtime is currently not supported by QuestPDF. " +
                $"Currently supported runtimes are: {supportedRuntimes}. ";

            if (SkNativeDependencyProvider.SupportedPlatforms.Contains(currentRuntime))
            {
                message += $"{paragraph}It appears that your current operating system distribution may be outdated. For optimal compatibility, please consider updating it to a more recent version.";
            }
            else
            {
                message += $"{paragraph}Your current runtime is detected as '{currentRuntime}'.";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                message += $"{paragraph}Please always set the 'Platform target' to either 'X86' or 'X64' in your startup project settings. Please do not use the 'Any CPU' option.";

            if (RuntimeInformation.ProcessArchitecture is Architecture.Arm)
                message += $"{paragraph}Please consider setting the 'Platform target' property to 'Arm64' in your project settings.";
            
            throw new Exception(message, innerException);
        }
    }
    
    private static Exception? CheckIfExceptionIsThrownWhenLoadingNativeDependencies()
    {
        try
        {
            var random = new Random();
            
            var a = random.Next();
            var b = random.Next();
        
            var expected = a + b;
            var returned = API.check_compatibility_by_calculating_sum(a, b);
        
            if (expected != returned)
                throw new Exception();

            return null;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int check_compatibility_by_calculating_sum(int a, int b);
    }
}