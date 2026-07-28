#nullable enable

using System.Linq;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

public class TextGlyphRunTests
{
    [Test]
    public void ParagraphIsRenderedAsSingleGlyphRun()
    {
        var content = RenderDocumentAndReturnContentCode(false);
        var operators = PdfInspector.GetContentStreamOperators(content).ToList();
        
        Assert.That(operators, Does.Not.Contain("Tj"));
        
        // "fly ang fight"
        Assert.That(operators.Count(op => op is "TJ"), Is.EqualTo(1));
    }
    
    [Test]
    public void LigatureGlyphsAreAnnotatedWithActualText()
    {
        var content = RenderDocumentAndReturnContentCode(true);
        var operators = PdfInspector.GetContentStreamOperators(content).ToList();
        
        Assert.That(operators, Does.Not.Contain("Tj"));
        
        // "fl"
        // "y and "
        // "fi"
        // "ght"
        Assert.That(operators.Count(op => op is "TJ"), Is.EqualTo(4));
        
        Assert.That(content, Does.Contain("/Span<</ActualText (fl) >> BDC"));
        Assert.That(content, Does.Contain("/Span<</ActualText (fi) >> BDC"));
    }
    
    private static string RenderDocumentAndReturnContentCode(bool useLigatures)
    {
        var textStyle = useLigatures
            ? TextStyle.Default.EnableFontFeature(FontFeatures.StandardLigatures)
            : TextStyle.Default.DisableFontFeature(FontFeatures.StandardLigatures);
        
         var documentBytes = Document
            .Create(document => document.Page(page =>
            {
                page.Size(75, 15);
                page.DefaultTextStyle(textStyle);
                page.Content().Text("fly and fight");
            }))
            .GeneratePdf();
         
         using var pdf = PdfInspector.Load(documentBytes);
         var page = pdf.Pages.Single();
         return pdf.GetStreamText(pdf.Resolve(page.GetProperty("/Contents")));
    }
}
