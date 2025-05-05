using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Skia;

namespace QuestPDF.Infrastructure;

static class StaticImageCache
{
    private static bool CacheIsEnabled { get; set; } = true;
    private static ConcurrentDictionary<string, Image> Items { get; set; } = new();

    private const int MaxCacheSize = 25_000_000;
    private const int MaxItemSize = 1_000_000;
    
    public static Image LoadFromCache(string filePath)
    {
        // check fallback path
        filePath = AdjustPath(filePath);

        // check if the image is already in cache
        if (Items.TryGetValue(filePath, out var cacheItem))
            return cacheItem;
        
        // check file size
        var fileInfo = new FileInfo(filePath);
        
        if (fileInfo.Length > MaxItemSize)
            return DirectlyLoadFromFile(filePath, false);
        
        // if cache is larger than expected, the usage might be different from loading static images
        if (!CacheIsEnabled)
            return DirectlyLoadFromFile(filePath, false);
        
        // create new cache item and add it to the cache
        var image = DirectlyLoadFromFile(filePath, true);
        Items.TryAdd(filePath, image);
        
        // check cache size
        CacheIsEnabled = Items.Values.Sum(x => x.SkImage.EncodedDataSize) < MaxCacheSize;
        
        // return cached value
        return image;
    }

    public static string AdjustPath(string filePath)
    {
        if (File.Exists(filePath))
            return filePath;
        
        if (Path.IsPathRooted(filePath))
            throw new DocumentComposeException($"Cannot load an image under the provided absolute path, file not found: {filePath}");

        var fallbackPath = Path.Combine(Helpers.Helpers.ApplicationFilesPath, filePath);

        if (!File.Exists(fallbackPath))
            throw new DocumentComposeException($"Cannot load an image under the provided relative path, file not found: {filePath}");

        return fallbackPath;
    }
    
    public static Image DirectlyLoadFromFile(string filePath, bool isShared)
    {
        filePath = AdjustPath(filePath);
        
        using var imageData = SkData.FromFile(filePath);
        return DecodeImage(imageData, isShared);
    }
    
    public static Image DecodeImage(SkData imageData, bool isShared)
    {
        try
        {
            var skImage = SkImage.FromData(imageData);
            var image = new Image(skImage);
            image.IsShared = isShared;
            
            return image;
        }
        catch
        {
            throw new DocumentComposeException("Cannot decode the provided image.");
        }
    }
}