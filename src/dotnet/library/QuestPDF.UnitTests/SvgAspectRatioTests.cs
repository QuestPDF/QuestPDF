using NUnit.Framework;
using SvgImage = QuestPDF.Infrastructure.SvgImage;

namespace QuestPDF.UnitTests;

public class SvgAspectRatioTests
{
    [Test]
    public void PercentageSizeWithoutViewBox_HasNoAspectRatio()
    {
        using var image = SvgImage.FromText(
            """<svg xmlns="http://www.w3.org/2000/svg" width="100%" height="1" />""");

        Assert.That(image.SkSvgImage.AspectRatio, Is.Zero);
    }

    [Test]
    public void PercentageSizeWithViewBox_UsesViewBoxRatio()
    {
        using var image = SvgImage.FromText(
            """<svg xmlns="http://www.w3.org/2000/svg" width="100%" height="100%" viewBox="0 0 200 100" />""");

        Assert.That(image.SkSvgImage.AspectRatio, Is.EqualTo(2f));
    }

    [Test]
    public void AbsoluteSize_UsesSizeRatio()
    {
        using var image = SvgImage.FromText(
            """<svg xmlns="http://www.w3.org/2000/svg" width="300" height="100" />""");

        Assert.That(image.SkSvgImage.AspectRatio, Is.EqualTo(3f));
    }
}
