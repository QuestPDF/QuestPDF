using QuestPDF.Infrastructure;

namespace QuestPDF.Helpers
{
    public static class PageSizes
    {
        public const int PointsPerInch = 72;
        
        public static Size A0 => new Size(2384, 3370);
        public static Size A1 => new Size(1684, 2384);
        public static Size A2 => new Size(1191, 1684);
        public static Size A3 => new Size(842, 1191);
        public static Size A3Transverse => new Size(842, 1191);
        public static Size A3Extra => new Size(913, 1262);
        public static Size A3ExtraTransverse => new Size(913, 1262);
        public static Size A3Rotated => new Size(1191, 842);
        public static Size A4 => new Size(595, 842);
        public static Size A4Transverse => new Size(595, 842);
        public static Size A4Extra => new Size(667, 914);
        public static Size A4Plus => new Size(595, 936);
        public static Size A4Rotated => new Size(842, 595);
        public static Size A4Small => new Size(595, 842);
        public static Size A5 => new Size(420, 595);
        public static Size A5Transverse => new Size(420, 595);
        public static Size A5Extra => new Size(492, 668);
        public static Size A5Rotated => new Size(595, 420);
        public static Size A6 => new Size(297, 420);
        public static Size A6Rotated => new Size(420, 297);
        public static Size A7 => new Size(210, 297);
        public static Size A8 => new Size(148, 210);
        public static Size A9 => new Size(105, 148);
        public static Size A10 => new Size(73, 105);		
		public static Size AnsiA => new Size(792, 612);
		public static Size AnsiB => new Size(1224,792);
        public static Size AnsiC => new Size(1224, 1584);
        public static Size AnsiD => new Size(1584, 2448);
        public static Size AnsiE => new Size(2448, 3168);		
        public static Size ARCHA => new Size(648, 864);
        public static Size ARCHB => new Size(864, 1296);
        public static Size ARCHC => new Size(1296, 1728);
        public static Size ARCHD => new Size(1728, 2592);
        public static Size ARCHE => new Size(2592, 3456);
		public static Size ARCH_E1 => new Size(2160, 3025);
		public static Size ARCH_E2 => new Size(1871, 2735);
		public static Size ARCH_E3 => new Size(1945, 2809);
		
        public static Size B0 => new Size(2920, 4127);
        public static Size B1 => new Size(2064, 2920);
        public static Size B2 => new Size(1460, 2064);
        public static Size B3 => new Size(1032, 1460);
        public static Size B4 => new Size(729, 1032);
        public static Size B4Rotated => new Size(1032, 729);
        public static Size B5 => new Size(516, 729);
        public static Size B5Transverse => new Size(516, 729);
        public static Size B5Rotated => new Size(729, 516);
        public static Size B6 => new Size(363, 516);
        public static Size B6Rotated => new Size(516, 363);
        public static Size B7 => new Size(258, 363);
        public static Size B8 => new Size(181, 258);
        public static Size B9 => new Size(127, 181);
        public static Size B10 => new Size(91, 127);
		
		public static Size C2 => new Size(1298, 1837);
		public static Size C3 => new Size(918, 1298);
        public static Size C4 => new Size(649, 918);
        public static Size C5 => new Size(459, 649);
        public static Size C6 => new Size(323, 459);
		
		public static Size D0 => new Size(3090, 2186);
		
        public static Size Comm10 => new Size(297, 684);
		
        public static Size DL => new Size(312, 624);
        public static Size DoublePostcard => new Size(567, 419.5f);
        public static Size DoublePostcardRotated => new Size(419.5f, 567);
		
