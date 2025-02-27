using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class StopPagingExamples
{
    [Test]
    public void Example()
    {
        const string bookDescription = "\"Master Modern C# Development\" is a comprehensive guide that takes you from the basics to advanced concepts in C# programming. Perfect for beginners and intermediate developers looking to enhance their skills with practical examples and real-world applications. Covering object-oriented programming, LINQ, asynchronous programming, and the latest .NET features, this book provides step-by-step explanations to help you write clean, efficient, and scalable code. Whether you're building desktop, web, or cloud applications, this resource equips you with the knowledge and best practices to become a confident C# developer.";
        
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Width(400)
                        .Height(300)
                        .StopPaging()
                        .Decoration(decoration =>
                        {
                            decoration.Before().Text("Book description:").Bold();
                            decoration.Content().Text(bookDescription);
                        });
                });
            })
            .GeneratePdf("stop-paging-enabled.pdf");
    }
}