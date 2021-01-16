using System;
using System.Linq;
using QuestPDF.Fluent;
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
                .MinHeight(100)
                .Section(section =>
                {
                    section
                        .Header()
                        .PaddingBottom(5)
                        .Text(Model.Title, Typography.Headline);

                    section.Content().PageableStack(column =>
                    {
                        foreach (var part in Model.Parts)
                        {
                            column.Element().Row(row =>
                            {
                                row.ConstantColumn(150).DarkCell().Text(part.Label, Typography.Normal);
                                var frame = row.RelativeColumn().LightCell();
                            
                                if (part is ReportSectionText text)
                                    frame.Text(text.Text, Typography.Normal);
                        
                                if (part is ReportSectionMap map)
                                    frame.Element(container => MapElement(container, map));
                        
                                if (part is ReportSectionPhotos photos)
                                    frame.Element(container => PhotosElement(container, photos));
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

            container.Stack(stack =>
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

            var rowCount = (int) Math.Ceiling(model.Photos.Count / 3f);

            container.Padding(-2).Stack(stack =>
            {
                foreach (var rowId in Enumerable.Range(0, rowCount))
                {
                    stack.Element().Row(row =>
                    {
                        foreach (var id in Enumerable.Range(0, 3))
                        {
                            var data = model.Photos.ElementAtOrDefault(rowId + id); 
                            row.RelativeColumn().Padding(2).Component(new ImageTemplate(data));
                        }
                    });
                }
            });
        }
    }
}