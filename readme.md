<a href="https://www.questpdf.com/" target="_blank">
  <img src="https://github.com/QuestPDF/example-invoice/raw/main/images/logo.svg" width="400"> 
</a>

---

[![Dotnet](https://img.shields.io/badge/platform-.NET-blue)](https://www.nuget.org/packages/QuestPDF/)
[![GitHub Repo stars](https://img.shields.io/github/stars/QuestPDF/QuestPDF)](https://github.com/QuestPDF/QuestPDF/stargazers)
[![Nuget version](https://img.shields.io/nuget/v/QuestPdf)](https://www.nuget.org/packages/QuestPDF/)
[![Nuget download](https://img.shields.io/nuget/dt/QuestPDF)](https://www.nuget.org/packages/QuestPDF/)
[![License](https://img.shields.io/github/license/QuestPDF/QuestPDF)](https://github.com/QuestPDF/QuestPDF/blob/main/LICENSE)
[![Sponsor project](https://img.shields.io/badge/sponsor-project-red)](https://github.com/sponsors/QuestPDF)

QuestPDF is an open-source .NET library for PDF documents generation.

It offers a layouting engine designed with a full paging support in mind. The document consists of many simple elements (e.g. border, background, image, text, padding, table, grid etc.) that are composed together to create more complex structures. This way, as a developer, you can understand the behavior of every element and use them with full confidence. Additionally, the document and all its elements support paging functionality. For example, an element can be moved to the next page (if there is not enough space) or even be split between pages like table's rows.

Unlike other libraries, it does not rely on the HTML-to-PDF conversion which in many cases is not reliable. Instead, it implements its own layouting engine that is optimized to cover all paging-related requirements.

## Please help by giving a star

Choosing a project dependency could be difficult. We need to ensure stability and maintainability of our projects. Surveys show that GitHub stars count play an important factor when assessing library quality. 

⭐ Please give this repository a star. It takes seconds and help thousands of developers! ⭐

<img src="https://user-images.githubusercontent.com/9263853/190931857-8ca52ec8-cc7d-4d12-9467-4442b3342fa1.png" width="700" />


## Please share with the community

As an open-source project without funding, I cannot afford advertising QuestPDF in a typical way. Instead, the library relies on community interactions. Please consider sharing a post about QuestPDF and the value it provides. It really does help!

[![GitHub Repo stars](https://img.shields.io/badge/share%20on-reddit-red?logo=reddit)](https://reddit.com/submit?url=https://github.com/QuestPDF/QuestPDF&title=QuestPDF)
[![GitHub Repo stars](https://img.shields.io/badge/share%20on-hacker%20news-orange?logo=ycombinator)](https://news.ycombinator.com/submitlink?u=https://github.com/QuestPDF/QuestPDF)
[![GitHub Repo stars](https://img.shields.io/badge/share%20on-twitter-03A9F4?logo=twitter)](https://twitter.com/share?url=https://github.com/QuestPDF/QuestPDF&t=QuestPDF)
[![GitHub Repo stars](https://img.shields.io/badge/share%20on-facebook-1976D2?logo=facebook)](https://www.facebook.com/sharer/sharer.php?u=https://github.com/QuestPDF/QuestPDF)
[![GitHub Repo stars](https://img.shields.io/badge/share%20on-linkedin-3949AB?logo=linkedin)](https://www.linkedin.com/shareArticle?url=https://github.com/QuestPDF/QuestPDF&title=QuestPDF)

## Support development

It doesn't matter if you are a professional developer, creating a startup or work for an established company. All of us care about our tools and dependencies, about stability and security, about time and money we can safe, about quality we can offer. Please consider sponsoring QuestPDF to give me an extra motivational push to develop the next great feature.

> If you represent a company and want to help the entire community, please consider sponsoring QuestPDF using one of the higher tiers. All developers will see your company logo and the description of your choice. It is a fantastic way to build a strong relationship with the community, show that you care, or even find the best professionals. The truth is, no classical advertisement campaign is as effective as real engagement.

Special thanks to all companies that decided to sponsor QuestPDF development. This makes .NET ecosystem a better place for all developers and businesses!

| Company                                                                              | Description                                                                                                                     |
|--------------------------------------------------------------------------------------|:--------------------------------------------------------------------------------------------------------------------------------|
| <img src="Resources/jetbrains-logo.svg" width="100px"> | [JetBrains](https://www.jetbrains.com/) supports this project as part of the OSS Power-Ups program. Thank you!<br/>100$ / month |
| <img src="https://avatars.githubusercontent.com/u/2712328?v=4" width="100px"> | [Mark Gould](https://github.com/markgould) supports this project. Thank you!<br/>100$ / month |

[![Sponsor project](https://img.shields.io/badge/%E2%9D%A4%EF%B8%8F%20sponsor-QuestPDF-red)](https://github.com/sponsors/QuestPDF)

## Installation

The library is available as a nuget package. You can install it as any other nuget package from your IDE, try to search by `QuestPDF`. You can find package details [on this webpage](https://www.nuget.org/packages/QuestPDF/).

```xml
// Package Manager
Install-Package QuestPDF

// .NET CLI
dotnet add package QuestPDF

// Package reference in .csproj file
<PackageReference Include="QuestPDF" Version="2022.9.0" />
```

[![Nuget version](https://img.shields.io/badge/package%20details-QuestPDF-blue?logo=nuget)](https://www.nuget.org/packages/QuestPDF/)

## Documentation

[![Getting started tutorial]( https://img.shields.io/badge/%F0%9F%9A%80%20read-getting%20started-blue)](https://www.questpdf.com/getting-started)
A short and easy to follow tutorial showing how to design an invoice document under 200 lines of code.


[![API reference](https://img.shields.io/badge/%F0%9F%93%96%20read-API%20reference-blue)](https://www.questpdf.com/api-reference/index.html)
A detailed description of behavior of all available components and how to use them with C# Fluent API.


[![Patterns and Practices](https://img.shields.io/badge/%E2%9C%A8%20read-patterns%20and%20practices-blue)](https://www.questpdf.com/design-patterns)
Everything that may help you designing great reports and create reusable code that is easy to maintain.

## QuestPDF Previewer

The QuestPDF Previewer is a tool designed to simplify and speed up your development lifecycle. First, it shows a preview of your document. But the real magic starts with the hot-reload capability! It observes your code and updates the preview every time you change the implementation. Get real-time results without the need of code recompilation. Save time and enjoy the task!

[![Learn more](https://img.shields.io/badge/%F0%9F%93%96%20Previewer-learn%20more-blue)](https://www.questpdf.com/document-previewer)


<img src="https://github.com/QuestPDF/QuestPDF-Documentation/blob/main/docs/public/previewer/animation.gif?raw=true" width="100%">

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
        page.PageColor(Colors.White);
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

<img src="https://github.com/QuestPDF/QuestPDF-Documentation/blob/main/docs/public/minimal-example-shadow.png?raw=true" width="250px">

## Are you ready for more?

The Fluent API of QuestPDF scales really well. It is easy to create and maintain even most complex documents. Read [the Getting started tutorial](https://www.questpdf.com/getting-started.html) to learn QuestPDF basics and implement an invoice under 200 lines of code. You can also investigate and play with the code from [the example repository](https://github.com/QuestPDF/example-invoice).

<img src="https://github.com/QuestPDF/QuestPDF-Documentation/blob/main/docs/public/invoice-small.png?raw=true" width="400px">


## QuestPDF on JetBrains OSS Power-Ups

QuestPDF was presented on one of the episodes of OSS Power-Ups hosted by JetBrains. Huge thanks for Matthias Koch and entire JetBrains team for giving me a chance to show QuestPDF. You are the best!

<a href="https://www.youtube.com/watch?v=-iYvZvpLX0g">
    <img src="https://github.com/QuestPDF/QuestPDF-Documentation/blob/main/docs/public/jetbrains-oss-powerups-youtube.png?raw=true" width="600px">
</a>


[![YouTube video about QuestPDF]( https://img.shields.io/badge/watch%20on-YouTube-red?logo=youtube)](https://www.youtube.com/watch?v=-iYvZvpLX0g)
