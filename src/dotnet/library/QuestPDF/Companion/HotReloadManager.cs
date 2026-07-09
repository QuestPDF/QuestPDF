#if NET6_0_OR_GREATER

using System;
using QuestPDF.Companion;

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(HotReloadManager))]

namespace QuestPDF.Companion
{
    /// <summary>
    /// Helper to subscribe to hot reload notifications.
    /// </summary>
    internal static class HotReloadManager
    {
        public static event EventHandler? UpdateApplicationRequested;

        public static void UpdateApplication(Type[]? _)
        {
            UpdateApplicationRequested?.Invoke(null, EventArgs.Empty);
        }
    }
}

#endif