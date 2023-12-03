using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Helpers;

namespace QuestPDF.Examples;

public class PlaceholderExamples
{
    private static void PrintRandomValues(Func<string> generator)
    {
        foreach (var i in Enumerable.Range(0, 3))
            Console.WriteLine(generator());
    }

    [Test]
    public void Label()
    {
        PrintRandomValues(Placeholders.Label);
    }

    [Test]
    public void Sentence()
    {
        Console.WriteLine(Placeholders.Sentence());
    }

    [Test]
    public void Question()
    {
        Console.WriteLine(Placeholders.Question());
    }

    [Test]
    public void Paragraph()
    {
        Console.WriteLine(Placeholders.Paragraph());
    }

    [Test]
    public void Paragraphs()
    {
        Console.WriteLine(Placeholders.Paragraphs());
    }

    [Test]
    public void Email()
    {
        PrintRandomValues(Placeholders.Email);
    }

    [Test]
    public void Name()
    {
        PrintRandomValues(Placeholders.Name);
    }

    [Test]
    public void PhoneNumber()
    {
        PrintRandomValues(Placeholders.PhoneNumber);
    }

    [Test]
    public void Time()
    {
        PrintRandomValues(Placeholders.Time);
    }

    [Test]
    public void ShortDate()
    {
        PrintRandomValues(Placeholders.ShortDate);
    }

    [Test]
    public void LongDate()
    {
        PrintRandomValues(Placeholders.LongDate);
    }
    
    [Test]
    public void DateTime()
    {
        PrintRandomValues(Placeholders.DateTime);
    }
    
    [Test]
    public void Integer()
    {
        PrintRandomValues(Placeholders.Integer);
    }
    
    [Test]
    public void Decimal()
    {
        PrintRandomValues(Placeholders.Decimal);
    }
    
    [Test]
    public void Percent()
    {
        PrintRandomValues(Placeholders.Percent);
    }
    
    [Test]
    public void BackgroundColor()
    {
        PrintRandomValues(Placeholders.BackgroundColor);
    }
    
    [Test]
    public void Color()
    {
        PrintRandomValues(Placeholders.Color);
    }
}
    