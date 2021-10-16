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
                .Stack(stack =>
                {
                    stack.Spacing(5);
                    stack.Item().Element(PhotoWithMaps);
                    stack.Item().Element(PhotoDetails);
                });
        }
        
        void PhotoWithMaps(IContainer container)
        {
            container
                .Row(row =>
                {
                    row.RelativeColumn(2).AspectRatio(4 / 3f).Background(Colors.Grey.Lighten3);

                    row.RelativeColumn().PaddingLeft(5).Stack(stack =>
                    {
                        stack.Spacing(7f);
                        
                        stack.Item().AspectRatio(4 / 3f).Background(Colors.Grey.Lighten3);
                        stack.Item().AspectRatio(4 / 3f).Background(Colors.Grey.Lighten3);
                    });
                });
        }

        void PhotoDetails(IContainer container)
        {
            container.Border(0.75f).BorderColor(Colors.Grey.Medium).Grid(grid =>
            {
                grid.Columns(6);
                
                grid.Item().LabelCell().Text("Date", Typography.Normal);
                grid.Item(2).ValueCell().Text(Model.Date?.ToString("g") ?? string.Empty, Typography.Normal);
                grid.Item().LabelCell().Text("Location", Typography.Normal);
                grid.Item(2).ValueCell().Text(Model.Location.Format(), Typography.Normal);
                
                grid.Item().LabelCell().Text("Comments", Typography.Normal);
                grid.Item(5).ValueCell().Text(Model.Comments, Typography.Normal);
            });
        }
    }
}