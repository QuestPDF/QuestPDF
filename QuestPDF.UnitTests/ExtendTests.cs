using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ExtendTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new Extend().MeasureWithoutChild();
        
        [Test]
        public void Draw_ShouldHandleNullChild() => new Extend().DrawWithoutChild();
    }
}