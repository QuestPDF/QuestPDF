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

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.MarginVertical(40);
                    page.MarginHorizontal(50);
                    
                    page.Size(PageSizes.A4);
                        
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.DefaultTextStyle(Typography.Normal);
                        
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Item().Row(row =>
                {
                    row.Spacing(50);
                    
                    row.RelativeColumn().PaddingTop(-10).Text(Model.Title, Typography.Title);
                    row.ConstantColumn(150).ExternalLink("https://www.questpdf.com").Image(Model.LogoData);
                });

                stack.Item().ShowOnce().PaddingVertical(15).Border(1f).BorderColor(Colors.Grey.Lighten1).ExtendHorizontal();
                
                stack.Item().ShowOnce().Grid(grid =>
                {
                    grid.Columns(2);
                    grid.Spacing(5);
                        
                    foreach (var field in Model.HeaderFields)
                    {
                        grid.Item().Text(text =>
                        {
                            text.Span($"{field.Label}: ", Typography.Normal.SemiBold());
                            text.Span(field.Value, Typography.Normal);
                        });
                    }
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Stack(stack =>
            {
                stack.Spacing(20);

                stack.Item().Component(new TableOfContentsTemplate(Model.Sections));
                
                stack.Item().PageBreak();
                
                foreach (var section in Model.Sections)
                    stack.Item().Location(section.Title).Component(new SectionTemplate(section));

                stack.Item().PageBreak();
                stack.Item().Location("Photos");
                
                foreach (var photo in Model.Photos)
                    stack.Item().Component(new PhotoTemplate(photo));
            });
        }
    }
}