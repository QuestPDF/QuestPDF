using System;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class TranslateTests
    {
        [TestCase(0, 0, "")]
        [TestCase(5, 0, "X=5")]
        [TestCase(-10, 0, "X=-10")]
        [TestCase(0, 15, "Y=15")]
        [TestCase(0, -20, "Y=-20")]
        [TestCase(30, -40, "X=30   Y=-40")]
        public void CompanionHint(float x, float y, string expected)
        {
            var container = EmptyContainer.Create();
            
            container.TranslateX(x).TranslateY(y);
            
            var translationElement = container.Child as Translate;
            var companionHint = translationElement?.GetCompanionHint();
            
            Assert.That(companionHint, Is.EqualTo(expected));
        }
        
        [Test]
        public void HorizontalTranslationIsCumulative()
        {
            var container = EmptyContainer.Create();
        
            container.TranslateX(-5).TranslateX(10).TranslateX(20);
        
            var rowContainer = container.Child as Translate;
            Assert.That(rowContainer?.TranslateX, Is.EqualTo(25));
        }
        
        [Test]
        public void VerticalTranslationIsCumulative()
        {
            var container = EmptyContainer.Create();
        
            container.TranslateY(5).TranslateY(-10).TranslateY(20);
        
            var rowContainer = container.Child as Translate;
            Assert.That(rowContainer?.TranslateY, Is.EqualTo(15));
        }
        
        [Test]
        public void HorizontalTranslationSupportsUnitConversion()
        {
            var container = EmptyContainer.Create();
        
            container.TranslateX(2, Unit.Inch);
        
            var rowContainer = container.Child as Translate;
            Assert.That(rowContainer?.TranslateX, Is.EqualTo(144));
        }
        
        [Test]
        public void VerticalTranslationSupportsUnitConversion()
        {
            var container = EmptyContainer.Create();
        
            container.TranslateY(3, Unit.Inch);
        
            var rowContainer = container.Child as Translate;
            Assert.That(rowContainer?.TranslateY, Is.EqualTo(216));
        }
    }
}