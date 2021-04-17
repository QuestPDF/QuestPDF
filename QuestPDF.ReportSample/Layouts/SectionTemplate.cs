using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample.Layouts
{
    public class SectionTemplate : IComponent
    {
        public ReportSection Model { get; set; }

        public SectionTemplate(ReportSection model)
        {
            Model = model;
        }
        
        public void Compose(IContainer container)
        {
            container
                .EnsureSpace()
                .Section(section =>
                {
                    section
                        .Header()
                        .PaddingBottom(5)
                        .Text(Model.Title, Typography.Headline);

                    section.Content().Border(0.75f).BorderColor(Colors.Grey.Medium).Stack(stack =>
                    {
                        foreach (var part in Model.Parts)
                        {
                            stack.Element().Row(row =>
                            {
                                row.ConstantColumn(150).LabelCell().Text(part.Label, Typography.Normal);
                                var frame = row.RelativeColumn().ValueCell();
                            
                                if (part is ReportSectionText text)
                                    frame.Text(text.Text, Typography.Normal);
                        
                                if (part is ReportSectionMap map)
                                    frame.Element(x => MapElement(x, map));
                        
                                if (part is ReportSectionPhotos photos)
                                    frame.Element(x => PhotosElement(x, photos));
                            });
                        }
                    });
                });
        }
        
        void MapElement(IContainer container, ReportSectionMap model)
        {
            if (model.ImageSource == null || model.Location == null)
            {
                container.Text("No location provided", Typography.Normal);
                return;
            }

            container.ShowEntire().Stack(stack =>
            {
                stack.Spacing(5);
                
                stack.Element().Component(new ImageTemplate(model.ImageSource));
                stack.Element().Text(model.Location.Format(), Typography.Normal);
            });
        }
        
        void PhotosElement(IContainer container, ReportSectionPhotos model)
        {
            if (model.Photos.Count == 0)
            {
                container.Text("No photos", Typography.Normal);
                return;
            }

            container.Debug().Debug().Grid(grid =>
            {
                grid.Spacing(5);
                grid.Columns(3);
                
                model.Photos.ForEach(x => grid.Element().Component(new ImageTemplate(x)));
            });
        }
    }
}