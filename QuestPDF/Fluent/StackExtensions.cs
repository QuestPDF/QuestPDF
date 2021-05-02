using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class StackDescriptor
    {
        private List<Element> Items { get; } = new List<Element>();
        private float StackSpacing { get; set; } = 0;
        
        public void Spacing(float value)
        {
            StackSpacing = value;
        }
        
        public IContainer Item()
        {
            var container = new Container();
            Items.Add(container);
            return container;
        }
        
        public void Item(Action<IContainer> handler)
        {
            handler?.Invoke(Item());
        }
        
        internal IComponent CreateStack()
        {
            return new TreeStack
            {
                Children = Items,
                Spacing = StackSpacing
            };
        }
        
        internal Element CreateStack2()
        {
            if (Items.Count == 0)
                return Empty.Instance;
            
            if (StackSpacing <= Size.Epsilon)
                return new Stack
                {
                    Children = Items
                };
            
            var children = Items
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
            element.Component(descriptor.CreateStack());
        }
    }
}