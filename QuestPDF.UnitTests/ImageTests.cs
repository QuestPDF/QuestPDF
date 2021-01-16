using Moq;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ImageTests
    {
        [Test]
        public void Draw_ShouldHandleNullChild()
        {
            Assert.DoesNotThrow(() =>
            {
                var image = new Image()
                {
                    Data = null
                };
                
                image.Draw(It.IsAny<ICanvas>(), Size.Zero);
            });
        }
    }
}