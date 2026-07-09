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
        public static readonly Color White = new(0xFFFFFFFF);
        public static readonly Color Transparent = new(0x00000000);

        public static class Red
        {
            public static readonly Color Lighten5 = new(0xFFFFEBEE);
            public static readonly Color Lighten4 = new(0xFFFFCDD2);
            public static readonly Color Lighten3 = new(0xFFEF9A9A);
            public static readonly Color Lighten2 = new(0xFFE57373);
            public static readonly Color Lighten1 = new(0xFFEF5350);
            
            public static readonly Color Medium = new(0xFFF44336);
            
            public static readonly Color Darken1 = new(0xFFE53935);
            public static readonly Color Darken2 = new(0xFFD32F2F);
            public static readonly Color Darken3 = new(0xFFC62828);
            public static readonly Color Darken4 = new(0xFFB71C1C);
            
            public static readonly Color Accent1 = new(0xFFFF8A80);
            public static readonly Color Accent2 = new(0xFFFF5252);
            public static readonly Color Accent3 = new(0xFFFF1744);
            public static readonly Color Accent4 = new(0xFFD50000);
        }

        public static class Pink
        {
            public static readonly Color Lighten5 = new(0xFFFCE4EC);
            public static readonly Color Lighten4 = new(0xFFF8BBD0);
            public static readonly Color Lighten3 = new(0xFFF48FB1);
            public static readonly Color Lighten2 = new(0xFFF06292);
            public static readonly Color Lighten1 = new(0xFFEC407A);
            
            public static readonly Color Medium = new(0xFFE91E63);
            
            public static readonly Color Darken1 = new(0xFFD81B60);
            public static readonly Color Darken2 = new(0xFFC2185B);
            public static readonly Color Darken3 = new(0xFFAD1457);
            public static readonly Color Darken4 = new(0xFF880E4F);
            
            public static readonly Color Accent1 = new(0xFFFF80AB);
            public static readonly Color Accent2 = new(0xFFFF4081);
            public static readonly Color Accent3 = new(0xFFF50057);
            public static readonly Color Accent4 = new(0xFFC51162);
        }

        public static class Purple
        {
            public static readonly Color Lighten5 = new(0xFFF3E5F5);
            public static readonly Color Lighten4 = new(0xFFE1BEE7);
            public static readonly Color Lighten3 = new(0xFFCE93D8);
            public static readonly Color Lighten2 = new(0xFFBA68C8);
            public static readonly Color Lighten1 = new(0xFFAB47BC);
            
            public static readonly Color Medium = new(0xFF9C27B0);
            
            public static readonly Color Darken1 = new(0xFF8E24AA);
            public static readonly Color Darken2 = new(0xFF7B1FA2);
            public static readonly Color Darken3 = new(0xFF6A1B9A);
            public static readonly Color Darken4 = new(0xFF4A148C);
            
            public static readonly Color Accent1 = new(0xFFEA80FC);
            public static readonly Color Accent2 = new(0xFFE040FB);
            public static readonly Color Accent3 = new(0xFFD500F9);
            public static readonly Color Accent4 = new(0xFFAA00FF);
        }

        public static class DeepPurple
        {
            public static readonly Color Lighten5 = new(0xFFEDE7F6);
            public static readonly Color Lighten4 = new(0xFFD1C4E9);
            public static readonly Color Lighten3 = new(0xFFB39DDB);
            public static readonly Color Lighten2 = new(0xFF9575CD);
            public static readonly Color Lighten1 = new(0xFF7E57C2);
            
            public static readonly Color Medium = new(0xFF673AB7);
            
            public static readonly Color Darken1 = new(0xFF5E35B1);
            public static readonly Color Darken2 = new(0xFF512DA8);
            public static readonly Color Darken3 = new(0xFF4527A0);
            public static readonly Color Darken4 = new(0xFF311B92);
            
            public static readonly Color Accent1 = new(0xFFB388FF);
            public static readonly Color Accent2 = new(0xFF7C4DFF);
            public static readonly Color Accent3 = new(0xFF651FFF);
            public static readonly Color Accent4 = new(0xFF6200EA);
        }

        public static class Indigo
        {
            public static readonly Color Lighten5 = new(0xFFE8EAF6);
            public static readonly Color Lighten4 = new(0xFFC5CAE9);
            public static readonly Color Lighten3 = new(0xFF9FA8DA);
            public static readonly Color Lighten2 = new(0xFF7986CB);
            public static readonly Color Lighten1 = new(0xFF5C6BC0);
            
            public static readonly Color Medium = new(0xFF3F51B5);
            
            public static readonly Color Darken1 = new(0xFF3949AB);
            public static readonly Color Darken2 = new(0xFF303F9F);
            public static readonly Color Darken3 = new(0xFF283593);
            public static readonly Color Darken4 = new(0xFF1A237E);
            
            public static readonly Color Accent1 = new(0xFF8C9EFF);
            public static readonly Color Accent2 = new(0xFF536DFE);
            public static readonly Color Accent3 = new(0xFF3D5AFE);
            public static readonly Color Accent4 = new(0xFF304FFE);
        }

        public static class Blue
        {
            public static readonly Color Lighten5 = new(0xFFE3F2FD);
            public static readonly Color Lighten4 = new(0xFFBBDEFB);
            public static readonly Color Lighten3 = new(0xFF90CAF9);
            public static readonly Color Lighten2 = new(0xFF64B5F6);
            public static readonly Color Lighten1 = new(0xFF42A5F5);
            
            public static readonly Color Medium = new(0xFF2196F3);
            
            public static readonly Color Darken1 = new(0xFF1E88E5);
            public static readonly Color Darken2 = new(0xFF1976D2);
            public static readonly Color Darken3 = new(0xFF1565C0);
            public static readonly Color Darken4 = new(0xFF0D47A1);
            
            public static readonly Color Accent1 = new(0xFF82B1FF);
            public static readonly Color Accent2 = new(0xFF448AFF);
            public static readonly Color Accent3 = new(0xFF2979FF);
            public static readonly Color Accent4 = new(0xFF2962FF);
        }

        public static class LightBlue
        {
            public static readonly Color Lighten5 = new(0xFFE1F5FE);
            public static readonly Color Lighten4 = new(0xFFB3E5FC);
            public static readonly Color Lighten3 = new(0xFF81D4FA);
            public static readonly Color Lighten2 = new(0xFF4FC3F7);
            public static readonly Color Lighten1 = new(0xFF29B6F6);
            
            public static readonly Color Medium = new(0xFF03A9F4);
            
            public static readonly Color Darken1 = new(0xFF039BE5);
            public static readonly Color Darken2 = new(0xFF0288D1);
            public static readonly Color Darken3 = new(0xFF0277BD);
            public static readonly Color Darken4 = new(0xFF01579B);
            
            public static readonly Color Accent1 = new(0xFF80D8FF);
            public static readonly Color Accent2 = new(0xFF40C4FF);
            public static readonly Color Accent3 = new(0xFF00B0FF);
            public static readonly Color Accent4 = new(0xFF0091EA);
        }

        public static class Cyan
        {
            public static readonly Color Lighten5 = new(0xFFE0F7FA);
            public static readonly Color Lighten4 = new(0xFFB2EBF2);
            public static readonly Color Lighten3 = new(0xFF80DEEA);
            public static readonly Color Lighten2 = new(0xFF4DD0E1);
            public static readonly Color Lighten1 = new(0xFF26C6DA);
            
            public static readonly Color Medium = new(0xFF00BCD4);
            
            public static readonly Color Darken1 = new(0xFF00ACC1);
            public static readonly Color Darken2 = new(0xFF0097A7);
            public static readonly Color Darken3 = new(0xFF00838F);
            public static readonly Color Darken4 = new(0xFF006064);
            
            public static readonly Color Accent1 = new(0xFF84FFFF);
            public static readonly Color Accent2 = new(0xFF18FFFF);
            public static readonly Color Accent3 = new(0xFF00E5FF);
            public static readonly Color Accent4 = new(0xFF00B8D4);
        }

        public static class Teal
        {
            public static readonly Color Lighten5 = new(0xFFE0F2F1);
            public static readonly Color Lighten4 = new(0xFFB2DFDB);
            public static readonly Color Lighten3 = new(0xFF80CBC4);
            public static readonly Color Lighten2 = new(0xFF4DB6AC);
            public static readonly Color Lighten1 = new(0xFF26A69A);
            
            public static readonly Color Medium = new(0xFF009688);
            
            public static readonly Color Darken1 = new(0xFF00897B);
            public static readonly Color Darken2 = new(0xFF00796B);
            public static readonly Color Darken3 = new(0xFF00695C);
            public static readonly Color Darken4 = new(0xFF004D40);
            
            public static readonly Color Accent1 = new(0xFFA7FFEB);
            public static readonly Color Accent2 = new(0xFF64FFDA);
            public static readonly Color Accent3 = new(0xFF1DE9B6);
            public static readonly Color Accent4 = new(0xFF00BFA5);
        }

        public static class Green
        {
            public static readonly Color Lighten5 = new(0xFFE8F5E9);
            public static readonly Color Lighten4 = new(0xFFC8E6C9);
            public static readonly Color Lighten3 = new(0xFFA5D6A7);
            public static readonly Color Lighten2 = new(0xFF81C784);
            public static readonly Color Lighten1 = new(0xFF66BB6A);
            
            public static readonly Color Medium = new(0xFF4CAF50);
            
            public static readonly Color Darken1 = new(0xFF43A047);
            public static readonly Color Darken2 = new(0xFF388E3C);
            public static readonly Color Darken3 = new(0xFF2E7D32);
            public static readonly Color Darken4 = new(0xFF1B5E20);
            
            public static readonly Color Accent1 = new(0xFFB9F6CA);
            public static readonly Color Accent2 = new(0xFF69F0AE);
            public static readonly Color Accent3 = new(0xFF00E676);
            public static readonly Color Accent4 = new(0xFF00C853);
        }

        public static class LightGreen
        {
            public static readonly Color Lighten5 = new(0xFFF1F8E9);
            public static readonly Color Lighten4 = new(0xFFDCEDC8);
            public static readonly Color Lighten3 = new(0xFFC5E1A5);
            public static readonly Color Lighten2 = new(0xFFAED581);
            public static readonly Color Lighten1 = new(0xFF9CCC65);
            
            public static readonly Color Medium = new(0xFF8BC34A);
            
            public static readonly Color Darken1 = new(0xFF7CB342);
            public static readonly Color Darken2 = new(0xFF689F38);
            public static readonly Color Darken3 = new(0xFF558B2F);
            public static readonly Color Darken4 = new(0xFF33691E);
            
            public static readonly Color Accent1 = new(0xFFCCFF90);
            public static readonly Color Accent2 = new(0xFFB2FF59);
            public static readonly Color Accent3 = new(0xFF76FF03);
            public static readonly Color Accent4 = new(0xFF64DD17);
        }

        public static class Lime
        {
            public static readonly Color Lighten5 = new(0xFFF9FBE7);
            public static readonly Color Lighten4 = new(0xFFF0F4C3);
            public static readonly Color Lighten3 = new(0xFFE6EE9C);
            public static readonly Color Lighten2 = new(0xFFDCE775);
            public static readonly Color Lighten1 = new(0xFFD4E157);
            
            public static readonly Color Medium = new(0xFFCDDC39);
            
            public static readonly Color Darken1 = new(0xFFC0CA33);
            public static readonly Color Darken2 = new(0xFFAFB42B);
            public static readonly Color Darken3 = new(0xFF9E9D24);
            public static readonly Color Darken4 = new(0xFF827717);
            
            public static readonly Color Accent1 = new(0xFFF4FF81);
            public static readonly Color Accent2 = new(0xFFEEFF41);
            public static readonly Color Accent3 = new(0xFFC6FF00);
            public static readonly Color Accent4 = new(0xFFAEEA00);
        }

        public static class Yellow
        {
            public static readonly Color Lighten5 = new(0xFFFFFDE7);
            public static readonly Color Lighten4 = new(0xFFFFF9C4);
            public static readonly Color Lighten3 = new(0xFFFFF59D);
            public static readonly Color Lighten2 = new(0xFFFFF176);
            public static readonly Color Lighten1 = new(0xFFFFEE58);
            
            public static readonly Color Medium = new(0xFFFFEB3B);
            
            public static readonly Color Darken1 = new(0xFFFDD835);
            public static readonly Color Darken2 = new(0xFFFBC02D);
            public static readonly Color Darken3 = new(0xFFF9A825);
            public static readonly Color Darken4 = new(0xFFF57F17);
            
            public static readonly Color Accent1 = new(0xFFFFFF8D);
            public static readonly Color Accent2 = new(0xFFFFFF00);
            public static readonly Color Accent3 = new(0xFFFFEA00);
            public static readonly Color Accent4 = new(0xFFFFD600);
        }

        public static class Amber
        {
            public static readonly Color Lighten5 = new(0xFFFFF8E1);
            public static readonly Color Lighten4 = new(0xFFFFECB3);
            public static readonly Color Lighten3 = new(0xFFFFE082);
            public static readonly Color Lighten2 = new(0xFFFFD54F);
            public static readonly Color Lighten1 = new(0xFFFFCA28);
            
            public static readonly Color Medium = new(0xFFFFC107);
            
            public static readonly Color Darken1 = new(0xFFFFB300);
            public static readonly Color Darken2 = new(0xFFFFA000);
            public static readonly Color Darken3 = new(0xFFFF8F00);
            public static readonly Color Darken4 = new(0xFFFF6F00);
            
            public static readonly Color Accent1 = new(0xFFFFE57F);
            public static readonly Color Accent2 = new(0xFFFFD740);
            public static readonly Color Accent3 = new(0xFFFFC400);
            public static readonly Color Accent4 = new(0xFFFFAB00);
        }

        public static class Orange
        {
            public static readonly Color Lighten5 = new(0xFFFFF3E0);
            public static readonly Color Lighten4 = new(0xFFFFE0B2);
            public static readonly Color Lighten3 = new(0xFFFFCC80);
            public static readonly Color Lighten2 = new(0xFFFFB74D);
            public static readonly Color Lighten1 = new(0xFFFFA726);
            
            public static readonly Color Medium = new(0xFFFF9800);
            
            public static readonly Color Darken1 = new(0xFFFB8C00);
            public static readonly Color Darken2 = new(0xFFF57C00);
            public static readonly Color Darken3 = new(0xFFEF6C00);
            public static readonly Color Darken4 = new(0xFFE65100);
            
            public static readonly Color Accent1 = new(0xFFFFD180);
            public static readonly Color Accent2 = new(0xFFFFAB40);
            public static readonly Color Accent3 = new(0xFFFF9100);
            public static readonly Color Accent4 = new(0xFFFF6D00);
        }

        public static class DeepOrange
        {
            public static readonly Color Lighten5 = new(0xFFFBE9E7);
            public static readonly Color Lighten4 = new(0xFFFFCCBC);
            public static readonly Color Lighten3 = new(0xFFFFAB91);
            public static readonly Color Lighten2 = new(0xFFFF8A65);
            public static readonly Color Lighten1 = new(0xFFFF7043);
            
            public static readonly Color Medium = new(0xFFFF5722);
            
            public static readonly Color Darken1 = new(0xFFF4511E);
            public static readonly Color Darken2 = new(0xFFE64A19);
            public static readonly Color Darken3 = new(0xFFD84315);
            public static readonly Color Darken4 = new(0xFFBF360C);
            
            public static readonly Color Accent1 = new(0xFFFF9E80);
            public static readonly Color Accent2 = new(0xFFFF6E40);
            public static readonly Color Accent3 = new(0xFFFF3D00);
            public static readonly Color Accent4 = new(0xFFDD2C00);
        }

        public static class Brown
        {
            public static readonly Color Lighten5 = new(0xFFEFEBE9);
            public static readonly Color Lighten4 = new(0xFFD7CCC8);
            public static readonly Color Lighten3 = new(0xFFBCAAA4);
            public static readonly Color Lighten2 = new(0xFFA1887F);
            public static readonly Color Lighten1 = new(0xFF8D6E63);
            
            public static readonly Color Medium = new(0xFF795548);
            
            public static readonly Color Darken1 = new(0xFF6D4C41);
            public static readonly Color Darken2 = new(0xFF5D4037);
            public static readonly Color Darken3 = new(0xFF4E342E);
            public static readonly Color Darken4 = new(0xFF3E2723);
        }

        public static class Grey
        {
            public static readonly Color Lighten5 = new(0xFFFAFAFA);
            public static readonly Color Lighten4 = new(0xFFF5F5F5);
            public static readonly Color Lighten3 = new(0xFFEEEEEE);
            public static readonly Color Lighten2 = new(0xFFE0E0E0);
            public static readonly Color Lighten1 = new(0xFFBDBDBD);
            
            public static readonly Color Medium = new(0xFF9E9E9E);
            
            public static readonly Color Darken1 = new(0xFF757575);
            public static readonly Color Darken2 = new(0xFF616161);
            public static readonly Color Darken3 = new(0xFF424242);
            public static readonly Color Darken4 = new(0xFF212121);
        }

        public static class BlueGrey
        {
            public static readonly Color Lighten5 = new(0xFFECEFF1);
            public static readonly Color Lighten4 = new(0xFFCFD8DC);
            public static readonly Color Lighten3 = new(0xFFB0BEC5);
            public static readonly Color Lighten2 = new(0xFF90A4AE);
            public static readonly Color Lighten1 = new(0xFF78909C);
            
            public static readonly Color Medium = new(0xFF607D8B);
            
            public static readonly Color Darken1 = new(0xFF546E7A);
            public static readonly Color Darken2 = new(0xFF455A64);
            public static readonly Color Darken3 = new(0xFF37474F);
            public static readonly Color Darken4 = new(0xFF263238);
        }
    }
}