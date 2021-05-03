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
                Title = Model.Title,
                Size = PageSizes.A4
            };
        }

        public void Compose(IContainer container)
        {
            container
                .PaddingVertical(40)
                .PaddingHorizontal(50)
                .Page(page =>
                {
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().PageNumber("Page {number}");
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
                        grid.Item().Stack(row =>
                        {   
                            row.Item().AlignLeft().Text(field.Label, Typography.Normal.SemiBold());
                            row.Item().Text(field.Value, Typography.Normal);
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