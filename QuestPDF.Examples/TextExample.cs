using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Examples
{
    public class TextExample : ExampleTestBase
    {
        [ShowResult]
        [ImageSize(1400, 600)]
        public void OldText(IContainer container)
        {
            var fonts = new[]
            {
                Fonts.Arial,
                Fonts.Calibri,
                Fonts.Cambria,
                Fonts.Candara,
                Fonts.ComicSans,
                Fonts.Consolas,
                Fonts.Corbel,
                Fonts.Courier,
                Fonts.CourierNew,
                Fonts.Georgia,
                Fonts.Impact,
                Fonts.LucidaConsole,
                Fonts.SegoeSD,
                Fonts.SegoeUI,
                Fonts.Tahoma,
                Fonts.TimesNewRoman,
                Fonts.TimesRoman,
                Fonts.Trebuchet,
                Fonts.Verdana,
            };
            
            container
                .Padding(50)
                .Grid(stack =>
                {
                    stack.Spacing(10);
                    stack.Columns(2);

                    foreach (var font in fonts)
                    {
                        stack
                            .Element()
                            .Box()
                            .Background(Placeholders.BackgroundColor())
                            .Text($"Lorem ipsum dolor sit amet {font}", TextStyle.Default.Size(24).FontType(font));
                    }
                });
        }
        
        //[ShowResult]
        [ImageSize(1400, 800)]
        public void Test(IContainer container)
        {
            List<TextElement> Lorem()
            {
                return new List<TextElement>
                {
                    new TextElement()
                    {
                        Style = TextStyle.Default.Size(32).BackgroundColor(Placeholders.BackgroundColor()),
                        Text = "Podstawowy łaciński Tabela znaków Unicode"
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.FontType("Segoe UI Emoji").Size(32).BackgroundColor(Placeholders.BackgroundColor()),
                        Text = "✔"
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.FontType("Segoe UI Emoji").Size(32).BackgroundColor(Placeholders.BackgroundColor()),
                        Text = "🥛"
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.FontType("Segoe UI Emoji").Size(32).BackgroundColor(Placeholders.BackgroundColor()),
                        Text = "🧀"
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.FontType("Segoe UI Emoji").Size(32).BackgroundColor(Placeholders.BackgroundColor()),
                        Text = "❤🚵‍♀️"
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.Size(32).BackgroundColor(Placeholders.BackgroundColor()),
                        Text = "Lorem ipsum "
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.Size(24).BackgroundColor(Placeholders.BackgroundColor()),
                        Text = " dolor "
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.Size(64).BackgroundColor(Placeholders.BackgroundColor()),
                        Text = " sijt "
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.Size(32).BackgroundColor(Placeholders.BackgroundColor()),
                        Text = " amet"
                    }
                };
            }

            Func<List<TextElement>> Source = () => Split(RandomText());

            container
                .Padding(50)
                .Box().Border(1).Stack(stack =>
                    {
                        stack
                            .Element()
                            .Box()
                            //.Background(Placeholders.BackgroundColor())
                            .Element(new TextRun()
                            {
                                Elements = Lorem()
                            });

                        stack
                            .Element()
                            .Box()
                            //.Background(Placeholders.BackgroundColor())
                            .Element(new TextRun()
                            {
                                Elements = Source()
                            });

                        stack
                            .Element()
                            .Box()
                            //.Background(Placeholders.BackgroundColor())
                            .Element(new TextRun()
                            {
                                Elements = Source()
                            });

                        stack
                            .Element()
                            .Box()
                            //.Background(Placeholders.BackgroundColor())
                            .Element(new TextRun()
                            {
                                Elements = Source()
                            });

                        stack
                            .Element()
                            .Box()
                            //.Background(Placeholders.BackgroundColor())
                            .Element(new TextRun()
                            {
                                Elements = Source()
                            });

                        stack
                            .Element()
                            .Box()
                            //.Background(Placeholders.BackgroundColor())
                            .Element(new TextRun()
                            {
                                Elements = Source()
                            });
                    });
        }

        List<TextElement> RandomText()
        {
            var sizes = new[] { 24, 32, 48, 64};
            
            return Placeholders
                .Sentence()
                .Split(" ")
                .Select(x => new TextElement
                {
                    Text = $"{x} ",
                    Style = TextStyle
                        .Default
                        //.Size(sizes[Placeholders.Random.Next(0, 3)])
                        .Size(24)
                        .BackgroundColor(Placeholders.BackgroundColor())
                        //.LineHeight((float)Placeholders.Random.NextDouble() / 2 + 1f)
                        .LineHeight(1.2f)
                })
                .ToList();
        }
        
        List<TextElement> Split(List<TextElement> elements)
        {
            return elements
                .SelectMany(x => x
                    .Text
                    .Select(y => new TextElement
                    {
                        Style = x.Style,
                        Text = y.ToString()
                    }))
                .ToList();
        }
    }
}