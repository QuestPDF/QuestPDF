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
                .Decoration(decoration =>
                {
                    decoration
                        .Header()
                        .PaddingBottom(5)
                        .Text(Model.Title, Typography.Headline);

                    decoration.Content().Border(0.75f).BorderColor(Colors.Grey.Medium).Stack(stack =>
                    {
                        foreach (var part in Model.Parts)
                        {
                            stack.Item().EnsureSpace(25).Row(row =>
                            {
                                row.ConstantColumn(150).LabelCell().Text(part.Label);
                                var frame = row.RelativeColumn().ValueCell();
                            
                                if (part is ReportSectionText text)
                                    frame.ShowEntire().Text(text.Text);
                        
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
                container.Text("No location provided");
                return;
            }

            container.ShowEntire().Stack(stack =>
            {
                stack.Spacing(5);
                
                stack.Item().MaxWidth(250).AspectRatio(4 / 3f).Component<ImagePlaceholder>();
                stack.Item().Text(model.Location.Format());
            });
        }
        
        void PhotosElement(IContainer container, ReportSectionPhotos model)
        {
            if (model.Photos.Count == 0)
            {
                container.Text("No photos");
                return;
            }

            container.Debug("Photos").Grid(grid =>
            {
                grid.Spacing(5);
                grid.Columns(3);
                
                model.Photos.ForEach(x => grid.Item().AspectRatio(4 / 3f).Component<ImagePlaceholder>());
            });
        }
    }
}