<a href="https://www.questpdf.com/" target="_blank">
  <img src="https://github.com/QuestPDF/example-invoice/raw/main/images/logo.svg" width="300"> 
</a>

---

[![Dotnet](https://img.shields.io/badge/platform-.NET-blue)](https://www.nuget.org/packages/QuestPDF/)
[![GitHub Repo stars](https://img.shields.io/github/stars/QuestPDF/QuestPDF)](https://github.com/QuestPDF/QuestPDF/stargazers)
[![Nuget version](https://img.shields.io/nuget/v/QuestPdf)](https://www.nuget.org/packages/QuestPDF/)
[![Nuget download](https://img.shields.io/nuget/dt/QuestPDF)](https://www.nuget.org/packages/QuestPDF/)
[![License](https://img.shields.io/github/license/QuestPDF/QuestPDF)](https://github.com/QuestPDF/QuestPDF/blob/main/LICENSE)
[![Sponsor project](https://img.shields.io/badge/sponsor-project-red)](https://github.com/sponsors/QuestPDF)

QuestPDF presents a new approach to PDF document generation. Unlike other libraries, it does not rely on the HTML-to-PDF conversion which in many cases is not reliable. Instead, it implements its own layouting engine that is optimized to cover all paging-related requirements. Then, everything is rendered using the SkiaSharp library (a Skia port for .NET, used in Chrome, Android, MAUI, etc.).

I have designed this layouting engine with full paging support in mind. The document consists of many simple elements (e.g. border, background, image, text, padding, table, grid etc.) that are composed together to create more complex structures. This way, as a developer, you can understand the behaviour of every element and use them with full confidence. Additionally, the document and all its elements support paging functionality. For example, an element can be moved to the next page (if there is not enough space) or even be split between pages like table's rows.

## Support QuestPDF

All great frameworks and libraries started from zero. Please help me make QuestPDF a commonly known library and an obvious choice for generating PDF documents. 

- ⭐ Give this repository a star,
- 💬 Share it with your team members,
- ❤ Please consider supporting

## Share

// consider removing logos

[![GitHub Repo stars](https://img.shields.io/badge/share%20on-reddit-red?logo=reddit)](https://reddit.com/submit?url=https://github.com/QuestPDF/QuestPDF&title=QuestPDF)

[![GitHub Repo stars](https://img.shields.io/badge/share%20on-hacker%20news-orange?logo=ycombinator)](https://news.ycombinator.com/submitlink?u=https://github.com/QuestPDF/QuestPDF)

[![GitHub Repo stars](https://img.shields.io/badge/share%20on-twitter-03A9F4?logo=twitter)](https://twitter.com/share?url=https://github.com/QuestPDF/QuestPDF&t=QuestPDF)

[![GitHub Repo stars](https://img.shields.io/badge/share%20on-facebook-1976D2?logo=facebook)](https://www.facebook.com/sharer/sharer.php?u=https://github.com/QuestPDF/QuestPDF)

[![GitHub Repo stars](https://img.shields.io/badge/share%20on-linkedin-3949AB?logo=linkedin)](https://www.linkedin.com/shareArticle?url=https://github.com/QuestPDF/QuestPDF&title=QuestPDF)

## Installation

The library is available as a nuget package. You can install it as any other nuget package from your IDE, try to search by `QuestPDF`. You can find package details [on this webpage](https://www.nuget.org/packages/QuestPDF/).

```xml
// Package Manager
Install-Package QuestPDF

// .NET CLI
dotnet add package QuestPDF

// Package reference in .csproj file
<PackageReference Include="QuestPDF" Version="2022.3.0" />
```

## Documentation

[![GitHub Repo stars]( https://img.shields.io/badge/%F0%9F%9A%80-getting%20started-blue)](https://www.linkedin.com/shareArticle?url=https://github.com/QuestPDF/QuestPDF&title=QuestPDF)
A short and easy to follow tutorial showing how to design an invoice document under 200 lines of code.


[![GitHub Repo stars](https://img.shields.io/badge/%F0%9F%93%96-API%20reference-blue)](https://www.linkedin.com/shareArticle?url=https://github.com/QuestPDF/QuestPDF&title=QuestPDF)
A detailed description of behavior of all available components and how to use them with C# Fluent API.


[![GitHub Repo stars](https://img.shields.io/badge/%F0%9F%94%8D-patterns%20and%20practices-blue)](https://www.linkedin.com/shareArticle?url=https://github.com/QuestPDF/QuestPDF&title=QuestPDF)
Everything that may help you designing great reports and create reusable code that is easy to maintain.


**[🚀 Getting started tutorial](https://www.questpdf.com/documentation/getting-started.html)** - a short and easy to follow tutorial showing how to design an invoice document under 200 lines of code.

**[📖 API Reference](https://www.questpdf.com/documentation/api-reference.html)** - a detailed description of behavior of all available components and how to use them with C# Fluent API.

**[ℹ️ Patterns and practices](https://www.questpdf.com/documentation/patterns-and-practices.html#document-metadata)** - everything that may help you designing great reports and create reusable code that is easy to maintain.

## Simplicity is the key

How easy it is to start and prototype with QuestPDF? Really easy thanks to its minimal API! Please analyse the code below:

```csharp
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

<img src="https://github.com/QuestPDF/example-invoice/raw/main/images/minimal-api.png" width="300px">

## Are you ready for more?

The Fluent API of QuestPDF scales really well. It is easy to create and maintain even most complex documents. Read [the Getting started tutorial](https://www.questpdf.com/documentation/getting-started.html) to learn QuestPDF basics and implement an invoice under 200 lines of code. You can also investigate and play with the code from [the example repository](https://github.com/QuestPDF/example-invoice).

<img src="https://github.com/QuestPDF/example-invoice/raw/main/images/invoice.png" width="400px">
