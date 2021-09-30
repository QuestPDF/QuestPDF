using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class InternalLinkTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<InternalLink>();
        
        // TODO: consider tests for the Draw method
    }
}