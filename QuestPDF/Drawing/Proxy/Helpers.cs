using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing.Proxy;

internal static class Helpers
{
    public static IReadOnlyCollection<DocumentElementProperty> GetElementConfiguration(this IElement element)
    {
        return element
            .GetType()
            .GetProperties()
            .Select(x => new
            {
                Property = x.Name.PrettifyName(),
                Value = x.GetValue(element)
            })
            .Where(x => !(x.Value is IElement))
            .Where(x => x.Value is string || !(x.Value is IEnumerable))
            .Where(x => !(x.Value is TextStyle))
            .Select(x => new DocumentElementProperty
            {
                Label = x.Property,
                Value = FormatValue(x.Value)
            })
            .ToList();

        string FormatValue(object value)
        {
            const int maxLength = 100;
                
            var text = value?.ToString() ?? "-";

            if (text.Length < maxLength)
                return text;

            return text.AsSpan(0, maxLength).ToString() + "...";
        }
    }
}