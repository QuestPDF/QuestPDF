using QuestPDF.Fluent;
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
                    stack.Element(PhotoWithMaps);
                    stack.Element(PhotoDetails);
                });
        }
        
        void PhotoWithMaps(IContainer container)
        {
            container
                .Row(row =>
                {
                    row.Spacing(5);
                    
                    row.RelativeColumn(2).Component(new ImageTemplate(Model.PhotoData));

                    row.RelativeColumn().Stack(stack =>
                    {
                        stack.Spacing(5);
                        
                        stack.Element().Component(new ImageTemplate(Model.MapDetailsSource));
                        stack.Element().Component(new ImageTemplate(Model.MapContextSource));
                    });
                });
        }

        void PhotoDetails(IContainer container)
        {
            container.Border(0.75f).Grid(grid =>
            {
                grid.Columns(6);
                
                grid.Element().DarkCell().Text("Date", Typography.Normal);
                grid.Element(2).LightCell().Text(Model.Date?.ToString("g") ?? string.Empty, Typography.Normal);
                grid.Element().DarkCell().Text("Location", Typography.Normal);
                grid.Element(2).LightCell().Text(Model.Location.Format(), Typography.Normal);
                
                grid.Element().DarkCell().Text("Comments", Typography.Normal);
                grid.Element(5).LightCell().Text(Model.Comments, Typography.Normal);
            });
        }
    }
}