using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample.Layouts
{
    public class ImagePlaceholder : IComponent
    {
        public static bool Solid { get; set; } = false;
        
        public void Compose(IContainer container)
        {
            if (Solid)
                container.Background(Placeholders.Color());
            
            else
                container.Image(Placeholders.Image);
        }
    }
}