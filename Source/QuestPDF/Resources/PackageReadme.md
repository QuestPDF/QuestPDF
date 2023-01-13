[![QuestPDF Homepage](https://img.shields.io/badge/Homepage-3366cc?style=for-the-badge)](https://www.questpdf.com)
[![Getting started](https://img.shields.io/badge/Getting%20started-3366cc?style=for-the-badge)](https://www.questpdf.com/getting-startedm)
[![API Reference](https://img.shields.io/badge/API%20Reference-3366cc?style=for-the-badge)](https://www.questpdf.com/api-reference/index)
[![Design patterns](https://img.shields.io/badge/Design%20patterns-3366cc?style=for-the-badge)](https://www.questpdf.com/design-patterns)
[![GitHub Repo stars](https://img.shields.io/github/stars/QuestPDF/QuestPDF?style=for-the-badge)](https://github.com/QuestPDF/QuestPDF/stargazers)
[![Nuget version](https://img.shields.io/nuget/v/QuestPdf?style=for-the-badge)](https://www.nuget.org/packages/QuestPDF/)
[![Nuget download](https://img.shields.io/nuget/dt/QuestPDF?style=for-the-badge)](https://www.nuget.org/packages/QuestPDF/)

<br />

### QuestPDF is a modern open-source .NET library for PDF document generation. Offering comprehensive layout engine powered by concise and discoverable C# Fluent API. Easily generate PDF documents, reports, invoices, etc.

![Usage animation](https://github.com/QuestPDF/QuestPDF-Documentation/blob/main/docs/public/previewer/animation.gif?raw=true)

👨‍💻 Design documents using C# and employ a code-only approach. Utilize your version control system to its fullest potential.

🧱 Compose document with a range of powerful and predictable structural elements, such as text, image, border, table, and many more.

⚙️ Utilize a comprehensive layout engine, specifically designed for document generation and paging support.

📖 Write code using concise and easy-to-understand C# Fluent API. Utilize IntelliSense to quickly discover available options.

🔗 Don't be limited to any proprietary scripting language or format. Follow your experience and leverage all modern C# features.

⌛ Save time thanks to a hot-reload capability, allowing real-time document preview without code recompilation.


<br />

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



## Let's get started

Begin exploring the QuestPDF library today. You are 250 lines of C# code away from creating a fully functional PDF invoice implementation.

Read the Getting Started tutorial to familiarize yourself with general library architecture, important layout structures as well as to better understand helpful patterns and practices.

[![Getting started tutorial](https://img.shields.io/badge/%F0%9F%9A%80%20read-getting%20started-blue?style=for-the-badge)](https://www.questpdf.com/getting-started)

![Example invoice](https://github.com/QuestPDF/QuestPDF-Documentation/blob/main/docs/public/invoice-small.png?raw=true)
