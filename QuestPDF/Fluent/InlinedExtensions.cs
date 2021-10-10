using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class InlinedDescriptor
    {
        private ICollection<Element> Children = new List<Element>();
        private float VerticalSpacingValue { get; set; }
        private VerticalAlignment BaselineAlignmentValue { get; set; }
        private float HorizontalSpacingValue { get; set; }
        private HorizontalAlignment HorizontalAlignmentValue { get; set; }
        
        public void Spacing(float value)
        {
            VerticalSpacing(value);
            HorizontalSpacing(value);
        }
        
        public void VerticalSpacing(float value) => VerticalSpacingValue = value;
        public void HorizontalSpacing(float value) => HorizontalSpacingValue = value;

        public void BaselineTop() => BaselineAlignmentValue = VerticalAlignment.Top;
        public void BaselineMiddle() => BaselineAlignmentValue = VerticalAlignment.Middle;
        public void BaselineBottom() => BaselineAlignmentValue = VerticalAlignment.Bottom;

        public void AlignLeft() => HorizontalAlignmentValue = HorizontalAlignment.Left;
        public void AlignCenter() => HorizontalAlignmentValue = HorizontalAlignment.Center;
        public void AlignRight() => HorizontalAlignmentValue = HorizontalAlignment.Right;
        
        public IContainer Item()
        {
            var container = new Container();
            Children.Add(container);
            return container;
        }

        internal Element Compose()
        {
            var elements = Children
                .Select(x => new InlinedElement
                {
                    Child = new Padding
                    {
                        Left = HorizontalSpacingValue,
                        Top = VerticalSpacingValue,
                        Child = x
                    }
                })
                .ToList();
            
            return new Padding
            {
                Left = -HorizontalSpacingValue,
                Top = -VerticalSpacingValue,
                
                Child = new Inlined
                {
                    Elements = elements,
                    
                    HorizontalAlignment = HorizontalAlignmentValue,
                    BaselineAlignment = BaselineAlignmentValue
                }
            };
        }
    }
    
    public static class InlinedExtensions
    {
        public static void Inlined(this IContainer element, Action<InlinedDescriptor> handler)
        {
            var descriptor = new InlinedDescriptor();
            handler(descriptor);
            
            element.Element(descriptor.Compose());
        }
    }
}