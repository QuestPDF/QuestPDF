#if NET8_0_OR_GREATER

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace QuestPDF.Companion
{
    internal static class CompanionPortResolver
    {
        private const int DefaultPortCandidateCount = 16;

        /// <summary>
        /// When the default port is requested and another (non-Companion) program owns it,
        /// tries the following ports, so that the preview works without any configuration.
        /// An explicitly chosen port is always used as is.
        /// </summary>
        public static async Task<int> ResolveWithFallback(int requestedPort, CancellationToken cancellationToken)
        {
            if (requestedPort != CompanionExtensions.DefaultPort)
                return requestedPort;

            using var identityClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(1)
            };

            foreach (var port in Enumerable.Range(CompanionExtensions.DefaultPort, DefaultPortCandidateCount))
            {
                // a port that can be bound is free, regardless of the protocol of any program that could use it
                if (CanBindPort(port))
                    return port;

                // a port that is taken is still fine when the owner is the Companion application
                if (await IsCompanionListening(identityClient, port, cancellationToken))
                    return port;
            }

            // every candidate is taken: use the requested port, so that the usual error message is reported
            return requestedPort;
        }

        private static bool CanBindPort(int port)
        {
            try
            {
                // exclusive use prevents binding next to a program listening on all addresses (SO_REUSEADDR on macOS and Linux)
                var listener = new TcpListener(IPAddress.Loopback, port) { ExclusiveAddressUse = true };
                listener.Start();
                listener.Stop();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        private static async Task<bool> IsCompanionListening(HttpClient client, int port, CancellationToken cancellationToken)
        {
            try
            {
                using var result = await client.GetAsync($"http://127.0.0.1:{port}{CompanionService.IdentityEndpoint}", cancellationToken);
                var identity = await result.Content.ReadAsStringAsync(cancellationToken);

                return result.IsSuccessStatusCode && identity == CompanionService.IdentityResponse;
            }
            catch when (!cancellationToken.IsCancellationRequested)
            {
                return false;
            }
        }
    }
}

#endif
