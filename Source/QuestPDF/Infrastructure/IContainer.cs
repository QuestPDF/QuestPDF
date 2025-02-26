using QuestPDF.Elements;

namespace QuestPDF.Infrastructure
{
    /// <summary>
    /// Represents a layout structure with exactly one child element.
    /// </summary>
    /// <remarks>
    /// The main purpose of this interface is to facilitate the Fluent API's construction.
    /// It's not intended to allow external creation of new container kinds or layout designs.
    /// </remarks>
    public interface IContainer
    {
        IElement? Child { get; set; }
        
        #if NETCOREAPP3_0_OR_GREATER
        public static IContainer Empty => new Container();
        #endif
    }
}