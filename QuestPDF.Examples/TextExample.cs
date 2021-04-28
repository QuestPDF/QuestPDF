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
        [ImageSize(1600, 500)]
        public void Test(IContainer container)
        {
            List<TextElement> Lorem()
            {
                return new List<TextElement>
                {
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