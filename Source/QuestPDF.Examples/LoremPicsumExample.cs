using System.Net;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class LoremPicsum : IComponent
    {
        public bool Greyscale { get; }

        public LoremPicsum(bool greyscale)
        {
            Greyscale = greyscale;
        }
        
        public void Compose(IContainer container)
        {
            var url = "https://picsum.photos/300/200";

            if (Greyscale)
                url += "?grayscale";

            using var client = new WebClient();
            client.Headers.Add("user-agent", "QuestPDF/1.0 Unit Testing");
            
            var response = client.DownloadData(url);
            container.Image(response);
        }
    }
    
    public class LoremPicsumExample
    {
        [Test]
        public void LoremPicsum()
        {
            RenderingTest
                .Create()
                .PageSize(350, 280)
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(25)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            column
                                .Item()
                                .Component(new LoremPicsum(true));
                    
                            column
                                .Item()
                                .AlignRight()
                                .Text("From Lorem Picsum");
                        });
                });
        }
    }
}
