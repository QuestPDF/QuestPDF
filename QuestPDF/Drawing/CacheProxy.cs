using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    internal class ElementProxy : ContainerElement
    {
        
    }
    
    internal class CacheProxy : ElementProxy
    {
        public Size? AvailableSpace { get; set; }
        public SpacePlan? MeasurementResult { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (MeasurementResult != null &&
                AvailableSpace != null &&
                IsClose(AvailableSpace.Value.Width, availableSpace.Width) &&
                IsClose(AvailableSpace.Value.Height, availableSpace.Height))
            {
                return MeasurementResult.Value;
            }

            AvailableSpace = availableSpace;
            MeasurementResult = base.Measure(availableSpace);

            return MeasurementResult.Value;
        }

        internal override void Draw(Size availableSpace)
        { 
            AvailableSpace = null;
            MeasurementResult = null;
            
            base.Draw(availableSpace);
        }

        private bool IsClose(float x, float y)
        {
            return Math.Abs(x - y) < Size.Epsilon;
        }
    }

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
                var spaceIndent = new string(' ', nestingLevel * 4);
                var wrapIndent = new string('>', nestingLevel * 4);

                var firstIndent = item.SpacePlan.Type == SpacePlanType.Wrap ? wrapIndent : spaceIndent;
                
                builder.AppendLine(firstIndent + item.Element);
                builder.AppendLine(spaceIndent + "Available space: " + item.AvailableSpace);
                builder.AppendLine(spaceIndent + "Take space: " + item.SpacePlan);
                
                nestingLevel++;
                item.Stack.ToList().ForEach(Traverse);
                nestingLevel--;
            }
        }
    }
    
    public class DebugStackItem
    {
        public IElement Element { get; internal set; }
        public Size AvailableSpace { get; internal set; }
        public SpacePlan SpacePlan { get; internal set; }

        public ICollection<DebugStackItem> Stack { get; internal set; } = new List<DebugStackItem>();
    }

    internal class DebuggingProxy : ElementProxy
    {
        private DebuggingState DebuggingState { get; }

        public DebuggingProxy(DebuggingState debuggingState)
        {
            DebuggingState = debuggingState;
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            DebuggingState.RegisterMeasure(Child, availableSpace);
            var spacePlan = base.Measure(availableSpace);
            DebuggingState.RegisterMeasureResult(Child, spacePlan);

            return spacePlan;
        }
    }
}