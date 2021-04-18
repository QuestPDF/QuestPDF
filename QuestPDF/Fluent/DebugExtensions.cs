using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class DebugExtensions
    {
        public static IContainer Debug(this IContainer parent, string text, string color)
        {
            var container = new Container();

            parent.Component(new Debug
            {
                Child = container,
                Text = text,
                Color = color
            });

            return container;
        }
        
        public static IContainer Debug(this IContainer parent, string text)
        {
            return parent.Debug(text, Colors.Red.Medium);
        }

        public static IContainer Debug(this IContainer parent)
        {
            return parent.Debug(string.Empty, Colors.Red.Medium);
        }
    }
}