using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class TextExample : ExampleTestBase
    {
        [ShowResult]
        [ImageSize(1200, 500)]
        public void Test(IContainer container)
        {
            List<TextElement> Lorem()
            {
                return new List<TextElement>
                {
                    new TextElement()
                    {
                        Style = TextStyle.Default.Size(32).BackgroundColor(Colors.Red.Lighten3),
                        Text = "Lorem ipsum "
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.Size(24).BackgroundColor(Colors.Orange.Lighten3),
                        Text = " dolor "
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.Size(64).BackgroundColor(Colors.Yellow.Lighten3),
                        Text = " sit "
                    },
                    new TextElement()
                    {
                        Style = TextStyle.Default.Size(32).BackgroundColor(Colors.Green.Lighten3),
                        Text = " amet"
                    }
                };
            }

            Func<List<TextElement>> Source = Lorem ;
            
            container
                .Padding(50)
                .Stack(row =>
                {
                    row.Spacing(50);
                    
                    row.Element().Box().Border(1).Stack(stack =>
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
                                Elements = Split(Source())
                            });
                    });
                    
                    row.Element().Box().Border(1).Stack(stack =>
                    {
                        stack
                            .Element()
                            .Box()
                            .Background(Placeholders.BackgroundColor())
                            .Element(new TextRun()
                            {
                                Elements = Dense(Source())
                            });

                        stack
                            .Element()
                            .Box()
                            .Background(Placeholders.BackgroundColor())
                            .Element(new TextRun()
                            {
                                Elements = Dense(Source())
                            });
                        
                        stack
                            .Element()
                            .Box()
                            .Background(Placeholders.BackgroundColor())
                            .Element(new TextRun()
                            {
                                Elements = Dense(Split(Source()))
                            });
                    });
                });
        }

        List<TextElement> RandomText()
        {
            return Placeholders
                .Sentence()
                .Split(" ")
                .Select(x => new TextElement
                {
                    Text = $"{x} ",
                    Style = TextStyle.Default.Size(Placeholders.Random.Next(24, 64))
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
        
        List<TextElement> Dense(List<TextElement> elements)
        {
            elements.ForEach(x => x.Style.IsDense = true);
            return elements;
        }
    }
}