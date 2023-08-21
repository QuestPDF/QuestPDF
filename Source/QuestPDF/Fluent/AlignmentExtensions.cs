using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class AlignmentExtensions
    {
        private static IContainer Alignment(this IContainer element, Action<Alignment> handler)
        {
            var alignment = element as Alignment ?? new Alignment();
            handler(alignment);
            
            return element.Element(alignment);
        }
        
        /// <summary>
        /// Aligns content horizontally to the left side.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignLeft(this IContainer element)
        {
            return element.Alignment(x => x.Horizontal = HorizontalAlignment.Left);
        }
        
        /// <summary>
        /// Aligns content horizontally to the center, ensuring equal space on both left and right sides.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignCenter(this IContainer element)
        {
            return element.Alignment(x => x.Horizontal = HorizontalAlignment.Center);
        }
        
        /// <summary>
        /// Aligns its content horizontally to the right side.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignRight(this IContainer element)
        {
            return element.Alignment(x => x.Horizontal = HorizontalAlignment.Right);
        }
        
        /// <summary>
        /// Aligns content vertically to the upper side.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignTop(this IContainer element)
        {
            return element.Alignment(x => x.Vertical = VerticalAlignment.Top);
        }
        
        /// <summary>
        /// Aligns content vertically to the center, ensuring equal space above and below.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignMiddle(this IContainer element)
        {
            return element.Alignment(x => x.Vertical = VerticalAlignment.Middle);
        }
        
        /// <summary>
        /// Aligns content vertically to the bottom side.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignBottom(this IContainer element)
        {
            return element.Alignment(x => x.Vertical = VerticalAlignment.Bottom);
        }
    }
}