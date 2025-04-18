using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class ElementPositionLocator : ContainerElement
    {
        public string Id { get; set; }

        internal override void Draw(Size availableSpace)
        {
            base.Draw(availableSpace);

            var matrix = Canvas.GetCurrentMatrix();

            var position = new PageElementLocation
            {
                Id = Id,

                PageNumber = PageContext.CurrentPage,

                Width = availableSpace.Width / matrix.ScaleX,
                Height = availableSpace.Height / matrix.ScaleY,

                X = matrix.TranslateX,
                Y = matrix.TranslateY
            };

            PageContext.CaptureContentPosition(position);
        }
    }
}