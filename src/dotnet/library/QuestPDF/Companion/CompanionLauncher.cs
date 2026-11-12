#if NET8_0_OR_GREATER

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace QuestPDF.Companion
{
    /// <summary>
    /// Starts the QuestPDF Companion application when it is not running yet.
    /// </summary>
    internal static class CompanionLauncher
    {
        private const string CommandName = "questpdf-companion";

        // ENOENT on macOS and Linux
        private const int FileNotFoundErrorCode = 2;

        /// <summary>
        /// Asks the operating system to start the Companion application listening on the given port.
        /// Returns as soon as the application has been requested to start: it may not accept connections yet.
        /// </summary>
        public static void Launch(int port)
        {
            // On Windows, `start` reports a missing command with a dialog window that blocks the launcher until it is closed,
            // so the command is looked up first. This is what every new user hits when Companion is not installed yet.
            if (OperatingSystem.IsWindows() && !IsCommandAvailableOnWindows())
                throw CreateCommandNotFoundException();

            try
            {
                var startInfo = CreateStartInfo(port);

                using var process = Process.Start(startInfo);

                if (process == null)
                    throw new InvalidOperationException("The process could not be started.");

                // On Windows, the launcher is a short-lived cmd.exe that only asks the shell to start Companion.
                // A failure to start the Companion executable surfaces here as a non-zero exit code.
                if (OperatingSystem.IsWindows() && process.WaitForExit(5000) && process.ExitCode != 0)
                    throw new InvalidOperationException($"The launcher exited with code {process.ExitCode}.");
            }
            catch (Win32Exception exception) when (!OperatingSystem.IsWindows() && exception.NativeErrorCode == FileNotFoundErrorCode)
            {
                throw CreateCommandNotFoundException(exception);
            }
            catch (Exception exception)
            {
                throw new Exception($"The QuestPDF Companion application is not running, and the library was not able to launch it automatically. Please start the application manually. Learn more: https://www.questpdf.com/companion/usage.html", exception);
            }
        }

        private static Exception CreateCommandNotFoundException(Exception? innerException = null)
        {
            return new Exception($"The QuestPDF Companion application is not running, and the library could not launch it: the '{CommandName}' command was not found. Most likely, the application is not installed yet: please install it. If it is already installed, please start it manually. Learn more: https://www.questpdf.com/companion/usage.html", innerException);
        }

        private static bool IsCommandAvailableOnWindows()
        {
            // the same places `start` searches: the current directory, then every PATH entry
            return (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
                .Split(Path.PathSeparator)
                .Prepend(Environment.CurrentDirectory)
                .Any(ContainsCommand);

            static bool ContainsCommand(string directory)
            {
                // PATH entries may be empty, padded with spaces or wrapped in quotes
                var directoryPath = directory.Trim().Trim('"');

                if (directoryPath.Length == 0)
                    return false;

                try
                {
                    return File.Exists(Path.Combine(directoryPath, $"{CommandName}.exe"));
                }
                catch (ArgumentException)
                {
                    // a malformed PATH entry
                    return false;
                }
            }
        }

        private static ProcessStartInfo CreateStartInfo(int port)
        {
            var portText = port.ToString(CultureInfo.InvariantCulture);

            if (!OperatingSystem.IsWindows())
            {
                var unixStartInfo = new ProcessStartInfo(CommandName)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                unixStartInfo.ArgumentList.Add(portText);
                return unixStartInfo;
            }

            // When the IDE stops the debugged app (e.g. Stop in Rider), it kills the app's whole process tree on Windows,
            // so a Companion started as a child process would close too. Starting it through `cmd /c start` detaches it
            // from that tree, so it stays open between runs.
            // CreateNoWindow hides only the cmd.exe console. Companion's own window must not be hidden, or it stays hidden for good.
            var commandInterpreterPath = Environment.GetEnvironmentVariable("ComSpec") is { Length: > 0 } configuredPath
                ? configuredPath
                : "cmd.exe";

            var windowsStartInfo = new ProcessStartInfo(commandInterpreterPath)
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // ArgumentList applies the standard Windows quoting; the empty string becomes the required "" window title.
            windowsStartInfo.ArgumentList.Add("/c");
            windowsStartInfo.ArgumentList.Add("start");
            windowsStartInfo.ArgumentList.Add("");
            windowsStartInfo.ArgumentList.Add(CommandName);
            windowsStartInfo.ArgumentList.Add(portText);
            return windowsStartInfo;
        }
    }
}

#endif
