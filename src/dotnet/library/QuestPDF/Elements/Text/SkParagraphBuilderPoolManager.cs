using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Skia.Text;

namespace QuestPDF.Elements.Text;

internal static class SkParagraphBuilderPoolManager
{
    // Get and Return always happen on the same thread within one call scope,
    // so the pool is thread-local to avoid lock contention on parallel renders
    [ThreadStatic] private static Dictionary<ParagraphStyle, Stack<SkParagraphBuilder>>? ObjectPool;

    public static SkParagraphBuilder Get(ParagraphStyle style)
    {
        var specificPool = GetPool(style);

        if (specificPool.Count > 0)
            return specificPool.Pop();

        var fontCollection = SkFontCollection.Create(FontManager.TypefaceProvider, FontManager.CurrentFontManager);
        return SkParagraphBuilder.Create(style, fontCollection);
    }

    public static void Return(SkParagraphBuilder builder)
    {
        builder.Reset();
        
        var specificPool = GetPool(builder.Style);
        specificPool.Push(builder);
    }

    private static Stack<SkParagraphBuilder> GetPool(ParagraphStyle style)
    {
        ObjectPool ??= new Dictionary<ParagraphStyle, Stack<SkParagraphBuilder>>();

        if (ObjectPool.TryGetValue(style, out var pool)) 
            return pool;
        
        var newPool = new Stack<SkParagraphBuilder>();
        ObjectPool[style] = newPool;
        return newPool;
    }
}