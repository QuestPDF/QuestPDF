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
                    page.Header(ComposeHeader);
                    page.Content(ComposeContent);
                    page.Footer().AlignCenter().PageNumber("Page {number}");
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeColumn().MaxWidth(300).Stack(stack =>
                {
                    stack.Spacing(5);
                    
                    stack
                        .Element()
                        .PaddingBottom(5)
                        .Text(Model.Title, Typography.Title);
                    
                    stack.Element().ShowOnce().Stack(table =>
                    {
                        table.Spacing(5);

                        foreach (var field in Model.HeaderFields)
                        {
                            table.Element().Row(row =>
                            {
                                row.ConstantColumn(50)
                                    .AlignLeft()
                                    .Text($"{field.Label}:", Typography.Normal);
                                
                                row.RelativeColumn()
                                    .PaddingLeft(10)
                                    .Text(field.Value, Typography.Normal);
                            });
                        }
                    });

                });
                
                row.ConstantColumn(150).Image(Model.LogoData);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).PageableStack(stack =>
            {
                stack.Spacing(20);

                foreach (var section in Model.Sections)
                    stack.Element().Component(new SectionTemplate(section));

                stack.Element().PageBreak();

                foreach (var photo in Model.Photos)
                    stack.Element().Component(new PhotoTemplate(photo));
            });
        }
    }
}