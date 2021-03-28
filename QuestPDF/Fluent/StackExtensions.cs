using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class StackDescriptor
    {
        private List<Element> Elements { get; } = new List<Element>();
        private float StackSpacing { get; set; } = 0;
        
        public void Spacing(float value)
        {
            StackSpacing = value;
        }
        
        public IContainer Element()
        {
            var container = new Container();
            Elements.Add(container);
            return container;
        }
        
        public void Element(Action<IContainer> handler)
        {
            handler?.Invoke(Element());
        }
        
        public void Element(IElement element)
        {
            Elements.Add(element as Element);
        }

        internal Element CreateStack()
        {
            if (Elements.Count == 0)
                return new Empty();
            
            if (StackSpacing <= Size.Epsilon)
                return new Stack
                {
                    Children = Elements
                };
            
            var children = Elements
                .Select(x => new Padding
                {
                    Bottom = StackSpacing,
                    Child = x
                })
                .Cast<Element>()
                .ToList();

            var stack = new Stack
            {
                Children = children
            };
            
            return new Padding
            {
                Bottom = -StackSpacing,
                Child = stack
            };
        }
    }
    
    public static class StackExtensions
    {
        public static void Stack(this IContainer element, Action<StackDescriptor> handler)
        {
            var descriptor = new StackDescriptor();
            handler(descriptor);
            element.Element(descriptor.CreateStack());
        }
    }
}