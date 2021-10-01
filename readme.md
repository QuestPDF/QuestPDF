<a href="https://www.questpdf.com/">
  <img src="https://github.com/QuestPDF/example-invoice/raw/main/images/logo.svg" width="400px"> 
</a>

## Overview

**Rely on solid fundamentals** - This library is created specifically for designing and arranging document layouts, with full paging support.  Alternative solutions, such as HTML-based converters, are not designed for this purpose and therefore are often unpredictable and do not produce desired results.

**Work with organized self-explanatory code** - The entire process of implementing PDF document, takes place in your code. Free yourself from slow visual designers and strange technological limitations. Follow simple yet highly effective approaches to create maintainable, high-quality code.

**Compose simple components into complex documents** - Do you remember the feeling when your code just works? When your ideas are becoming real without any effort? Working with simple, easy to understand, self-explanatory and highly composable layout elements is the key here!

**Create and reuse components** - Feel no fear of complex documents! Create custom, reusable components and divide the document's layout into easy to maintain pieces. Inject data to customize content and use slots to enhance composability. Decide how complex approaches your solution needs and follow the best path.

**Prototype with ease** - We understand that document generation is often tricky and require multiple iterations. The library offers additional prototyping tools such as random text generator or image placeholder element. By following best practices, you can develop a document without having data.

**Enjoy fast PDF generation** - QuestPDF is created upon SkiaSharp, a well-known graphical library, and converts your data into PDF documents. It offers a highly optimized layouting engine capable of generating over 1000 PDF files per minute per core. The entire process is thread-safe.

## Support QuestPDF

All great frameworks and libraries started from zero. Please help us to make QuestPDF a commonly known library and an obvious choice in case of generating PDF documents. It can be as easy as:
- Giving this repository a star ⭐ so more people will know about it,
- Observing 🤩 the library to know about each new realease,
- Trying our sample project to see how easy it is to create an invoice 📊,
- Sharing your thoughts 💬 with us and your colleagues,
- Simply using it 👨‍💻 and suggesting new features,
- Creating new features 🆕 for everybody.

## Installation

