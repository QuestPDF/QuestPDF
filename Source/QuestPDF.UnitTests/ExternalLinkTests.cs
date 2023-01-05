using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ExternalLinkTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<Hyperlink>();
        
        // TODO: consider tests for the Draw method
    }
}