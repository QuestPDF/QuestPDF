# QuestPDF - Modern PDF library for C# developers

QuestPDF is a production-ready library that lets you design documents the way you design software: with clean, maintainable, strong-typed C# code.
Stop fighting with HTML-to-PDF conversion. Build pixel-perfect reports, invoices, and exports using the language and tools you already love.

[![GitHub Stars and Stargazers](https://img.shields.io/github/stars/QuestPDF/QuestPDF?style=for-the-badge&label=GitHub%20Stars&logo=github&color=FFEB3B&logoColor=white)](https://github.com/QuestPDF/QuestPDF)
<br>
[![Nuget package download](https://img.shields.io/nuget/dt/QuestPDF?style=for-the-badge&label=NuGet%20downloads&logo=nuget&color=0277BD&logoColor=white)](https://www.nuget.org/packages/QuestPDF/)
<br>
[![QuestPDF License](https://img.shields.io/badge/LICENSE-Community%20MIT%20and%20professional-2E7D32?style=for-the-badge&logo=googledocs&logoColor=white)](https://www.questpdf.com/license.html)

<hr>

Important links:

[Home Page](https://www.questpdf.com)
&nbsp;&nbsp;•&nbsp;&nbsp;
[Quick Start](https://www.questpdf.com/quick-start.html)
&nbsp;&nbsp;•&nbsp;&nbsp;
[Real-world Invoice Tutorial](https://www.questpdf.com/invoice-tutorial.html)
&nbsp;&nbsp;•&nbsp;&nbsp;
[Features Overview](https://www.questpdf.com/features-overview.html)
&nbsp;&nbsp;•&nbsp;&nbsp;
[NuGet](https://www.nuget.org/packages/QuestPDF)

<hr>

## 🚀 Quick start

Learn how easy it is to design, implement and generate PDF documents using QuestPDF. <br>
Effortlessly create documents of all types such as invoices and reports.

```c#
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

// set your license here:
// QuestPDF.Settings.License = LicenseType.Community;

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

The code above produces the following PDF document:

<img width="300" alt="image" src="https://github.com/user-attachments/assets/79c62304-b895-431e-b012-8e9a39ff7db9" />

[![Learn about QuestPDF Companion App](https://img.shields.io/badge/read-tutorial-0288D1?style=for-the-badge)]([https://www.questpdf.com/features-overview.html)

<br>
<hr>
<br>

## Everything you need to generate PDFs

From layout and styling to production features, QuestPDF gives you the flexibility to create documents of any complexity.

- Page attributes (header, footer, background, watermark, margin),
- Text (font style, paragraph style, page numbers),
- Styled containers (background, border, rounded corners, colors and gradients, shadows),
- Line (vertical and horizontal, colors and gradients, dash pattern)
- Images (PNG, JPG, WEBP, SVG),
- Table,
- List,
- Layers,
- Alignment,
- Size Controls,
- Page Breaking Control,
- Padding,
- Z-index,
- Hyperlinks,
- Integrations (maps, charts, barcodes, QR codes),
- and more...

[![Learn about QuestPDF Companion App](https://img.shields.io/badge/explore%20all%20features-0288D1?style=for-the-badge)](https://www.questpdf.com/features-overview.html)

<br>
<hr>
<br>

## Please help by giving a star ⭐

GitHub stars guide developers toward great tools. If you find this project valuable, please give it a star – it helps the community and takes just a second! 

<img src="https://github.com/user-attachments/assets/722c5331-1bf0-4909-9be1-be3056bb4131" width="700" />

<br>
<hr>
<br>

## Familiar Programming Patterns

Use your existing programming language and patterns to ship faster with less training. 

Loops, conditionals, functions are natively supported. Leverage IntelliSense, inspections, navigation, and safe refactoring.

```csharp
container.Column(column =>
{
    column.Item().Text("Order Items").Bold();

    if (Model.ShowSummary)
        column.Item().Element(ComposeOrderSummary);

    foreach (var item in Model.Items)
        column.Item().Element(c => ComposeItem(c, item));
});
```

Review document changes like any other code. Get clean diffs, PR approvals, and traceable history.

```diff
void ComposeItem(IContainer container, OrderItem item)
{
    container
        .Border(1, Colors.Grey.Darken2)
        .Background(item.HighlightColor)
-       .Padding(12)
+       .Padding(16)
        .Row(row =>
        {
            row.RelativeItem().Text(item.Name);
            row.AutoItem().Text($"{item.Price:F2} USD");
        });
}
```

<br>
<hr>
<br>

## Companion App

Accelerate development with live document preview and hot-reload capability. See your changes instantly without recompiling.

- Explore PDF document hierarchy and navigate its structure
- Quickly magnify and measure content
- Debug runtime exceptions with stack traces and code snippets
- Identify, understand and solve layout errors

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/7ab596d4-eebc-44e6-b36d-c358b16ed0ba">
  <source media="(prefers-color-scheme: light)" srcset="https://github.com/user-attachments/assets/39d4c08c-6a78-4743-8837-208c0c1718fd">
  <img src="https://github.com/user-attachments/assets/ce394258-1f10-498d-b65f-26c9fbed2994" width="600">
</picture>

[![Learn about QuestPDF Companion App](https://img.shields.io/badge/learn%20more-0288D1?style=for-the-badge)](https://www.questpdf.com/companion/usage.html)

<br>
<hr>
<br>

## Enterprise-grade foundations

A robust layout engine engineered for high performance, complete data privacy, and seamless integration into your infrastructure.

- **Source-available** - Entire QuestPDF source code is available for review and customization, ensuring transparency and compliance with your organization's requirements.

- **Complete Data Privacy** - QuestPDF runs entirely within your infrastructure with no external API calls, internet requirement, or background data collection. As a company, we do not access, collect, store, or process your private data.

- **Comprehensive Layout Engine** - A powerful layout engine built specifically for PDF generation. Gain full control over document structure, precise content positioning, and automatic pagination.

- **Advanced Language Support** - Create multilingual documents with full RTL language support, advanced text shaping, and bi-directional layout handling.

- **High Performance** - Generate thousands of pages per second while maintaining minimal CPU and memory usage. Perfect for high-throughput enterprise applications.

- **Optimized File Size** - Drastically reduce file sizes without compromising quality. Benefit from automatic font subsetting, optimal image compression, and efficient file compression.

<br>
<hr>
<br>

## Perform common PDF operations

Leverage a powerful C# Fluent API to create, customize, and manage your PDF documents with ease.
- Merge documents
- Attach files
- Extract pages
- Encrypt / decrypt
- Extend metadata
– Limit access
- Optimize for Web
- Overlay / underlay

```c#
DocumentOperation
    .LoadFile("input.pdf")
    .TakePages("1-10")
    .MergeFile("appendix.pdf", "1-z") // all pages
    .AddAttachment(new DocumentAttachment
    {
        FilePath = "metadata.xml"
    })
    .Encrypt(new Encryption256Bit
    {
        OwnerPassword = "mypassword",
        AllowPrinting = true,
        AllowContentExtraction = false
    })
    .Save("final-document.pdf");
```

[![Learn Document Operation API](https://img.shields.io/badge/learn%20more-0288D1?style=for-the-badge)](https://www.questpdf.com/concepts/document-operations.html)

<br>
<hr>
<br>

## Works everywhere you do

Deploy on any major operating system and integrate seamlessly with your favorite IDEs, cloud platforms, and development tools.

| Platform | Support |
|----------|---------|
| **Operating Systems** | Windows, Linux, macOS |
| **Frameworks** | .NET 6+ and .NET Framework 4.6.2+ |
| **Cloud** | Azure, AWS, Google Cloud, Others |
| **Containers** | Docker, Kubernetes |
| **IDEs** | Visual Studio, VS Code, JetBrains Rider, Others |

<br>
<hr>
<br>

## Industry-standard PDF compliance

Generate PDF documents that meet the strictest archival and accessibility requirements. Every build is automatically validated using the open-source veraPDF and Mustang tools.

- PDF/A (Archival):
  - Purpose: ISO 19005 standard for long-term preservation. Ensures PDFs remain readable and visually identical for decades without external dependencies.
  - Supported Standards: `PDF/A-2b`, `PDF/A-2u`, `PDF/A-2a`, `PDF/A-3b`, `PDF/A-3u`, `PDF/A-3a`

- PDF/UA (Accessibility):
  - Purpose: ISO 14289 standard for universal accessibility. Includes full support for screen readers and assistive technologies for people with disabilities.
  - Supported Standards: `PDF/UA-1`

- EN 16931 (E-Invoicing):
  - Purpose: European standard for electronic invoicing. Embeds structured invoice data (XML) within PDF documents for automated processing.
  - Supported Standards: `ZUGFeRD`, `Factur-X`

<br>
<hr>
<br>

## Fair and Sustainable License

A model that benefits everyone. Commercial licensing provides businesses with legal safety and long-term stability, while funding a feature-complete, unrestricted library for the open-source community.

- Actively maintained with regular feature, quality, and security updates
- Full source code available on GitHub
- All features included in every tier without restrictions
- Predictable pricing: no per-seat, per-server, or usage fees

> [!TIP]
> Free for individuals, non-profits, all FOSS projects, and organizations under $1M in annual revenue.

[![QuestPDF Pricing](https://img.shields.io/badge/view%20pricing-388E3C?style=for-the-badge)](https://www.questpdf.com/license.html)
[![QuestPDF License Terms](https://img.shields.io/badge/license%20terms-666666?style=for-the-badge)](https://www.questpdf.com/license/guide.html)

<br>
<hr>
<br>

## See a real-world example

Follow our detailed tutorial and see how easy it is to generate a fully functional invoice with fewer than 250 lines of C# code.

- Step-by-step guidance
- Production-ready code
- Best practices included

<img width="300" alt="Example Invoice" src="https://github.com/user-attachments/assets/881008ed-136d-4661-926a-7ad5431e95a1" />

[![Read getting started tutorial](https://img.shields.io/badge/read%20tutorial-0288D1?style=for-the-badge)](https://www.questpdf.com/invoice-tutorial.html)

<br>
<hr>
<br>

## Community QuestPDF

We are incredibly grateful to our .NET Community for their positive reviews and recommendations of the QuestPDF library. 
Your support and feedback are invaluable and motivate us to keep improving and expanding this project. 
Thank you for helping us grow and reach more developers!

### Nick Chapsas: The Easiest Way to Create PDFs in .NET

[![Nick Chapsas The Easiest Way to Create PDFs in .NET](https://img.youtube.com/vi/_M0IgtGWnvE/0.jpg)](https://www.youtube.com/watch?v=_M0IgtGWnvE)

### Claudio Bernasconi: QuestPDF - The BEST PDF Generator for .NET?!

[![Claudio Bernasconi QuestPDF - The BEST PDF Generator for .NET?!](https://img.youtube.com/vi/T89A_7dz1P8/0.jpg)](https://www.youtube.com/watch?v=T89A_7dz1P8)

### JetBrains: OSS Power-Ups: QuestPDF

[![JetBrains OSS Power-Ups: QuestPDF](https://img.youtube.com/vi/-iYvZvpLX0g/0.jpg)](https://www.youtube.com/watch?v=-iYvZvpLX0g)

### Programming with Felipe Gavilan: Generating PDFs with C# - Very Easy (two examples)

[![Felipe Gavilan Generating PDFs with C# - Very Easy (two examples)](https://img.youtube.com/vi/bhR4Cmg16gs/0.jpg)](https://www.youtube.com/watch?v=bhR4Cmg16gs)

### Learning materials

- `PDF Generation using QuestPDF in ASP.NET Core` written by [Olufemi Oyedepo]([olufemioyedepo](https://github.com/olufemioyedepo)): [Part 1](https://medium.com/@olufemioyedepo/pdf-generation-using-questpdf-in-asp-net-core-part-1-5ef9a63d462a) [Part 2](https://medium.com/@olufemioyedepo/pdf-generation-using-questpdf-in-asp-net-core-part-2-aadec120c31a)

