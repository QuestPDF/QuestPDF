using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class SimpleRotateTests
    {
        #region Cumulative rotation
        
        [Test]
        public void RotateRightIsCumulative()
        {
            var container = EmptyContainer.Create();
            container
                .RotateRight()
                .RotateRight()
                .RotateRight()
                .RotateRight()
                .RotateRight();
        
            var rotation = container.Child as SimpleRotate;
            Assert.That(rotation?.TurnCount, Is.EqualTo(5));
        }
        
        [Test]
        public void RotateLeftIsCumulative()
        {
            var container = EmptyContainer.Create();
            container
                .RotateLeft()
                .RotateLeft()
                .RotateLeft()
                .RotateLeft()
                .RotateLeft()
                .RotateLeft();
        
            var rotation = container.Child as SimpleRotate;
            Assert.That(rotation?.TurnCount, Is.EqualTo(-6));
        }
        
        [Test]
        public void RotateRightAndLeftCanBeCombined()
        {
            var container = EmptyContainer.Create();
            container
                .RotateRight()
                .RotateRight()
                .RotateRight()
                .RotateRight()
                .RotateLeft()
                .RotateLeft()
                .RotateLeft();
        
            var rotation = container.Child as SimpleRotate;
            Assert.That(rotation?.TurnCount, Is.EqualTo(1));
        }
        
        #endregion
        
        #region Companion Hint
        
        [Test]
        public void NoRotationCompanionHint()
        {
            var container = EmptyContainer.Create();
            container.RotateRight().RotateLeft();
        
            var rotation = container.Child as SimpleRotate;
            Assert.That(rotation?.GetCompanionHint(), Is.EqualTo("No rotation"));
        }
        
        [Test]
        public void RotateRightCompanionHint()
        {
            var container = EmptyContainer.Create();
            container.RotateRight();
        
            var rotation = container.Child as SimpleRotate;
            Assert.That(rotation?.GetCompanionHint(), Is.EqualTo("90 deg clockwise"));
        }
        
        [Test]
        public void DoubleRotateLeftCompanionHint()
        {
            var container = EmptyContainer.Create();
            container.RotateLeft().RotateLeft();
        
            var rotation = container.Child as SimpleRotate;
            Assert.That(rotation?.GetCompanionHint(), Is.EqualTo("180 deg counter-clockwise"));
        }
        
        #endregion
    }
}