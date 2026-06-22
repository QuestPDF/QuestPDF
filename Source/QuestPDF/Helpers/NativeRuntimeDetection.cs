using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace QuestPDF.Helpers;

internal static class NativeRuntimeDetection
{
    public static readonly string[] SupportedPlatforms =
    {
        "win-x86",
        "win-x64",
        "win-arm64",
        "linux-x64",
        "linux-musl-x64",
        "linux-arm64",
        "osx-x64",
        "osx-arm64"
    };

    public static bool IsCurrentRuntimeSupported()
    {
        return SupportedPlatforms.Contains(RuntimePlatform.Value);
    }

    public static readonly Lazy<string> RuntimePlatform = new(() =>
    {
        return $"{GetSystemIdentifier()}-{GetProcessArchitecture()}";

        static string GetSystemIdentifier()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return IsLinuxMusl.Value ? "linux-musl" : "linux";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";

            return "other";
        }

        static string GetProcessArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture.ToString().ToLower();
        }
    });

    private static readonly Lazy<string?> LddCommandOutput = new(() =>
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return null;

        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ldd",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);

            if (process == null)
                return null;

            var standardOutputText = process.StandardOutput.ReadToEnd();
            var standardErrorText = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return standardOutputText + standardErrorText;
        }
        catch
        {
            return null;
        }
    });

    private static readonly Lazy<bool> IsLinuxMusl = new(() =>
    {
        var lddCommandOutput = LddCommandOutput.Value ?? string.Empty;
        return lddCommandOutput.IndexOf("musl", StringComparison.InvariantCultureIgnoreCase) >= 0;
    });

    public static readonly Lazy<Version?> GlibcVersion = new(() =>
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return null;

        // strategy 1: use gnu_get_libc_version
        try
        {
            var versionPtr = gnu_get_libc_version();
            var versionText = Marshal.PtrToStringAnsi(versionPtr);
            return Version.Parse(versionText);
        }
        catch
        {
            // ignore
        }

        // strategy 2: use ldd command
        var lddCommandOutput = LddCommandOutput.Value ?? string.Empty;

        if (string.IsNullOrEmpty(lddCommandOutput))
            return null;

        var match = Regex.Match(lddCommandOutput, @"ldd \(.+\) (?<version>\d+\.\d+)");

        if (!match.Success)
            return null;

        var versionGroup = match.Groups["version"];

        if (!versionGroup.Success)
            return null;

        return Version.TryParse(versionGroup.Value, out var parsedVersion) ? parsedVersion : null;
    });

    [DllImport("libc", EntryPoint = "gnu_get_libc_version")]
    private static extern IntPtr gnu_get_libc_version();
}
