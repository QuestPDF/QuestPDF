using System;
using System.Runtime.InteropServices;
using QuestPDF.Drawing.Exceptions;

namespace QuestPDF.Helpers
{
    internal sealed class NativeDependencyCompatibilityChecker
    {
        private const string ExceptionBaseMessage = "The QuestPDF library has encountered an issue while loading one of its dependencies.";
        private const string Paragraph = "\n\n";
        
        private static readonly Version RequiredGlibcVersionOnLinux = Version.Parse("2.28");
        
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
            // test with dotnet-based mechanism where native files are provided
            // in the "runtimes/{rid}/native" folder on Core, or by the targets file on .NET Framework
            var innerException = CheckIfExceptionIsThrownWhenLoadingNativeDependencies();

            if (innerException == null)
            {
                EnsureNativeVersionCompatibility();
                return;
            }
            
            EnsureOperatingSystemIsSupported();
            EnsureRuntimeIdentifierIsSupported();
            EnsureLinuxGlibcVersionIsSupported();

            // detect platform, copy appropriate native files and test compatibility again
            NativeDependencyProvider.EnsureNativeFileAvailability();
            
            innerException = CheckIfExceptionIsThrownWhenLoadingNativeDependencies();

            if (innerException == null)
            {
                EnsureNativeVersionCompatibility();
                return;
            }

            ThrowGeneralCompatibilityException(innerException, ExceptionHint.Invoke());
        }
        
        private static void EnsureOperatingSystemIsSupported()
        {
            try
            {
                CheckAndThrow();
            }
            catch (Exception e) when (e is not PlatformNotSupportedException)
            {
                Throw("unknown platform");
            }

            static void CheckAndThrow()
            {
#if NET6_0_OR_GREATER
                if (OperatingSystem.IsBrowser())
                    Throw("WebAssembly / Browser", "For example, perform this operation on Blazor Server (not WASM).");

                if (OperatingSystem.IsAndroid())
                    Throw("Android");

                if (OperatingSystem.IsMacCatalyst())
                    Throw("Mac Catalyst");

                if (OperatingSystem.IsIOS())
                    Throw("iOS / iPadOS");

                if (OperatingSystem.IsTvOS())
                    Throw("tvOS");

                if (OperatingSystem.IsWatchOS())
                    Throw("watchOS");
#endif
            
#if NET8_0_OR_GREATER
                if (OperatingSystem.IsWasi())
                    Throw("WASI");
#endif
                
                var isDesktopOrServer = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                
                if (!isDesktopOrServer)
                    Throw("non-desktop/server environment");
            }

            static void Throw(string platformName, string? details = null)
            {
                var message = $"The current platform ({platformName}) is not supported. " +
                              "QuestPDF relies on native dependencies that are only available on Windows, Linux, and macOS (desktop/server environments). " +
                              "To resolve this issue, please consider moving the PDF generation logic to a server, backend, or cloud environment, and then transfer the generated document to the client.";

                if (!string.IsNullOrWhiteSpace(details))
                    message += $" {details}";

                throw new PlatformNotSupportedException(message);
            }
        }

        private static void EnsureRuntimeIdentifierIsSupported()
        {
            if (NativeDependencyProvider.IsCurrentRuntimeSupported())
                return;

            var supportedRuntimes = string.Join(", ", NativeDependencyProvider.SupportedPlatforms);
            var currentRuntime = NativeDependencyProvider.GetRuntimePlatform();
            
            throw new PlatformNotSupportedException(
                $"{ExceptionBaseMessage}{Paragraph}Your runtime is not supported by QuestPDF. " +
                $"The following runtimes are supported: {supportedRuntimes}. " +
                $"Your current runtime is detected as '{currentRuntime}'. ");
        }

        private static void EnsureLinuxGlibcVersionIsSupported()
        {
            var glibcVersion = NativeDependencyProvider.GetGlibcVersion();
            
            if (glibcVersion == null || glibcVersion >= RequiredGlibcVersionOnLinux) 
                return;
            
            throw new PlatformNotSupportedException(
                $"{ExceptionBaseMessage}{Paragraph}" +
                $"Please consider updating your operating system distribution. " +
                $"Current GLIBC version: {glibcVersion}. " +
                $"The minimum required version is {RequiredGlibcVersionOnLinux}. ");
        }
        
        private void EnsureNativeVersionCompatibility()
        {
            if (CheckNativeLibraryVersion())
                return;
            
            throw new InitializationException(
                $"{ExceptionBaseMessage}{Paragraph}" +
                $"The loaded native library version is incompatible with the current QuestPDF version.{Paragraph}" +
                $"To resolve this issue, please: 1) Clean and rebuild your solution, 2) Remove the bin and obj folders, and 3) Ensure all projects in your solution use the same QuestPDF NuGet package version.{Paragraph}" +
                $"If you have copied any QuestPDF-related native files manually, please make sure to replace them with the updated ones provided by the NuGet package.");
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
        
        private static void ThrowGeneralCompatibilityException(Exception innerException, string? exceptionHint)
        {
            var message = $"{ExceptionBaseMessage}{Paragraph}" +
                          $"The reason could not be automatically determined. " +
                          $"QuestPDF requires additional native shared libraries to run. " +
                          $"They are distributed with the QuestPDF NuGet package under the 'runtimes/{NativeDependencyProvider.GetRuntimePlatform()}/native' folder. " +
                          $"Please ensure that the appropriate native files are copied to the application output or publish directory and are available to the native loader.";
            
            if (!string.IsNullOrWhiteSpace(exceptionHint))  
                message += $"{Paragraph}{exceptionHint}";
            
            throw new InitializationException(message, innerException);
        }
    }
}
