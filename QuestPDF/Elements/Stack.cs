using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Stack : Element
    {
        public float Spacing { get; set; }
        public bool Pageable { get; set; } = true;
        
        public ICollection<Element?> Children { get; internal set; } = new List<Element?>();
        private Queue<Element?> ChildrenQueue { get; set; }
        
        private void Initialize()
        {
            if (!Pageable)
                ChildrenQueue = null;
                
            ChildrenQueue ??= new Queue<Element>(Children.Where(x => x != null));
        }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            Initialize();
            
            if(!ChildrenQueue.Any())
                return new FullRender(Size.Zero);

            var heightOnCurrentPage = 0f;

            foreach (var renderer in ChildrenQueue)
            {
                var space = renderer.Measure(new Size(availableSpace.Width, availableSpace.Height - heightOnCurrentPage));

                if (space is Wrap)
                {
                    if (!Pageable)
                        return new Wrap();
                    
                    if (heightOnCurrentPage < Size.Epsilon)
                        return new Wrap();
                    
                    return new PartialRender(availableSpace.Width, heightOnCurrentPage - Spacing);
                }

                var size = space as Size;
                
                if (size.Height < Size.Epsilon)
                    continue;

                heightOnCurrentPage += size.Height + Spacing;

                if (space is PartialRender)
                {
                    if (!Pageable)
                        return new Wrap();
                    
                    return new PartialRender(availableSpace.Width, heightOnCurrentPage - Spacing);   
                }
            }
            
            return new FullRender(availableSpace.Width, heightOnCurrentPage - Spacing);
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

                if (size.Height < Size.Epsilon)
                {
                    ChildrenQueue.Dequeue();
                    continue;
                }

                canvas.Translate(new Position(0, topOffset));
                child.Draw(canvas, new Size(availableSpace.Width, size.Height));
                canvas.Translate(new Position(0, -topOffset));
                
                topOffset += size.Height + Spacing;

                if (space is PartialRender)
                    break;
                
                ChildrenQueue.Dequeue();
            }
        }
    }
}