using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.VisualTests;

public class TextStyleTests
{
    private static readonly IEnumerable<Color> FontColor_Values = [ Colors.Red.Darken3, Colors.Green.Darken3, Colors.Blue.Darken3 ];
    
    [Test, TestCaseSource(nameof(FontColor_Values))]
    public void FontsColor(Color fontColor)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>  
                {
                    text.Span("Text can be displayed in different ");
                    text.Span("font colors").FontColor(fontColor);
                    text.Span(".");
                });
        });
    }
    
    private static readonly IEnumerable<Color> BackgroundColor_Values = [ Colors.Red.Lighten4, Colors.Green.Lighten4, Colors.Blue.Lighten4 ];
    
    [Test, TestCaseSource(nameof(BackgroundColor_Values))]
    public void BackgroundColor(Color backgroundColor)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>  
                {
                    text.Span("Text can be displayed with different ");
                    text.Span("background colors").BackgroundColor(backgroundColor);
                    text.Span(".");
                });
        });
    }
    
    [Test]
    public void FontSize([Values(12, 16, 24)] float fontSize)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>  
                {
                    text.Span("Text can be displayed in different ");
                    text.Span($"font sizes ({fontSize}pt)").FontSize(fontSize);
                    text.Span(".");
                });
        });
    }
    
    [Test]
    public void LineHeight([Values(0.75f, 1f, 1.5f)] float lineHeight)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(400)
                .Text(text =>  
                {
                    text.Span($"Line height: {lineHeight}\n\n").Bold().FontColor(Colors.Blue.Darken2);
                    text.Span(Placeholders.LoremIpsum()).LineHeight(lineHeight);
                });
        });
    }
    
    [Test]
    public void WordSpacing([Values(-0.1f, 0f, 0.25f, 1f)] float wordSpacing)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(400)
                .Text(text =>  
                {
                    text.Span($"Word spacing: {wordSpacing}\n\n").Bold().FontColor(Colors.Blue.Darken2);
                    text.Span(Placeholders.LoremIpsum()).WordSpacing(wordSpacing);
                });
        });
    }
    
    [Test]
    public void LetterSpacing([Values(-0.1f, 0f, 0.1f, 0.25f)] float letterSpacing)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Width(400)
                .Text(text =>  
                {
                    text.Span($"Letter spacing: {letterSpacing}\n\n").Bold().FontColor(Colors.Blue.Darken2);
                    text.Span(Placeholders.LoremIpsum()).LetterSpacing(letterSpacing);
                });
        });
    }
    
    [Test]
    public void Italic()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        { 
            container
                .Text(text =>  
                {
                    text.Span("The ");
                    text.Span("italic effect").Italic();
                    text.Span(" slants the letters slightly to the right.");
                });
        });
    }
    
    #region Font Decoration
    
    [Test]
    public void FontDecoration_Underline()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("Text can be decorated with ");
                    text.Span("an underline").Underline().DecorationThickness(2).DecorationColor(Colors.Red.Medium);
                    text.Span(".");
                });
        });
    }
    
    [Test]
    public void FontDecoration_Strikethrough()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("Text can be decorated with ");
                    text.Span("a strikeout").Strikethrough().DecorationThickness(2).DecorationColor(Colors.Red.Medium);
                    text.Span(".");
                });
        });
    }
    
    [Test]
    public void FontDecoration_Overline()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("Text can be decorated with ");
                    text.Span("an overline").Overline().DecorationThickness(2).DecorationColor(Colors.Red.Medium);
                    text.Span(".");
                });
        });
    }
    
    [Test]
    public void FontDecoration_Combined()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("Text may have ");

                    text.Span("multiple decoration")
                        .Underline()
                        .Strikethrough()
                        .Overline()
                        .DecorationThickness(2)
                        .DecorationColor(Colors.Blue.Medium);
                    
                    text.Span(" applied to it.");
                });
        });
    }
    
    private static readonly IEnumerable<Color> FontDecoration_Color_Values = [ Colors.Red.Medium, Colors.Green.Medium, Colors.Blue.Medium ];
    
    [Test, TestCaseSource(nameof(FontDecoration_Color_Values))]
    public void FontDecoration_Color(Color decorationColor)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("The color of ");
                    text.Span("the text decoration").Underline().DecorationDashed().DecorationThickness(2).DecorationColor(decorationColor);
                    text.Span(" can be changed.");
                });
        });
    }
    
    [Test]
    public void FontDecoration_Thickness([Values(1f, 2f, 3f)] float thickness)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("The thickness of ");
                    text.Span("the text decoration").Underline().DecorationWavy().DecorationThickness(thickness).DecorationColor(Colors.Red.Medium);
                    text.Span(" can be changed.");
                });
        });
    }
    
    [Test]
    public void FontDecoration_Solid()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("a solid-line text decoration").Underline().DecorationSolid();
                    text.Span(".");
                });
        });
    }
    
    [Test]
    public void FontDecoration_Double()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("a double-line text decoration").Underline().DecorationDouble();
                    text.Span(".");
                });
        });
    }
    
    [Test]
    public void FontDecoration_Wavy()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("a wavy-line text decoration").Underline().DecorationWavy();
                    text.Span(".");
                });
        });
    }
    
    [Test]
    public void FontDecoration_Dotted()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("a dotted text decoration").Underline().DecorationDotted();
                    text.Span(".");
                });
        });
    }
    
    [Test]
    public void FontDecoration_Dashed()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("a dashed text decoration").Underline().DecorationDashed();
                    text.Span(".");
                });
        });
    }
    
    #endregion
    
    #region Font Weight
    
    [Test]
    public void FontWeight_200_ExtraLight()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("an extra-light").ExtraLight().BackgroundColor(Colors.Grey.Lighten3);
                    text.Span(" font weight.");
                });
        }); 
    }
    
    [Test]
    public void FontWeight_300_Light()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("a light").Light().BackgroundColor(Colors.Grey.Lighten3);
                    text.Span(" font weight.");
                });
        });
    }
    
    [Test]
    public void FontWeight_400_Regular()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("a regular").NormalWeight().BackgroundColor(Colors.Grey.Lighten3);
                    text.Span(" font weight.");
                });
        });
    }
    
    [Test]
    public void FontWeight_500_Medium()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("a medium").Medium().BackgroundColor(Colors.Grey.Lighten3);
                    text.Span(" font weight.");
                });
        });
    }
    
    [Test]
    public void FontWeight_600_SemiBold()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("a semi-bold").SemiBold().BackgroundColor(Colors.Grey.Lighten3);
                    text.Span(" font weight.");
                });
        });
    }
    
    [Test]
    public void FontWeight_700_Bold()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("a bold").Bold().BackgroundColor(Colors.Grey.Lighten3);
                    text.Span(" font weight.");
                });
        });
    }
    
    [Test]
    public void FontWeight_800_ExtraBold()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("This example shows ");
                    text.Span("an extra-bold").ExtraBold().BackgroundColor(Colors.Grey.Lighten3 );
                    text.Span(" font weight.");
                });
        });
    }
    
    #endregion
    
    #region Font Position
    
    [Test]
    public void FontPosition_Subscript()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("Chemical formula of sulfuric acid: H");
                    text.Span("2").Subscript();
                    text.Span("SO");
                    text.Span("4").Subscript();
                    text.Span(".");
                });
        });
    }

    [Test]
    public void FontPosition_Superscript()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text(text =>
                {
                    text.Span("Enstein's equation: E=mc");
                    text.Span("2").Superscript(); 
                });
        });
    }
    
    #endregion
    
    #region Font Features
    
    [Test]
    public void FontFeatures_StandardLigatures_Enabled()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text("final")
                .EnableFontFeature(FontFeatures.StandardLigatures);
        });
    }

    [Test]
    public void FontFeatures_StandardLigatures_Disabled()
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Text("final")
                .DisableFontFeature(FontFeatures.StandardLigatures);
        });
    }
    
    #endregion
}