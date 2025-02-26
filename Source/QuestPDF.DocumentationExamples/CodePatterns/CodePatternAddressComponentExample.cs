using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.CodePatterns;

public class CodePatternAddressComponentExample
{
    [Test]
    public void Example()
    {
        var address = new Address
        {
            CompanyName = "Apple",
            PostalCode = "95014",
            Country = "United States",
            City = "Cupertino",
            Street = "One Apple Park Way"
        };
        
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(600, 1200));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Component(new AddressComponent(address));
                });
            })
            .GenerateImages(x => $"code-pattern-component-address.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }

    public class Address
    {
        public string CompanyName { get; set; }
        
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
    }
    
    public class AddressComponent : IComponent
    {
        private Address Address { get; }

        public AddressComponent(Address address)
        {
            Address = address;
        }
        
        public void Compose(IContainer container)
        {
            container
                .Column(column =>
                {
                    column.Spacing(10);
                    
                    AddItem("Company name", Address.CompanyName);
                    AddItem("Postal code", Address.PostalCode);
                    AddItem("Country", Address.Country);
                    AddItem("City", Address.City);
                    AddItem("Street", Address.Street);
                    
                    void AddItem(string label, string value)
                    {
                        column.Item().Text(text =>
                        {
                            text.Span($"{label}: ").Bold();
                            text.Span(value);
                        });
                    }
                });
        }
    }
}