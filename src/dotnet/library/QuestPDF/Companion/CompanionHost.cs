#if NET8_0_OR_GREATER

using System;
using System.Net;

namespace QuestPDF.Companion
{
    /// <summary>
    /// Decides which machine the library connects to.
    /// By default, the Companion application runs on the same machine and is reached through the loopback address.
    /// The QUESTPDF_COMPANION_HOST environment variable points the library to another machine,
    /// e.g. "host.docker.internal" when the application runs in a container and the Companion application on the host.
    /// </summary>
    internal sealed record CompanionHost(string Name, bool IsLocal)
    {
        internal const string EnvironmentVariableName = "QUESTPDF_COMPANION_HOST";

        private static readonly CompanionHost Loopback = new("127.0.0.1", true);

        public static CompanionHost Resolve()
        {
            var configured = Environment.GetEnvironmentVariable(EnvironmentVariableName)?.Trim();

            if (string.IsNullOrEmpty(configured))
                return Loopback;

            if (Uri.CheckHostName(configured) == UriHostNameType.Unknown)
                throw new ArgumentException($"The {EnvironmentVariableName} environment variable should contain a host name or an IP address, without a scheme or a port. Current value: '{configured}'.");

            return new CompanionHost(configured, IsLoopback(configured));
        }

        public Uri CreateBaseAddress(int port)
        {
            return new UriBuilder(Uri.UriSchemeHttp, Name, port).Uri;
        }

        private static bool IsLoopback(string host)
        {
            if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                return true;

            return IPAddress.TryParse(host.Trim('[', ']'), out var address) && IPAddress.IsLoopback(address);
        }
    }
}

#endif
