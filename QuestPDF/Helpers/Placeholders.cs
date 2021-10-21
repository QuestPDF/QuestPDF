using System;
using System.Linq;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Helpers
{
    public static class Placeholders
    {
        public static readonly Random Random = new Random();
        
        #region Word Cache

        private const string CommonParagraph =
            "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod " +
            "tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim " +
            "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea " +
            "commodo consequat. Duis aute irure dolor in reprehenderit in voluptate " +
            "velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint " +
            "occaecat cupidatat non proident, sunt in culpa qui officia deserunt " +
            "mollit anim id est laborum.";

        private static readonly string[] LatinWords =
        {
            "exercitationem", "perferendis", "perspiciatis", "laborum", "eveniet",
            "sunt", "iure", "nam", "nobis", "eum", "cum", "officiis", "excepturi",
            "odio", "consectetur", "quasi", "aut", "quisquam", "vel", "eligendi",
            "itaque", "non", "odit", "tempore", "quaerat", "dignissimos",
            "facilis", "neque", "nihil", "expedita", "vitae", "vero", "ipsum",
            "nisi", "animi", "cumque", "pariatur", "velit", "modi", "natus",
            "iusto", "eaque", "sequi", "illo", "sed", "ex", "et", "voluptatibus",
            "tempora", "veritatis", "ratione", "assumenda", "incidunt", "nostrum",
            "placeat", "aliquid", "fuga", "provident", "praesentium", "rem",
            "necessitatibus", "suscipit", "adipisci", "quidem", "possimus",
            "voluptas", "debitis", "sint", "accusantium", "unde", "sapiente",
            "voluptate", "qui", "aspernatur", "laudantium", "soluta", "amet",
            "quo", "aliquam", "saepe", "culpa", "libero", "ipsa", "dicta",
            "reiciendis", "nesciunt", "doloribus", "autem", "impedit", "minima",
            "maiores", "repudiandae", "ipsam", "obcaecati", "ullam", "enim",
            "totam", "delectus", "ducimus", "quis", "voluptates", "dolores",
            "molestiae", "harum", "dolorem", "quia", "voluptatem", "molestias",
            "magni", "distinctio", "omnis", "illum", "dolorum", "voluptatum", "ea",
            "quas", "quam", "corporis", "quae", "blanditiis", "atque", "deserunt",
            "laboriosam", "earum", "consequuntur", "hic", "cupiditate",
            "quibusdam", "accusamus", "ut", "rerum", "error", "minus", "eius",
            "ab", "ad", "nemo", "fugit", "officia", "at", "in", "id", "quos",
            "reprehenderit", "numquam", "iste", "fugiat", "sit", "inventore",
            "beatae", "repellendus", "magnam", "recusandae", "quod", "explicabo",
            "doloremque", "aperiam", "consequatur", "asperiores", "commodi",
            "optio", "dolor", "labore", "temporibus", "repellat", "veniam",
            "architecto", "est", "esse", "mollitia", "nulla", "a", "similique",
            "eos", "alias", "dolore", "tenetur", "deleniti", "porro", "facere",
            "maxime", "corrupti"
        };
        
        private static readonly string[] LongLatinWords = LatinWords.Where(x => x.Length > 8).ToArray();

        #endregion

        #region Text

        private static string RandomWord()
        {
            var index = Random.Next(0, LatinWords.Length);
            return LatinWords[index];
        }
        
        private static string LongRandomWord()
        {
            var index = Random.Next(0, LongLatinWords.Length);
            return LongLatinWords[index];
        }

        private static string RandomWords(int min, int max)
        {
            var length = Random.Next(min, max + 1);

            var words = Enumerable
                .Range(0, length)
                .Select(x => RandomWord());

            return string.Join(" ", words);
        }

        public static string LoremIpsum() => CommonParagraph;
        
        public static string Label() => RandomWords(2, 3).FirstCharToUpper();
        public static string Sentence() => RandomWords(6, 12).FirstCharToUpper() + ".";
        public static string Question() => RandomWords(4, 8).FirstCharToUpper() + "?";

        public static string Paragraph()
        {
            var length = Random.Next(3, 6);

            var sentences = Enumerable
                .Range(0, length)
                .Select(x => Sentence());

            return string.Join(" ", sentences);
        }
        
        public static string Paragraphs()
        {
            var length = Random.Next(2, 5);

            var sentences = Enumerable
                .Range(0, length)
                .Select(x => Paragraph());

            return string.Join("\n", sentences);
        }
        
        public static string Email()
        {
            return $"{LongRandomWord()}{Random.Next(10, 99)}@{LongRandomWord()}.com";
        }

        public static string Name()
        {
            return LongRandomWord().FirstCharToUpper() + " " + LongRandomWord().FirstCharToUpper();
        }
        
        public static string PhoneNumber()
        {
            return $"{Random.Next(100, 999)}-{Random.Next(100, 999)}-{Random.Next(1000, 9999)}";
        }
        
        private static string FirstCharToUpper(this string text)
        {
            return text.First().ToString().ToUpper() + text.Substring(1);
        }

        #endregion
        
        #region Time

        private static DateTime RandomDate() => System.DateTime.Now - TimeSpan.FromDays(Random.NextDouble());

        public static string Time() => RandomDate().ToString("T");
        public static string ShortDate() => RandomDate().ToString("d");
        public static string LongDate() => RandomDate().ToString("D");
        public static string DateTime() => RandomDate().ToString("G");

        #endregion

        #region Numbers

        public static string Integer() => Random.Next(0, 10_000).ToString();
        public static string Decimal() => (Random.NextDouble() * Random.Next(0, 100)).ToString("N2");
        public static string Percent() => (Random.NextDouble() * 100).ToString("N0") + "%";

        #endregion

        #region Visual

        private static readonly string[] BackgroundColors =
        {
            Colors.Red.Lighten3,
            Colors.Pink.Lighten3,
            Colors.Purple.Lighten3,
            Colors.DeepPurple.Lighten3,
            Colors.Indigo.Lighten3,
            Colors.Blue.Lighten3,
            Colors.LightBlue.Lighten3,
            Colors.Cyan.Lighten3,
            Colors.Teal.Lighten3,
            Colors.Green.Lighten3,
            Colors.LightGreen.Lighten3,
            Colors.Lime.Lighten3,
            Colors.Yellow.Lighten3,
            Colors.Amber.Lighten3,
            Colors.Orange.Lighten3,
            Colors.DeepOrange.Lighten3,
            Colors.Brown.Lighten3,
            Colors.Grey.Lighten3,
            Colors.BlueGrey.Lighten3
        };
        
        public static string BackgroundColor()
        {
            var index = Random.Next(0, BackgroundColors.Length);
            return BackgroundColors[index];
        }
        
        public static string Color()
        {
            var colors = new[]
            {
                Colors.Red.Medium,
                Colors.Pink.Medium,
                Colors.Purple.Medium,
                Colors.DeepPurple.Medium,
                Colors.Indigo.Medium,
                Colors.Blue.Medium,
                Colors.LightBlue.Medium,
                Colors.Cyan.Medium,
                Colors.Teal.Medium,
                Colors.Green.Medium,
                Colors.LightGreen.Medium,
                Colors.Lime.Medium,
                Colors.Yellow.Medium,
                Colors.Amber.Medium,
                Colors.Orange.Medium,
                Colors.DeepOrange.Medium,
                Colors.Brown.Medium,
                Colors.Grey.Medium,
                Colors.BlueGrey.Medium
            };

            var index = Random.Next(0, colors.Length);
            return colors[index];
        }

        public static byte[] Image(int width, int height)
        {
            return Image(new Size(width, height));
        }
        
        public static byte[] Image(Size size)
        {
            // shuffle corner positions
            var targetPositions = new[]
            {
                new SKPoint(0, 0),
                new SKPoint(size.Width, 0),
                new SKPoint(0, size.Height),
                new SKPoint(size.Width, size.Height)
            };
            
            var positions = targetPositions
                .OrderBy(x => Random.Next())
                .ToList();
            
            // rand and shuffle colors
            var colors = BackgroundColors
                .OrderBy(x => Random.Next())
                .Take(4)
                .Select(SKColor.Parse)
                .ToArray();
            
            // create image with white background
            var imageInfo = new SKImageInfo((int)size.Width, (int)size.Height);
            using var surface = SKSurface.Create(imageInfo);
   
            using var backgroundPaint = new SKPaint
            {
                Color = SKColors.White
            };
            
            surface.Canvas.DrawRect(0, 0, size.Width, size.Height, backgroundPaint);

            // draw gradient
            SKShader GetForegroundShader(int index)
            {
                var radius = Math.Max(size.Width, size.Height);
                var color = colors[index];
                
                return SKShader.CreateRadialGradient(
                    positions[index], radius,
                    new[] {color, color.WithAlpha(0)}, new[] {0, 1f},
                    SKShaderTileMode.Decal);
            }
            
            using var shaderPaint = new SKPaint
            {
                Shader = SKShader.CreateCompose(
                    SKShader.CreateCompose(GetForegroundShader(0), GetForegroundShader(1)),
                    SKShader.CreateCompose(GetForegroundShader(2), GetForegroundShader(3)))
            };
            
            surface.Canvas.DrawRect(0, 0, size.Width, size.Height, shaderPaint);
            
            // return result as an image
            surface.Canvas.Save();
            return surface.Snapshot().Encode(SKEncodedImageFormat.Jpeg, 90).ToArray();
        }
        
        #endregion
    }
}