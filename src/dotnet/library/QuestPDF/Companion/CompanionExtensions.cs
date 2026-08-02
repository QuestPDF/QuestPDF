using System;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Companion
{
    public static class CompanionExtensions
    {
        static CompanionExtensions()
        {
            LicenseChecker.ValidateLicense();
        }

        #if NET6_0_OR_GREATER
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="companion.support"]/*' />
        public static void ShowInCompanion(this IDocument document, int port = 12500)
        {
            document.ShowInCompanionAsync(port).GetAwaiter().GetResult();
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="companion.support"]/*' />
        public static Task ShowInCompanionAsync(this IDocument document, int port = 12500, CancellationToken cancellationToken = default)
        {
            // run the entire session on the thread pool so that no await captures the caller's SynchronizationContext;
            // otherwise the blocking ShowInCompanion entry point could deadlock in UI applications
            return Task.Run(() => CompanionSession.RunNewCompanionSession(document, port, cancellationToken));
        }

        #else

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="companion.notSupported"]/*' />
        public static void ShowInCompanion(this IDocument document, int port = 12500)
        {
            throw new Exception("The hot-reload feature requires .NET 6 or later.");
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="companion.notSupported"]/*' />
        public static async Task ShowInCompanionAsync(this IDocument document, int port = 12500, CancellationToken cancellationToken = default)
        {
            throw new Exception("The hot-reload feature requires .NET 6 or later.");
        }

        #endif
    }
}
