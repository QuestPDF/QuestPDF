using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests.TestEngine
{
    public static class SingleChildTests
    {
        internal static void MeasureWithoutChild<T>(this T element) where T : ContainerElement
        {
            element.Child = null;
            element.Measure(Size.Zero).Should().BeEquivalentTo(new FullRender(Size.Zero));
        }
        
        internal static void DrawWithoutChild<T>(this T element) where T : ContainerElement
        {
            // component does not throw an exception when called with null child
            Assert.DoesNotThrow(() =>
            {
                element.Child = null;
                
                // component does not perform any canvas operation when called with null child
                TestPlan
                    .For(x => element)
                    .DrawElement(new Size(200, 100))
                    .CheckDrawResult();
            });
        }
    }
}