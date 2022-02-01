<a href="https://www.questpdf.com/">
  <img src="https://github.com/QuestPDF/example-invoice/raw/main/images/logo.svg" width="400px"> 
</a>

QuestPDF presents a new approach to PDF document generation. Unlike other libraries, it does not rely on the HTML-to-PDF conversion which in many cases is not reliable. Instead, it implements its own layouting engine that is optimized to cover all paging-related requirements. Then, everything is rendered using the SkiaSharp library (a Skia port for .NET, used in Chrome, Android, MAUI, etc.).

I have designed this layouting engine with full paging support in mind. The document consists of many simple elements (e.g. border, background, image, text, padding, table, grid etc.) that are composed together to create more complex structures. This way, as a developer, you can understand the behaviour of every element and use them with full confidence. Additionally, the document and all its elements support paging functionality. For example, an element can be moved to the next page (if there is not enough space) or even be split between pages like table's rows.

## Support QuestPDF

All great frameworks and libraries started from zero. Please help me make QuestPDF a commonly known library and an obvious choice in case of generating PDF documents. Please give it a start ⭐ and share with your colleagues 💬👨‍💻.

## Installation

The library is available as a nuget package. You can install it as any other nuget package from your IDE, try to search by `QuestPDF`. You can find package details [on this webpage](https://www.nuget.org/packages/QuestPDF/).

```
Install-Package QuestPDF
```

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
            table.Cell().Text(Model.Items.IndexOf(item) + 1);
            table.Cell().Text(item.Name);
            table.Cell().AlignRight().Text($"{item.Price}$");
            table.Cell().AlignRight().Text(item.Quantity);
            table.Cell().AlignRight().Text($"{item.Price * item.Quantity}$");
            
            table.Cell().ColumnSpan(5)
                 .PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        }
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
