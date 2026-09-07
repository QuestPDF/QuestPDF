namespace QuestPDF.Infrastructure;

/// <summary>
/// Describes a single typeface (font face) available to the QuestPDF library, as returned by the <c>FontManager</c> class.
/// </summary>
public sealed class FontInfo
{
    /// <summary>
    /// Family name under which the typeface can be requested, e.g. with the <c>TextStyle.FontFamily</c> method.
    /// A typeface available under several names is described once per name.
    /// </summary>
    public string FamilyName { get; internal init; }
    
    /// <summary>
    /// PostScript name of the face, e.g. "Lato-Light". Identifies the face and appears as the font name in the PDF output.
    /// </summary>
    public string PostScriptName { get; internal init; }
    
    /// <summary>
    /// Weight from 100 (thin) to 900 (black); 400 is normal, 700 is bold. For variable fonts, the weight of the default instance.
    /// </summary>
    public int Weight { get; internal init; }
    
    /// <summary>
    /// True for italic and oblique typefaces.
    /// </summary>
    public bool IsItalic { get; internal init; }
    
    /// <summary>
    /// True for variable fonts, whose weight (and possibly other properties) can vary along their axes.
    /// </summary>
    public bool IsVariable { get; internal init; }
}
