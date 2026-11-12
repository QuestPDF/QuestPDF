using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ColumnTests
    {
        #region Fluent API

        [TestCase(float.MinValue)]
        [TestCase(-5)]
        [TestCase(-float.Epsilon)]
        public void NegativeSpacingThrowsException(float spacingValue)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                EmptyContainer
                    .Create()
                    .Column(column =>
                    {
                        column.Spacing(spacingValue);
                    });
            });

            Assert.That(exception.Message, Does.Contain("The column spacing cannot be negative."));
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void SpacingMustBeFiniteNumber(float spacingValue)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                EmptyContainer
                    .Create()
                    .Column(column =>
                    {
                        column.Spacing(spacingValue);
                    });
            });

            Assert.That(exception.Message, Does.Contain("The column spacing must be a finite number."));
        }

        [TestCase(0)]
        [TestCase(float.Epsilon)]
        [TestCase(10)]
        public void ValidSpacingIsCorrectlyApplied(float spacingValue)
        {
            var container = EmptyContainer.Create();

            container.Column(column =>
            {
                column.Spacing(spacingValue);
            });

            var column = container.Child as Column;
            Assert.That(column?.Spacing, Is.EqualTo(spacingValue));
        }

        [Test]
        public void SpacingSupportsUnitConversion()
        {
            var container = EmptyContainer.Create();

            container.Column(column =>
            {
                column.Spacing(5, Unit.Inch);
            });

            var column = container.Child as Column;
            Assert.That(column?.Spacing, Is.EqualTo(360));
        }

        [Test]
        public void ItemsAreRegisteredInOrderAndReturnedToCaller()
        {
            var container = EmptyContainer.Create();
            
            IContainer first = null;
            IContainer second = null;
            IContainer third = null;

            container.Column(column =>
            {
                first = column.Item();
                second = column.Item();
                third = column.Item();
            });

            var column = container.Child as Column;
            Assert.That(column?.Items, Is.EqualTo([first, second, third]));
        }

        #endregion

        #region Proxy

        [Test]
        public void CreateProxy_WrapsEveryItem()
        {
            var first = new ElementMock { Id = "first" };
            var second = new ElementMock { Id = "second" };
            var third = new ElementMock { Id = "third" };

            var column = new Column
            {
                Items = { first, second, third }
            };

            column.CreateProxy(x => new ProxyMock { Child = x });

            Assert.That(column.Items, Is.All.TypeOf<ProxyMock>());

            var proxiedItems = column.Items.Cast<ProxyMock>().Select(x => x.Child);
            Assert.That(proxiedItems, Is.EqualTo([first, second, third]));
        }

        #endregion

        #region Measure

        [TestCase(-1, 300)]
        [TestCase(400, -1)]
        public void Measure_ReturnsWrap_WhenAvailableSpaceIsNegative(float width, float height)
        {
            var column = new Column
            {
                Items = { new ElementMock { MeasureFunc = _ => SpacePlan.FullRender(100, 50) } }
            };

            var result = column.Measure(new Size(width, height));

            Assert.That(result.Type, Is.EqualTo(SpacePlanType.Wrap));
            Assert.That(result.WrapReason, Is.EqualTo("The available space is negative."));
        }

        #endregion

        #region State

        [Test]
        public void ColumnSupportsStatefulOperations()
        {
            var container = EmptyContainer.Create();

            container.Column(column =>
            {
                foreach (var _ in Enumerable.Range(0, 10))
                    column.Item();
            });

            var column = container.Child as Column;
            Assert.That(column?.Items.Count, Is.EqualTo(10));

            // the state is the index of the first item that is not fully rendered yet
            Assert.That(column.GetState(), Is.EqualTo(0));

            column.SetState(7);
            Assert.That(column.GetState(), Is.EqualTo(7));
            Assert.That(column.CurrentRenderingIndex, Is.EqualTo(7));

            // the soft reset is used by repeating content (e.g. page header), the column must start again from its first item
            column.ResetState(hardReset: false);
            Assert.That(column.GetState(), Is.EqualTo(0));

            column.SetState(7);
            column.ResetState(hardReset: true);
            Assert.That(column.GetState(), Is.EqualTo(0));
        }

        #endregion
    }
}