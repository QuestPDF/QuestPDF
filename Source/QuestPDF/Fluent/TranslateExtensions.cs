using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class TranslateExtensions
    {
        private static IContainer Translate(this IContainer element, float x = 0, float y = 0)
        {
            var translate = element as Translate ?? new Translate();

            translate.TranslateX += x;
            translate.TranslateY += y;
            
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
            value = value.ToPoints(unit);
            return element.Translate(x: value);
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
            value = value.ToPoints(unit);
            return element.Translate(y: value);
        }
    }
}