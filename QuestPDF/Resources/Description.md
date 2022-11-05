[![Dotnet](https://img.shields.io/badge/platform-.NET-blue)](https://www.nuget.org/packages/QuestPDF/)
[![GitHub Repo stars](https://img.shields.io/github/stars/QuestPDF/QuestPDF)](https://github.com/QuestPDF/QuestPDF/stargazers)
[![Nuget version](https://img.shields.io/nuget/v/QuestPdf)](https://www.nuget.org/packages/QuestPDF/)
[![Nuget download](https://img.shields.io/nuget/dt/QuestPDF)](https://www.nuget.org/packages/QuestPDF/)
[![License](https://img.shields.io/github/license/QuestPDF/QuestPDF)](https://github.com/QuestPDF/QuestPDF/blob/main/LICENSE)
[![Sponsor project](https://img.shields.io/badge/sponsor-project-red)](https://github.com/sponsors/QuestPDF)

QuestPDF is an open-source .NET library for PDF documents generation.

It offers a layout engine designed with a full paging support in mind. The document consists of many simple elements (e.g. border, background, image, text, padding, table, grid etc.) that are composed together to create more complex structures. This way, as a developer, you can understand the behavior of every element and use them with full confidence. Additionally, the document and all its elements support paging functionality. For example, an element can be moved to the next page (if there is not enough space) or even be split between pages like table's rows. All of it done with predictable and discoverable Fluent API.

## Installation

The library is available as a nuget package. You can install it as any other nuget package from your IDE, try to search by `QuestPDF`. You can find package details [on this webpage](https://www.nuget.org/packages/QuestPDF/).

```xml
// Package Manager
Install-Package QuestPDF

// .NET CLI
dotnet add package QuestPDF

// Package reference in .csproj file
<PackageReference Include="QuestPDF" Version="2022.11.0" />
```

## Documentation

[![Getting started tutorial]( https://img.shields.io/badge/%F0%9F%9A%80%20read-getting%20started-blue)](https://www.questpdf.com/documentation/getting-started.html)
A short and easy to follow tutorial showing how to design an invoice document under 200 lines of code.


[![API reference](https://img.shields.io/badge/%F0%9F%93%96%20read-API%20reference-blue)](https://www.questpdf.com/documentation/api-reference.html)
A detailed description of behavior of all available components and how to use them with C# Fluent API.


[![Patterns and Practices](https://img.shields.io/badge/%F0%9F%94%8D%20read-patterns%20and%20practices-blue)](https://www.questpdf.com/documentation/patterns-and-practices.html#document-metadata)
Everything that may help you designing great reports and create reusable code that is easy to maintain.

## Simplicity is the key

How easy it is to start and prototype with QuestPDF? Really easy thanks to its minimal API! Please analyse the code below:

```#
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

// code in your main method
Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);
        page.Background(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(20));
        
        page.Header()
            .Text("Hello PDF!")
            .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);
        
        page.Content()
            .PaddingVertical(1, Unit.Centimetre)
            .Column(x =>
            {
                x.Spacing(20);
                
                x.Item().Text(Placeholders.LoremIpsum());
                x.Item().Image(Placeholders.Image(200, 100));
            });
        
        page.Footer()
            .AlignCenter()
            .Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
            });
    });
})
.GeneratePdf("hello.pdf");
```

And compare it to the produced PDF file:

![invoice](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/main/images/minimal-example-shadow.png)

## Are you ready for more?

The Fluent API of QuestPDF scales really well. It is easy to create and maintain even most complex documents. Read [the Getting started tutorial](https://www.questpdf.com/documentation/getting-started.html) to learn QuestPDF basics and implement an invoice under 200 lines of code. You can also investigate and play with the code from [the example repository](https://github.com/QuestPDF/example-invoice).

![invoice](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/main/images/invoice-small.png)
