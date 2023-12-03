using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Drawing;
using SkiaSharp;
using static SkiaSharp.SKFontStyleSlant;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class FontStyleSetTests
    {
        private static void ExpectComparisonOrder(SKFontStyle target, SKFontStyle[] styles)
        {
            for (var i = 0; i < styles.Length - 1; i++)
            {
                var currentStyle = styles[i];
                var nextStyle = styles[i + 1];
                
                FontStyleSet.IsBetterMatch(target, currentStyle, nextStyle).Should().BeTrue();
                FontStyleSet.IsBetterMatch(target, nextStyle, currentStyle).Should().BeFalse();
            }
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_CondensedWidth()
        {
            var styles = new[]
            {
                new SKFontStyle(500, 5, Upright),
                new SKFontStyle(500, 4, Upright),
                new SKFontStyle(500, 3, Upright),
                new SKFontStyle(500, 6, Upright)
            };
            
            ExpectComparisonOrder(new SKFontStyle(500, 5, Upright), styles);
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_ExpandedWidth()
        {
            var styles = new[]
            {
                new SKFontStyle(500, 6, Upright),
                new SKFontStyle(500, 7, Upright),
                new SKFontStyle(500, 8, Upright),
                new SKFontStyle(500, 5, Upright)
            };
            
            ExpectComparisonOrder(new SKFontStyle(500, 6, Upright), styles);
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_ItalicSlant()
        {
            var styles = new[]
            {
                new SKFontStyle(500, 5, Italic),
                new SKFontStyle(500, 5, Oblique),
                new SKFontStyle(500, 5, Upright)
            };
        
            ExpectComparisonOrder(new SKFontStyle(500, 5, Italic), styles);
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_ObliqueSlant()
        {
            var styles = new[]
            {
                new SKFontStyle(500, 5, Oblique),
                new SKFontStyle(500, 5, Italic),
                new SKFontStyle(500, 5, Upright)
            };
        
            ExpectComparisonOrder(new SKFontStyle(500, 5, Oblique), styles);
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_UprightSlant()
        {
            var styles = new[]
            {
                new SKFontStyle(500, 5, Upright),
                new SKFontStyle(500, 5, Oblique),
                new SKFontStyle(500, 5, Italic)
            };
        
            ExpectComparisonOrder(new SKFontStyle(500, 5, Upright), styles);
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_ThinWeight()
        {
            var styles = new[]
            {
                new SKFontStyle(300, 5, Upright),
                new SKFontStyle(200, 5, Upright),
                new SKFontStyle(100, 5, Upright),
                new SKFontStyle(400, 5, Upright)
            };
        
            ExpectComparisonOrder(new SKFontStyle(300, 5, Upright), styles);
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_RegularWeight()
        {
            var styles = new[]
            {
                new SKFontStyle(500, 5, Upright),
                new SKFontStyle(300, 5, Upright),
                new SKFontStyle(100, 5, Upright),
                new SKFontStyle(600, 5, Upright)
            };
        
            ExpectComparisonOrder(new SKFontStyle(400, 5, Upright), styles);
        }

        [Test]
        public void FontStyleSet_IsBetterMatch_BoldWeight()
        {
            var styles = new[]
            {
                new SKFontStyle(600, 5, Upright),
                new SKFontStyle(700, 5, Upright),
                new SKFontStyle(800, 5, Upright),
                new SKFontStyle(500, 5, Upright)
            };
        
            ExpectComparisonOrder(new SKFontStyle(600, 5, Upright), styles);
        }

        [Test]
        public void FontStyleSet_RespectsPriority()
        {
            var styles = new[]
            {
                new SKFontStyle(600, 5, Italic),
                new SKFontStyle(600, 6, Upright),
                new SKFontStyle(500, 6, Italic)
            };
        
            ExpectComparisonOrder(new SKFontStyle(500, 5, Upright), styles);
        }
    }
}