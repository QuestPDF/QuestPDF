using FluentAssertions;
using NUnit.Framework;

namespace QuestPDF.UnitTests
{
    [SetUpFixture]
    public class TestsBase
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            AssertionOptions.AssertEquivalencyUsing(options => options
                .IncludingNestedObjects()
                .IncludingInternalProperties()
                .AllowingInfiniteRecursion()
                .RespectingRuntimeTypes()
                .WithTracing()
                .WithStrictOrdering());
        }
    }
}