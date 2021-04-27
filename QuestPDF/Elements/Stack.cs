using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Stack : Element
    {
        public ICollection<Element?> Children { get; internal set; } = new List<Element?>();
        private Queue<Element?> ChildrenQueue { get; set; } = new Queue<Element?>();
        
        private void Initialize()
        {
            if (ChildrenQueue.Count == 0)
                ChildrenQueue = new Queue<Element>(Children.Where(x => x != null));
        }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            Initialize();
            
            if(!ChildrenQueue.Any())
                return new FullRender(Size.Zero);

            var heightOnCurrentPage = 0f;
            var maxWidth = 0f;

            foreach (var renderer in ChildrenQueue)
            {
                var space = renderer.Measure(new Size(availableSpace.Width, availableSpace.Height - heightOnCurrentPage));

                if (space is Wrap)
                {
                    if (heightOnCurrentPage < Size.Epsilon)
                        return new Wrap();
                    
                    return new PartialRender(maxWidth, heightOnCurrentPage);
                }

                var size = space as Size;
                heightOnCurrentPage += size.Height;

                if (size.Width > maxWidth)
                    maxWidth = size.Width;

                if (space is PartialRender)
                    return new PartialRender(maxWidth, heightOnCurrentPage);
            }
            
            return new FullRender(maxWidth, heightOnCurrentPage);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            Initialize();
            
            var topOffset = 0f;

            while (ChildrenQueue.Any())
            {
                var child = ChildrenQueue.Peek();
                
                var restSpace = new Size(availableSpace.Width, availableSpace.Height - topOffset);
                var space = child.Measure(restSpace);
                
                if (space is Wrap)
                    break;

                var size = space as Size;
                
                canvas.Translate(new Position(0, topOffset));
                child.Draw(canvas, new Size(availableSpace.Width, size.Height));
                canvas.Translate(new Position(0, -topOffset));
                
                topOffset += size.Height;

                if (space is PartialRender)
                    break;
                
                ChildrenQueue.Dequeue();
            }
        }
    }
}