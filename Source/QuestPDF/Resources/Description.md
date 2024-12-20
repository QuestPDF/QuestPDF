<a href="https://www.questpdf.com/" target="_blank">
  <img src="https://github.com/QuestPDF/example-invoice/raw/main/images/logo.svg" width="400"> 
</a>

---

[![QuestPDF Homepage](https://img.shields.io/badge/Homepage-blue?style=for-the-badge)](https://www.questpdf.com)
[![Dotnet](https://img.shields.io/badge/platform-.NET-blue?style=for-the-badge)](https://www.nuget.org/packages/QuestPDF/)
[![GitHub Repo stars](https://img.shields.io/github/stars/QuestPDF/QuestPDF?style=for-the-badge)](https://github.com/QuestPDF/QuestPDF/stargazers)
[![Nuget version](https://img.shields.io/nuget/v/QuestPdf?style=for-the-badge)](https://www.nuget.org/packages/QuestPDF/)
[![Nuget download](https://img.shields.io/nuget/dt/QuestPDF?style=for-the-badge)](https://www.nuget.org/packages/QuestPDF/)
[![License](https://img.shields.io/github/license/QuestPDF/QuestPDF?style=for-the-badge)](https://github.com/QuestPDF/QuestPDF/blob/main/LICENSE)

<br />

### QuestPDF is a modern open-source .NET library for PDF document generation. Offering comprehensive layout engine powered by concise and discoverable C# Fluent API.

<img src="https://github.com/QuestPDF/QuestPDF-Documentation/blob/main/docs/public/previewer/animation.gif?raw=true" width="100%">

<table>
<tr>
    <td>👨‍💻</td>
    <td>Design PDF documents using C# and employ a code-only approach. Utilize your version control system to its fullest potential.</td>
</tr>
<tr>
    <td>🧱</td>
    <td>Compose PDF document with a range of powerful and predictable structural elements, such as text, image, border, table, and many more.</td>
</tr>
<tr>
    <td>⚙️</td>
    <td>Utilize a comprehensive layout engine, specifically designed for PDF document generation and paging support.</td>
</tr>
<tr>
    <td>📖</td>
    <td>Write code using concise and easy-to-understand C# Fluent API. Utilize IntelliSense to quickly discover available options.</td>
</tr>
<tr>
    <td>🔗</td>
    <td>Don't be limited to any proprietary scripting language or format. Follow your experience and leverage all modern C# features.</td>
</tr>
<tr>
    <td>⌛</td>
    <td>Save time thanks to a hot-reload capability, allowing real-time PDF document preview without code recompilation.</td>
</tr>
</table>

<br />

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



## Let's get started

Begin exploring the QuestPDF library today. You are 250 lines of C# code away from creating a fully functional PDF invoice implementation.

Read the Getting Started tutorial to familiarize yourself with general library architecture, important layout structures as well as to better understand helpful patterns and practices.

[![Getting started tutorial](https://img.shields.io/badge/%F0%9F%9A%80%20read-getting%20started-blue?style=for-the-badge)](https://www.questpdf.com/getting-started)

<img src="https://github.com/QuestPDF/QuestPDF-Documentation/blob/main/docs/public/invoice-small.png?raw=true" width="400px">
