using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using QuestPDF.Drawing.Exceptions;

namespace QuestPDF.Helpers
{
    internal sealed class NativeDependencyCompatibilityChecker
    {
        private const string ExceptionBaseMessage = "The QuestPDF library has encountered an issue while loading one of its dependencies.";
        private const string Paragraph = "\n\n";
        
        private static readonly Version RequiredGlibcVersionOnLinux = Version.Parse("2.28");
        
        private readonly object Lock = new();
        private bool IsCompatibilityChecked { get; set; } = false;

        public Action ExecuteNativeCode { get; set; } = () => { };
        public Func<bool> CheckNativeLibraryVersion { get; set; } = () => true;
        public Func<string> ExceptionHint { get; set; } = () => string.Empty;
        
        public void Test()
        {
            if (IsCompatibilityChecked)
                return;

            lock (Lock)
            {
                if (IsCompatibilityChecked)
                    return;

                EnsureNativeDependencyIsAvailable();
                EnsureNativeVersionCompatibility();
                IsCompatibilityChecked = true;
            }
        }

        private void EnsureNativeDependencyIsAvailable()
        {
            // test with dotnet-based mechanism where native files are provided
            // in the "runtimes/{rid}/native" folder on Core, or by the targets file on .NET Framework
            if (CheckIfNativeCodeCanBeInvoked())
                return;

            EnsureOperatingSystemIsSupported();
            EnsureRuntimeIdentifierIsSupported();
            EnsureLinuxGlibcVersionIsSupported();

            // first attempt: preload the native dependencies directly from the "runtimes/{rid}/native"
            NativeDependencyProvider.TryPreloadNativeDependencies();

            if (CheckIfNativeCodeCanBeInvoked())
            {
                WarnIfNativeDependenciesAreSuccessfullyResolvedViaFallbackOnModernRuntime();
                return;
            }

            // second attempt: copy the appropriate native files next to the application and test again
            NativeDependencyProvider.TryCopyNativeDependenciesToApplicationDirectory();

            if (CheckIfNativeCodeCanBeInvoked())
            {
                WarnIfNativeDependenciesAreSuccessfullyResolvedViaFallbackOnModernRuntime();
                return;
            }

            ThrowNativeDependencyLoadException();
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
                if (OperatingSystem.IsMacCatalyst())
                    Throw("Mac Catalyst");
#endif
                
#if NET5_0_OR_GREATER
                if (OperatingSystem.IsBrowser())
                    Throw("WebAssembly / Browser", "For example, perform this operation on Blazor Server (not WASM).");

                if (OperatingSystem.IsAndroid())
                    Throw("Android");
                
                if (OperatingSystem.IsIOS())
                    Throw("iOS / iPadOS");

                if (OperatingSystem.IsTvOS())
                    Throw("tvOS");

                if (OperatingSystem.IsWatchOS())
                    Throw("watchOS");
                
                if (OperatingSystem.IsFreeBSD())
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
                var message = $"QuestPDF cannot run on the current platform ({platformName}). " +
                              "It depends on native components that are available only on Windows, Linux, and macOS (desktop and server environments), " +
                              "and therefore cannot generate documents directly on mobile, browser (WebAssembly), or other sandboxed platforms. " +
                              "The recommended solution is to move PDF generation to a backend service (for example, a Web API, server, or cloud function) and then deliver the generated file to the client.";

                if (!string.IsNullOrWhiteSpace(details))
                    message += $" {details}";

                throw new PlatformNotSupportedException(message);
            }
        }

        private static void EnsureRuntimeIdentifierIsSupported()
        {
            if (NativeRuntimeDetection.IsCurrentRuntimeSupported())
                return;

            var supportedRuntimes = string.Join(", ", NativeRuntimeDetection.SupportedPlatforms);
            var currentRuntime = NativeRuntimeDetection.RuntimePlatform.Value;
            
            throw new PlatformNotSupportedException(
                $"{ExceptionBaseMessage}{Paragraph}" +
                $"QuestPDF does not ship native components for your current runtime ('{currentRuntime}'). " +
                $"The supported runtimes are: {supportedRuntimes}.{Paragraph}" +
                $"This typically happens on 32-bit Linux, 32-bit ARM devices (such as some Raspberry Pi configurations), or operating systems other than Windows, Linux, and macOS. " +
                $"To resolve this issue, please run your application on one of the supported runtimes listed above — on Linux and ARM devices this usually means switching to a 64-bit operating system.");
        }

        private static void EnsureLinuxGlibcVersionIsSupported()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return;
            
            var glibcVersion = NativeRuntimeDetection.GlibcVersion.Value;
            
            if (glibcVersion == null || glibcVersion >= RequiredGlibcVersionOnLinux) 
                return;
            
