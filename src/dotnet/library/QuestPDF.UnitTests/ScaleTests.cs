using System;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ScaleTests
    {
        [TestCase(2, 2, ExpectedResult = "A=2")]
        [TestCase(-3, -3, ExpectedResult = "A=-3")]
        [TestCase(4, 1, ExpectedResult = "H=4")]
        [TestCase(1, -2, ExpectedResult = "V=-2")]
        [TestCase(4, 5, ExpectedResult = "H=4   V=5")]
        [TestCase(1.2345f, -2.3456f, ExpectedResult = "H=1.2   V=-2.3")]
        public string CompanionHint(float horizontal, float vertical)
        {
            var container = EmptyContainer.Create();

            container.ScaleHorizontal(horizontal).ScaleVertical(vertical);
            
            var translationElement = container.Child as Scale;
            return translationElement?.GetCompanionHint();
        }     
        
        #region Cumulative Property
        
        [Test]
        public void HorizontalScaleIsCumulative()
        {
            var container = EmptyContainer.Create();
        
            container.ScaleHorizontal(3).ScaleHorizontal(0.5f).ScaleHorizontal(-4);
        
            var rowContainer = container.Child as Scale;
            Assert.That(rowContainer?.ScaleX, Is.EqualTo(-6));
            Assert.That(rowContainer?.ScaleY, Is.EqualTo(1));
        }
        
        [Test]
        public void VerticalScaleIsCumulative()
        {
            var container = EmptyContainer.Create();
        
            container.ScaleVertical(2).ScaleVertical(-0.25f).ScaleVertical(-3f);
        
            var rowContainer = container.Child as Scale;
            Assert.That(rowContainer?.ScaleX, Is.EqualTo(1));
            Assert.That(rowContainer?.ScaleY, Is.EqualTo(1.5f));
        }
        
        [Test]
        public void ScaleIsCumulative()
        {
            var container = EmptyContainer.Create();
        
            container.ScaleHorizontal(-5f).ScaleVertical(3).Scale(-0.25f);
        
            var rowContainer = container.Child as Scale;
            Assert.That(rowContainer?.ScaleX, Is.EqualTo(1.25f));
            Assert.That(rowContainer?.ScaleY, Is.EqualTo(-0.75f));
        }
        
        #endregion
        
        #region Flip
        
        [Test]
        public void FlipHorizontalAppliesCorrectScale()
        {
            var container = EmptyContainer.Create();
        
            container.FlipHorizontal();
        
            var rowContainer = container.Child as Scale;
            Assert.That(rowContainer?.ScaleX, Is.EqualTo(-1));
            Assert.That(rowContainer?.ScaleY, Is.EqualTo(1));
        }
        
        [Test]
        public void FlipVerticalAppliesCorrectScale()
        {
            var container = EmptyContainer.Create();
        
            container.FlipVertical();
        
            var rowContainer = container.Child as Scale;
            Assert.That(rowContainer?.ScaleX, Is.EqualTo(1));
            Assert.That(rowContainer?.ScaleY, Is.EqualTo(-1));
        }
        
        [Test]
        public void FlipOverAppliesCorrectScale()
        {
            var container = EmptyContainer.Create();
        
            container.FlipOver();
        
            var rowContainer = container.Child as Scale;
            Assert.That(rowContainer?.ScaleX, Is.EqualTo(-1));
            Assert.That(rowContainer?.ScaleY, Is.EqualTo(-1));
        }
        
        #endregion

        #region Zero Scale Validation

        [Test]
        public void ScaleCannotBeZero()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                EmptyContainer.Create().Scale(0);
            });
            
            Assert.That(exception.Message, Is.EqualTo("Vertical scale factor cannot be zero. (Parameter 'factor')"));
        }
        
        [Test]
        public void VerticalScaleCannotBeZero()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                EmptyContainer.Create().ScaleVertical(0);
            });
            
            Assert.That(exception.Message, Is.EqualTo("Vertical scale factor cannot be zero. (Parameter 'factor')"));
        }
        
        [Test]
        public void HorizontalScaleCannotBeZero()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                EmptyContainer.Create().ScaleHorizontal(0);
            });
            
            Assert.That(exception.Message, Is.EqualTo("Vertical scale factor cannot be zero. (Parameter 'factor')"));
        }

        #endregion
    }
}