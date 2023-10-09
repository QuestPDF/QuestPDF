using SkiaSharp;

namespace QuestPDF.Previewer;

class Helpers
{
    public static void GeneratePdfFromDocumentSnapshots(string filePath, ICollection<DocumentSnapshot.PageSnapshot> pages)
    {
        using var stream = File.Create(filePath);
            
        using var document = SKDocument.CreatePdf(stream);
            
        foreach (var page in pages)
        {
            using var canvas = document.BeginPage(page.Width, page.Height);
            canvas.DrawPicture(page.Picture);
            document.EndPage();
        }
    }
}