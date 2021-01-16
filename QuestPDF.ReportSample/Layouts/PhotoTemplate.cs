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
                .Stack(stack =>
                {
                    stack.Element(PhotoWithMaps);
                    stack.Element(PhotoDetails);
                });
        }
        
        void PhotoWithMaps(IContainer container)
        {
            container
                .Padding(-3)
                .PaddingBottom(3)
                .Row(row =>
                {
                    row.RelativeColumn(2).Padding(3).Component(new ImageTemplate(Model.PhotoData));

                    row.RelativeColumn().Stack(stack =>
                    {
                        stack.Element().Padding(3).Component(new ImageTemplate(Model.MapDetailsSource));
                        stack.Element().Padding(3).Component(new ImageTemplate(Model.MapContextSource));
                    });
                });
        }

        void PhotoDetails(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Element().Row(row =>
                {
                    row.RelativeColumn().DarkCell().Text("Date", Typography.Normal);
                    row.RelativeColumn(2).LightCell().Text(Model.Date?.ToString("g") ?? string.Empty, Typography.Normal);
                    row.RelativeColumn().DarkCell().Text("Location", Typography.Normal);
                    row.RelativeColumn(2).LightCell().Text(Model.Location.Format(), Typography.Normal);
                });
                
                stack.Element().Row(row =>
                {
                    row.RelativeColumn().DarkCell().Text("Comments", Typography.Normal);
                    row.RelativeColumn(5).LightCell().Text(Model.Comments, Typography.Normal);
                });
            });
        }
    }
}