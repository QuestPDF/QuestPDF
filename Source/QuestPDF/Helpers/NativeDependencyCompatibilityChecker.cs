using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace QuestPDF.Helpers
{
    internal sealed class NativeDependencyCompatibilityChecker
    {
        private static readonly Version RequiredGlibcVersionOnLinux = Version.Parse("2.29");
        
        private bool IsCompatibilityChecked { get; set; } = false;

        public Action ExecuteNativeCode { get; set; } = () => { };
        public Func<bool> CheckNativeLibraryVersion { get; set; } = () => true;
        public Func<string> ExceptionHint { get; set; } = () => string.Empty;
        
        public void Test()
        {
            if (IsCompatibilityChecked)
                return;

            TestOnce();
            IsCompatibilityChecked = true;
        }
        
        private void TestOnce()
        {
            if (IsCompatibilityChecked)
                return;
            
            const string exceptionBaseMessage = "The QuestPDF library has encountered an issue while loading one of its dependencies.";
            const string paragraph = "\n\n";
                
            // test with dotnet-based mechanism where native files are provided
            // in the "runtimes/{rid}/native" folder on Core, or by the targets file on .NET Framework
            var innerException = CheckIfExceptionIsThrownWhenLoadingNativeDependencies();

            if (innerException == null)
            {
                EnsureNativeVersionCompatibility();
                return;
            }

            if (!NativeDependencyProvider.IsCurrentPlatformSupported())
                ThrowCompatibilityException(innerException);
            
            // detect platform, copy appropriate native files and test compatibility again
            NativeDependencyProvider.EnsureNativeFileAvailability();
            
            innerException = CheckIfExceptionIsThrownWhenLoadingNativeDependencies();

            if (innerException == null)
            {
                EnsureNativeVersionCompatibility();
                return;
            }

            ThrowCompatibilityException(innerException);
            
            void ThrowCompatibilityException(Exception innerException)
            {
                var supportedRuntimes = string.Join(", ", NativeDependencyProvider.SupportedPlatforms);
                var currentRuntime = NativeDependencyProvider.GetRuntimePlatform();
                var isRuntimeSupported = NativeDependencyProvider.SupportedPlatforms.Contains(currentRuntime);

                var message = $"{exceptionBaseMessage}";
                
                if (!isRuntimeSupported)
                {
                    message += $"{paragraph}Your runtime is not supported by QuestPDF. " +
                                $"The following runtimes are supported: {supportedRuntimes}. " +
                                $"Your current runtime is detected as '{currentRuntime}'. ";
                }
                
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    message += $"{paragraph}Please always set the 'Platform target' to either 'X86' or 'X64' in your startup project settings. Please do not use the 'Any CPU' option.";
                
                if (RuntimeInformation.ProcessArchitecture is Architecture.Arm)
                    message += $"{paragraph}Please consider setting the 'Platform target' property to 'Arm64' in your project settings.";

                var hint = ExceptionHint.Invoke();
                
                if (!string.IsNullOrEmpty(hint))
                    message += $"{paragraph}{hint}";

                if (isRuntimeSupported)
                {
                    var glibcVersion = NativeDependencyProvider.GetGlibcVersion();

                    if (glibcVersion != null && glibcVersion < RequiredGlibcVersionOnLinux)
                    {
                        message += $"{paragraph}Please consider updating your operating system distribution. " +
                                   $"Current GLIBC version: {glibcVersion}. " +
                                   $"The minimum required version is {RequiredGlibcVersionOnLinux}. ";
                    }
                    else
                    {
                        message += $"{paragraph}If the problem persists, it may mean that your current operating system distribution is outdated. For optimal compatibility, please consider updating it to a more recent version.";
                    }
                }
                
                throw new Exception(message, innerException);
            }

            void EnsureNativeVersionCompatibility()
            {
                if (!CheckNativeLibraryVersion())
                    throw new Exception($"{exceptionBaseMessage}{paragraph}The loaded native library version is incompatible with the current QuestPDF version. To resolve this issue, please: 1) Clean and rebuild your solution, 2) Remove the bin and obj folders, and 3) Ensure all projects in your solution use the same QuestPDF NuGet package version.");
            }
        }
    
        private Exception? CheckIfExceptionIsThrownWhenLoadingNativeDependencies()
        {
            try
            {
                ExecuteNativeCode();
                return null;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }
    }
}
