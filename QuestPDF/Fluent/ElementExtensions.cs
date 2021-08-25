using System;
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

        internal static T Element<T>(this IContainer element, T child) where T : IElement
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
        
        public static void Element<TParent>(this TParent parent, Action<IContainer> handler) where TParent : IContainer
        {
            handler(parent.Container());
        }
        
        public static IContainer Element<TParent>(this TParent parent, Func<IContainer, IContainer> handler) where TParent : IContainer
        {
            return handler(parent.Container()).Container();
        }

        public static void PageNumber(this IContainer element, string textFormat = "{pdf:currentPage} / {pdf:totalPages}", TextStyle? style = null)
        {
            element.Element(new PageNumber
            {
                TextFormat = textFormat,
                //TextStyle = style ?? TextStyle.Default // TODO
            });
        }
        
        public static IContainer AspectRatio(this IContainer element, float ratio, AspectRatioOption option = AspectRatioOption.FitWidth)
        {
            return element.Element(new AspectRatio
            {
                Ratio = ratio,
                Option = option
            });
        }

        public static IContainer Background(this IContainer element, string color)
        {
            return element.Element(new Background
            {
                Color = color
            });
        }

        public static void Placeholder(this IContainer element, string? text = null)
        {
            element.Component(new Elements.Placeholder
            {
                Text = text ?? string.Empty
            });
        }

        public static IContainer ShowOnce(this IContainer element)
        {
            return element.Element(new ShowOnce());
        }
        
        public static IContainer ShowEntire(this IContainer element)
        {
            return element.Element(new ShowEntire());
        }

        public static IContainer EnsureSpace(this IContainer element, float minHeight = Elements.EnsureSpace.DefaultMinHeight)
        {
            return element.Element(new EnsureSpace
            {
                MinHeight = minHeight
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
        
        public static IContainer ShowIf(this IContainer element, bool condition)
        {
            return condition ? element : new Container();
        }
        
        public static void Canvas(this IContainer element, DrawOnCanvas handler)
        {
            element.Element(new Canvas
            {
                Handler = handler
            });
        }
        
        public static IContainer Box(this IContainer element)
        {
            return element.Element(new Box());
        }
        
        public static IContainer Unconstrained(this IContainer element)
        {
            return element.Element(new Unconstrained());
        }
    }
}
