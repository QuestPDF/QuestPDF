using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class DecorationLastPageState
    {
        public bool IsLastPage { get; set; }
    }

    internal sealed class DecorationElementLayout
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
    }

    internal sealed class Decoration : Element, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        internal Element Before { get; set; } = new DebugPointer(DebugPointerType.ElementStructure, "Before");
        internal Element Content { get; set; } = new DebugPointer(DebugPointerType.ElementStructure, "Content");
        internal Element After { get; set; } = new DebugPointer(DebugPointerType.ElementStructure, "After");

        private DecorationLastPageState? LastPageState { get; set; }
        private bool HasLastPageElements { get; set; }
        private bool LastPageStateWired { get; set; }

        internal override IEnumerable<Element?> GetChildren()
        {
            yield return Before;
            yield return Content;
            yield return After;
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
            
            var itemMeasurements = new[]
            {
                layout.Before.Measurement,
                layout.Content.Measurement,
                layout.After.Measurement
            };
            
            var width = itemMeasurements.Max(x => x.Width);
            var height = itemMeasurements.Sum(x => x.Height);
            var size = new Size(width, height);
            
            if (width > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap("The content slot requires more horizontal space than available.");
            
            if (height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap("The content slot requires more vertical space than available.");
            
            var willBeFullyRendered = itemMeasurements.All(x => x.Type is SpacePlanType.Empty or SpacePlanType.FullRender);

            return willBeFullyRendered
                ? SpacePlan.FullRender(size)
                : SpacePlan.PartialRender(size);
        }

        internal override void Draw(Size availableSpace)
        {
            var layout = PlanLayout(availableSpace);
            
            var drawingCommands = new[]
            {
                layout.Before,
                layout.Content,
                layout.After
            };
            
            var width = drawingCommands.Max(x => x.Measurement.Width);
            
            foreach (var command in drawingCommands)
            {
                var elementSize = new Size(width, command.Measurement.Height);
                
                var offset = ContentDirection == ContentDirection.LeftToRight
                    ? command.Offset
                    : new Position(availableSpace.Width - width, command.Offset.Y);
                
                Canvas.Translate(offset);
                command.Element.Draw(elementSize);
                Canvas.Translate(offset.Reverse());
            }
        }

        private DecorationElementLayout PlanLayout(Size availableSpace)
        {
            EnsureLastPageStateWired();

            SpacePlan GetDecorationMeasurement(Element element)
            {
                var measurement = element.Measure(availableSpace);

                if (measurement.Type is SpacePlanType.PartialRender or SpacePlanType.Wrap)
                    return SpacePlan.Wrap("Decoration slot (before or after) does not fit fully on the page.");

                return measurement;
            }
            
            // Phase 1: Measure assuming this is NOT the last page
            if (LastPageState != null)
                LastPageState.IsLastPage = false;

            var beforeMeasurement = GetDecorationMeasurement(Before);
            var afterMeasurement = GetDecorationMeasurement(After);
            
            var contentSpace = new Size(availableSpace.Width, availableSpace.Height - beforeMeasurement.Height - afterMeasurement.Height);
            var contentMeasurement = Content.Measure(contentSpace);

            // Phase 2: If Content fits completely (candidate last page) and we have
            // SkipLast/ShowLast elements, re-evaluate with last-page decorations
            if (HasLastPageElements && contentMeasurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
            {
                LastPageState!.IsLastPage = true;

                var lastBeforeMeasurement = GetDecorationMeasurement(Before);
                var lastAfterMeasurement = GetDecorationMeasurement(After);

                // If last-page decorations don't fit, revert to not-last-page behavior
                if (lastBeforeMeasurement.Type == SpacePlanType.Wrap || lastAfterMeasurement.Type == SpacePlanType.Wrap)
                {
                    LastPageState.IsLastPage = false;
                }
                else
                {
                    var lastContentSpace = new Size(availableSpace.Width, availableSpace.Height - lastBeforeMeasurement.Height - lastAfterMeasurement.Height);
                    var lastContentMeasurement = Content.Measure(lastContentSpace);

                    if (lastContentMeasurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                    {
                        // Converged: still the last page with updated decorations
                        beforeMeasurement = lastBeforeMeasurement;
                        afterMeasurement = lastAfterMeasurement;
                        contentMeasurement = lastContentMeasurement;
                    }
                    else
                    {
                        // Oscillation: last-page decorations changed available space enough
                        // that Content no longer fits. Revert to not-last-page behavior.
                        LastPageState.IsLastPage = false;
                    }
                }
            }

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

        private void EnsureLastPageStateWired()
        {
            if (LastPageStateWired)
                return;

            LastPageStateWired = true;
            LastPageState = new DecorationLastPageState();
            HasLastPageElements = false;

            WireSubtree(Before);
            WireSubtree(After);

            if (!HasLastPageElements)
                LastPageState = null;
        }

        private void WireSubtree(Element? root)
        {
            Traverse(root);

            void Traverse(Element? element)
            {
                if (element == null || element is Decoration)
                    return;

                switch (element)
                {
                    case SkipLast skipLast:
                        skipLast.PageState = LastPageState;
                        HasLastPageElements = true;
                        break;
                    case ShowLast showLast:
                        showLast.PageState = LastPageState;
                        HasLastPageElements = true;
                        break;
                }

                if (element is ContainerElement containerElement)
                {
                    Traverse(containerElement.Child);
                }
                else
                {
                    foreach (var child in element.GetChildren())
                        Traverse(child);
                }
            }
        }
    }
}
