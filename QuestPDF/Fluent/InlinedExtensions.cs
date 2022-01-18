using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class InlinedDescriptor
    {
        internal Inlined Inlined { get; } = new Inlined();
        
        public void Spacing(float value, Unit unit = Unit.Point)
        {
            VerticalSpacing(value, unit);
            HorizontalSpacing(value, unit);
        }
        
        public void VerticalSpacing(float value, Unit unit = Unit.Point)
        {
            Inlined.VerticalSpacing = value.ToPoints(unit);
        }

        public void HorizontalSpacing(float value, Unit unit = Unit.Point)
        {
            Inlined.HorizontalSpacing = value.ToPoints(unit);
        }

        public void BaselineTop() => Inlined.BaselineAlignment = VerticalAlignment.Top;
        public void BaselineMiddle() => Inlined.BaselineAlignment = VerticalAlignment.Middle;
        public void BaselineBottom() => Inlined.BaselineAlignment = VerticalAlignment.Bottom;

        public void AlignLeft() => Inlined.ElementsAlignment = InlinedAlignment.Left;
        public void AlignCenter() => Inlined.ElementsAlignment = InlinedAlignment.Center;
        public void AlignRight() => Inlined.ElementsAlignment = InlinedAlignment.Right;
        public void AlignJustify() => Inlined.ElementsAlignment = InlinedAlignment.Justify;
        public void AlignSpaceAround() => Inlined.ElementsAlignment = InlinedAlignment.SpaceAround;
        
        public IContainer Item()
        {
            var container = new InlinedElement();
            Inlined.Elements.Add(container);
            return container;
        }
    }
    
    public static class InlinedExtensions
    {
        public static void Inlined(this IContainer element, Action<InlinedDescriptor> handler)
        {
            var descriptor = new InlinedDescriptor();
            handler(descriptor);
            
            element.Element(descriptor.Inlined);
        }
    }
}