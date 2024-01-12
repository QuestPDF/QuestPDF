using System.Linq;
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
                        .Before()
                        .PaddingBottom(5)
                        .Text(Model.Title)
                        .Style(Typography.Headline);

                    decoration.Content().Border(0.75f).BorderColor(Colors.Grey.Medium).Column(column =>
                    {
                        foreach (var part in Model.Parts)
                        {
                            column.Item().EnsureSpace(25).Row(row =>
                            {
                                row.ConstantItem(150).LabelCell().Text(part.Label);
                                var frame = row.RelativeItem().ValueCell();
                            
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

        static void MapElement(IContainer container, ReportSectionMap model)
        {
            if (model.Location == null)
            {
                container.Text("No location provided");
                return;
            }

            container.ShowEntire().Column(column =>
            {
                column.Spacing(5);
                
                column.Item().MaxWidth(250).AspectRatio(4 / 3f).Component<ImagePlaceholder>();
                column.Item().Text(model.Location.Format());
            });
        }

        static void PhotosElement(IContainer container, ReportSectionPhotos model)
        {
            if (model.PhotoCount == 0)
            {
                container.Text("No photos").Style(Typography.Normal);
                return;
            }

            container.DebugArea("Photos").Grid(grid =>
            {
                grid.Spacing(5);
                grid.Columns(3);
                
                Enumerable
                    .Range(0, model.PhotoCount)
                    .ToList()
                    .ForEach(x => grid.Item().AspectRatio(4 / 3f).Component<ImagePlaceholder>());
            });
        }
    }
}