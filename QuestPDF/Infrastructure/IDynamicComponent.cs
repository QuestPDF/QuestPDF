using System;
using QuestPDF.Elements;
using QuestPDF.Helpers;

namespace QuestPDF.Infrastructure
{
    internal class DynamicComponentProxy
    {
        internal Action<object> SetState { get; private set; }
        internal Func<object> GetState { get; private set; }
        internal Func<DynamicContext, DynamicComponentComposeResult> Compose { get; private set; }
        
        internal static DynamicComponentProxy CreateFrom<TState>(IDynamicComponent<TState> component) where TState : struct
        {
            return new DynamicComponentProxy
            {
                GetState = () => component.State,
                SetState = x => component.State = (TState)x,
                Compose = component.Compose
            };
        }
    }

    public class DynamicComponentComposeResult
    {
        public IElement Content { get; set; }
        public bool HasMoreContent { get; set; }
    }
    
    public interface IDynamicComponent<TState> where TState : struct
    {
        TState State { get; set; }
        DynamicComponentComposeResult Compose(DynamicContext context);
    }
}