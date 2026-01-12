## QuestPDF - Modern PDF library for C# developers

QuestPDF is a production-ready library that lets you design documents the way you design software: with clean, maintainable, strong-typed C# code.
Stop fighting with HTML-to-PDF conversion. Build pixel-perfect reports, invoices, and exports using the language and tools you already love.

[![GitHub Stars and Stargazers](https://img.shields.io/github/stars/QuestPDF/QuestPDF?style=for-the-badge&label=GitHub%20Stars&logo=github&color=FFEB3B&logoColor=white)](https://github.com/QuestPDF/QuestPDF)

[![Nuget package download](https://img.shields.io/nuget/dt/QuestPDF?style=for-the-badge&label=NuGet%20downloads&logo=nuget&color=0277BD&logoColor=white)](https://www.nuget.org/packages/QuestPDF/)

[![QuestPDF License](https://img.shields.io/badge/LICENSE-Community%20and%20commercial-2E7D32?style=for-the-badge&logo=googledocs&logoColor=white)](https://www.questpdf.com/license.html)

---

[Home Page](https://www.questpdf.com)

[Quick Start](https://www.questpdf.com/quick-start.html)

[Real-world Invoice Tutorial](https://www.questpdf.com/invoice-tutorial.html)

[Features Overview](https://www.questpdf.com/features-overview.html)

[License](https://www.questpdf.com/license/)

[NuGet](https://www.nuget.org/packages/QuestPDF)

---

## 🚀 Quick start

Learn how easy it is to design, implement and generate PDF documents using QuestPDF.
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

![Preview of a PDF document showing the Hello World example](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/f6b28c965e26fe43630316f589339db465c8197e/docs/public/nuget/hello-world.png)

[![Quick Start Tutorial](https://img.shields.io/badge/read-tutorial-0288D1?style=for-the-badge)](https://www.questpdf.com/quick-start.html)

> The library is free for individuals, non-profits, all FOSS projects, and organizations under $1M in annual revenue.
> Read more about licensing [here](https://www.questpdf.com/license/)


## Everything you need to generate PDFs

From layout and styling to production features, QuestPDF gives you the flexibility to create documents of any complexity.

### 🎨 Visual Content:
- Page attributes (header, footer, background, watermark, margin),
- Text (font style, paragraph style, page numbers),
- Styled containers (background, border, rounded corners, colors and gradients, shadows),
- Lines (vertical and horizontal, colors and gradients, dash pattern)
- Images (PNG, JPG, WEBP, SVG),

### 🔀 Layout:
- Tables,
- Lists,
- Layers,
- Column / Row,
- Inlined,

### 📐 Positional:
- Alignment,
- Size Controls (width / height),
- Padding,

### 🛠️ Other:
- Page Breaking Control,
- Aspect Ratio,
- Integrations (maps, charts, barcodes, QR codes),
- Hyperlinks,
- Z-index,

[![Explore All QuestPDF Features](https://img.shields.io/badge/explore%20all%20features-0288D1?style=for-the-badge)](https://www.questpdf.com/features-overview.html)


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


## Companion App

Accelerate development with live document preview and hot-reload capability. See your changes instantly without recompiling.

- Explore PDF document hierarchy and navigate its structure
- Quickly magnify and measure content
- Debug runtime exceptions with stack traces and code snippets
- Identify, understand and solve layout errors

![Screenshot showing the QuestPDF Companion App](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/f6b28c965e26fe43630316f589339db465c8197e/docs/public/nuget/companion-light.png)

[![Learn about QuestPDF Companion App](https://img.shields.io/badge/learn%20more-0288D1?style=for-the-badge)]([https://www.questpdf.com/companion/features.html](https://www.questpdf.com/companion/usage.html))

[![Learn about QuestPDF Companion App](https://img.shields.io/badge/features-666666?style=for-the-badge)](https://www.questpdf.com/companion/features.html)


## Enterprise-grade foundations

- **Predictable Development** — Eliminate CSS debugging, browser quirks, and layout surprises common with HTML-to-PDF tools. What you code is what you get.

- **Source-available** - Entire QuestPDF source code is available for review and customization, ensuring transparency and compliance with your organization's requirements.

- **Complete Data Privacy** - QuestPDF runs entirely within your infrastructure with no external API calls, internet requirement, or background data collection. As a company, we do not access, collect, store, or process your private data.

- **Comprehensive Layout Engine** - A powerful layout engine built specifically for PDF generation. Gain full control over document structure, precise content positioning, and automatic pagination.

- **Advanced Language Support** - Create multilingual documents with full RTL language support, advanced text shaping, and bi-directional layout handling.

- **High Performance** - Generate thousands of pages per second while maintaining minimal CPU and memory usage. Perfect for high-throughput enterprise applications.

- **Optimized File Size** - Drastically reduce file sizes without compromising quality. Benefit from automatic font subsetting, optimal image compression, and efficient file compression.



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


## Works everywhere you do

Deploy on any major operating system and integrate seamlessly with your favorite IDEs, cloud platforms, and development tools.

| Platform | Support |
|----------|---------|
| **Operating Systems** | Windows, Linux, macOS |
| **Frameworks** | .NET 6+ and .NET Framework 4.6.2+ |
| **Cloud** | Azure, AWS, Google Cloud, Others |
| **Containers** | Docker, Kubernetes |
| **IDEs** | Visual Studio, VS Code, JetBrains Rider, Others |



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



## Fair and Sustainable License

A model that benefits everyone. Commercial licensing provides businesses with legal safety and long-term stability, while funding a feature-complete, unrestricted library for the open-source community.

- Actively maintained with regular feature, quality, and security updates
- Full source code available on GitHub
- All features included in every tier without restrictions
- Predictable pricing: no per-seat, per-server, or usage fees

> The library is free for individuals, non-profits, all FOSS projects, and organizations under $1M in annual revenue.

[![QuestPDF Pricing](https://img.shields.io/badge/view%20pricing-388E3C?style=for-the-badge)](https://www.questpdf.com/license.html)

[![QuestPDF License Terms](https://img.shields.io/badge/license%20terms-666666?style=for-the-badge)](https://www.questpdf.com/license/guide.html)



## See a real-world example

Follow our detailed tutorial and see how easy it is to generate a fully functional invoice with fewer than 250 lines of C# code.

- Step-by-step guidance
- Production-ready code
- Best practices included

![Preview of a PDF document being an output of the Real-World Invoice tutorial](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/f6b28c965e26fe43630316f589339db465c8197e/docs/public/nuget/invoice.jpg)

[![Read Real-world Invoice Tutorial](https://img.shields.io/badge/read%20tutorial-0288D1?style=for-the-badge)](https://www.questpdf.com/invoice-tutorial.html)


## Community QuestPDF

We are incredibly grateful to our .NET Community for their positive reviews and recommendations of the QuestPDF library.
Your support and feedback are invaluable and motivate us to keep improving and expanding this project.
Thank you for helping us grow and reach more developers!

### Nick Chapsas: The Easiest Way to Create PDFs in .NET

[![Nick Chapsas The Easiest Way to Create PDFs in .NET](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/f6b28c965e26fe43630316f589339db465c8197e/docs/public/nuget/youtube-nick-chapsas.jpg)](https://www.youtube.com/watch?v=_M0IgtGWnvE)

### JetBrains: OSS Power-Ups: QuestPDF

[![JetBrains OSS Power-Ups: QuestPDF](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/f6b28c965e26fe43630316f589339db465c8197e/docs/public/nuget/youtube-jetbrains.jpg)](https://www.youtube.com/watch?v=-iYvZvpLX0g)
