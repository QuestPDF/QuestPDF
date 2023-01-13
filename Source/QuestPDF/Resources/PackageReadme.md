[![QuestPDF Homepage](https://img.shields.io/badge/Homepage%20link-blue?style=badge)](https://www.questpdf.com)
[![Getting started tutorial]( https://img.shields.io/badge/%F0%9F%9A%80%20read-Getting%20Started-blue)](https://www.questpdf.com/getting-started.html)
[![API reference](https://img.shields.io/badge/%F0%9F%93%96%20read-API%20Reference-blue)](https://www.questpdf.com/api-reference/index.html)
[![Patterns and Practices](https://img.shields.io/badge/%F0%9F%94%8D%20read-Patterns%20and%20Practices-blue)](https://www.questpdf.com/design-patterns.html)
[![GitHub Repo stars](https://img.shields.io/github/stars/QuestPDF/QuestPDF?style=badge)](https://github.com/QuestPDF/QuestPDF/stargazers)
[![Nuget version](https://img.shields.io/nuget/v/QuestPdf?style=badge)](https://www.nuget.org/packages/QuestPDF/)
[![Nuget download](https://img.shields.io/nuget/dt/QuestPDF?style=badge)](https://www.nuget.org/packages/QuestPDF/)



### QuestPDF is a modern open-source .NET library for PDF document generation. Offering comprehensive layout engine powered by concise and discoverable C# Fluent API. Easily generate PDF documents, reports, invoices, exports etc.

![Usage animation of the PDF library](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/main/docs/public/previewer/animation.gif)

👨‍💻 Design PDF documents using C# and employ a code-only approach. Utilize your version control system to its fullest potential.

🧱 Compose PDF document with a range of powerful and predictable structural elements, such as text, image, border, table, and many more.

⚙️ Utilize a comprehensive layout engine, specifically designed for PDF document generation and paging support.

📖 Write code using concise and easy-to-understand C# Fluent API. Utilize IntelliSense to quickly discover available options.

🔗 Don't be limited to any proprietary scripting language or format. Follow your experience and leverage all modern C# features.

⌛ Save time thanks to a hot-reload capability, allowing real-time PDF document preview without code recompilation.


## Simplicity is the key

How easy it is to start and prototype with QuestPDF? Really easy thanks to its minimal API! Please analyse the code below that generates basic PDF document:

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

![Minimal PDF example](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/main/docs/public/minimal-example-shadow.png)


## Let's get started

Begin exploring the QuestPDF library today. You are 250 lines of C# code away from creating a fully functional PDF invoice implementation.

Read the Getting Started tutorial to familiarize yourself with general library architecture, important layout structures as well as to better understand helpful patterns and practices.

Easily start designing your PDF documents, reports, invoices, exports and even more.

[![Getting started tutorial](https://img.shields.io/badge/%F0%9F%9A%80%20read-getting%20started%20tutorial-blue?style=for-the-badge)](https://www.questpdf.com/getting-started)

![Example invoice](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/main/docs/public/invoice-small.png)
