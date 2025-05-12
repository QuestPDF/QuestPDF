## QuestPDF - Modern PDF library for C# developers

Generate and manipulate PDF documents in your .NET applications
using the open-source QuestPDF library and its C# Fluent API.

[![QuestPDF Homepage](https://img.shields.io/badge/project%20homepage-2E7D32?style=for-the-badge&logo=googledocs&logoColor=white)](https://www.questpdf.com)
[![QuestPDF License](https://img.shields.io/badge/LICENSE-Community%20MIT%20and%20professional-2E7D32?style=for-the-badge&logo=googledocs&logoColor=white)](https://www.questpdf.com/license/)
[![GitHub Stars and Stargazers](https://img.shields.io/github/stars/QuestPDF/QuestPDF?style=for-the-badge&label=GitHub%20Stars&logo=github&color=FFEB3B&logoColor=white)](https://github.com/QuestPDF/QuestPDF)
[![Nuget package download](https://img.shields.io/nuget/dt/QuestPDF?style=for-the-badge&label=NuGet%20downloads&logo=nuget&color=0277BD&logoColor=white)](https://www.nuget.org/packages/QuestPDF/)



## Quick Start

Learn how easy it is to design, implement and generate PDF documents using QuestPDF. 
Effortlessly create documents of all types such as invoices and reports.

[![Learn Quick Start tutorial](https://img.shields.io/badge/read%20quick%20start%20tutorial-0288D1?style=for-the-badge)](https://www.questpdf.com/quick-start.html)

```c#
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

![Simple PDF generated implemented in C# and generated with the QuestPDF library](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/refs/heads/main/docs/public/homepage/quick-start-animation/step13.webp)

## Code-Focused Paradigm

### Modular and Maintainable C# Code

Implement modular PDF layouts with reusable well-organized classes and methods. 
Refactor safely with IntelliSense - your logic stays seamlessly integrated with your domain code.

### Familiar Programming Concepts

Use conditions, loops, LINQ, and extension methods to effortlessly generate dynamic, data-driven PDF documents tailored to your unique business needs.

### Git-Friendly Workflow

Enjoy straightforward C# code reviews, meaningful pull-request diffs, and cleaner version control histories.

```csharp
.Column(column =>
{
    if (Model.Comments != null)
        column.Item().Text(Model.Comments);

    foreach(var item in Model.Items)
       column.Item().Element(c => CreateItem(c, item);
});
```

```diff
void CreateItem(IContainer container, Item item)
{
    container
-       .Background(Colors.Grey.Lighten2)
+       .Background(item.Color)
        .Padding(10)
        .Text(item.Text);
}
```



## Companion App

Accelerate your development with live document preview powered by the hot-reload capability, eliminating the need for C# code recompilation.

- Explore PDF document structure and hierarchy
- Quickly magnify and measure content
- Debug runtime exceptions with stack traces and code snippets
- Identify, understand and solve layout errors

[![Learn about QuestPDF Companion App](https://img.shields.io/badge/learn%20more-0288D1?style=for-the-badge)](https://www.questpdf.com/companion/usage.html)


![QuestPDF Companion App helping .NET developers work more efficiently with C# implementation of the PDF document](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/refs/heads/main/docs/public/companion/application-light.png)



## Exactly what you need

### Comprehensive Layout Engine
A powerful layout engine built specifically for PDF generation. Gain full control over document structure, precise content positioning, and automatic pagination for complex reports and invoices.

### Rich Toolkit
Accelerate your PDF development workflow with a rich set of reusable components and over 50 layout elements. Easily implement data-driven documents using a Fluent C# API.

### High Performance
Generate PDF files at scale with up to thousands of pages per second - while maintaining minimal CPU and memory usage. Perfect for high-throughput .NET applications.

### Advanced Language Support
Create multilingual PDF documents with full support for right-to-left (RTL) languages, advanced text shaping, and bi-directional layout handling.



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



## Multiplatform

Supports all major operating systems and works seamlessly with leading IDEs, cloud platforms, and modern development tools.
- `Technologies`: modern dotnet, legacy .NET Framework, Docker
- `Operating systems`: Windows, Linux, MacOS
- `Cloud providers`: Azure, AWS, Google Cloud
- `IDE`: Visual Studio, Visual Code, JetBrains Rider, others



## Fair and Sustainable License

By offering free access to most users and premium licenses for larger organizations, the project maintains its commitment to excellence:

- Long-term and sustainable development
- Regular feature, performance, quality and security updates
- Active community and enterprise support

> Free for individuals, non-profits, and businesses under $1M in annual revenue, as well as all FOSS projects.

[![QuestPDF License and Pricing](https://img.shields.io/badge/check%20pricing-388E3C?style=for-the-badge)](https://www.questpdf.com/license/)



## Let's get started

Follow our detailed tutorial, and see how easy it is to generate a fully functional invoice with fewer than 250 lines of C# code.

[![Read getting started tutorial](https://img.shields.io/badge/read%20getting%20started%20tutorial-0288D1?style=for-the-badge)](https://www.questpdf.com/getting-started.html)

![Invoice PDF document created in the Getting Started tutorial of the QuestPDF library](https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/refs/heads/main/docs/public/homepage/invoice.png)
