using System.Net;
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

            if(Greyscale)
                url += "?grayscale";

            using var client = new WebClient();
            var response = client.DownloadData(url);
            container.Image(response);
        }
    }
    
    public class LoremPicsumExample : ExampleTestBase
    {
        [ShowResult]
        [ImageSize(350, 280)]
        public void LoremPicsum(IContainer container)
        {
            container
                .Background("#FFF")
                .Padding(25)
                .Stack(column =>
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
        }
    }
}