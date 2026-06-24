<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/9535de26-aeac-42ac-ac45-019a2893ed6b">
  <source media="(prefers-color-scheme: light)" srcset="https://github.com/user-attachments/assets/e9d197ee-0cad-44f0-9efa-80ad68c31abe">
  <img src="https://github.com/user-attachments/assets/e9d197ee-0cad-44f0-9efa-80ad68c31abe" width="300">
</picture>

<br>
<br>

## Modern PDF library for C# developers

QuestPDF is a production-ready library that lets you design documents the way you design software: with clean, maintainable, strong-typed C# code.
Stop fighting with HTML-to-PDF conversion. Build pixel-perfect reports, invoices, and exports using the language and tools you already love.

The library is free for individuals, non-profits, all FOSS projects, and organizations under $1M in annual revenue. [Read more](https://www.questpdf.com/pricing.html)

<br>

[![GitHub Stars and Stargazers](https://img.shields.io/github/stars/QuestPDF/QuestPDF?style=for-the-badge&label=GitHub%20Stars&logo=github&color=FFEB3B&logoColor=white)](https://github.com/QuestPDF/QuestPDF)
<br>
[![Nuget package download](https://img.shields.io/nuget/dt/QuestPDF?style=for-the-badge&label=NuGet%20downloads&logo=nuget&color=0277BD&logoColor=white)](https://www.nuget.org/packages/QuestPDF/)
<br>
[![QuestPDF License](https://img.shields.io/badge/LICENSE-Community%20and%20commercial-2E7D32?style=for-the-badge&logo=googledocs&logoColor=white)](https://www.questpdf.com/pricing/)

<br>
<hr>

[Home Page](https://www.questpdf.com)
&nbsp;&nbsp;•&nbsp;&nbsp;
[Quick Start](https://www.questpdf.com/quick-start.html)
&nbsp;&nbsp;•&nbsp;&nbsp;
[Invoice Tutorial](https://www.questpdf.com/invoice-tutorial.html)
&nbsp;&nbsp;•&nbsp;&nbsp;
[Features Overview](https://www.questpdf.com/features-overview.html)
&nbsp;&nbsp;•&nbsp;&nbsp;
[Pricing](https://www.questpdf.com/pricing/)
&nbsp;&nbsp;•&nbsp;&nbsp;
[License](https://www.questpdf.com/license/)
&nbsp;&nbsp;•&nbsp;&nbsp;
[NuGet](https://www.nuget.org/packages/QuestPDF)

<hr>
<br>


## 🚀 Quick start

Learn how easy it is to design, implement and generate PDF documents using QuestPDF. <br>
Effortlessly create documents of all types such as invoices and reports.

```c#
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

// set your license here:
// QuestPDF.Settings.License = LicenseType.Evaluation;

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

<br>

The code above produces the following PDF document:

<img width="300" alt="image" src="https://github.com/user-attachments/assets/46784e17-e8be-41d2-87f4-d6322c3b5e52" />

<br>
<br>
<br>

## Please help by giving a star ⭐

GitHub stars guide developers toward great tools. If you find this project valuable, please give it a star – it helps the community and takes just a second! 

<img src="https://github.com/user-attachments/assets/96a1d6ee-5df8-48e3-9507-a3649f339b4a" width="700" />

<br>
<br>

## Installation

QuestPDF is available as a NuGet package. You can install it through your IDE by searching for phrase `QuestPDF`. If you are not familiar how to do that, please refer to the following guides: 
- [Visual Studio](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio)
- [Visual Code](https://code.visualstudio.com/docs/csharp/package-management)
- [JetBrains Rider](https://www.jetbrains.com/help/rider/Using_NuGet.html)

Or use the following command in your terminal:

```bash
dotnet add package QuestPDF
```

<br>
<br>

## Everything you need to generate PDFs

QuestPDF provides 60+ production-ready features for building complex, data-driven PDF documents with clean and maintainable C# code.

<br>

**🧱 Layout:**<br>
Page setup, headers, footers, watermarks, tables, rows, columns, lists, layers, inlined content, multi-column flow, sections, decorations

<br>

**🎨 Visual Content:**<br>
Rich text, fonts, colors, backgrounds, borders, rounded corners, shadows, lines, raster images, SVG, placeholders, canvas graphics

<br>

**📐 Positioning:**<br>
Alignment, width, height, padding, aspect ratio, rotation, scaling, scale-to-fit, offset, flip, extend, shrink, unconstrained layout, Z-index

<br>

**🌊 Content Flow:**<br>
Page breaks, prevent page breaks, ensure space, show entire, repeat, show once, skip once, conditional rendering, stop paging

<br>

**🧩 Dynamic Documents:**<br>
Loops, conditions, reusable components, dynamic components, lazy loading, custom helpers, extension methods, strongly-typed models

<br>

**🌍 Internationalization:**<br>
RTL layout direction, bidirectional text, advanced text shaping, font fallback, multilingual documents, accessibility support

<br>

[![Explore All QuestPDF Features](https://img.shields.io/badge/explore%20all%20features-0288D1?style=for-the-badge)](https://www.questpdf.com/features-overview.html)

<br>
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
<br>

## Companion App

Accelerate development with live document preview and hot-reload capability. See your changes instantly without recompiling.

- 🧭 Explore PDF document hierarchy and navigate its structure
- 🔍 Quickly magnify and measure content
- 🐞 Debug runtime exceptions with stack traces and code snippets
- 🛠️ Identify, understand and solve layout errors
  
<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/d2cbade8-4252-4fc7-ac0d-d408a6373bdf">
  <source media="(prefers-color-scheme: light)" srcset="https://github.com/user-attachments/assets/9e2ae731-d710-4776-9fa8-508ac8b0a6a9">
  <img src="https://github.com/user-attachments/assets/ce394258-1f10-498d-b65f-26c9fbed2994" width="600">
</picture>

<br> 

[![Learn about QuestPDF Companion App](https://img.shields.io/badge/learn%20more-0288D1?style=for-the-badge)](https://www.questpdf.com/companion/usage.html)
[![Learn about QuestPDF Companion App](https://img.shields.io/badge/features-666666?style=for-the-badge)](https://www.questpdf.com/companion/features.html)

<br>
<br>

## Enterprise-grade foundations

**🧩 Dynamic Content Is Just Code**<br>
Use your existing programming language and development patterns to ship faster with less training. Loops, conditionals, functions, reusable components, and data-driven generation are supported natively.

<br>

**🔀 Version Control Friendly**<br>
Review document changes like any other code. Get clean diffs, pull request approvals, traceable history, and safer collaboration across your team.

<br>

**✨ Ready for AI**<br>
QuestPDF’s semantic Fluent API helps AI assistants generate layouts, refactor components, explain document structure, and troubleshoot issues more effectively.

<br>

**🎯 Predictable Development**<br>
Eliminate CSS debugging, browser quirks, and layout surprises common with HTML-to-PDF tools. What you code is what you get.

<br>

**👁️ Source-available**<br>
Entire QuestPDF source code is available for review and customization, ensuring transparency and compliance with your organization's requirements.

<br>

**🔒 Complete Data Privacy**<br>
QuestPDF runs entirely within your infrastructure with no external API calls, internet requirement, or background data collection. As a company, we do not access, collect, store, or process your private data.

<br>

**🏗️ Comprehensive Layout Engine**<br>
A powerful layout engine built specifically for PDF generation. Gain full control over document structure, precise content positioning, and automatic pagination.

<br>

**🌐 Advanced Language Support**<br>
Create multilingual documents with full RTL language support, advanced text shaping, and bi-directional layout handling.

<br>

**⚡ High Performance**<br>
Generate thousands of pages per second while maintaining minimal CPU and memory usage. Perfect for high-throughput enterprise applications.

<br>

**📦 Optimized File Size**<br>
Drastically reduce file sizes without compromising quality. Benefit from automatic font subsetting, optimal image compression, and efficient file compression.


<br>
<br>

## Perform common PDF operations

Leverage a powerful C# Fluent API to create, customize, and manage your PDF documents with ease.

* 🔗 Merge documents
* 📎 Attach files
* ✂️ Extract pages
* 🔐 Encrypt / decrypt
* 🏷️ Extend metadata
* 🚫 Limit access
* ⚡ Optimize for Web
* 📑 Overlay / underlay

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
<br>

## Industry-standard PDF compliance

Generate PDF documents that meet the strictest archival and accessibility requirements. Every build is automatically validated using the open-source veraPDF and Mustang tools.

**🗄️ PDF/A (Archival)**

- Purpose: ISO 19005 standard for long-term preservation. Ensures PDFs remain readable and visually identical for decades without external dependencies.
- Supported Standards: `PDF/A-2b`, `PDF/A-2u`, `PDF/A-2a`, `PDF/A-3b`, `PDF/A-3u`, `PDF/A-3a`

<br>

**♿ PDF/UA (Accessibility)**

- Purpose: ISO 14289 standard for universal accessibility. Includes full support for screen readers and assistive technologies for people with disabilities.
- Supported Standards: `PDF/UA-1`

<br>

**🧾 EN 16931 (E-Invoicing):**

- Purpose: European standard for electronic invoicing. Embeds structured invoice data (XML) within PDF documents for automated processing.
- Supported Standards: `ZUGFeRD`, `Factur-X`

<br>
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
<br>

## See a real-world example

Follow our detailed tutorial and see how easy it is to generate a fully functional invoice with fewer than 250 lines of C# code.

- Step-by-step guidance
- Production-ready code
- Best practices included

<img width="300" alt="Example Invoice" src="https://github.com/user-attachments/assets/dd268fe2-e558-4b9a-944d-b2f41ce52940" />

[![Read Real-world Invoice Tutorial](https://img.shields.io/badge/read%20tutorial-0288D1?style=for-the-badge)](https://www.questpdf.com/invoice-tutorial.html)

<br>
<br>

## Community QuestPDF

We are incredibly grateful to our .NET Community for their positive reviews and recommendations of the QuestPDF library. 
Your support and feedback are invaluable and motivate us to keep improving and expanding this project. 
Thank you for helping us grow and reach more developers!

### Nick Chapsas: The Easiest Way to Create PDFs in .NET

[![Nick Chapsas The Easiest Way to Create PDFs in .NET](https://github.com/user-attachments/assets/5c7fc84b-65d6-4ec2-9cc2-b2acbc9764d0)](https://www.youtube.com/watch?v=_M0IgtGWnvE)

### JetBrains: OSS Power-Ups: QuestPDF

[![JetBrains OSS Power-Ups: QuestPDF](https://github.com/user-attachments/assets/3519b532-c2aa-430e-ab1b-f40edd3fa120)](https://www.youtube.com/watch?v=-iYvZvpLX0g)

