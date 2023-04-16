using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class DebugExtensions
    {
        public static IContainer DebugArea(this IContainer parent, string text, string color)
        {
            ColorValidator.Validate(color);
            
            var container = new Container();

            parent.Component(new DebugArea
            {
                Child = container,
                Text = text,
                Color = color
            });

            return container;
        }
        
        public static IContainer DebugArea(this IContainer parent, string text)
        {
            return parent.DebugArea(text, Colors.Red.Medium);
        }

        public static IContainer DebugArea(this IContainer parent)
        {
            return parent.DebugArea(string.Empty, Colors.Red.Medium);
        }
        
        /// <summary>
        /// Creates a virtual element that is visible on the elements trace when the layout overflow exception is thrown.
        /// This can be used to easily identify elements inside the elements trace tree and faster find issue root cause.
        /// </summary>
        public static IContainer DebugPointer(this IContainer parent, string elementTraceText)
        {
            return parent.DebugPointer(elementTraceText, true);
        }
        
        internal static IContainer DebugPointer(this IContainer parent, string elementTraceText, bool highlight)
        {
            return parent.Element(new DebugPointer
            {
                Target = elementTraceText,
                Highlight = highlight
            });
        }
    }
}