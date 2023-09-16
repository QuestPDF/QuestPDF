using QuestPDF.Elements;

namespace QuestPDF.Infrastructure
{
    interface ISlot
    {
        
    }

    class Slot : Container, ISlot
    {
        
    }
    
    /// <summary>
    /// <para>This interface represents a reusable document fragment.</para>
    /// <para>
    /// Components serve as modular building blocks for abstracting document layouts. 
    /// They promote code reusability across multiple sections or types of documents. 
    /// Using a component, you can generate content based on its internal state.
    /// </para>
    /// </summary>
    /// <example>
    /// Consider the scenario of a company-wide page header.
    /// Instead of replicating the same header design across various documents, a single component can be created and referenced wherever needed.
    /// </example>
    public interface IComponent
    {
        /// <summary>
        /// Method invoked by the library to compose document content. 
        /// </summary>
        void Compose(IContainer container);
    }
}