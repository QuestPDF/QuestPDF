using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample.Layouts
{
    public class StandardReport : IDocument
    {
        private ReportModel Model { get; }

        public StandardReport(ReportModel model)
        {
            Model = model;
        }
        
        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata()
            {
                Title = Model.Title
            };
        }
        
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.DefaultTextStyle(Typography.Normal);
                    
                    page.MarginVertical(40);
                    page.MarginHorizontal(50);
                    
                    page.Size(PageSizes.A4);
                        
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber().Format(x => x?.FormatAsRomanNumeral() ?? "-----");
                        text.Span(" / ");
                        text.TotalPages().Format(x => x?.FormatAsRomanNumeral() ?? "-----");
                    });
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.Spacing(50);
                    
                    row.RelativeItem().PaddingTop(-5).Text(Model.Title).Style(Typography.Title);
                    row.ConstantItem(90).Hyperlink("https://www.questpdf.com").MaxHeight(30).Component<ImagePlaceholder>();
                });

                column.Item().ShowOnce().PaddingVertical(15).Border(1f).BorderColor(Colors.Grey.Lighten1).ExtendHorizontal();
                
                column.Item().ShowOnce().Grid(grid =>
                {
                    grid.Columns(2);
                    grid.Spacing(5);
                        
                    foreach (var field in Model.HeaderFields)
                    {
                        grid.Item().Text(text =>
                        {
                            text.Span($"{field.Label}: ").SemiBold();
                            text.Span(field.Value);
                        });
                    }
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(20);
                
                column.Item().Component(new TableOfContentsTemplate(Model.Sections));
                
                column.Item().PageBreak();
                
                foreach (var section in Model.Sections)
                    column.Item().Section(section.Title).Component(new SectionTemplate(section));
                
                column.Item().PageBreak();
                column.Item().Section("Photos");
                
                foreach (var photo in Model.Photos)
                    column.Item().Component(new PhotoTemplate(photo));
            });
        }
    }
}
