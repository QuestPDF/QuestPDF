using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class DebugExtensions
    {
        /// <summary>
        /// Draws a labeled box around its inner content.
        /// Useful for visual debugging and pinpointing output from specific code blocks.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/debug-area.html">Learn more</a>
        /// </summary>
        /// <param name="text">Optional label displayed within the box.</param>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static IContainer DebugArea(this IContainer parent, string? text = null, string color = Colors.Red.Medium)
        {
            ColorValidator.Validate(color);
            
            var container = new Container();

            parent.Component(new DebugArea
            {
                Child = container,
                Text = text ?? string.Empty,
                Color = color
            });

            return container;
        }

        /// <summary>
        /// <para>Inserts a virtual debug element visible in the "element trace" provided along with the <see cref="DocumentLayoutException" />.</para>
        /// <para>Helps with understanding and navigation of that "element trace" tree hierarchy.</para>
        /// <a href="https://www.questpdf.com/api-reference/debug-pointer.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// This debug element does not appear in the final PDF output.
        /// </remarks>
        /// <param name="elementTraceText">Text visible somewhere in the "element trace" content identifying given document fragment.</param>
        public static IContainer DebugPointer(this IContainer parent, string label)
        {
            return parent.InspectionPointer(label, InspectorPointerType.Custom);
        }
        
        internal static IContainer InspectionPointer(this IContainer parent, string? label, InspectorPointerType type)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                return parent;
            
            if (string.IsNullOrWhiteSpace(label))
                return parent;
            
            return parent.Element(new InspectorPointer
            {
                Label = label.PrettifyName(),
                Type = type
            });
        }
    }
}