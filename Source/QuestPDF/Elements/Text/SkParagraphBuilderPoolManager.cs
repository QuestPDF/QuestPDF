using System;
using System.Collections.Concurrent;
using QuestPDF.Drawing;
using QuestPDF.Skia.Text;

namespace QuestPDF.Elements.Text;

internal static class SkParagraphBuilderPoolManager
{
    private static ConcurrentDictionary<(ParagraphStyleConfiguration, bool useEnvironmentFonts), ConcurrentBag<SkParagraphBuilder>> ObjectPool { get; } = new();

    public static SkParagraphBuilder Get(ParagraphStyleConfiguration configuration)
    {
        var specificPool = GetPool(configuration);
        
        if (specificPool.TryTake(out var builder))
            return builder;

        var fontCollection = Settings.UseEnvironmentFonts
            ? FontManager.GlobalFontCollection
            : FontManager.LocalFontCollection;
        
        return SkParagraphBuilder.Create(configuration, fontCollection);
    }

    public static void Return(SkParagraphBuilder builder)
    {
        builder.Reset();
        
        var specificPool = GetPool(builder.Configuration);
        specificPool.Add(builder);
    }

    private static ConcurrentBag<SkParagraphBuilder> GetPool(ParagraphStyleConfiguration configuration)
    {
        var key = (configuration, Settings.UseEnvironmentFonts);
        return ObjectPool.GetOrAdd(key, _ => new ConcurrentBag<SkParagraphBuilder>());
    }
}