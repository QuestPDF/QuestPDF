using NUnit.Framework;
using QuestPDF.Drawing;
using SkiaSharp;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class FontStyleSetTests
    {
        private void ExpectComparisonOrder(SKFontStyle target, params SKFontStyle[] styles)
        {
            for (int i = 0; i < styles.Length - 1; i++)
            {
                Assert.True(FontStyleSet.IsBetterMatch(target, styles[i], styles[i + 1]));
                Assert.False(FontStyleSet.IsBetterMatch(target, styles[i + 1], styles[i]));
            }
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_CondensedWidth()
        {
            ExpectComparisonOrder(
                new SKFontStyle(500, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 4, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 3, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 6, SKFontStyleSlant.Upright)
            );
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_ExpandedWidth()
        {
            ExpectComparisonOrder(
                new SKFontStyle(500, 6, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 6, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 7, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 8, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 5, SKFontStyleSlant.Upright)
            );
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_ItalicSlant()
        {
            ExpectComparisonOrder(
                new SKFontStyle(500, 5, SKFontStyleSlant.Italic),
                new SKFontStyle(500, 5, SKFontStyleSlant.Italic),
                new SKFontStyle(500, 5, SKFontStyleSlant.Oblique),
                new SKFontStyle(500, 5, SKFontStyleSlant.Upright)
            );
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_ObliqueSlant()
        {
            ExpectComparisonOrder(
                new SKFontStyle(500, 5, SKFontStyleSlant.Oblique),
                new SKFontStyle(500, 5, SKFontStyleSlant.Oblique),
                new SKFontStyle(500, 5, SKFontStyleSlant.Italic),
                new SKFontStyle(500, 5, SKFontStyleSlant.Upright)
            );
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_UprightSlant()
        {
            ExpectComparisonOrder(
                new SKFontStyle(500, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 5, SKFontStyleSlant.Oblique),
                new SKFontStyle(500, 5, SKFontStyleSlant.Italic)
            );
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_ThinWeight()
        {
            ExpectComparisonOrder(
                new SKFontStyle(300, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(300, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(200, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(100, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(400, 5, SKFontStyleSlant.Upright)
            );
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_RegularWeight()
        {
            ExpectComparisonOrder(
                new SKFontStyle(400, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(300, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(100, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(600, 5, SKFontStyleSlant.Upright)
            );
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_BoldWeight()
        {
            ExpectComparisonOrder(
                new SKFontStyle(600, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(600, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(700, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(800, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 5, SKFontStyleSlant.Upright)
            );
        }

        [Test]
        public void FontStyleSet_RespectsPriority()
        {
            ExpectComparisonOrder(
                new SKFontStyle(500, 5, SKFontStyleSlant.Upright),
                new SKFontStyle(600, 5, SKFontStyleSlant.Italic),
                new SKFontStyle(600, 6, SKFontStyleSlant.Upright),
                new SKFontStyle(500, 6, SKFontStyleSlant.Italic)
            );
        }
    }
}