using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ScaleExtensions
    {
        private static IContainer ScaleValue(this IContainer element, float x = 1, float y = 1)
        {
            var scale = element as Scale ?? new Scale();

            scale.ScaleX *= x;
            scale.ScaleY *= y;
            
            return element.Element(scale);
        }
        
        /// <summary>
        /// Scales its inner content proportionally.
        /// <a href="https://www.questpdf.com/api-reference/scale.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="scale.remarks"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="scale.factorParam"]/*' />
        public static IContainer Scale(this IContainer element, float factor)
        {
            return element.ScaleValue(x: factor, y: factor);
        }
        
        /// <summary>
        /// Scales the available horizontal space (along the X axis), causing content to appear expanded or squished, rather than simply larger or smaller.
        /// <a href="https://www.questpdf.com/api-reference/scale.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="scale.remarks"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="scale.factorParam"]/*' />
        public static IContainer ScaleHorizontal(this IContainer element, float factor)
        {
            return element.ScaleValue(x: factor);
        }
        
        /// <summary>
        /// Scales the available vertical space (along the Y axis), causing content to appear expanded or squished, rather than simply larger or smaller.
        /// <a href="https://www.questpdf.com/api-reference/scale.html">Learn more</a> 
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="scale.remarks"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="scale.factorParam"]/*' />
        public static IContainer ScaleVertical(this IContainer element, float factor)
        {
            return element.ScaleValue(y: factor);
        }
        
        /// <summary>
        /// Flips its content to create a mirror image along the Y axis, swapping elements from left to right.
        /// <a href="https://www.questpdf.com/api-reference/flip.html">Learn more</a>
        /// </summary>
        /// <example>
        /// Elements on the left will appear on the right.
        /// </example>
        public static IContainer FlipHorizontal(this IContainer element)
        {
            return element.ScaleValue(x: -1);
        }
        
        /// <summary>
        /// Flips its content to create a mirror image along the X axis, moving elements from the top to the bottom.
        /// <a href="https://www.questpdf.com/api-reference/flip.html">Learn more</a>
        /// </summary>
        /// <example>
        /// Elements at the top will be positioned at the bottom.
        /// </example>
        public static IContainer FlipVertical(this IContainer element)
        {
            return element.ScaleValue(y: -1);
        }
        
        /// <summary>
        /// Creates a mirror image of its content across both axes.
        /// <a href="https://www.questpdf.com/api-reference/flip.html">Learn more</a>
        /// </summary>
        /// <example>
        /// Elements originally in the top-left corner will be positioned in the bottom-right corner.
        /// </example>
        public static IContainer FlipOver(this IContainer element)
        {
            return element.ScaleValue(x: -1, y: -1);
        }
    }
}