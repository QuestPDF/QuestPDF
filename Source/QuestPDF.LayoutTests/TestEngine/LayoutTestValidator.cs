using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal static class LayoutTestValidator
{
    public static void Validate(LayoutTestResult result)
    {
        if (result.ActualLayout.GeneratesInfiniteLayout && !result.ExpectedLayout.GeneratesInfiniteLayout)
            throw new LayoutTestException("Provided content generates unexpected infinite layout");
        
        if (!result.ActualLayout.GeneratesInfiniteLayout && result.ExpectedLayout.GeneratesInfiniteLayout)
            throw new LayoutTestException("Provided content is expected to generate infinite layout but it does not");

        var actualPages = result.ActualLayout.Pages;
        var expectedPages = result.ExpectedLayout.Pages;
        
        if (actualPages.Count != expectedPages.Count)
            throw new LayoutTestException($"Content return layout with {actualPages.Count} pages but expected {expectedPages.Count} pages");
        
        foreach (var i in Enumerable.Range(0, actualPages.Count))
        {
            try
            {
                var actualPage = actualPages.ElementAt(i);
                var expectedPage = expectedPages.ElementAt(i);

                ValidatePage(actualPage, expectedPage);
            }
            catch (LayoutTestException exception)
            {
                throw new LayoutTestException($"Found issue on page number {i + 1}: {exception.Message}");
            }
            catch (Exception exception)
            {
                throw new LayoutTestException($"Encountered exception during validating page number {i + 1}", exception);
            }
        }

        static void ValidatePage(LayoutTestResult.PageLayout actualLayout, LayoutTestResult.PageLayout expectedLayout)
        {
            if (Math.Abs(actualLayout.RequiredArea.Width - expectedLayout.RequiredArea.Width) > Size.Epsilon)
                throw new LayoutTestException($"Taken horizontal area is equal to {actualLayout.RequiredArea.Width} but expected {expectedLayout.RequiredArea.Width}");
            
            if (Math.Abs(actualLayout.RequiredArea.Height - expectedLayout.RequiredArea.Height) > Size.Epsilon)
                throw new LayoutTestException($"Taken vertical area is equal to {actualLayout.RequiredArea.Height} but expected {expectedLayout.RequiredArea.Height}");
            
            if (actualLayout.Mocks.Count != expectedLayout.Mocks.Count)
                throw new LayoutTestException($"Visible {actualLayout.Mocks.Count} mocks but expected {expectedLayout.Mocks.Count}");

            ValidatePositionAndSizeOfMocks(actualLayout, expectedLayout);
            ValidateDrawingOrder(actualLayout, expectedLayout);
        }

        static void ValidatePositionAndSizeOfMocks(LayoutTestResult.PageLayout actualLayout, LayoutTestResult.PageLayout expectedLayout)
        {
            foreach (var expectedMock in expectedLayout.Mocks)
            {
                var matchingActualMock = actualLayout
                    .Mocks
                    .Where(x => x.MockId == expectedMock.MockId)
                    .Where(x => Position.Equal(x.Position, expectedMock.Position))
                    .Where(x => Size.Equal(x.Size, expectedMock.Size))
                    .Count();

                if (matchingActualMock == 0)
                    throw new Exception($"Cannot find '{expectedMock.MockId}' mock on position {expectedMock.Position} and size {expectedMock.Size}");
                
                if (matchingActualMock > 1)
                    throw new Exception($"Found multiple '{expectedMock.MockId}' mocks on position {expectedMock.Position} and size {expectedMock.Size}");
            }
        }

        static void ValidateDrawingOrder(LayoutTestResult.PageLayout actualLayout, LayoutTestResult.PageLayout expectedLayout)
        {
            var actualOverlaps = GetOverlappingItems(actualLayout.Mocks).ToList();
            var expectedOverlaps = GetOverlappingItems(expectedLayout.Mocks).ToList();
            
            foreach (var expectedOverlap in expectedOverlaps)
            {
                var matchingActualElements = actualOverlaps.Count(actualOverlap => actualOverlap == expectedOverlap);

                if (matchingActualElements != 1)
                    throw new Exception($"Mock '{expectedOverlap.belowMockId}' should be visible below '{expectedOverlap.aboveMockId}' mock");
            }
            
            IEnumerable<(string belowMockId, string aboveMockId)> GetOverlappingItems(ICollection<LayoutTestResult.MockLayoutPosition> items)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    for (var j = i + 1; j < items.Count; j++)
                    {
                        var beforeChild = items.ElementAt(i);
                        var afterChild = items.ElementAt(j);

                        var beforeBoundingBox = BoundingBox.From(beforeChild.Position, beforeChild.Size);
                        var afterBoundingBox = BoundingBox.From(afterChild.Position, afterChild.Size);

                        var intersection = BoundingBoxExtensions.Intersection(beforeBoundingBox, afterBoundingBox);
                        
                        if (intersection == null)
                            continue;

                        yield return (beforeChild.MockId, afterChild.MockId);
                    }
                }
            }
        }
    }
}