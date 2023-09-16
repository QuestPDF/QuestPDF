#if NET6_0_OR_GREATER

using System;

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(QuestPDF.Previewer.HotReloadManager))]

namespace QuestPDF.Previewer
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