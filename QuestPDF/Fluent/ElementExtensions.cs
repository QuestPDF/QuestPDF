using System;
using System.IO;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ElementExtensions
    {
        internal static Container Create(Action<IContainer> factory)
        {
            var container = new Container();
            factory(container);
            return container;
        }

        public static byte[] Generate(this IDocument document)
        {
            using var stream = new MemoryStream();
            document.Generate(stream);
            return stream.ToArray();
        }
        
        public static void Generate(this IDocument document, string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            document.Generate(stream);
        }

        public static void Generate(this IDocument document, Stream stream)
        {
            DocumentGenerator.Generate(stream, document);
        }

        public static T Element<T>(this IContainer element, T child) where T : IElement
        {
            if (element?.Child != null && element.Child is Empty == false)
            {
                var message = $"Container {element.GetType().Name} already contains an {element.Child.GetType().Name} child. " +
                              $"Tried to assign {child?.GetType()?.Name}." +
                              $"You should not assign multiple elements to single-child containers.";
                
                throw new DocumentComposeException(message);
            }

            if (element != child as Element)
                element.Child = child as Element;
            
            return child;
        }
        
        public static void Element(this IContainer element, Action<IContainer> handler)
        {
            var container = new Container();
            element.Element(container);
            handler?.Invoke(container);
        }

        public static void Image(this IContainer element, byte[] data)
        {
            element.Element(new Image
            {
                Data = data
            });
        }
        
        public static void DynamicImage(this IContainer element, Func<Size, byte[]> imageSource)
        {
            element.Element(new DynamicImage
            {
                Source = imageSource
            });
        }
        
        public static void PageNumber(this IContainer element, string textFormat = "{number}", TextStyle? style = null)
        {
            element.Element(new PageNumber
            {
                TextFormat = textFormat,
                TextStyle = style ?? TextStyle.Default
            });
        }
        
        public static IContainer AspectRatio(this IContainer element, float ratio)
        {
            return element.Element(new AspectRatio
            {
                Ratio = ratio
            });
        }

        public static IContainer Background(this IContainer element, string color)
        {
            return element.Element(new Background
            {
                Color = color
            });
        }
        
        public static void Placeholder(this IContainer element)
        {
            element.Component<Placeholder>();
        }

        public static IContainer ShowOnce(this IContainer element)
        {
            var alignment = new ShowOnce();

            return element.Element(alignment);
        }

        public static void Text(this IContainer element, object text, TextStyle? style = null)
        {
            style ??= TextStyle.Default;

            if (element is Alignment alignment)
                style.Alignment = alignment.Horizontal;
            
            element.Element(new Text()
            {
                Value = text.ToString(),
                Style = style
            });
        }
        
        public static void PageBreak(this IContainer element)
        {
            element.Element(new PageBreak());
        }
        
        public static IContainer Container(this IContainer element)
        {
            return element.Element(new Container());
        }
        
        public static IContainer ExternalLink(this IContainer element, string url)
        {
            return element.Element(new ExternalLink
            {
                Url = url
            });
        }
        
        public static IContainer Location(this IContainer element, string locationName)
        {
            return element.Element(new InternalLocation
            {
                LocationName = locationName
            });
        }
        
        public static IContainer InternalLink(this IContainer element, string locationName)
        {
            return element.Element(new InternalLink
            {
                LocationName = locationName
            });
        }
    }
}