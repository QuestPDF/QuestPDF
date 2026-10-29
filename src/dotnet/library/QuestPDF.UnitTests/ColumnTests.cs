using System;
using System.Linq;
using NUnit.Framework;
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

            container.Column(column =>
            {
                first = column.Item();
                second = column.Item();
            });

            var column = container.Child as Column;
            Assert.That(column?.Items, Is.EqualTo([first, second]));
        }

        #endregion

        #region Proxy

        [Test]
        public void CreateProxy_WrapsEveryItem()
        {
            var first = new ElementMock { Id = "first" };
            var second = new ElementMock { Id = "second" };

            var column = new Column
            {
                Items = { first, second }
            };

            column.CreateProxy(x => new ProxyMock { Child = x });

            Assert.That(column.Items, Is.All.TypeOf<ProxyMock>());

            var proxiedItems = column.Items.Cast<ProxyMock>().Select(x => x.Child);
            Assert.That(proxiedItems, Is.EqualTo([first, second]));
        }

        #endregion
    }
}