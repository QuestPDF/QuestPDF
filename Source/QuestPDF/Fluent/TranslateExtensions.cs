using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class TranslateExtensions
    {
        private static IContainer Translate(this IContainer element, Action<Translate> handler)
        {
            var translate = element as Translate ?? new Translate();
            handler(translate);
            
            return element.Element(translate);
        }

        /// <summary>
        /// Moves content along the horizontal axis.
        /// A positive value moves content to the right; a negative value moves it to the left.
        /// Does not alter the available space.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/translate.html">Learn more</a>
        /// </summary>
        public static IContainer TranslateX(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Translate(x => x.TranslateX += value.ToPoints(unit));
        }
   
        /// <summary>
        /// Moves content along the vertical axis.
        /// A positive value moves content downwards, a negative value moves it upwards.
        /// Does not alter the available space.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/translate.html">Learn more</a>
        /// </summary>
        public static IContainer TranslateY(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Translate(x => x.TranslateY += value.ToPoints(unit));
        }
    }
}