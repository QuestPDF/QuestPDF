[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(QuestPDF.Previewer.HotReloadManager))]

namespace QuestPDF.Previewer
{
    /// <summary>
    /// Helper for subscribing to hot reload notifications.
    /// </summary>
    internal static class HotReloadManager
    {
        private static readonly List<Action> _actions = new();

        public static void Register(Action action)
        {
            _actions.Add(action);
        }

        public static void Unregister(Action action)
        {
            _actions.Remove(action);
        }

        public static void ClearCache(Type[]? _) { }

        public static void UpdateApplication(Type[]? _)
        {
            foreach (var action in _actions)
                action();
        }
    }
}
