using System.Numerics;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class ElementPositionLocator : ContainerElement
    {
        public string Id { get; set; }

        internal override void Draw(Size availableSpace)
        {
            base.Draw(availableSpace);

            var transform = Canvas.GetCurrentMatrix().ToMatrix4x4();

            var scaleX = new Vector2(transform.M11, transform.M12).Length();
            var scaleY = new Vector2(transform.M21, transform.M22).Length();

            var actualPosition = Vector2.Transform(Vector2.Zero, transform);

            var position = new PageElementLocation
            {
                Id = Id,

                PageNumber = PageContext.CurrentPage,

                Width = availableSpace.Width * scaleX,
                Height = availableSpace.Height * scaleY,

                X = actualPosition.X,
                Y = actualPosition.Y,
                
                Transform = transform
            };

            PageContext.CaptureContentPosition(position);
        }
    }
}