using System;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class GridTests
    {
        #region Alignment
        
        [Test]
        public void AlignLeft()
        {
            // arrange
            var structure = new Container();

            var childA = TestPlan.CreateUniqueElement();
            var childB = TestPlan.CreateUniqueElement();
            var childC = TestPlan.CreateUniqueElement();
            var childD = TestPlan.CreateUniqueElement();
            var childE = TestPlan.CreateUniqueElement();

            // act
            structure
                .Grid(grid =>
                {
                    grid.AlignLeft();
                    
                    grid.Element(6).Element(childA);
                    grid.Element(4).Element(childB);
                    grid.Element(4).Element(childC);
                    grid.Element(2).Element(childD);
                    grid.Element(8).Element(childE);
                });
            
            // assert
            var expected = new Container();
            
            expected.Stack(stack =>
            {
                stack.Element().Row(row =>
                {
                    row.RelativeColumn(6).Element(childA);
                    row.RelativeColumn(4).Element(childB);
                    row.RelativeColumn(2).Element(new Empty());
                });
                
                stack.Element().Row(row =>
                {
                    row.RelativeColumn(4).Element(childC);
                    row.RelativeColumn(2).Element(childD);
                    row.RelativeColumn(6).Element(new Empty());
                });
                
                stack.Element().Row(row =>
                {
                    row.RelativeColumn(8).Element(childE);
                    row.RelativeColumn(4).Element(new Empty());
                });
            });
            
            structure.Child.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering().IncludingAllRuntimeProperties());
        }
        
        [Test]
        public void AlignCenter()
        {
            // arrange
            var structure = new Container();

            var childA = TestPlan.CreateUniqueElement();
            var childB = TestPlan.CreateUniqueElement();
            var childC = TestPlan.CreateUniqueElement();
            var childD = TestPlan.CreateUniqueElement();
            var childE = TestPlan.CreateUniqueElement();

            // act
            structure
                .Grid(grid =>
                {
                    grid.AlignCenter();
                    
                    grid.Element(6).Element(childA);
                    grid.Element(4).Element(childB);
                    grid.Element(4).Element(childC);
                    grid.Element(2).Element(childD);
                    grid.Element(8).Element(childE);
                });
            
            // assert
            var expected = new Container();
            
            expected.Stack(stack =>
            {
                stack.Element().Row(row =>
                {
                    row.RelativeColumn(1).Element(new Empty());
                    row.RelativeColumn(6).Element(childA);
                    row.RelativeColumn(4).Element(childB);
                    row.RelativeColumn(1).Element(new Empty());
                });
                
                stack.Element().Row(row =>
                {
                    row.RelativeColumn(3).Element(new Empty());
                    row.RelativeColumn(4).Element(childC);
                    row.RelativeColumn(2).Element(childD);
                    row.RelativeColumn(3).Element(new Empty());
                });
                
                stack.Element().Row(row =>
                {
                    row.RelativeColumn(2).Element(new Empty());
                    row.RelativeColumn(8).Element(childE);
                    row.RelativeColumn(2).Element(new Empty());
                });
            });

            structure.Child.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering().IncludingAllRuntimeProperties());
        }
        
        [Test]
        public void AlignRight()
        {
            // arrange
            var structure = new Container();

            var childA = TestPlan.CreateUniqueElement();
            var childB = TestPlan.CreateUniqueElement();
            var childC = TestPlan.CreateUniqueElement();
            var childD = TestPlan.CreateUniqueElement();
            var childE = TestPlan.CreateUniqueElement();

            // act
            structure
                .Grid(grid =>
                {
                    grid.AlignRight();
                    
                    grid.Element(6).Element(childA);
                    grid.Element(4).Element(childB);
                    grid.Element(4).Element(childC);
                    grid.Element(2).Element(childD);
                    grid.Element(8).Element(childE);
                });
            
            // assert
            var expected = new Container();
            
            expected.Container().Stack(stack =>
            {
                stack.Element().Row(row =>
                {
                    row.RelativeColumn(2).Element(new Empty());
                    row.RelativeColumn(6).Element(childA);
                    row.RelativeColumn(4).Element(childB);
                });
                
                stack.Element().Row(row =>
                {
                    row.RelativeColumn(6).Element(new Empty());
                    row.RelativeColumn(4).Element(childC);
                    row.RelativeColumn(2).Element(childD);
                });
                
                stack.Element().Row(row =>
                {
                    row.RelativeColumn(4).Element(new Empty());
                    row.RelativeColumn(8).Element(childE);
                });
            });
            
            structure.Child.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering().IncludingAllRuntimeProperties());
        }
        
        #endregion
    }
}