using QuestPDF.Infrastructure;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace QuestPDF.Helpers
{
    /// <summary>
    /// Offers a palette of colors defined by the Google Material Design System guidelines.
    /// Each primary color (like red, blue, green) has associated shades and accent variations.
    /// <br />
    /// <a href="https://m2.material.io/design/color/the-color-system.html#tools-for-picking-colors">Learn more</a>
    /// </summary>
    /// <example>
    /// Access colors using the following pattern:
    /// <code>
    /// Colors.Black;
    /// Colors.Red.Lighten5;
    /// Colors.Blue.Medium; 
    /// Colors.Green.Darken4; 
    /// Colors.Yellow.Accent2; 
    /// </code>
    /// </example>
    public static class Colors
    {
        public static readonly Color Black = new(0xFF000000);
        public static readonly Color White = new(0xFFFFFF);
        public static readonly Color Transparent = new(0x00000000);

        public static class Red
        {
            public static readonly Color Lighten5 = new(0xFFEBEE);
            public static readonly Color Lighten4 = new(0xFFCDD2);
            public static readonly Color Lighten3 = new(0xEF9A9A);
            public static readonly Color Lighten2 = new(0xE57373);
            public static readonly Color Lighten1 = new(0xEF5350);
            
            public static readonly Color Medium = new(0xF44336);
            
            public static readonly Color Darken1 = new(0xE53935);
            public static readonly Color Darken2 = new(0xD32F2F);
            public static readonly Color Darken3 = new(0xC62828);
            public static readonly Color Darken4 = new(0xB71C1C);
            
            public static readonly Color Accent1 = new(0xFF8A80);
            public static readonly Color Accent2 = new(0xFF5252);
            public static readonly Color Accent3 = new(0xFF1744);
            public static readonly Color Accent4 = new(0xD50000);
        }

        public static class Pink
        {
            public static readonly Color Lighten5 = new(0xFCE4EC);
            public static readonly Color Lighten4 = new(0xF8BBD0);
            public static readonly Color Lighten3 = new(0xF48FB1);
            public static readonly Color Lighten2 = new(0xF06292);
            public static readonly Color Lighten1 = new(0xEC407A);
            
            public static readonly Color Medium = new(0xE91E63);
            
            public static readonly Color Darken1 = new(0xD81B60);
            public static readonly Color Darken2 = new(0xC2185B);
            public static readonly Color Darken3 = new(0xAD1457);
            public static readonly Color Darken4 = new(0x880E4F);
            
            public static readonly Color Accent1 = new(0xFF80AB);
            public static readonly Color Accent2 = new(0xFF4081);
            public static readonly Color Accent3 = new(0xF50057);
            public static readonly Color Accent4 = new(0xC51162);
        }

        public static class Purple
        {
            public static readonly Color Lighten5 = new(0xF3E5F5);
            public static readonly Color Lighten4 = new(0xE1BEE7);
            public static readonly Color Lighten3 = new(0xCE93D8);
            public static readonly Color Lighten2 = new(0xBA68C8);
            public static readonly Color Lighten1 = new(0xAB47BC);
            
            public static readonly Color Medium = new(0x9C27B0);
            
            public static readonly Color Darken1 = new(0x8E24AA);
            public static readonly Color Darken2 = new(0x7B1FA2);
            public static readonly Color Darken3 = new(0x6A1B9A);
            public static readonly Color Darken4 = new(0x4A148C);
            
            public static readonly Color Accent1 = new(0xEA80FC);
            public static readonly Color Accent2 = new(0xE040FB);
            public static readonly Color Accent3 = new(0xD500F9);
            public static readonly Color Accent4 = new(0xAA00FF);
        }

        public static class DeepPurple
        {
            public static readonly Color Lighten5 = new(0xEDE7F6);
            public static readonly Color Lighten4 = new(0xD1C4E9);
            public static readonly Color Lighten3 = new(0xB39DDB);
            public static readonly Color Lighten2 = new(0x9575CD);
            public static readonly Color Lighten1 = new(0x7E57C2);
            
            public static readonly Color Medium = new(0x673AB7);
            
            public static readonly Color Darken1 = new(0x5E35B1);
            public static readonly Color Darken2 = new(0x512DA8);
            public static readonly Color Darken3 = new(0x4527A0);
            public static readonly Color Darken4 = new(0x311B92);
            
            public static readonly Color Accent1 = new(0xB388FF);
            public static readonly Color Accent2 = new(0x7C4DFF);
            public static readonly Color Accent3 = new(0x651FFF);
            public static readonly Color Accent4 = new(0x6200EA);
        }

        public static class Indigo
        {
            public static readonly Color Lighten5 = new(0xE8EAF6);
            public static readonly Color Lighten4 = new(0xC5CAE9);
            public static readonly Color Lighten3 = new(0x9FA8DA);
            public static readonly Color Lighten2 = new(0x7986CB);
            public static readonly Color Lighten1 = new(0x5C6BC0);
            
            public static readonly Color Medium = new(0x3F51B5);
            
            public static readonly Color Darken1 = new(0x3949AB);
            public static readonly Color Darken2 = new(0x303F9F);
            public static readonly Color Darken3 = new(0x283593);
            public static readonly Color Darken4 = new(0x1A237E);
            
            public static readonly Color Accent1 = new(0x8C9EFF);
            public static readonly Color Accent2 = new(0x536DFE);
            public static readonly Color Accent3 = new(0x3D5AFE);
            public static readonly Color Accent4 = new(0x304FFE);
        }

        public static class Blue
        {
            public static readonly Color Lighten5 = new(0xE3F2FD);
            public static readonly Color Lighten4 = new(0xBBDEFB);
            public static readonly Color Lighten3 = new(0x90CAF9);
            public static readonly Color Lighten2 = new(0x64B5F6);
            public static readonly Color Lighten1 = new(0x42A5F5);
            
            public static readonly Color Medium = new(0x2196F3);
            
            public static readonly Color Darken1 = new(0x1E88E5);
            public static readonly Color Darken2 = new(0x1976D2);
            public static readonly Color Darken3 = new(0x1565C0);
            public static readonly Color Darken4 = new(0x0D47A1);
            
            public static readonly Color Accent1 = new(0x82B1FF);
            public static readonly Color Accent2 = new(0x448AFF);
            public static readonly Color Accent3 = new(0x2979FF);
            public static readonly Color Accent4 = new(0x2962FF);
        }

        public static class LightBlue
        {
            public static readonly Color Lighten5 = new(0xE1F5FE);
            public static readonly Color Lighten4 = new(0xB3E5FC);
            public static readonly Color Lighten3 = new(0x81D4FA);
            public static readonly Color Lighten2 = new(0x4FC3F7);
            public static readonly Color Lighten1 = new(0x29B6F6);
            
            public static readonly Color Medium = new(0x03A9F4);
            
            public static readonly Color Darken1 = new(0x039BE5);
            public static readonly Color Darken2 = new(0x0288D1);
            public static readonly Color Darken3 = new(0x0277BD);
            public static readonly Color Darken4 = new(0x01579B);
            
            public static readonly Color Accent1 = new(0x80D8FF);
            public static readonly Color Accent2 = new(0x40C4FF);
            public static readonly Color Accent3 = new(0x00B0FF);
            public static readonly Color Accent4 = new(0x0091EA);
        }

        public static class Cyan
        {
            public static readonly Color Lighten5 = new(0xE0F7FA);
            public static readonly Color Lighten4 = new(0xB2EBF2);
            public static readonly Color Lighten3 = new(0x80DEEA);
            public static readonly Color Lighten2 = new(0x4DD0E1);
            public static readonly Color Lighten1 = new(0x26C6DA);
            
            public static readonly Color Medium = new(0x00BCD4);
            
            public static readonly Color Darken1 = new(0x00ACC1);
            public static readonly Color Darken2 = new(0x0097A7);
            public static readonly Color Darken3 = new(0x00838F);
            public static readonly Color Darken4 = new(0x006064);
            
            public static readonly Color Accent1 = new(0x84FFFF);
            public static readonly Color Accent2 = new(0x18FFFF);
            public static readonly Color Accent3 = new(0x00E5FF);
            public static readonly Color Accent4 = new(0x00B8D4);
        }

        public static class Teal
        {
            public static readonly Color Lighten5 = new(0xE0F2F1);
            public static readonly Color Lighten4 = new(0xB2DFDB);
            public static readonly Color Lighten3 = new(0x80CBC4);
            public static readonly Color Lighten2 = new(0x4DB6AC);
            public static readonly Color Lighten1 = new(0x26A69A);
            
            public static readonly Color Medium = new(0x009688);
            
            public static readonly Color Darken1 = new(0x00897B);
            public static readonly Color Darken2 = new(0x00796B);
            public static readonly Color Darken3 = new(0x00695C);
            public static readonly Color Darken4 = new(0x004D40);
            
            public static readonly Color Accent1 = new(0xA7FFEB);
            public static readonly Color Accent2 = new(0x64FFDA);
            public static readonly Color Accent3 = new(0x1DE9B6);
            public static readonly Color Accent4 = new(0x00BFA5);
        }

        public static class Green
        {
            public static readonly Color Lighten5 = new(0xE8F5E9);
            public static readonly Color Lighten4 = new(0xC8E6C9);
            public static readonly Color Lighten3 = new(0xA5D6A7);
            public static readonly Color Lighten2 = new(0x81C784);
            public static readonly Color Lighten1 = new(0x66BB6A);
            
            public static readonly Color Medium = new(0x4CAF50);
            
            public static readonly Color Darken1 = new(0x43A047);
            public static readonly Color Darken2 = new(0x388E3C);
            public static readonly Color Darken3 = new(0x2E7D32);
            public static readonly Color Darken4 = new(0x1B5E20);
            
            public static readonly Color Accent1 = new(0xB9F6CA);
            public static readonly Color Accent2 = new(0x69F0AE);
            public static readonly Color Accent3 = new(0x00E676);
            public static readonly Color Accent4 = new(0x00C853);
        }

        public static class LightGreen
        {
            public static readonly Color Lighten5 = new(0xF1F8E9);
            public static readonly Color Lighten4 = new(0xDCEDC8);
            public static readonly Color Lighten3 = new(0xC5E1A5);
            public static readonly Color Lighten2 = new(0xAED581);
            public static readonly Color Lighten1 = new(0x9CCC65);
            
            public static readonly Color Medium = new(0x8BC34A);
            
            public static readonly Color Darken1 = new(0x7CB342);
            public static readonly Color Darken2 = new(0x689F38);
            public static readonly Color Darken3 = new(0x558B2F);
            public static readonly Color Darken4 = new(0x33691E);
            
            public static readonly Color Accent1 = new(0xCCFF90);
            public static readonly Color Accent2 = new(0xB2FF59);
            public static readonly Color Accent3 = new(0x76FF03);
            public static readonly Color Accent4 = new(0x64DD17);
        }

        public static class Lime
        {
            public static readonly Color Lighten5 = new(0xF9FBE7);
            public static readonly Color Lighten4 = new(0xF0F4C3);
            public static readonly Color Lighten3 = new(0xE6EE9C);
            public static readonly Color Lighten2 = new(0xDCE775);
            public static readonly Color Lighten1 = new(0xD4E157);
            
            public static readonly Color Medium = new(0xCDDC39);
            
            public static readonly Color Darken1 = new(0xC0CA33);
            public static readonly Color Darken2 = new(0xAFB42B);
            public static readonly Color Darken3 = new(0x9E9D24);
            public static readonly Color Darken4 = new(0x827717);
            
            public static readonly Color Accent1 = new(0xF4FF81);
            public static readonly Color Accent2 = new(0xEEFF41);
            public static readonly Color Accent3 = new(0xC6FF00);
            public static readonly Color Accent4 = new(0xAEEA00);
        }

        public static class Yellow
        {
            public static readonly Color Lighten5 = new(0xFFFDE7);
            public static readonly Color Lighten4 = new(0xFFF9C4);
            public static readonly Color Lighten3 = new(0xFFF59D);
            public static readonly Color Lighten2 = new(0xFFF176);
            public static readonly Color Lighten1 = new(0xFFEE58);
            
            public static readonly Color Medium = new(0xFFEB3B);
            
            public static readonly Color Darken1 = new(0xFDD835);
            public static readonly Color Darken2 = new(0xFBC02D);
            public static readonly Color Darken3 = new(0xF9A825);
            public static readonly Color Darken4 = new(0xF57F17);
            
            public static readonly Color Accent1 = new(0xFFFF8D);
            public static readonly Color Accent2 = new(0xFFFF00);
            public static readonly Color Accent3 = new(0xFFEA00);
            public static readonly Color Accent4 = new(0xFFD600);
        }

        public static class Amber
        {
            public static readonly Color Lighten5 = new(0xFFF8E1);
            public static readonly Color Lighten4 = new(0xFFECB3);
            public static readonly Color Lighten3 = new(0xFFE082);
            public static readonly Color Lighten2 = new(0xFFD54F);
            public static readonly Color Lighten1 = new(0xFFCA28);
            
            public static readonly Color Medium = new(0xFFC107);
            
            public static readonly Color Darken1 = new(0xFFB300);
            public static readonly Color Darken2 = new(0xFFA000);
            public static readonly Color Darken3 = new(0xFF8F00);
            public static readonly Color Darken4 = new(0xFF6F00);
            
            public static readonly Color Accent1 = new(0xFFE57F);
            public static readonly Color Accent2 = new(0xFFD740);
            public static readonly Color Accent3 = new(0xFFC400);
            public static readonly Color Accent4 = new(0xFFAB00);
        }

        public static class Orange
        {
            public static readonly Color Lighten5 = new(0xFFF3E0);
            public static readonly Color Lighten4 = new(0xFFE0B2);
            public static readonly Color Lighten3 = new(0xFFCC80);
            public static readonly Color Lighten2 = new(0xFFB74D);
            public static readonly Color Lighten1 = new(0xFFA726);
            
            public static readonly Color Medium = new(0xFF9800);
            
            public static readonly Color Darken1 = new(0xFB8C00);
            public static readonly Color Darken2 = new(0xF57C00);
            public static readonly Color Darken3 = new(0xEF6C00);
            public static readonly Color Darken4 = new(0xE65100);
            
            public static readonly Color Accent1 = new(0xFFD180);
            public static readonly Color Accent2 = new(0xFFAB40);
            public static readonly Color Accent3 = new(0xFF9100);
            public static readonly Color Accent4 = new(0xFF6D00);
        }

        public static class DeepOrange
        {
            public static readonly Color Lighten5 = new(0xFBE9E7);
            public static readonly Color Lighten4 = new(0xFFCCBC);
            public static readonly Color Lighten3 = new(0xFFAB91);
            public static readonly Color Lighten2 = new(0xFF8A65);
            public static readonly Color Lighten1 = new(0xFF7043);
            
            public static readonly Color Medium = new(0xFF5722);
            
            public static readonly Color Darken1 = new(0xF4511E);
            public static readonly Color Darken2 = new(0xE64A19);
            public static readonly Color Darken3 = new(0xD84315);
            public static readonly Color Darken4 = new(0xBF360C);
            
            public static readonly Color Accent1 = new(0xFF9E80);
            public static readonly Color Accent2 = new(0xFF6E40);
            public static readonly Color Accent3 = new(0xFF3D00);
            public static readonly Color Accent4 = new(0xDD2C00);
        }

        public static class Brown
        {
            public static readonly Color Lighten5 = new(0xEFEBE9);
            public static readonly Color Lighten4 = new(0xD7CCC8);
            public static readonly Color Lighten3 = new(0xBCAAA4);
            public static readonly Color Lighten2 = new(0xA1887F);
            public static readonly Color Lighten1 = new(0x8D6E63);
            
            public static readonly Color Medium = new(0x795548);
            
            public static readonly Color Darken1 = new(0x6D4C41);
            public static readonly Color Darken2 = new(0x5D4037);
            public static readonly Color Darken3 = new(0x4E342E);
            public static readonly Color Darken4 = new(0x3E2723);
        }

        public static class Grey
        {
            public static readonly Color Lighten5 = new(0xFAFAFA);
            public static readonly Color Lighten4 = new(0xF5F5F5);
            public static readonly Color Lighten3 = new(0xEEEEEE);
            public static readonly Color Lighten2 = new(0xE0E0E0);
            public static readonly Color Lighten1 = new(0xBDBDBD);
            
            public static readonly Color Medium = new(0x9E9E9E);
            
            public static readonly Color Darken1 = new(0x757575);
            public static readonly Color Darken2 = new(0x616161);
            public static readonly Color Darken3 = new(0x424242);
            public static readonly Color Darken4 = new(0x212121);
        }

        public static class BlueGrey
        {
            public static readonly Color Lighten5 = new(0xECEFF1);
            public static readonly Color Lighten4 = new(0xCFD8DC);
            public static readonly Color Lighten3 = new(0xB0BEC5);
            public static readonly Color Lighten2 = new(0x90A4AE);
            public static readonly Color Lighten1 = new(0x78909C);
            
            public static readonly Color Medium = new(0x607D8B);
            
            public static readonly Color Darken1 = new(0x546E7A);
            public static readonly Color Darken2 = new(0x455A64);
            public static readonly Color Darken3 = new(0x37474F);
            public static readonly Color Darken4 = new(0x263238);
        }
    }
}