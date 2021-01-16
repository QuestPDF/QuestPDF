using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Elements
{
    internal class PageNumber : Element
    {
        public string TextFormat { get; set; } = "";
        public TextStyle? TextStyle { get; set; }
        private Text? TextElement { get; set; }
        
        private int Number { get; set; } = 1;

        internal override ISpacePlan Measure(Size availableSpace)
        {
            InitializeTextElement();

            TextElement.Value = TextFormat.Replace("{number}", Number.ToString());
            return TextElement.Measure(availableSpace);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            InitializeTextElement();
            
            TextElement.Draw(canvas, availableSpace);
            Number++;
        }

        private void InitializeTextElement()
        {
            TextElement ??= new Text()
            {
                Style = TextStyle
            };
        }
    }
}