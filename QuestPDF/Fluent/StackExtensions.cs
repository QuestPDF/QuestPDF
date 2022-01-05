using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class StackDescriptor
    {
        internal Stack Stack { get; } = new Stack();

        public void Spacing(float value)
        {
            Stack.Spacing = value;
        }
        
        public IContainer Item()
        {
            var container = new Container();
            Stack.Items.Add(container);
            return container;
        }
    }
    
    public static class StackExtensions
    {
        public static void Stack(this IContainer element, Action<StackDescriptor> handler)
        {
            var descriptor = new StackDescriptor();
            handler(descriptor);
            element.Component(descriptor.Stack);
        }
    }
}