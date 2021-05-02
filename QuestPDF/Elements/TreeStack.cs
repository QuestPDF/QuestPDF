using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using IComponent = QuestPDF.Infrastructure.IComponent;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace QuestPDF.Elements
{
    internal class TreeStack : IComponent
    {
        public ICollection<Element> Children { get; internal set; } = new List<Element>();
        public float Spacing { get; set; } = 0;
        
        public void Compose(IContainer container)
        {
            var elements = AddSpacing(Children);

            container
                .PaddingBottom(-Spacing)    
                .Element(BuildTree(elements.ToArray()));

            ICollection<Element> AddSpacing(ICollection<Element> elements)
            {
                if (Spacing < Size.Epsilon)
                    return elements;
                
                return elements
                    .Select(x => new Padding
                    {
                        Bottom = Spacing,
                        Child = x
                    })
                    .Cast<Element>()
                    .ToList();
            }

            Element BuildTree(Span<Element> elements)
            {
                if (elements.IsEmpty)
                    return Empty.Instance;

                if (elements.Length == 1)
                    return elements[0];

                var half = elements.Length / 2;
                
                return new SimpleStack
                {
                    Current = BuildTree(elements.Slice(0, half)),
                    Next = BuildTree(elements.Slice(half))
                };
            }
        }
    }
}