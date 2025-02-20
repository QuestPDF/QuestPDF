using System;
using System.Linq;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Helpers
{
    public static class Placeholders
    {
        static Placeholders()
        {
            SkNativeDependencyCompatibilityChecker.Test();
        }
        
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

        /// <summary>
        /// Returns the commonly used 'Lorem ipsum dolor sit amet' placeholder text.
        /// </summary>
        public static string LoremIpsum()
        {
            return CommonParagraph;
        }

        /// <summary>
        /// Generates a random text ideal for concise labels like product names.
        /// </summary>
        /// <example>
        /// "Beatae dolor" <br />
        /// "Rerum quibusdam perspiciatis" <br />
        /// "Fugiat aperiam officiis"
        /// </example>
        public static string Label()
        {
            return RandomWords(2, 3).FirstCharToUpper();
        }

        /// <summary>
        /// Generates random text ideal for single sentences, like product description.
        /// </summary>
        /// <example>
        /// Vero a id optio consequuntur dignissimos repellendus provident blanditiis.
        /// </example>
        public static string Sentence()
        {
            return RandomWords(6, 12).FirstCharToUpper() + ".";
        }

        /// <summary>
        /// Generates random text formatted as a question.
        /// </summary>
        /// <example>
        /// Sequi enim voluptas quasi modi aspernatur dolorem?
        /// </example>
        public static string Question()
        {
            return RandomWords(4, 8).FirstCharToUpper() + "?";
        }

        /// <summary>
        /// Generates random text suited for paragraphs, like detailed product description.
        /// </summary>
        public static string Paragraph()
        {
            var length = Random.Next(3, 6);

            var sentences = Enumerable
                .Range(0, length)
                .Select(x => Sentence());

            return string.Join(" ", sentences);
        }
        
        /// <summary>
        /// Generates random text ideal for multiple paragraphs, resembling an article.
        /// </summary>
        public static string Paragraphs()
        {
            var length = Random.Next(2, 5);

            var sentences = Enumerable
                .Range(0, length)
                .Select(x => Paragraph());

            return string.Join("\n", sentences);
        }
        
        /// <summary>
        /// Generates random text in the format of an email address.
        /// </summary>
        /// <example>
        /// consequuntur35@blanditiis.com
        /// </example>
        public static string Email()
        {
            return $"{LongRandomWord()}{Random.Next(10, 99)}@{LongRandomWord()}.com";
        }

        /// <summary>
        /// Generates random text looking like a two-word name, with capitalized initials.
        /// </summary>
        /// <example>
        /// "Voluptates Inventore" <br />
        /// "Praesentium Consectetur" <br />
        /// "Voluptatibus Molestias" <br />
        /// </example>
        public static string Name()
        {
            return LongRandomWord().FirstCharToUpper() + " " + LongRandomWord().FirstCharToUpper();
        }
        
        /// <summary>
        /// Generates random text in the format of a phone number.
        /// </summary>
        /// <example>
        /// 180-204-1358
        /// </example>
        public static string PhoneNumber()
        {
            return $"{Random.Next(100, 999)}-{Random.Next(100, 999)}-{Random.Next(1000, 9999)}";
        }
        
        /// <summary>
        /// Generates random text resembling a webpage address.
        /// </summary>
        /// <example>
        /// www.libero.com
        /// </example>
        public static string WebpageUrl()
        {
            return $"www.{LongRandomWord()}.com";
        }
        
        private static string FirstCharToUpper(this string text)
        {
            return text.First().ToString().ToUpper() + text.Substring(1);
        }

        #endregion
        
        #region Time

        private static DateTime RandomDate()
        {
            var dayOffset = (Random.NextDouble() - 0.5) * 1000;
            return System.DateTime.Now - TimeSpan.FromDays(dayOffset);
        }

        /// <summary>
        /// Generates random text representation of a random time.
        /// </summary>
        /// <example>
        /// 18:34:47
        /// </example>
        public static string Time()
        {
            return RandomDate().ToString("T");
        }

        /// <summary>
        /// Generates random text that resembles a date value using short formatting.
        /// </summary>
        /// <example>
        /// 04/09/2023
        /// </example>
        public static string ShortDate()
        {
            return RandomDate().ToString("d");
        }

        /// <summary>
        /// Generates random text that resembles a full date value.
        /// </summary>
        /// <example>
        /// Monday, 18 November 2024
        /// </example>
        public static string LongDate()
        {
            return RandomDate().ToString("D");
        }

        /// <summary>
        /// Generates random text that resembles a datetime value.
        /// </summary>
        /// <example>
        /// 04/03/2024 20:43:15
        /// </example>
        public static string DateTime()
        {
            return RandomDate().ToString("G");
        }

        #endregion

        #region Numbers

        /// <summary>
        /// Generates random text mimicking an integer value, ranging from 0 to 10,000.
        /// </summary>
        public static string Integer()
        {
            return Random.Next(0, 10_000).ToString();
        }

        /// <summary>
        /// Generates random text in the style of a local-formatted decimal, values from 0 to 100 with two decimal points precision.
        /// </summary>
        /// <example>
        /// 1,28 <br />
        /// 7,94 <br />
        /// 67,30
        /// </example>
        public static string Decimal()
        {
            return (Random.NextDouble() * Random.Next(0, 100)).ToString("N2");
        }

        /// <summary>
        /// Generates random text resembling a percentage value.
        /// </summary>
        /// <example>
        /// 48% <br />
        /// 14% <br />
        /// 23%
        /// </example>
        public static string Percent()
        {
            return (Random.NextDouble() * 100).ToString("N0") + "%";
        }

        #endregion

        #region Visual

        private static readonly Color[] BackgroundColors =
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
        
        /// <summary>
        /// Returns a random bright color from the Material Design palette.
        /// </summary>
        /// <example>
        /// #ffab91 <br />
        /// #bcaaa4 <br />
        /// #ffab91
        /// </example>
        public static Color BackgroundColor()
        {
            var index = Random.Next(0, BackgroundColors.Length);
            return BackgroundColors[index];
        }
        
        /// <summary>
        /// Returns a random color from the Material Design palette.
        /// </summary>
        /// <example>
        /// #9e9e9e <br />
        /// #f44336 <br />
        /// #9c27b0
        /// </example>
        public static Color Color()
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

        /// <summary>
        /// Generates a random image with a soft color gradient with provided <paramref name="width" /> and <paramref name="height" />.
        /// </summary>
        /// <remarks>
        /// Caution: using this method may significantly reduce document generation performance. Please do not use it when performing benchmarks.
        /// </remarks>
        /// <returns>Random image encoded in the JPEG format.</returns>
        public static byte[] Image(int width, int height)
        {
            return Image(new ImageSize(width, height));
        }
        
        /// <summary>
        /// Generates a random image with a soft color gradient.
        /// </summary>
        /// <remarks>
        /// For performance reasons, this method may reduce the <paramref name="size" /> argument to at most 64 pixels, while preserving its aspect ratio.
        /// </remarks>
        /// <returns>Random image encoded in the JPEG format.</returns>
        public static byte[] Image(ImageSize size)
        {
            size = LimitSize(size);
            
            var colors = BackgroundColors
                .OrderBy(_ => Random.Next())
                .Take(2)
                .ToArray();
            
            using var placeholderImage = SkImage.GeneratePlaceholder(size.Width, size.Height, colors[0], colors[1]);
            using var imageData = placeholderImage.GetEncodedData();
            return imageData.ToBytes();

            static ImageSize LimitSize(ImageSize size, int maxSize = 64)
            {
                if (size.Width < maxSize && size.Height < maxSize)
                    return size;

                return size.Width > size.Height
                    ? new ImageSize(maxSize, maxSize * size.Height / size.Width)
                    : new ImageSize(maxSize * size.Width / size.Height, maxSize);
            }
        }
        
        #endregion
    }
}