        public static Size Env9 => new Size(279, 639);
        public static Size Env10 => new Size(297, 684);
        public static Size Env11 => new Size(324, 747);
        public static Size Env12 => new Size(342, 792);
        public static Size Env14 => new Size(360, 828);
        public static Size EnvC0 => new Size(2599, 3676);
        public static Size EnvC1 => new Size(1837, 2599);
        public static Size EnvC2 => new Size(1298, 1837);
        public static Size EnvC3 => new Size(918, 1296);
        public static Size EnvC4 => new Size(649, 918);
        public static Size EnvC5 => new Size(459, 649);
        public static Size EnvC6 => new Size(323, 459);
        public static Size EnvC65 => new Size(324, 648);
        public static Size EnvC7 => new Size(230, 323);
        public static Size EnvChou3 => new Size(340, 666);
        public static Size EnvChou3Rotated => new Size(666, 340);
        public static Size EnvChou4 => new Size(255, 581);
        public static Size EnvChou4Rotated => new Size(581, 255);
        public static Size EnvDL => new Size(312, 624);
        public static Size EnvInvite => new Size(624, 624);
        public static Size EnvISOB4 => new Size(708, 1001);
        public static Size EnvISOB5 => new Size(499, 709);
        public static Size EnvISOB6 => new Size(499, 354);
        public static Size EnvItalian => new Size(312, 652);
        public static Size EnvKaku2 => new Size(680, 941);
        public static Size EnvKaku2Rotated => new Size(941, 680);
        public static Size EnvKaku3 => new Size(612, 785);
        public static Size EnvKaku3Rotated => new Size(785, 612);
        public static Size EnvMonarch => new Size(279, 540);
        public static Size EnvPersonal => new Size(261, 468);
        public static Size EnvPRC1 => new Size(289, 468);
        public static Size EnvPRC1Rotated => new Size(468, 289);
        public static Size EnvPRC2 => new Size(289, 499);
        public static Size EnvPRC2Rotated => new Size(499, 289);
        public static Size EnvPRC3 => new Size(354, 499);
        public static Size EnvPRC3Rotated => new Size(499, 354);
        public static Size EnvPRC4 => new Size(312, 590);
        public static Size EnvPRC4Rotated => new Size(590, 312);
        public static Size EnvPRC5 => new Size(312, 624);
        public static Size EnvPRC5Rotated => new Size(624, 312);
        public static Size EnvPRC6 => new Size(340, 652);
        public static Size EnvPRC6Rotated => new Size(652, 340);
        public static Size EnvPRC7 => new Size(454, 652);
        public static Size EnvPRC7Rotated => new Size(652, 454);
        public static Size EnvPRC8 => new Size(340, 876);
        public static Size EnvPRC8Rotated => new Size(876, 340);
        public static Size EnvPRC9 => new Size(649, 918);
        public static Size EnvPRC9Rotated => new Size(918, 649);
        public static Size EnvPRC10 => new Size(918, 1298);
        public static Size EnvPRC10Rotated => new Size(1298, 918);
        public static Size EnvYou4 => new Size(298, 666);
        public static Size EnvYou4Rotated => new Size(666, 298);		
        public static Size Executive => new Size(522, 756);
		
        public static Size FanFoldUS => new Size(1071, 792);
        public static Size FanFoldGerman => new Size(612, 864);
        public static Size FanFoldGermanLegal => new Size(612, 936);
        public static Size Folio => new Size(595, 935);
		
        public static Size ISOB0 => new Size(2835, 4008);
        public static Size ISOB1 => new Size(2004, 2835);
        public static Size ISOB2 => new Size(1417, 2004);
        public static Size ISOB3 => new Size(1001, 1417);
        public static Size ISOB4 => new Size(709, 1001);
        public static Size ISOB5 => new Size(499, 709);
        public static Size ISOB5Extra => new Size(569.7f, 782);
        public static Size ISOB6 => new Size(354, 499);
        public static Size ISOB7 => new Size(249, 354);
        public static Size ISOB8 => new Size(176, 249);
        public static Size ISOB9 => new Size(125, 176);
        public static Size ISOB10 => new Size(88, 125);
		
        public static Size Ledger => new Size(1224, 792);
        public static Size Legal => new Size(612, 1008);
        public static Size LegalExtra => new Size(684, 1080);
        public static Size Letter => new Size(612, 792);
        public static Size LetterTransverse => new Size(612, 792);
        public static Size LetterExtra => new Size(684, 864);
        public static Size LetterExtraTransverse => new Size(684, 864);
        public static Size LetterPlus => new Size(612, 913.7f);
        public static Size LetterRotated => new Size(792, 612);
        public static Size LetterSmall => new Size(612, 792);
		
        public static Size Monarch => new Size(279, 540);
		
        public static Size Note => new Size(612, 792);
		
        public static Size Postcard => new Size(284, 419);
        public static Size PostcardRotated => new Size(419, 284);
		
        public static Size PRC16K => new Size(414, 610);
        public static Size PRC16KRotated => new Size(610, 414);
        public static Size PRC32K => new Size(275, 428);
        public static Size PRC32KBig => new Size(275, 428);
        public static Size PRC32KBigRotated => new Size(428, 275);
        public static Size PRC32KRotated => new Size(428, 275);
		
        public static Size Quarto => new Size(610, 780);
		
		public static Size Standard10x11 => new Size(720, 792);
        public static Size Standard10x13 => new Size(720, 936);
        public static Size Standard10x14 => new Size(720, 1008);
        public static Size Standard12x11 => new Size(864, 792);
        public static Size Standard15x11 => new Size(1080, 792);
        public static Size Standard7x9 => new Size(504, 648);
        public static Size Standard8x10 => new Size(576, 720);
        public static Size Standard9x11 => new Size(648, 792);
        public static Size Standard9x12 => new Size(648, 864);
        public static Size Statement => new Size(396, 612);
        public static Size SuperA => new Size(643, 1009);
        public static Size SuperB => new Size(864, 1380);
		
        public static Size Tabloid => new Size(792, 1224);
        public static Size TabloidExtra => new Size(864, 1296);
    }
}
