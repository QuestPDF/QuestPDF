using System;
using QuestPDF.Elements;
using QuestPDF.Helpers;

namespace QuestPDF.Infrastructure
{
    internal class DynamicComponentProxy
    {
        internal Action<object> SetState { get; private set; }
        internal Func<object> GetState { get; private set; }
        internal Action<DynamicContext, IDynamicContainer> Compose { get; private set; }
        
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
    
    public interface IDynamicComponent<TState> where TState : struct
    {
        TState State { get; set; }
        void Compose(DynamicContext context, IDynamicContainer dynamicContainer);
    }
}