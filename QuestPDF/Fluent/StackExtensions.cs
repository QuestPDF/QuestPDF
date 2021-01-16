using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class StackDescriptor
    {
        private Stack Stack { get; }

        internal StackDescriptor(Stack stack)
        {
            Stack = stack;
        }

        public void Spacing(float value)
        {
            Stack.Spacing = value;
        }
        
        public IContainer Element()
        {
            var container = new Container();
            Stack.Children.Add(container);
            return container;
        }
        
        public void Element(Action<IContainer> handler)
        {
            var container = new Container();
            Stack.Children.Add(container);
            handler?.Invoke(container);
        }
        
        public void Element(IElement element)
        {
            Stack.Children.Add(element as Element);
        }
    }
    
    public static class StackExtensions
    {
        public static void PageableStack(this IContainer element, Action<StackDescriptor> handler)
        {
            var column = new Stack
            {
                Pageable = true
            };

            element.Element(column);
            
            var descriptor = new StackDescriptor(column);
            handler(descriptor);
        }
        
        public static void Stack(this IContainer element, Action<StackDescriptor> handler)
        {
            var column = new Stack
            {
                Pageable = false
            };

            element.Element(column);
            
            var descriptor = new StackDescriptor(column);
            handler(descriptor);
        }
    }
}