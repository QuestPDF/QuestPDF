#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;

namespace QuestPDF.Helpers
{
    /// <summary>
    /// Contains a collection of fonts defined by the PDF standard.
    /// </summary>
    [Obsolete("This class has been deprecated since version 2026.9. The listed font families are not guaranteed to be available in the target environment. Please pass the font family name as a plain string instead, and use the FontManager.GetRegisteredFonts and FontManager.GetSystemFonts methods to check which fonts are actually available.")]
    public static class Fonts
    {
        public const string Arial = "Arial";
        public const string Calibri = "Calibri";
        public const string Cambria = "Cambria";
        public const string Candara = "Candara";
        public const string ComicSans = "Comic Sans MS";
        public const string Consolas = "Consolas";
        public const string Corbel = "Corbel";
        public const string Courier = "Courier";
        public const string CourierNew = "Courier New";
        public const string Georgia = "Georgia";
        public const string Impact = "Impact";
        public const string Lato = "Lato";
        public const string LucidaConsole = "Lucida Console";
        public const string SegoeSD = "Segoe SD";
        public const string SegoeUI = "Segoe UI";
        public const string Tahoma = "Tahoma";
        public const string TimesNewRoman = "Times New Roman";
        public const string TimesRoman = "Times Roman";
        public const string Trebuchet = "Trebuchet MS";
        public const string Verdana = "Verdana";
    }
}