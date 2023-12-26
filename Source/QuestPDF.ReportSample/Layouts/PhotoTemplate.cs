using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample.Layouts
{
    public class PhotoTemplate : IComponent
    {
        public ReportPhoto Model { get; set; }

        public PhotoTemplate(ReportPhoto model)
        {
            Model = model;
        }
        
        public void Compose(IContainer container)
        {
            container
                .ShowEntire()
                .Column(column =>
                {
                    column.Spacing(5);
                    column.Item().Element(PhotoWithMaps);
                    column.Item().Element(PhotoDetails);
                });
        }
        
        void PhotoWithMaps(IContainer container)
        {
            container
                .Row(row =>
                {
                    row.RelativeItem(2).AspectRatio(4 / 3f).Component<ImagePlaceholder>();

                    row.RelativeItem().PaddingLeft(5).Column(column =>
                    {
                        column.Spacing(7f);
                        
                        column.Item().AspectRatio(4 / 3f).Component<ImagePlaceholder>();
                        column.Item().AspectRatio(4 / 3f).Component<ImagePlaceholder>();
                    });
                });
        }

        void PhotoDetails(IContainer container)
        {
            container.Border(0.75f).BorderColor(Colors.Grey.Medium).Grid(grid =>
            {
                grid.Columns(6);

                grid.Item().LabelCell().Text("Date");
                grid.Item(2).ValueCell().Text(Model.Date?.ToString("g") ?? string.Empty);
                grid.Item().LabelCell().Text("Location");
                grid.Item(2).ValueCell().Text(Model.Location.Format());
                
                grid.Item().LabelCell().Text("Comments");
                grid.Item(5).ValueCell().Text(Model.Comments);
            });
        }
    }
}