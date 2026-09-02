using System.Linq;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.UnitTests;

[TestFixture]
public class TextWrappingTests
{
    [Test]
    public void TestStability()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(50);
                    
                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        for (float i = 25; i < 150; i += 1f)
                        {
                            column.Item()
                                .ShowEntire()
                                .Width(i)
                                .Background(Colors.Red.Lighten3)
                                .Text("Ser\u00ADver\u00ADkon\u00ADfi\u00ADgu\u00ADra\u00ADti\u00ADons\u00ADda\u00ADtei.");
                        }
                    });
                });
            })
            .GeneratePdf();
    }
}