The library is available as a nuget package. You can install it as any other nuget package from your IDE, try to search by `QuestPDF`. You can find package details [on this webpage](https://www.nuget.org/packages/QuestPDF/).

<a href="https://www.nuget.org/packages/QuestPDF/">
  <img src="https://github.com/QuestPDF/example-invoice/raw/main/images/nuget.svg" width="200px">  
</a>

## Documentation

**[Release notes and roadmap](https://www.questpdf.com/documentation/releases.html)** - everything that is planned for future library iterations, description of new features and information about potential breaking changes.
**[Getting started tutorial](https://www.questpdf.com/documentation/getting-started.html)** - a short and easy to follow tutorial showing how to design an invoice document under 200 lines of code.
**[API Reference](https://www.questpdf.com/documentation/api-reference.html)** - a detailed description of behavior of all available components and how to use them with C# Fluent API.
**[Patterns and practices](https://www.questpdf.com/documentation/patterns-and-practices.html#document-metadata)** - everything that may help you designing great reports and reusable code that is easy to maintain.

## Example invoice

Do you believe that creating a complete invoice document can take less than 200 lines of code? We have prepared for you a step-by-step instruction that shows every detail of this implementation and describes the best patterns and practices.

For tutorial, documentation and API reference, please visit [the QuestPDF documentation](https://www.questpdf.com/documentation/getting-started.html).

<a href="https://github.com/QuestPDF/example-invoice">
  <img src="https://github.com/QuestPDF/example-invoice/raw/main/images/invoice.png" width="595px">
</a>

Here you can find an example code showing how easy is to write and understand the fluent API.

**General document structure** with header, content and footer:

```csharp
public void Compose(IDocumentContainer container)
{
    container
        .Page(page =>
        {
            page.Margin(50);
            
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            
            page.Footer().AlignCenter().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        });
}
```

**The header area** consists of basic invoice information along with a logo placeholder.

```csharp
void ComposeHeader(IContainer container)
{
    var titleTextStyle = TextStyle.Default.Size(20).SemiBold().Color(Colors.Blue.Medium);
    
    container.Row(row =>
    {
        row.RelativeColumn().Stack(stack =>
        {
            stack.Item().Text($"Invoice #{Model.InvoiceNumber}", titleStyle);

            stack.Item().Text(text =>
            {
                text.Span("Issue date: ", TextStyle.Default.SemiBold());
                text.Span($"{Model.IssueDate:d}");
            });

            stack.Item().Text(text =>
            {
                text.Span("Due date: ", TextStyle.Default.SemiBold());
                text.Span($"{Model.DueDate:d}");
            });
        });
        
        row.ConstantColumn(100).Height(50).Placeholder();
    });
}
```

Implementation of **the content area** that contains seller and customer details, then listing of all bought products, then a comments section.

```csharp
void ComposeContent(IContainer container)
{
    container.PaddingVertical(40).Stack(column => 
    {
        column.Spacing(20);
        
        column.Item().Row(row =>
        {
            row.RelativeColumn().Component(new AddressComponent("From", Model.SellerAddress));
            row.ConstantColumn(50);
            row.RelativeColumn().Component(new AddressComponent("For", Model.CustomerAddress));
        });

        column.Item().Element(ComposeTable);

        var totalPrice = Model.Items.Sum(x => x.Price * x.Quantity);
        
        column
            .Item()
            .PaddingRight(5)
            .AlignRight()
            .Text($"Grand total: {totalPrice}$", TextStyle.Default.SemiBold());

        if (!string.IsNullOrWhiteSpace(Model.Comments))
            column.Item().PaddingTop(25).Element(ComposeComments);
    });
}
```

**The table and comments** codes are extracted into separate methods to increase clarity:

```csharp
void ComposeTable(IContainer container)
{
    var headerStyle = TextStyle.Default.SemiBold();
    
    container.Decoration(decoration =>
    {
        // header
        decoration.Header().BorderBottom(1).Padding(5).Row(row => 
        {
            row.ConstantColumn(25).Text("#", headerStyle);
            row.RelativeColumn(3).Text("Product", headerStyle);
            row.RelativeColumn().AlignRight().Text("Unit price", headerStyle);
            row.RelativeColumn().AlignRight().Text("Quantity", headerStyle);
            row.RelativeColumn().AlignRight().Text("Total", headerStyle);
        });

        // content
        decoration
            .Content()
            .Stack(column =>
            {
                foreach (var item in Model.Items)
                {
                    column
                    .Item()
                    .ShowEntire()
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(5)
                    .Row(row => 
                    {
                        row.ConstantColumn(25).Text(Model.Items.IndexOf(item) + 1);
                        row.RelativeColumn(3).Text(item.Name);
                        row.RelativeColumn().AlignRight().Text($"{item.Price}$");
                        row.RelativeColumn().AlignRight().Text(item.Quantity);
                        row.RelativeColumn().AlignRight().Text($"{item.Price * item.Quantity}$");
                    });
                }
            });
    });
}
```

```csharp
void ComposeComments(IContainer container)
{
    container.ShowEntire().Background(Colors.Grey.Lighten3).Padding(10).Stack(message => 
    {
        message.Spacing(5);
        message.Item().Text("Comments", TextStyle.Default.Size(14).SemiBold());
        message.Item().Text(Model.Comments);
    });
}
```

**The address details section** is implemented using components. This way the code can be easily reused for both seller and customer:

```csharp
public class AddressComponent : IComponent
{
    private string Title { get; }
    private Address Address { get; }

    public AddressComponent(string title, Address address)
    {
        Title = title;
        Address = address;
    }
    
    public void Compose(IContainer container)
    {
        container.ShowEntire().Stack(column =>
        {
            column.Spacing(5);

            column
                .Item()
                .BorderBottom(1)
                .PaddingBottom(5)
                .Text(Title, TextStyle.Default.SemiBold());
            
            column.Item().Text(Address.CompanyName);
            column.Item().Text(Address.Street);
            column.Item().Text($"{Address.City}, {Address.State}");
            column.Item().Text(Address.Email);
            column.Item().Text(Address.Phone);
        });
    }
}
```
