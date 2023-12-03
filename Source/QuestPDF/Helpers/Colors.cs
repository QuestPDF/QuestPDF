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
        public const string Black = "#000000";
        public const string White = "#ffffff";
        public const string Transparent = "#00000000";

        public static class Red
        {
            public const string Lighten5 = "#ffebee";
            public const string Lighten4 = "#ffcdd2";
            public const string Lighten3 = "#ef9a9a";
            public const string Lighten2 = "#e57373";
            public const string Lighten1 = "#ef5350";
            
            public const string Medium = "#f44336";
            
            public const string Darken1 = "#e53935";
            public const string Darken2 = "#d32f2f";
            public const string Darken3 = "#c62828";
            public const string Darken4 = "#b71c1c";
            
            public const string Accent1 = "#ff8a80";
            public const string Accent2 = "#ff5252";
            public const string Accent3 = "#ff1744";
            public const string Accent4 = "#d50000";
        }

        public static class Pink
        {
            public const string Lighten5 = "#fce4ec";
            public const string Lighten4 = "#f8bbd0";
            public const string Lighten3 = "#f48fb1";
            public const string Lighten2 = "#f06292";
            public const string Lighten1 = "#ec407a";
            
            public const string Medium = "#e91e63";
            
            public const string Darken1 = "#d81b60";
            public const string Darken2 = "#c2185b";
            public const string Darken3 = "#ad1457";
            public const string Darken4 = "#880e4f";
            
            public const string Accent1 = "#ff80ab";
            public const string Accent2 = "#ff4081";
            public const string Accent3 = "#f50057";
            public const string Accent4 = "#c51162";
        }

        public static class Purple
        {
            public const string Lighten5 = "#f3e5f5";
            public const string Lighten4 = "#e1bee7";
            public const string Lighten3 = "#ce93d8";
            public const string Lighten2 = "#ba68c8";
            public const string Lighten1 = "#ab47bc";
            
            public const string Medium = "#9c27b0";
            
            public const string Darken1 = "#8e24aa";
            public const string Darken2 = "#7b1fa2";
            public const string Darken3 = "#6a1b9a";
            public const string Darken4 = "#4a148c";
            
            public const string Accent1 = "#ea80fc";
            public const string Accent2 = "#e040fb";
            public const string Accent3 = "#d500f9";
            public const string Accent4 = "#aa00ff";
        }

        public static class DeepPurple
        {
            public const string Lighten5 = "#ede7f6";
            public const string Lighten4 = "#d1c4e9";
            public const string Lighten3 = "#b39ddb";
            public const string Lighten2 = "#9575cd";
            public const string Lighten1 = "#7e57c2";
            
            public const string Medium = "#673ab7";
            
            public const string Darken1 = "#5e35b1";
            public const string Darken2 = "#512da8";
            public const string Darken3 = "#4527a0";
            public const string Darken4 = "#311b92";
            
            public const string Accent1 = "#b388ff";
            public const string Accent2 = "#7c4dff";
            public const string Accent3 = "#651fff";
            public const string Accent4 = "#6200ea";
        }

        public static class Indigo
        {
            public const string Lighten5 = "#e8eaf6";
            public const string Lighten4 = "#c5cae9";
            public const string Lighten3 = "#9fa8da";
            public const string Lighten2 = "#7986cb";
            public const string Lighten1 = "#5c6bc0";
            
            public const string Medium = "#3f51b5";
            
            public const string Darken1 = "#3949ab";
            public const string Darken2 = "#303f9f";
            public const string Darken3 = "#283593";
            public const string Darken4 = "#1a237e";
            
            public const string Accent1 = "#8c9eff";
            public const string Accent2 = "#536dfe";
            public const string Accent3 = "#3d5afe";
            public const string Accent4 = "#304ffe";
        }

        public static class Blue
        {
            public const string Lighten5 = "#e3f2fd";
            public const string Lighten4 = "#bbdefb";
            public const string Lighten3 = "#90caf9";
            public const string Lighten2 = "#64b5f6";
            public const string Lighten1 = "#42a5f5";
            
            public const string Medium = "#2196f3";
            
            public const string Darken1 = "#1e88e5";
            public const string Darken2 = "#1976d2";
            public const string Darken3 = "#1565c0";
            public const string Darken4 = "#0d47a1";
            
            public const string Accent1 = "#82b1ff";
            public const string Accent2 = "#448aff";
            public const string Accent3 = "#2979ff";
            public const string Accent4 = "#2962ff";
        }

        public static class LightBlue
        {
            public const string Lighten5 = "#e1f5fe";
            public const string Lighten4 = "#b3e5fc";
            public const string Lighten3 = "#81d4fa";
            public const string Lighten2 = "#4fc3f7";
            public const string Lighten1 = "#29b6f6";
            
            public const string Medium = "#03a9f4";
            
            public const string Darken1 = "#039be5";
            public const string Darken2 = "#0288d1";
            public const string Darken3 = "#0277bd";
            public const string Darken4 = "#01579b";
            
            public const string Accent1 = "#80d8ff";
            public const string Accent2 = "#40c4ff";
            public const string Accent3 = "#00b0ff";
            public const string Accent4 = "#0091ea";
        }

        public static class Cyan
        {
            public const string Lighten5 = "#e0f7fa";
            public const string Lighten4 = "#b2ebf2";
            public const string Lighten3 = "#80deea";
            public const string Lighten2 = "#4dd0e1";
            public const string Lighten1 = "#26c6da";
            
            public const string Medium = "#00bcd4";
            
            public const string Darken1 = "#00acc1";
            public const string Darken2 = "#0097a7";
            public const string Darken3 = "#00838f";
            public const string Darken4 = "#006064";
            
            public const string Accent1 = "#84ffff";
            public const string Accent2 = "#18ffff";
            public const string Accent3 = "#00e5ff";
            public const string Accent4 = "#00b8d4";
        }

        public static class Teal
        {
            public const string Lighten5 = "#e0f2f1";
            public const string Lighten4 = "#b2dfdb";
            public const string Lighten3 = "#80cbc4";
            public const string Lighten2 = "#4db6ac";
            public const string Lighten1 = "#26a69a";
            
            public const string Medium = "#009688";
            
            public const string Darken1 = "#00897b";
            public const string Darken2 = "#00796b";
            public const string Darken3 = "#00695c";
            public const string Darken4 = "#004d40";
            
            public const string Accent1 = "#a7ffeb";
            public const string Accent2 = "#64ffda";
            public const string Accent3 = "#1de9b6";
            public const string Accent4 = "#00bfa5";
        }

        public static class Green
        {
            public const string Lighten5 = "#e8f5e9";
            public const string Lighten4 = "#c8e6c9";
            public const string Lighten3 = "#a5d6a7";
            public const string Lighten2 = "#81c784";
            public const string Lighten1 = "#66bb6a";
            
            public const string Medium = "#4caf50";
            
            public const string Darken1 = "#43a047";
            public const string Darken2 = "#388e3c";
            public const string Darken3 = "#2e7d32";
            public const string Darken4 = "#1b5e20";
            
            public const string Accent1 = "#b9f6ca";
            public const string Accent2 = "#69f0ae";
            public const string Accent3 = "#00e676";
            public const string Accent4 = "#00c853";
        }

        public static class LightGreen
        {
            public const string Lighten5 = "#f1f8e9";
            public const string Lighten4 = "#dcedc8";
            public const string Lighten3 = "#c5e1a5";
            public const string Lighten2 = "#aed581";
            public const string Lighten1 = "#9ccc65";
            
            public const string Medium = "#8bc34a";
            
            public const string Darken1 = "#7cb342";
            public const string Darken2 = "#689f38";
            public const string Darken3 = "#558b2f";
            public const string Darken4 = "#33691e";
            
            public const string Accent1 = "#ccff90";
            public const string Accent2 = "#b2ff59";
            public const string Accent3 = "#76ff03";
            public const string Accent4 = "#64dd17";
        }

        public static class Lime
        {
            public const string Lighten5 = "#f9fbe7";
            public const string Lighten4 = "#f0f4c3";
            public const string Lighten3 = "#e6ee9c";
            public const string Lighten2 = "#dce775";
            public const string Lighten1 = "#d4e157";
            
            public const string Medium = "#cddc39";
            
            public const string Darken1 = "#c0ca33";
            public const string Darken2 = "#afb42b";
            public const string Darken3 = "#9e9d24";
            public const string Darken4 = "#827717";
            
            public const string Accent1 = "#f4ff81";
            public const string Accent2 = "#eeff41";
            public const string Accent3 = "#c6ff00";
            public const string Accent4 = "#aeea00";
        }

        public static class Yellow
        {
            public const string Lighten5 = "#fffde7";
            public const string Lighten4 = "#fff9c4";
            public const string Lighten3 = "#fff59d";
            public const string Lighten2 = "#fff176";
            public const string Lighten1 = "#ffee58";
            
            public const string Medium = "#ffeb3b";
            
            public const string Darken1 = "#fdd835";
            public const string Darken2 = "#fbc02d";
            public const string Darken3 = "#f9a825";
            public const string Darken4 = "#f57f17";
            
            public const string Accent1 = "#ffff8d";
            public const string Accent2 = "#ffff00";
            public const string Accent3 = "#ffea00";
            public const string Accent4 = "#ffd600";
        }

        public static class Amber
        {
            public const string Lighten5 = "#fff8e1";
            public const string Lighten4 = "#ffecb3";
            public const string Lighten3 = "#ffe082";
            public const string Lighten2 = "#ffd54f";
            public const string Lighten1 = "#ffca28";
            
            public const string Medium = "#ffc107";
            
            public const string Darken1 = "#ffb300";
            public const string Darken2 = "#ffa000";
            public const string Darken3 = "#ff8f00";
            public const string Darken4 = "#ff6f00";
            
            public const string Accent1 = "#ffe57f";
            public const string Accent2 = "#ffd740";
            public const string Accent3 = "#ffc400";
            public const string Accent4 = "#ffab00";
        }

        public static class Orange
        {
            public const string Lighten5 = "#fff3e0";
            public const string Lighten4 = "#ffe0b2";
            public const string Lighten3 = "#ffcc80";
            public const string Lighten2 = "#ffb74d";
            public const string Lighten1 = "#ffa726";
            
            public const string Medium = "#ff9800";
            
            public const string Darken1 = "#fb8c00";
            public const string Darken2 = "#f57c00";
            public const string Darken3 = "#ef6c00";
            public const string Darken4 = "#e65100";
            
            public const string Accent1 = "#ffd180";
            public const string Accent2 = "#ffab40";
            public const string Accent3 = "#ff9100";
            public const string Accent4 = "#ff6d00";
        }

        public static class DeepOrange
        {
            public const string Lighten5 = "#fbe9e7";
            public const string Lighten4 = "#ffccbc";
            public const string Lighten3 = "#ffab91";
            public const string Lighten2 = "#ff8a65";
            public const string Lighten1 = "#ff7043";
            
            public const string Medium = "#ff5722";
            
            public const string Darken1 = "#f4511e";
            public const string Darken2 = "#e64a19";
            public const string Darken3 = "#d84315";
            public const string Darken4 = "#bf360c";
            
            public const string Accent1 = "#ff9e80";
            public const string Accent2 = "#ff6e40";
            public const string Accent3 = "#ff3d00";
            public const string Accent4 = "#dd2c00";
        }

        public static class Brown
        {
            public const string Lighten5 = "#efebe9";
            public const string Lighten4 = "#d7ccc8";
            public const string Lighten3 = "#bcaaa4";
            public const string Lighten2 = "#a1887f";
            public const string Lighten1 = "#8d6e63";
            
            public const string Medium = "#795548";
            
            public const string Darken1 = "#6d4c41";
            public const string Darken2 = "#5d4037";
            public const string Darken3 = "#4e342e";
            public const string Darken4 = "#3e2723";
        }

        public static class Grey
        {
            public const string Lighten5 = "#fafafa";
            public const string Lighten4 = "#f5f5f5";
            public const string Lighten3 = "#eeeeee";
            public const string Lighten2 = "#e0e0e0";
            public const string Lighten1 = "#bdbdbd";
            
            public const string Medium = "#9e9e9e";
            
            public const string Darken1 = "#757575";
            public const string Darken2 = "#616161";
            public const string Darken3 = "#424242";
            public const string Darken4 = "#212121";
        }

        public static class BlueGrey
        {
            public const string Lighten5 = "#eceff1";
            public const string Lighten4 = "#cfd8dc";
            public const string Lighten3 = "#b0bec5";
            public const string Lighten2 = "#90a4ae";
            public const string Lighten1 = "#78909c";
            
            public const string Medium = "#607d8b";
            
            public const string Darken1 = "#546e7a";
            public const string Darken2 = "#455a64";
            public const string Darken3 = "#37474f";
            public const string Darken4 = "#263238";
        }
    }
}