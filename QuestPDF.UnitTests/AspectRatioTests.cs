using Moq;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class AspectRatioTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new AspectRatio().MeasureWithoutChild();
        
        [Test]
        public void Draw_ShouldHandleNullChild() => new AspectRatio().DrawWithoutChild();
    }
}