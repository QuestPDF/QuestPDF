using System;
using System.Collections.Concurrent;
using QuestPDF.Drawing;
using QuestPDF.Skia.Text;

namespace QuestPDF.Elements.Text;

internal static class SkParagraphBuilderPoolManager
{
    private static ConcurrentDictionary<ParagraphStyleConfiguration, ConcurrentBag<SkParagraphBuilder>> ObjectPool { get; } = new();

    public static SkParagraphBuilder Get(ParagraphStyleConfiguration configuration)
    {
        var specificPool = GetPool(configuration);
        
        if (specificPool.TryTake(out var builder))
            return builder;
        
        return SkParagraphBuilder.Create(configuration, FontManager.CurrentFontCollection);
    }

    public static void Return(SkParagraphBuilder builder)
    {
        builder.Reset();
        
        var specificPool = GetPool(builder.Configuration);
        specificPool.Add(builder);
    }

    private static ConcurrentBag<SkParagraphBuilder> GetPool(ParagraphStyleConfiguration configuration)
    {
        return ObjectPool.GetOrAdd(configuration, _ => new ConcurrentBag<SkParagraphBuilder>());
    }
}