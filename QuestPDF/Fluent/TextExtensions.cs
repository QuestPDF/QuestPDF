using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class TextDescriptor
    {
        private TextBlock TextBlock { get; }
        
        internal TextDescriptor(TextBlock textBlock)
        {
            TextBlock = textBlock;
        }
        
        public void AlignLeft()
        {
            TextBlock.Alignment = HorizontalAlignment.Left;
        }
        
        public void AlignCenter()
        {
            TextBlock.Alignment = HorizontalAlignment.Center;
        }
        
        public void AlignRight()
        {
            TextBlock.Alignment = HorizontalAlignment.Right;
        }
        
        public void Span(string text, TextStyle? style = null)
        {
            TextBlock.Children.Add(new TextItem
            {
                Text = text,
                Style = style ?? TextStyle.Default
            });
        }
    }
    
    public static class TextExtensions
    {
        public static void Text(this IContainer element, Action<TextDescriptor> content)
        {
            var textBlock = new TextBlock();

            if (element is Alignment alignment)
                textBlock.Alignment = alignment.Horizontal;
            
            var descriptor = new TextDescriptor(textBlock);
            content?.Invoke(descriptor);
            
            element.Element(textBlock);
        }
        
        public static void Text(this IContainer element, object text, TextStyle? style = null)
        {
            element.Text(x => x.Span(text.ToString(), style));
        }
    }
}