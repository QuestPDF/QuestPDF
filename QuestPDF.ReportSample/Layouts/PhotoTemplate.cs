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
                    stack.Item(PhotoWithMaps);
                    stack.Item(PhotoDetails);
                });
        }
        
        void PhotoWithMaps(IContainer container)
        {
            container
                .Row(row =>
                {
                    row.RelativeColumn(2).AspectRatio(4 / 3f).Image(Placeholders.Image);

                    row.RelativeColumn().PaddingLeft(5).Stack(stack =>
                    {
                        stack.Spacing(7f);
                        
                        stack.Item().AspectRatio(4 / 3f).Image(Placeholders.Image);
                        stack.Item().AspectRatio(4 / 3f).Image(Placeholders.Image);
                    });
                });
        }

        void PhotoDetails(IContainer container)
        {
            container.Border(0.75f).BorderColor(Colors.Grey.Medium).Grid(grid =>
            {
                grid.Columns(6);
                
                grid.Element().LabelCell().Text("Date", Typography.Normal);
                grid.Element(2).ValueCell().Text(Model.Date?.ToString("g") ?? string.Empty, Typography.Normal);
                grid.Element().LabelCell().Text("Location", Typography.Normal);
                grid.Element(2).ValueCell().Text(Model.Location.Format(), Typography.Normal);
                
                grid.Element().LabelCell().Text("Comments", Typography.Normal);
                grid.Element(5).ValueCell().Text(Model.Comments, Typography.Normal);
            });
        }
    }
}