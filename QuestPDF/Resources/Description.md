## QuestPDF Overview

QuestPDF is an open-source, modern and battle-tested library that can help you with generating PDF documents by offering friendly, discoverable and predictable C# fluent API.

## Features

**Rely on solid fundamentals** - This library is created specifically for designing and arranging document layouts, with full paging support.  Alternative solutions, such as HTML-based converters, are not designed for this purpose and therefore are often unpredictable and do not produce desired results.

**Work with organized self-explanatory code** - The entire process of implementing PDF document, takes place in your code. Free yourself from slow visual designers and strange technological limitations. Follow simple yet highly effective approaches to create maintainable, high-quality code.

**Compose simple components into complex documents** - Do you remember the feeling when your code just works? When your ideas are becoming real without any effort? Working with simple, easy to understand, self-explanatory and highly composable layout elements is the key here!

**Create and reuse components** - Feel no fear of complex documents! Create custom, reusable components and divide the document's layout into easy to maintain pieces. Inject data to customize content and use slots to enhance composability. Decide how complex approaches your solution needs and follow the best path.

**Prototype with ease** - We understand that document generation is often tricky and require multiple iterations. The library offers additional prototyping tools such as random text generator or image placeholder element. By following best practices, you can develop a document without having data.

**Enjoy fast PDF generation** - QuestPDF is created upon SkiaSharp, a well-known graphical library, and converts your data into PDF documents. It offers a highly optimized layouting engine capable of generating over 1000 PDF files per minute per core. The entire process is thread-safe.

## Learning resources

**[Release notes and roadmap](https://www.questpdf.com/documentation/releases.html)** - everything that is planned for future library iterations, description of new features and information about potential breaking changes.

**[Getting started tutorial](https://www.questpdf.com/documentation/getting-started.html)** - a short and easy to follow tutorial showing how to design an invoice document under 200 lines of code.

**[API Reference](https://www.questpdf.com/documentation/api-reference.html)** - a detailed description of behavior of all available components and how to use them with C# Fluent API.

**[Patterns and practices](https://www.questpdf.com/documentation/patterns-and-practices.html#document-metadata)** - everything that may help you designing great reports and reusable code that is easy to maintain.

## Example invoice

Do you believe that creating a complete invoice document can take less than 200 lines of code? We have prepared for you a step-by-step instruction that shows every detail of this implementation and describes the best patterns and practices.

For tutorial, documentation and API reference, please visit [the QuestPDF documentation](https://www.questpdf.com/documentation/getting-started.html).

![invoice](https://raw.githubusercontent.com/QuestPDF/example-invoice/main/images/invoice.png)

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
        {
            column.Item().Text($"Invoice #{Model.InvoiceNumber}", titleStyle);

            column.Item().Text(text =>
            {
                text.Span("Issue date: ", TextStyle.Default.SemiBold());
                text.Span($"{Model.IssueDate:d}");
            });

            column.Item().Text(text =>
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
    container.PaddingVertical(40).column(column => 
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
    
    container.Table(table =>
    {
        table.ColumnsDefinition(columns =>
        {
            columns.ConstantColumn(25);
            columns.RelativeColumn(3);
            columns.RelativeColumn();
            columns.RelativeColumn();
            columns.RelativeColumn();
        });
        
        table.Header(header =>
        {
            header.Cell().Text("#", headerStyle);
            header.Cell().Text("Product", headerStyle);
            header.Cell().AlignRight().Text("Unit price", headerStyle);
            header.Cell().AlignRight().Text("Quantity", headerStyle);
            header.Cell().AlignRight().Text("Total", headerStyle);
            
            header.Cell().ColumnSpan(5)
                  .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
        });
        
        foreach (var item in Model.Items)
        {
            table.Cell().Element(CellStyle).Text(Model.Items.IndexOf(item) + 1);
            table.Cell().Element(CellStyle).Text(item.Name);
            table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price}$");
            table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity);
            table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price * item.Quantity}$");
            
            static IContainer CellStyle(IContainer container)
            {
                container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
            }
        }
    });
}
```

```csharp
void ComposeComments(IContainer container)
{
    container.ShowEntire().Background(Colors.Grey.Lighten3).Padding(10).column(message => 
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
        container.ShowEntire().column(column =>
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
