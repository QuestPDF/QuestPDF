using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

static class MapboxStaticMapRenderer
{
    private const string MapboxBaseUrl = "https://api.mapbox.com/styles/v1/mapbox/streets-v12/static";
    private const string AccessToken = "pk.eyJ1IjoibWFyY2luLXppYWJlayIsImEiOiJjbTc5cHZkZTUwNmM4MmxxdGN2cnRxMTBpIn0.8G-_nwFqjjfNQUCmHSOqKw";

    public static async Task<byte[]?> FetchStaticMapAsync(double longitude, double latitude, float zoom, int width, int height)
    {
        var longitudeString = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var latitudeString = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var url = $"{MapboxBaseUrl}/{longitudeString},{latitudeString},{zoom},0,0/{width}x{height}@2x?access_token={AccessToken}";

        using var client = new HttpClient();
        
        try
        {
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}


public class MapExample
{
    [Test]
    public async Task SimpleExample()
    {
        var map = await MapboxStaticMapRenderer.FetchStaticMapAsync(19.9376052f, 50.0616087f, 10, 500, 400);
        
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.ContinuousSize(550);
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Item().Text("Map of Kraków").FontSize(20).Bold();
                            column.Item().Text("Capital of Lesser Poland Voivodeship").FontSize(16).Light();
                            column.Item().Height(15);

                            column.Item()
                                .Background(Colors.Grey.Lighten3)
                                .ShowIf(map != null)
                                .Image(map);
                        });
                });
            })
            .GenerateImages(x => "map.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}