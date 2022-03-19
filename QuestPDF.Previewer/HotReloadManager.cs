using System.Reflection.Metadata;

[assembly: MetadataUpdateHandler(typeof(HotReloadManager))]

internal static class HotReloadManager
{
    public static event Action? OnApplicationChanged;
    public static void UpdateApplication(Type[]? _) => OnApplicationChanged?.Invoke();
}