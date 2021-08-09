using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class TextDescriptor
    {
        internal ICollection<Element> Elements = new List<Element>();
        
        internal TextDescriptor()
        {
            
        }
        
        public void Span(string text, TextStyle? style = null)
        {
            text.Split(' ')
                .Select(x => $"{x} ")
                .Select(x => new TextItem
                {
                    Value = x,
                    Style = style ?? TextStyle.Default
                })
                .ToList()
                .ForEach(Elements.Add);
        }

        public IContainer Element()
        {
            var container = new Container();
            Elements.Add(container);
            return container;
        }
    }
    
    public static class TextExtensions
    {
        public static void Text(this IContainer element, Action<TextDescriptor> content)
        {
            var descriptor = new TextDescriptor();
            content?.Invoke(descriptor);

            element.Element(new TextBlock()
            {
                Children = descriptor.Elements.ToList()
            });
        }
        
        public static void Text(this IContainer element, object text, TextStyle? style = null)
        {
            element.Text(x => x.Span(text.ToString(), style));
        }
    }
}