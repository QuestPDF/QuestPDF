using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace QuestPDF.Helpers;

public static class FontFeatures
{
    public const string AccessAllAlternates = "aalt";
    public const string AboveBaseForms = "abvf";
    public const string AboveBaseMarkPositioning = "abvm";
    public const string AboveBaseSubstitutions = "abvs";
    public const string AlternativeFractions = "afrc";
    public const string Akhand = "akhn";
    public const string KerningForAlternateProportionalWidths = "apkn";
    public const string BelowBaseForms = "blwf";
    public const string BelowBaseMarkPositioning = "blwm";
    public const string BelowBaseSubstitutions = "blws";
    public const string ContextualAlternates = "calt";
    public const string CaseSensitiveForms = "case";
    public const string GlyphCompositionDecomposition = "ccmp";
    public const string ConjunctFormAfterRo = "cfar";
    public const string ContextualHalfWidthSpacing = "chws";
    public const string ConjunctForms = "cjct";
    public const string ContextualLigatures = "clig";
    public const string CenteredCjkPunctuation = "cpct";
    public const string CapitalSpacing = "cpsp";
    public const string ContextualSwash = "cswh";
    public const string CursivePositioning = "curs";
    public const string PetiteCapitalsFromCapitals = "c2pc";
    public const string SmallCapitalsFromCapitals = "c2sc";
    public const string Distances = "dist";
    public const string DiscretionaryLigatures = "dlig";
    public const string Denominators = "dnom";
    public const string DotlessForms = "dtls";
    public const string ExpertForms = "expt";
    public const string FinalGlyphOnLineAlternates = "falt";
    public const string TerminalForms2 = "fin2";
    public const string TerminalForms3 = "fin3";
    public const string TerminalForms = "fina";
    public const string FlattenedAccentForms = "flac";
    public const string Fractions = "frac";
    public const string FullWidths = "fwid";
    public const string HalfForms = "half";
    public const string HalantForms = "haln";
    public const string AlternateHalfWidths = "halt";
    public const string HistoricalForms = "hist";
    public const string HorizontalKanaAlternates = "hkna";
    public const string HistoricalLigatures = "hlig";
    public const string Hangul = "hngl";
    public const string HalfWidths = "hwid";
    public const string InitialForms = "init";
    public const string IsolatedForms = "isol";
    public const string Italics = "ital";
    public const string JustificationAlternates = "jalt";
    public const string JIS78Forms = "jp78";
    public const string JIS83Forms = "jp83";
    public const string JIS90Forms = "jp90";
    public const string JIS2004Forms = "jp04";
    public const string Kerning = "kern";
    public const string LeftBounds = "lfbd";
    public const string StandardLigatures = "liga";
    public const string LeadingJamoForms = "ljmo";
    public const string LiningFigures = "lnum";
    public const string LocalizedForms = "locl";
    public const string LeftToRightAlternates = "ltra";
    public const string LeftToRightMirroredForms = "ltrm";
    public const string MarkPositioning = "mark";
    public const string MedialForms2 = "med2";
    public const string MedialForms = "medi";
    public const string MathematicalGreek = "mgrk";
    public const string MarkToMarkPositioning = "mkmk";
    public const string MarkPositioningViaSubstitution = "mset";
    public const string AlternateAnnotationForms = "nalt";
    public const string NlcKanjiForms = "nlck";
    public const string NuktaForms = "nukt";
    public const string Numerators = "numr";
    public const string OldstyleFigures = "onum";
    public const string OpticalBounds = "opbd";
    public const string Ordinals = "ordn";
    public const string Ornaments = "ornm";
    public const string ProportionalAlternateWidths = "palt";
    public const string PetiteCapitals = "pcap";
    public const string ProportionalKana = "pkna";
    public const string ProportionalFigures = "pnum";
    public const string PreBaseForms = "pref";
    public const string PreBaseSubstitutions = "pres";
    public const string PostBaseForms = "pstf";
    public const string PostBaseSubstitutions = "psts";
    public const string ProportionalWidths = "pwid";
    public const string QuarterWidths = "qwid";
    public const string Randomize = "rand";
    public const string RequiredContextualAlternates = "rclt";
    public const string RakarForms = "rkrf";
    public const string RequiredLigatures = "rlig";
    public const string RephForm = "rphf";
    public const string RightBounds = "rtbd";
    public const string RightToLeftAlternates = "rtla";
    public const string RightToLeftMirroredForms = "rtlm";
    public const string RubyNotationForms = "ruby";
    public const string RequiredVariationAlternates = "rvrn";
    public const string StylisticAlternates = "salt";
    public const string ScientificInferiors = "sinf";
    public const string OpticalSize = "size";
    public const string SmallCapitals = "smcp";
    public const string SimplifiedForms = "smpl";
    public const string MathScriptStyleAlternates = "ssty";
    public const string StretchingGlyphDecomposition = "stch";
    public const string Subscript = "subs";
    public const string Superscript = "sups";
    public const string Swash = "swsh";
    public const string Titling = "titl";
    public const string TrailingJamoForms = "tjmo";
    public const string TraditionalNameForms = "tnam";
    public const string TabularFigures = "tnum";
    public const string TraditionalForms = "trad";
    public const string ThirdWidths = "twid";
    public const string Unicase = "unic";
    public const string AlternateVerticalMetrics = "valt";
    public const string KerningForAlternateProportionalVerticalMetrics = "vapk";
    public const string VattuVariants = "vatu";
    public const string VerticalContextualHalfWidthSpacing = "vchw";
    public const string VerticalAlternates = "vert";
    public const string AlternateVerticalHalfMetrics = "vhal";
    public const string VowelJamoForms = "vjmo";
    public const string VerticalKanaAlternates = "vkna";
    public const string VerticalKerning = "vkrn";
    public const string ProportionalAlternateVerticalMetrics = "vpal";
    public const string VerticalAlternatesAndRotation = "vrt2";
    public const string VerticalAlternatesForRotation = "vrtr";
    public const string SlashedZero = "zero";

    /// <summary>
    /// (JIS X 0212-1990 Kanji Forms)
    /// </summary>
    public const string HojoKanjiForms  = "hojo";
    
    public static string CharacterVariant(int value)
    {
        if (value < 1 || value > 99)
            throw new ArgumentOutOfRangeException(nameof(value), "Character Variant value must be between 1 and 99.");
        
        return $"cv{value:00}";
    }
    
    public static string StylisticSet(int value)
    {
        if (value < 1 || value > 20)
            throw new ArgumentOutOfRangeException(nameof(value), "Character Variant value must be between 1 and 20.");
        
        return $"ss{value:00}";
    }
}