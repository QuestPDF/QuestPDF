using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class PaddingTests
    {
        [TestCase(0, 0, 0, 0, "")]
        [TestCase(10, 0, 0, 0, "L=10")]
        [TestCase(0, 15, 0, 0, "T=15")]
        [TestCase(0, 0, 20, 0, "R=20")]
        [TestCase(0, 0, 0, 25, "B=25")]
        [TestCase(50, 0, 50, 0, "H=50")]
        [TestCase(0, 60, 0, 60, "V=60")]
        [TestCase(10, -20, 10, -30, "L=10   T=-20   R=10   B=-30")]
        [TestCase(-5, -10, 15, -10, "L=-5   T=-10   R=15   B=-10")]
        [TestCase(1.234f, -2.345f, 3.456f, -4.567f, "L=1.2   T=-2.3   R=3.5   B=-4.6")]
        [TestCase(5, 5, 5, 5, "A=5")]
        public void CompanionHint(float left, float top, float right, float bottom, string expected)
        {
            var container = EmptyContainer.Create();
            
            container
                .PaddingLeft(left)
                .PaddingTop(top)
                .PaddingRight(right)
                .PaddingBottom(bottom);
            
            var translationElement = container.Child as Padding;
            var companionHint = translationElement?.GetCompanionHint();
            
            Assert.That(companionHint, Is.EqualTo(expected));
        }
        
        #region Cumulative Property
        
        [Test]
        public void PaddingLeftIsCumulative()
        {
            var container = EmptyContainer.Create();
        
            container.PaddingLeft(-20).PaddingLeft(25).PaddingLeft(30);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Left, Is.EqualTo(35));
        }
        
        [Test]
        public void PaddingTopIsCumulative()
        {
            var container = EmptyContainer.Create();
        
            container.PaddingTop(20).PaddingTop(-25).PaddingTop(30);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Top, Is.EqualTo(25));
        }
        
        [Test]
        public void PaddingRightIsCumulative()
        {
            var container = EmptyContainer.Create();
        
            container.PaddingRight(20).PaddingRight(25).PaddingRight(-30);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Right, Is.EqualTo(15));
        }
        
        [Test]
        public void PaddingBottomIsCumulative()
        {
            var container = EmptyContainer.Create();
        
            container.PaddingBottom(-20).PaddingBottom(-25).PaddingBottom(30);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Bottom, Is.EqualTo(-15));
        }
        
        #endregion
        
        #region Simple Asignment
        
        [Test]
        public void PaddingVerticalShorthandWorksCorrectly()
        {
            var container = EmptyContainer.Create();

            container.PaddingVertical(123);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Left, Is.EqualTo(0));
            Assert.That(rowContainer?.Top, Is.EqualTo(123));
            Assert.That(rowContainer?.Right, Is.EqualTo(0));
            Assert.That(rowContainer?.Bottom, Is.EqualTo(123));
        }
        
        [Test]
        public void PaddingHorizontalShorthandWorksCorrectly()
        {
            var container = EmptyContainer.Create();

            container.PaddingHorizontal(234);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Left, Is.EqualTo(234));
            Assert.That(rowContainer?.Top, Is.EqualTo(0));
            Assert.That(rowContainer?.Right, Is.EqualTo(234));
            Assert.That(rowContainer?.Bottom, Is.EqualTo(0));
        }
        
        [Test]
        public void PaddingAllShorthandWorksCorrectly()
        {
            var container = EmptyContainer.Create();

            container.Padding(456);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Left, Is.EqualTo(456));
            Assert.That(rowContainer?.Top, Is.EqualTo(456));
            Assert.That(rowContainer?.Right, Is.EqualTo(456));
            Assert.That(rowContainer?.Bottom, Is.EqualTo(456));
        }
        
        #endregion
        
        #region Unit Conversion
        
        [Test]
        public void PaddingLeftAppliesUnitConversion()
        {
            var container = EmptyContainer.Create();

            container.PaddingLeft(2, Unit.Inch);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Left, Is.EqualTo(144));
        }
        
        [Test]
        public void PaddingTopAppliesUnitConversion()
        {
            var container = EmptyContainer.Create();

            container.PaddingTop(3, Unit.Inch);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Top, Is.EqualTo(216));
        }
        
        [Test]
        public void PaddingRightAppliesUnitConversion()
        {
            var container = EmptyContainer.Create();

            container.PaddingRight(4, Unit.Inch);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Right, Is.EqualTo(288));
        }
        
        [Test]
        public void PaddingBottomAppliesUnitConversion()
        {
            var container = EmptyContainer.Create();

            container.PaddingBottom(5, Unit.Inch);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Bottom, Is.EqualTo(360));
        }
        
        [Test]
        public void PaddingVerticalAppliesUnitConversion()
        {
            var container = EmptyContainer.Create();

            container.PaddingVertical(6, Unit.Inch);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Top, Is.EqualTo(432));
            Assert.That(rowContainer?.Bottom, Is.EqualTo(432));
        }
        
        [Test]
        public void PaddingHorizontalAppliesUnitConversion()
        {
            var container = EmptyContainer.Create();

            container.PaddingHorizontal(7, Unit.Inch);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Left, Is.EqualTo(504));
            Assert.That(rowContainer?.Right, Is.EqualTo(504));
        }
        
        [Test]
        public void PaddingAllAppliesUnitConversion()
        {
            var container = EmptyContainer.Create();

            container.Padding(8, Unit.Inch);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Left, Is.EqualTo(576));
            Assert.That(rowContainer?.Top, Is.EqualTo(576));
            Assert.That(rowContainer?.Right, Is.EqualTo(576));
            Assert.That(rowContainer?.Bottom, Is.EqualTo(576));
        }
        
        #endregion
        
        [Test]
        public void PaddingAppliesCorrectValues()
        {
            var container = EmptyContainer.Create();

            container
                .PaddingLeft(20)
                .PaddingTop(25)
                .PaddingRight(30)
                .PaddingBottom(35)
                .PaddingVertical(-5)
                .PaddingHorizontal(-15)
                .Padding(10);
        
            var rowContainer = container.Child as Padding;
            Assert.That(rowContainer?.Left, Is.EqualTo(15));
            Assert.That(rowContainer?.Top, Is.EqualTo(30));
            Assert.That(rowContainer?.Right, Is.EqualTo(25));
            Assert.That(rowContainer?.Bottom, Is.EqualTo(40));
        }
    }
}