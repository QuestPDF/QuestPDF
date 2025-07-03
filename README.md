# Modern PDF library for C# developers

Generate and manipulate PDF documents in your .NET applications <br>
using the open-source QuestPDF library and its C# Fluent API.

[![QuestPDF Homepage](https://img.shields.io/badge/project%20homepage-2E7D32?style=for-the-badge&logo=googledocs&logoColor=white)](https://www.questpdf.com)
[![QuestPDF License](https://img.shields.io/badge/LICENSE-Community%20MIT%20and%20professional-2E7D32?style=for-the-badge&logo=googledocs&logoColor=white)](https://www.questpdf.com/license.html)
<br>
[![GitHub Stars and Stargazers](https://img.shields.io/github/stars/QuestPDF/QuestPDF?style=for-the-badge&label=GitHub%20Stars&logo=github&color=FFEB3B&logoColor=white)](https://github.com/QuestPDF/QuestPDF)
[![Nuget package download](https://img.shields.io/nuget/dt/QuestPDF?style=for-the-badge&label=NuGet%20downloads&logo=nuget&color=0277BD&logoColor=white)](https://www.nuget.org/packages/QuestPDF/)


<br><br>

## Quick start  👋

Learn how easy it is to design, implement and generate PDF documents using QuestPDF. <br>
Effortlessly create documents of all types such as invoices and reports.

[![Learn Quick Start tutorial](https://img.shields.io/badge/read%20quick%20start%20tutorial-0288D1?style=for-the-badge)](https://www.questpdf.com/quick-start.html)

![questpdf-animation](https://github.com/user-attachments/assets/47d843d6-73d0-4abe-bf4e-1c3701e4a34c)

<br><br>

## Please help by giving a star ⭐

GitHub stars guide developers toward great tools. If you find this project valuable, please give it a star – it helps the community and takes just a second! 

<img src="https://github.com/user-attachments/assets/050bd413-16ea-4121-8871-fc38566e1e26" width="700" />

<br>

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

<br><br>

## QuestPDF Companion App

Accelerate your development with live document preview powered by the hot-reload capability, eliminating the need for C# code recompilation.
- Explore PDF document structure and hierarchy
- Quickly magnify and measure content
- Debug runtime exceptions with stack traces and code snippets
- Identify, understand and solve layout errors

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/7ab596d4-eebc-44e6-b36d-c358b16ed0ba">
  <source media="(prefers-color-scheme: light)" srcset="https://github.com/user-attachments/assets/39d4c08c-6a78-4743-8837-208c0c1718fd">
  <img src="https://github.com/user-attachments/assets/ce394258-1f10-498d-b65f-26c9fbed2994" width="600">
</picture>

[![Learn about QuestPDF Companion App](https://img.shields.io/badge/learn%20more-0288D1?style=for-the-badge)](https://www.questpdf.com/companion/usage.html)

<br><br>

## Exactly what you need

### Comprehensive Layout Engine
A powerful layout engine built specifically for PDF generation. Gain full control over document structure, precise content positioning, and automatic pagination for complex reports and invoices.

### Rich Toolkit
Accelerate your PDF development workflow with a rich set of reusable components and over 50 layout elements. Easily implement data-driven documents using a Fluent C# API.

### High Performance
Generate PDF files at scale with up to thousands of pages per second - while maintaining minimal CPU and memory usage. Perfect for high-throughput .NET applications.

### Advanced Language Support
Create multilingual PDF documents with full support for right-to-left (RTL) languages, advanced text shaping, and bi-directional layout handling.

<br><br>

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

<br><br>

## Multiplatform

Supports all major operating systems and works seamlessly with leading IDEs, cloud platforms, and modern development tools.
- `Technologies`: modern dotnet, legacy .NET Framework, Docker
- `Operating systems`: Windows, Linux, MacOS
- `Cloud providers`: Azure, AWS, Google Cloud
- `IDE`: Visual Studio, Visual Code, JetBrains Rider, others

<br><br>

## Fair and Sustainable License

By offering free access to most users and premium licenses for larger organizations, the project maintains its commitment to excellence:

- Long-term and sustainable development
- Regular feature, performance, quality and security updates
- Active community and enterprise support

> Free for individuals, non-profits, and businesses under $1M in annual revenue, as well as all FOSS projects.

[![QuestPDF License and Pricing](https://img.shields.io/badge/check%20pricing-388E3C?style=for-the-badge)](https://www.questpdf.com/license.html)

<br><br>

## Let's get started

Follow our detailed tutorial, and see how easy it is to generate a fully functional invoice with fewer than 250 lines of C# code.

<img src="https://raw.githubusercontent.com/QuestPDF/QuestPDF-Documentation/refs/heads/main/docs/public/homepage/invoice.png" width="400px" />

[![Read getting started tutorial](https://img.shields.io/badge/read%20getting%20started%20tutorial-0288D1?style=for-the-badge)](https://www.questpdf.com/getting-started.html)

<br><br>

## Cummunity QuestPDF

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

