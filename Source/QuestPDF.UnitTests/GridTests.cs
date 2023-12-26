using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
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
                    
                    grid.Item(6).Element(childA);
                    grid.Item(4).Element(childB);
                    grid.Item(4).Element(childC);
                    grid.Item(2).Element(childD);
                    grid.Item(8).Element(childE);
                });
            
            // assert
            var expected = new Container();
            
            expected.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem(6).Element(childA);
                    row.RelativeItem(4).Element(childB);
                    row.RelativeItem(2);
                });
                
                column.Item().Row(row =>
                {
                    row.RelativeItem(4).Element(childC);
                    row.RelativeItem(2).Element(childD);
                    row.RelativeItem(6);
                });
                
                column.Item().Row(row =>
                {
                    row.RelativeItem(8).Element(childE);
                    row.RelativeItem(4);
                });
            });
            
            TestPlan.CompareOperations(structure, expected);
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
                    
                    grid.Item(6).Element(childA);
                    grid.Item(4).Element(childB);
                    grid.Item(4).Element(childC);
                    grid.Item(2).Element(childD);
                    grid.Item(8).Element(childE);
                });
            
            // assert
            var expected = new Container();
            
            expected.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem(1);
                    row.RelativeItem(6).Element(childA);
                    row.RelativeItem(4).Element(childB);
                    row.RelativeItem(1);
                });
                
                column.Item().Row(row =>
                {
                    row.RelativeItem(3);
                    row.RelativeItem(4).Element(childC);
                    row.RelativeItem(2).Element(childD);
                    row.RelativeItem(3);
                });
                
                column.Item().Row(row =>
                {
                    row.RelativeItem(2);
                    row.RelativeItem(8).Element(childE);
                    row.RelativeItem(2);
                });
            });

            TestPlan.CompareOperations(structure, expected);
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
                    
                    grid.Item(6).Element(childA);
                    grid.Item(4).Element(childB);
                    grid.Item(4).Element(childC);
                    grid.Item(2).Element(childD);
                    grid.Item(8).Element(childE);
                });
            
            // assert
            var expected = new Container();
            
            expected.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem(2);
                    row.RelativeItem(6).Element(childA);
                    row.RelativeItem(4).Element(childB);
                });
                
                column.Item().Row(row =>
                {
                    row.RelativeItem(6);
                    row.RelativeItem(4).Element(childC);
                    row.RelativeItem(2).Element(childD);
                });
                
                column.Item().Row(row =>
                {
                    row.RelativeItem(4);
                    row.RelativeItem(8).Element(childE);
                });
            });
            
            TestPlan.CompareOperations(structure, expected);
        }
        
        #endregion
        
        #region Spacing
        
        [Test]
        public void Spacing()
        {
            // arrange
            var structure = new Container();

            var childA = TestPlan.CreateUniqueElement();
            var childB = TestPlan.CreateUniqueElement();
            var childC = TestPlan.CreateUniqueElement();
            var childD = TestPlan.CreateUniqueElement();

            // act
            structure
                .Grid(grid =>
                {
                    grid.Columns(16);
                    grid.AlignCenter();
                    
                    grid.VerticalSpacing(20);
                    grid.HorizontalSpacing(30);

                    grid.Item(5).Element(childA);
                    grid.Item(5).Element(childB);
                    grid.Item(10).Element(childC);
                    grid.Item(12).Element(childD);
                });
            
            // assert
            var expected = new Container();
            
            expected.Column(column =>
            {
                column.Spacing(20);
                
                column.Item().Row(row =>
                {
                    row.Spacing(30);
                    
                    row.RelativeItem(3);
                    row.RelativeItem(5).Element(childA);
                    row.RelativeItem(5).Element(childB);
                    row.RelativeItem(3);
                });
                
                column.Item().Row(row =>
                {
                    row.Spacing(30);
                    
                    row.RelativeItem(3);
                    row.RelativeItem(10).Element(childC);
                    row.RelativeItem(3);
                });
                
                column.Item().Row(row =>
                {
                    row.Spacing(30);
                    
                    row.RelativeItem(2);
                    row.RelativeItem(12).Element(childD);
                    row.RelativeItem(2);
                });
            });
            
            TestPlan.CompareOperations(structure, expected);
        }
        
        #endregion
    }
}