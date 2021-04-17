using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class RowTests
    {
        #region Spacing
        
        [Test]
        public void Fluent_WithoutSpacing()
        {
            // arrange
            var structure = new Container();

            var childA = TestPlan.CreateUniqueElement();
            var childB = TestPlan.CreateUniqueElement();
            var childC = TestPlan.CreateUniqueElement();

            // act
            structure
                .Row(stack =>
                {
                    stack.Spacing(0);

                    stack.ConstantColumn(100).Element(childA);
                    stack.RelativeColumn(2).Element(childB);
                    stack.RelativeColumn(3).Element(childC);
                });
            
            // assert
            var expected = new Row()
            {
                Children = new List<RowElement>
                {
                    new ConstantRowElement(100)
                    {
                        Child = childA
                    },
                    new RelativeRowElement(2)
                    {
                        Child = childB
                    },
                    new RelativeRowElement(3)
                    {
                        Child = childC
                    },
                }
            };
            
            structure.Child.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering().IncludingAllRuntimeProperties());
        }
        
        [Test]
        public void Fluent_WithSpacing()
        {
            // arrange
            var structure = new Container();

            var childA = TestPlan.CreateUniqueElement();
            var childB = TestPlan.CreateUniqueElement();
            var childC = TestPlan.CreateUniqueElement();

            // act
            structure
                .Row(stack =>
                {
                    stack.Spacing(25);

                    stack.ConstantColumn(100).Element(childA);
                    stack.RelativeColumn(2).Element(childB);
                    stack.RelativeColumn(3).Element(childC);
                });
            
            // assert
            var expected = new Padding
            {
                Right = -25,
                Child = new Row()
                {
                    Children = new List<RowElement>
                    {
                        new ConstantRowElement(100)
                        {
                            Child = childA
                        },
                        new ConstantRowElement(25),
                        new RelativeRowElement(2)
                        {
                            Child = childB
                        },
                        new ConstantRowElement(25),
                        new RelativeRowElement(3)
                        {
                            Child = childC
                        },
                        new ConstantRowElement(25)
                    }
                }
            };
            
            structure.Child.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering().IncludingAllRuntimeProperties());
        }
        
        #endregion
    }
}