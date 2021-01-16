using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class AspectRatio : ContainerElement
    {
        public float Ratio { get; set; } = 1;
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            if(Child == null)
                return new FullRender(Size.Zero);
            
            var size = GetSize(availableSpace);
            
            if (size.Height > availableSpace.Height + Size.Epsilon)
                return new Wrap();
            
            return new FullRender(size);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            if (Child == null)
                return;
            
            var size = GetSize(availableSpace);
            
            if (size.Height > availableSpace.Height)
                return;

            Child.Draw(canvas, size);
        }
        
        private Size GetSize(Size availableSpace)
        {
            return new Size(availableSpace.Width, (int)(availableSpace.Width / Ratio));
        }
    }
}