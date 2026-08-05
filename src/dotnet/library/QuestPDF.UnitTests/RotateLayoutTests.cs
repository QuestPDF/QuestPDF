using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class RotateLayoutTests
    {
        #region Cumulative rotation
        
        [Test]
        public void RotateRightIsCumulative()
        {
            var container = EmptyContainer.Create();
            container
                .RotateLayoutClockwise()
                .RotateLayoutClockwise()
                .RotateLayoutClockwise()
                .RotateLayoutClockwise()
                .RotateLayoutClockwise();
        
            var rotation = container.Child as RotateLayout;
            Assert.That(rotation?.TurnCount, Is.EqualTo(5));
        }
        
        [Test]
        public void RotateLeftIsCumulative()
        {
            var container = EmptyContainer.Create();
            container
                .RotateLayoutCounterclockwise()
                .RotateLayoutCounterclockwise()
                .RotateLayoutCounterclockwise()
                .RotateLayoutCounterclockwise()
                .RotateLayoutCounterclockwise()
                .RotateLayoutCounterclockwise();
        
            var rotation = container.Child as RotateLayout;
            Assert.That(rotation?.TurnCount, Is.EqualTo(-6));
        }
        
        [Test]
        public void RotateRightAndLeftCanBeCombined()
        {
            var container = EmptyContainer.Create();
            container
                .RotateLayoutClockwise()
                .RotateLayoutClockwise()
                .RotateLayoutClockwise()
                .RotateLayoutClockwise()
                .RotateLayoutCounterclockwise()
                .RotateLayoutCounterclockwise()
                .RotateLayoutCounterclockwise();
        
            var rotation = container.Child as RotateLayout;
            Assert.That(rotation?.TurnCount, Is.EqualTo(1));
        }
        
        #endregion
        
        #region Companion Hint
        
        [Test]
        public void NoRotationCompanionHint()
        {
            var container = EmptyContainer.Create();
            container.RotateLayoutClockwise().RotateLayoutCounterclockwise();
        
            var rotation = container.Child as RotateLayout;
            Assert.That(rotation?.GetCompanionHint(), Is.EqualTo("No rotation"));
        }
        
        [Test]
        public void RotateRightCompanionHint()
        {
            var container = EmptyContainer.Create();
            container.RotateLayoutClockwise();
        
            var rotation = container.Child as RotateLayout;
            Assert.That(rotation?.GetCompanionHint(), Is.EqualTo("90 deg clockwise"));
        }
        
        [Test]
        public void DoubleRotateLeftCompanionHint()
        {
            var container = EmptyContainer.Create();
            container.RotateLayoutCounterclockwise().RotateLayoutCounterclockwise();
        
            var rotation = container.Child as RotateLayout;
            Assert.That(rotation?.GetCompanionHint(), Is.EqualTo("180 deg counter-clockwise"));
        }
        
        #endregion
    }
}