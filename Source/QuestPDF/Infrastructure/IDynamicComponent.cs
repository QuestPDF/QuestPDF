using System;
using QuestPDF.Elements;

namespace QuestPDF.Infrastructure
{
    internal sealed class DynamicComponentProxy
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

    /// <summary>
    /// Represents the output from the DynamicComponent describing what should be rendered on the current page.
    /// </summary>
    public class DynamicComponentComposeResult
    {
        /// <summary>
        /// Any content created with the <see cref="DynamicContext.CreateElement" /> method that should be drawn on the currently rendered page.
        /// </summary>
        public IElement Content { get; set; }
        
        /// <summary>
        /// Set to true if the dynamic component has additional content for the next page.
        /// Set to false if all content from the dynamic component has been rendered.
        /// </summary>
        public bool HasMoreContent { get; set; }
    }
    
    /// <summary>
    /// Represents a section of the document dynamically created based on its inner state.
    /// Components are page-aware, understand their positioning, can dynamically construct other content elements, and assess their dimensions, enabling complex layout creations.
    /// </summary>
    /// <remarks>
    /// Though dynamic components offer great flexibility, be cautious of potential performance impacts.
    /// </remarks>
    /// <typeparam name="TState">Structure type representing the internal state of the component.</typeparam>
    public interface IDynamicComponent<TState> where TState : struct
    {
        /// <summary>
        /// Represents the component's current state.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The state should remain read-only.
        /// Avoid direct state modifications.
        /// For any alterations, generate a new struct instance and reassign the State property.
        /// </para>
        /// <para>Remember, the QuestPDF library can invoke the Compose method more than once for each page and might adjust the state internally.</para>
        /// </remarks>
        TState State { get; set; }
        
        /// <summary>
        /// Method invoked by the library to plan and create new content for each page. 
        /// </summary>
        /// <remarks>
        /// Remember, the QuestPDF library can invoke the Compose method more than once for each page and might adjust the state internally.
        /// </remarks>
        /// <param name="context">Context offering additional information (like current page number, entire document size) and the capability to produce dynamic content elements.</param>
        /// <returns>Representation of content that should be placed on the current page.</returns>
        DynamicComponentComposeResult Compose(DynamicContext context);
    }
}