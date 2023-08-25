namespace QuestPDF.Infrastructure
{
    /// <summary>
    /// Represents the document abstraction, including its content, metadata, and configuration settings.
    /// <a href="https://www.questpdf.com/getting-started.html">Learn more</a>
    /// </summary>
    /// <remarks>
    /// <para>Implement this interface to centralize your entire document's structure within a single class for easy management.</para>
    /// <para>For a different approach, consider the <a href="https://www.questpdf.com/quick-start.html">Minimal API</a> pathway.</para>
    /// </remarks>
    public interface IDocument
    {
#if NETCOREAPP3_0_OR_GREATER
        /// <summary>
        /// Provides metadata values like author and keywords used in PDF creation.
        /// </summary>
        /// <remarks>
        /// Override this method to customize document's metadata.
        /// </remarks>
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default; 

        /// <summary>
        /// Provides document generation settings, such as default image DPI and compression rate.
        /// </summary>
        /// <remarks>
        /// Override this to customize default configurations.
        /// </remarks>
        public DocumentSettings GetSettings() => DocumentSettings.Default;
#else
        DocumentMetadata GetMetadata();
        DocumentSettings GetSettings();
#endif
        
        /// <summary>
        /// Configures the document content by specifying its layout structure and visual element.
        /// </summary>
        /// <param name="container">The document container used for defining content via the FluentAPI.</param>
        void Compose(IDocumentContainer container);
    }
}