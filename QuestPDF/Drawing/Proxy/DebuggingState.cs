using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Drawing.Proxy
{
    internal class DebuggingState
    {
        private DebugStackItem Root { get; set; }
        private Stack<DebugStackItem> Stack { get; set; }

        public DebuggingState()
        {
            Reset();
        }
        
        public void Reset()
        {
            Root = null;
            Stack = new Stack<DebugStackItem>();
        }
        
        public void RegisterMeasure(IElement element, Size availableSpace)
        {
            if (element.GetType() == typeof(Container))
                return;
            
            var item = new DebugStackItem
            {
                Element = element,
                AvailableSpace = availableSpace
            };

            Root ??= item;
            
            if (Stack.Any())
                Stack.Peek().Stack.Add(item);

            Stack.Push(item);
        }

        public void RegisterMeasureResult(IElement element, SpacePlan spacePlan)
        {
            if (element.GetType() == typeof(Container))
                return;
            
            var item = Stack.Pop();

            if (item.Element != element)
                throw new Exception();
            
            item.SpacePlan = spacePlan;
        }
        
        public LayoutRenderingTrace BuildTrace()
        {
            return Traverse(Root);

            LayoutRenderingTrace Traverse(DebugStackItem item)
            {
                return  new LayoutRenderingTrace
                {
                    ElementType = item.Element.GetType().Name,
                    IsSingleChildContainer = item.Element is ContainerElement,
                    ElementProperties = item.Element.GetElementConfiguration().ToList(),
                    AvailableSpace = new QuestPDF.Previewer.Size()
                    {
                        Width = item.AvailableSpace.Width,
                        Height = item.AvailableSpace.Height
                    },
                    SpacePlan = new QuestPDF.Previewer.SpacePlan()
                    {
                        Width = item.SpacePlan.Width,
                        Height = item.SpacePlan.Height,
                        Type = item.SpacePlan.Type
                    },
                    Children = item.Stack.Select(Traverse).ToList()
                };
            }
        }
    }
}