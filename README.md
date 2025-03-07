<a href="https://www.questpdf.com/" target="_blank">
  <img src="https://github.com/QuestPDF/example-invoice/raw/main/images/logo.svg" width="400"> 
</a>

---

[![QuestPDF Homepage](https://img.shields.io/badge/Homepage-blue?style=for-the-badge)](https://www.questpdf.com)
[![Dotnet](https://img.shields.io/badge/platform-.NET-blue?style=for-the-badge)](https://www.nuget.org/packages/QuestPDF/)
[![GitHub Repo stars](https://img.shields.io/github/stars/QuestPDF/QuestPDF?style=for-the-badge)](https://github.com/QuestPDF/QuestPDF/stargazers)
[![Nuget version](https://img.shields.io/nuget/v/QuestPdf?style=for-the-badge)](https://www.nuget.org/packages/QuestPDF/)
[![Nuget download](https://img.shields.io/nuget/dt/QuestPDF?style=for-the-badge)](https://www.nuget.org/packages/QuestPDF/)
[![QuestPDF License](https://img.shields.io/badge/LICENSE%20details-Community%20MIT%20and%20professional-green?style=for-the-badge)](https://www.questpdf.com/license/)

<br />

### QuestPDF is a modern open-source .NET library for PDF document generation. Offering comprehensive layout engine powered by concise and discoverable C# Fluent API.

https://github.com/user-attachments/assets/a674c413-34c4-47b5-b559-f279b1bf46c0

<br />
<br />
<br />

## Please help by giving a star

GitHub stars guide developers toward great tools. If you find this project valuable, please give it a star – it helps the community and takes just a second! ⭐

<img src="https://github.com/user-attachments/assets/d32d2fe5-9d82-4dc3-9da5-6025aa204add" width="700" />


<br />

## QuestPDF Companion App

Accelerate your development with live document preview powered by the hot-reload capability, eliminating the need for code recompilation:
- Explore document structure and hierarchy
- Quickly magnify and measure content
- Debug runtime exceptions with stack traces and code snippets
- Identify, understand and solve layout errors

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/7ab596d4-eebc-44e6-b36d-c358b16ed0ba">
  <source media="(prefers-color-scheme: light)" srcset="https://github.com/user-attachments/assets/39d4c08c-6a78-4743-8837-208c0c1718fd">
  <img src="https://github.com/user-attachments/assets/ce394258-1f10-498d-b65f-26c9fbed2994" width="600">
</picture>

[![Companion App](https://img.shields.io/badge/%F0%9F%9A%80%20read-companion%20app-blue?style=for-the-badge)](https://www.questpdf.com/companion/features.html)

<br />

## What you need is here

`Comprehensive Layout Engine` - A layout engine tailored for document generation, offering advanced paging and precise content control.

`Rich Toolkit` - Craft documents with intuitive, reusable components and over 50 layout elements for complex designs.

`High Performance` - Generate thousands of pages per second with minimal CPU and memory usage.

`Advanced Language Support` - Seamlessly create multilingual documents with support for RTL, text shaping, and bi-directional content.

<br />

## Code-Focused Paradigm

Using C# to design PDF documents leverages powerful control structures like if-statements, for-loops, and methods, enabling dynamic and highly customizable content generation.

It promotes best practices such as modular design and reusability while seamlessly integrating with source control systems for collaboration and versioning.

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

<br />

## Multiplatform

The library supports all major operating systems, integrates seamlessly with leading IDEs as well as popular cloud platforms and technologies to ensure maximum flexibility.

- `Technologies`: modern dotnet, legacy .NET Framework, Docker
- `Operating systems`: Windows, Linux, MacOS
- `Cloud providers`: Azure, AWS, Google Cloud
- `IDE`: Visual Studio, Visual Code, JetBrains Rider, others

<br />

## Perform common PDF operations

- Merge documents
- Attach files
- Extract pages
- Encrypt / decrypt
- Extend metadata
- Limit access
- Optimize for Web
- Overlay / underlay

```csharp
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

[![Getting started tutorial](https://img.shields.io/badge/%F0%9F%9A%80%20read-document%20operations-blue?style=for-the-badge)](https://www.questpdf.com/concepts/document-operations.html)

<br />

## Let's get started

Follow our detailed tutorial, and see how easy it is to produce a fully functional invoice with fewer than 250 lines of C# code.

[![Getting started tutorial](https://img.shields.io/badge/%F0%9F%9A%80%20read-getting%20started-blue?style=for-the-badge)](https://www.questpdf.com/getting-started.html)

<img src="https://github.com/QuestPDF/QuestPDF-Documentation/blob/main/docs/public/invoice-small.png?raw=true" width="400px">

<br />


## Sustainable and Fair License

By offering free access to most users and premium licenses for larger organizations, the project maintains its commitment to excellence while ensuring sustainable, long-term development for all.

> [!WARNING]
> The library is free to use for any individual or business with less than 1 million USD annual gross revenue, or operates as a non-profit organization, or is a FOSS project.

[![Library license details](https://img.shields.io/badge/%F0%9F%93%9C%0A%20read-license%20details-blue?style=for-the-badge)](https://www.questpdf.com/license/)

<br />


## QuestPDF on YouTube

We are incredibly grateful to the YouTube Community for their positive reviews and recommendations of the QuestPDF library. Your support and feedback are invaluable and motivate us to keep improving and expanding this project. Thank you for helping us grow and reach more developers!

#### Nick Chapsas: The Easiest Way to Create PDFs in .NET

[![Nick Chapsas The Easiest Way to Create PDFs in .NET](https://img.youtube.com/vi/_M0IgtGWnvE/0.jpg)](https://www.youtube.com/watch?v=_M0IgtGWnvE)

#### Claudio Bernasconi: QuestPDF - The BEST PDF Generator for .NET?!

[![Claudio Bernasconi QuestPDF - The BEST PDF Generator for .NET?!](https://img.youtube.com/vi/T89A_7dz1P8/0.jpg)](https://www.youtube.com/watch?v=T89A_7dz1P8)

#### JetBrains: OSS Power-Ups: QuestPDF

[![JetBrains OSS Power-Ups: QuestPDF](https://img.youtube.com/vi/-iYvZvpLX0g/0.jpg)](https://www.youtube.com/watch?v=-iYvZvpLX0g)

#### Programming with Felipe Gavilan: Generating PDFs with C# - Very Easy (two examples)

[![Felipe Gavilan Generating PDFs with C# - Very Easy (two examples)](https://img.youtube.com/vi/bhR4Cmg16gs/0.jpg)](https://www.youtube.com/watch?v=bhR4Cmg16gs)