            throw new PlatformNotSupportedException(
                $"{ExceptionBaseMessage}{Paragraph}" +
                $"Your Linux environment provides GLIBC {glibcVersion}, but QuestPDF requires at least GLIBC {RequiredGlibcVersionOnLinux}.{Paragraph}" +
                $"This usually happens on older Linux distributions or outdated Docker base images. " +
                $"To resolve this issue, please upgrade to a newer distribution or base image — for example, Debian 10+, Ubuntu 20.04+, or a recent Alpine image (which uses musl instead of GLIBC).");
        }
        
        private void EnsureNativeVersionCompatibility()
        {
            if (CheckNativeLibraryVersion())
                return;

            var message =
                $"{ExceptionBaseMessage}{Paragraph}" +
                $"The loaded native library belongs to a different QuestPDF version than the one your application references. " +
                $"This usually happens after upgrading QuestPDF when older native files remain in the output folder, or when projects within the same solution reference different QuestPDF versions.{Paragraph}" +
                $"To resolve this issue, please: 1) ensure every project in your solution references the same QuestPDF NuGet package version, 2) delete the 'bin' and 'obj' folders, 3) restore NuGet packages, and 4) rebuild the solution.{Paragraph}" +
                $"If you have copied any QuestPDF native files manually, please replace them with the matching files provided by the NuGet package.";

            var exceptionHint = ExceptionHint.Invoke();

            if (!string.IsNullOrWhiteSpace(exceptionHint))
                message += $"{Paragraph}{exceptionHint}";

            throw new InitializationException(message);
        }
        
        private Exception? GetExceptionThrownWhenInvokingNativeCode()
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
        
        private bool CheckIfNativeCodeCanBeInvoked()
        {
            return GetExceptionThrownWhenInvokingNativeCode() == null;
        }

        private void ThrowNativeDependencyLoadException()
        {
            var innerException = GetExceptionThrownWhenInvokingNativeCode();
            
            if (innerException == null)
                return; 
            
            var platform = NativeRuntimeDetection.RuntimePlatform.Value;

            var message = $"{ExceptionBaseMessage}{Paragraph}" +
                          $"QuestPDF's native components could not be loaded, and the exact cause could not be determined automatically. " +
                          $"These components are shipped with the QuestPDF NuGet package under the 'runtimes/{platform}/native' folder and must be present in your application's output or publish directory.{Paragraph}" +
                          $"To resolve this issue, please: " +
                          $"1) confirm that the 'runtimes/{platform}/native' folder is included in your build output (this can be missing in single-file or trimmed publishes), " +
                          $"2) when publishing for a specific runtime (a self-contained publish or 'dotnet publish -r <RID>'), make sure the RuntimeIdentifier matches the machine you deploy to rather than the machine you build on — for example '{platform}' for this environment, 'linux-musl-x64' for Alpine Linux, or 'osx-arm64' for Apple Silicon; for a framework-dependent deployment, prefer not pinning a RID at build time so that every 'runtimes/{{rid}}/native' folder is preserved and resolved by the .NET runtime, " +
                          $"3) verify that no antivirus or security policy is blocking the native files from being loaded.";

            var architectureMismatchHint = GetArchitectureMismatchHint(innerException);

            if (!string.IsNullOrWhiteSpace(architectureMismatchHint))
                message += $"{Paragraph}{architectureMismatchHint}";

            var exceptionHint = ExceptionHint.Invoke();
            
            if (!string.IsNullOrWhiteSpace(exceptionHint))
                message += $"{Paragraph}{exceptionHint}";

            throw new InitializationException(message, innerException);
        }

        private static string? GetArchitectureMismatchHint(Exception innerException)
        {
            if (innerException is not BadImageFormatException)
                return null;

            var processArchitecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
            var osArchitecture = RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant();

            var hint = 
                "This error usually indicates an architecture (bitness) mismatch between your application process and the native QuestPDF binary. " +
                $"Your process runs as '{processArchitecture}' on a '{osArchitecture}' operating system.{Paragraph}" +
                "Please ensure your process architecture (x86, x64, or Arm64) matches one of the shipped native runtimes. " +
                $"A common cause is building on one architecture and running on another — for example, building on Apple Silicon (arm64) and then running the artifact as x64 in Docker or CI.{Paragraph}" +
                "When you publish for a specific runtime, set the RuntimeIdentifier to match the target machine (for example 'dotnet publish -r linux-x64', or 'linux-musl-x64' for Alpine Linux).";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                hint += $"{Paragraph}On .NET Framework, set an explicit 'Platform target' (x86/x64/Arm64) instead of 'Any CPU' so the process bitness is deterministic. " +
                        "When hosting in IIS, make sure the application pool bitness ('Enable 32-Bit Applications') matches your build.";

            return hint;
        }

        private static void WarnIfNativeDependenciesAreSuccessfullyResolvedViaFallbackOnModernRuntime()
        {
#if NETCOREAPP3_0_OR_GREATER
            Trace.TraceWarning(
                "QuestPDF successfully loaded its native dependencies through its own fallback recovery mechanism, after the standard .NET native library resolution had already failed. " +
                "On .NET Core 3.0 and newer, the runtime normally resolves these binaries from the runtimes/{rid}/native folder automatically. " +
                "Reaching this fallback means the files were present on disk but were not part of the resolved dependency graph, so the runtime did not load them on its own. " +
                "The usual cause is a non-standard deployment. " +
                "Generation works now because the files happened to be reachable, but relying on this fallback is fragile - changes to how the application is built, published, or hosted could prevent the libraries from loading. " +
                "PDF generation will continue normally; this message is purely informational.");
#endif
        }
    }
}
