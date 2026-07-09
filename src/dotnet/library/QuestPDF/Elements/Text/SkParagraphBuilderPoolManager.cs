using System;
using System.Collections.Concurrent;
using QuestPDF.Drawing;
using QuestPDF.Skia.Text;

namespace QuestPDF.Elements.Text;

internal static class SkParagraphBuilderPoolManager
{
    private static ConcurrentDictionary<ParagraphStyle, ConcurrentBag<SkParagraphBuilder>> ObjectPool { get; } = new();

    public static SkParagraphBuilder Get(ParagraphStyle style)
    {
        var specificPool = GetPool(style);
        
        if (specificPool.TryTake(out var builder))
            return builder;
        
        var fontCollection = SkFontCollection.Create(FontManager.TypefaceProvider, FontManager.CurrentFontManager);
        return SkParagraphBuilder.Create(style, fontCollection);
    }

    public static void Return(SkParagraphBuilder builder)
    {
        builder.Reset();
        
        var specificPool = GetPool(builder.Style);
        specificPool.Add(builder);
    }

    private static ConcurrentBag<SkParagraphBuilder> GetPool(ParagraphStyle style)
    {
        return ObjectPool.GetOrAdd(style, _ => new ConcurrentBag<SkParagraphBuilder>());
    }
}