using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal ref struct DecorationElementLayout
    {
        public ItemCommand Before { get; set; }
        public ItemCommand Content { get; set; }
        public ItemCommand After { get; set; }
        
        public struct ItemCommand
        {
            public Element Element;
            public SpacePlan Measurement;
            public Position Offset;
        }
        
        public float TotalWidth => Math.Max(Before.Measurement.Width, Math.Max(Content.Measurement.Width, After.Measurement.Width));
        public float TotalHeight => Before.Measurement.Height + Content.Measurement.Height + After.Measurement.Height;
    }

    internal sealed class Decoration : Element, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        internal Element Before { get; set; } = new DebugPointer(DebugPointerType.ElementStructure, "Before");
        internal Element Content { get; set; } = new DebugPointer(DebugPointerType.ElementStructure, "Content");
        internal Element After { get; set; } = new DebugPointer(DebugPointerType.ElementStructure, "After");

        internal override IReadOnlyList<Element?> GetChildren()
        {
            return [Before, Content, After];
        }
        
        internal override void CreateProxy(Func<Element?, Element?> create)
        {
            Before = create(Before);
            Content = create(Content);
            After = create(After);
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            var layout = PlanLayout(availableSpace);

            if (layout.Content.Measurement.Type == SpacePlanType.Empty)
                return SpacePlan.Empty();
            
            if (layout.Content.Measurement.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap("The primary content does not fit on the page.");

            if (layout.Before.Measurement.Type == SpacePlanType.Wrap)
                return layout.Before.Measurement;
            
            if (layout.After.Measurement.Type == SpacePlanType.Wrap)
                return layout.After.Measurement;
            
            var size = new Size(layout.TotalWidth, layout.TotalHeight);
            
            if (size.Width > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap("The content slot requires more horizontal space than available.");
            
            if (size.Height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap("The content slot requires more vertical space than available.");
            
            var willBeFullyRendered = layout.Content.Measurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender;

            return willBeFullyRendered
                ? SpacePlan.FullRender(size)
                : SpacePlan.PartialRender(size);
        }

        internal override void Draw(Size availableSpace)
        {
            var layout = PlanLayout(availableSpace);
            var contentWidth = layout.TotalWidth;

            DrawCommand(layout.Before);
            DrawCommand(layout.Content);
            DrawCommand(layout.After);
            
            void DrawCommand(DecorationElementLayout.ItemCommand command)
            {
                var elementSize = new Size(contentWidth, command.Measurement.Height);
                
                var offset = ContentDirection == ContentDirection.LeftToRight
                    ? command.Offset
                    : new Position(availableSpace.Width - contentWidth, command.Offset.Y);
                
                Canvas.Translate(offset);
                command.Element.Draw(elementSize);
                Canvas.Translate(offset.Reverse());
            }
        }

        private DecorationElementLayout PlanLayout(Size availableSpace)
        {
            SpacePlan GetDecorationMeasurement(Element element)
            {
                var measurement = element.Measure(availableSpace);

                if (measurement.Type is SpacePlanType.PartialRender or SpacePlanType.Wrap)
                    return SpacePlan.Wrap("Decoration slot (before or after) does not fit fully on the page.");

                return measurement;
            }
            
            var beforeMeasurement = GetDecorationMeasurement(Before);
            var afterMeasurement = GetDecorationMeasurement(After);
            
            var contentSpace = new Size(availableSpace.Width, availableSpace.Height - beforeMeasurement.Height - afterMeasurement.Height);
            var contentMeasurement = Content.Measure(contentSpace);

            return new DecorationElementLayout
            {
                Before = new DecorationElementLayout.ItemCommand
                {
                    Element = Before,
                    Measurement = beforeMeasurement,
                    Offset = Position.Zero
                },
                Content = new DecorationElementLayout.ItemCommand
                {
                    Element = Content,
                    Measurement = contentMeasurement,
                    Offset = new Position(0, beforeMeasurement.Height)
                },
                After = new DecorationElementLayout.ItemCommand
                {
                    Element = After,
                    Measurement = afterMeasurement,
                    Offset = new Position(0, beforeMeasurement.Height + contentMeasurement.Height)
                },
            };
        }
    }
}