using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class PageBreakTests
    {
        [Test]
        public void Measure_ShouldReturnWrapThenFullRender()
        {
            var pageBreak = new PageBreak();
            
            pageBreak.Measure(Helpers.RandomSize).Should().BeOfType<PartialRender>();
            pageBreak.Draw(null, Size.Zero);
            pageBreak.Measure(Helpers.RandomSize).Should().BeEquivalentTo(new FullRender(Size.Zero));
        }
    }
}