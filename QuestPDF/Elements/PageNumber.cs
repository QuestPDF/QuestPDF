using System;
using System.Text.RegularExpressions;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Elements
{
    internal class PageNumber : Element
    {
        public string TextFormat { get; set; } = "";
        private Text TextElement { get; set; } = new Text();

        public TextStyle? TextStyle
        {
            get => TextElement?.Style;
            set => TextElement.Style = value;
        }

        internal override void HandleVisitor(Action<Element?> visit)
        {
            TextElement.HandleVisitor(visit);
            base.HandleVisitor(visit);
        }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            TextElement.Value = Regex.Replace(TextFormat, @"{pdf:[ \w]+}", "123");
            return TextElement.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            TextElement.Value = GetText();
            TextElement.Draw(availableSpace);
        }

        private string GetText()
        {
            var result = TextFormat;
            
            foreach (var location in PageContext.GetRegisteredLocations())
                result = result.Replace($"{{pdf:{location}}}", PageContext.GetLocationPage(location).ToString());

            return result;
        }
    }
}