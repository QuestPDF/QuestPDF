using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
        
        public string BuildTrace()
        {
            var builder = new StringBuilder();
            var nestingLevel = 0;

            Traverse(Root);
            return builder.ToString();

            void Traverse(DebugStackItem item)
            {
                var indent = new string(' ', nestingLevel * 4);
                var title = item.Element.GetType().Name;

                if (item.Element is DebugPointer debugPointer)
                {
                    title = debugPointer.Target;
                    title += debugPointer.Highlight ? " 🌟" : string.Empty;
                }
                
                if (item.SpacePlan.Type == SpacePlanType.Wrap)
                    title = "🔥 " + title;

                builder.AppendLine(indent + title);
                builder.AppendLine(indent + new string('-', title.Length));
                
                builder.AppendLine(indent + "Available space: " + item.AvailableSpace);
                builder.AppendLine(indent + "Requested space: " + item.SpacePlan);
                
                foreach (var configuration in GetElementConfiguration(item.Element))
                    builder.AppendLine(indent + configuration);

                builder.AppendLine();
                
                nestingLevel++;
                item.Stack.ToList().ForEach(Traverse);
                nestingLevel--;
            }

            static IEnumerable<string> GetElementConfiguration(IElement element)
            {
                if (element is DebugPointer)
                    return Enumerable.Empty<string>();
                
                return element
                    .GetType()
                    .GetProperties()
                    .Select(x => new
                    {
                        Property = x.Name.PrettifyName(),
                        Value = x.GetValue(element)
                    })
                    .Where(x => !(x.Value is IElement))
                    .Where(x => !(x.Value is IEnumerable))
                    .Where(x => !(x.Value is TextStyle))
                    .Select(x => $"{x.Property}: {FormatValue(x.Value)}");

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