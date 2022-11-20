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
                    ElementProperties = GetElementConfiguration(item.Element).ToList(),
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

            static IEnumerable<DocumentElementProperty> GetElementConfiguration(IElement element)
            {
                if (element is DebugPointer)
                    return Enumerable.Empty<DocumentElementProperty>();
                
                return element
                    .GetType()
                    .GetProperties()
                    .Select(x => new
                    {
                        Property = x.Name.PrettifyName(),
                        Value = x.GetValue(element)
                    })
                    .Where(x => !(x.Value is IElement))
                    .Where(x => x.Value is string || !(x.Value is IEnumerable))
                    .Where(x => !(x.Value is TextStyle))
                    .Select(x => new DocumentElementProperty
                    {
                        Label = x.Property,
                        Value = FormatValue(x.Value)
                    });

                string FormatValue(object value)
                {
                    const int maxLength = 100;
                    
                    var text = value?.ToString() ?? "-";

                    if (text.Length < maxLength)
                        return text;

                    return text.AsSpan(0, maxLength).ToString() + "...";
                }
            }
        }
    }
}