using System.Collections.Generic;

namespace QuestPDF.Infrastructure
{
    internal sealed class DocumentLocation
    {
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public int PageStart { get; set; }
        public int PageEnd { get; set; }
        public int Length => PageEnd - PageStart + 1;
    }
    
    internal interface IPageContext
    {
        bool IsInitialRenderingPhase { get; }
        int DocumentLength { get; }
        int CurrentPage { get; }
        void SetSectionPage(string name);
        DocumentLocation? GetLocation(string name);
        string GetDocumentLocationName(string locationName);

        void CaptureContentPosition(PageElementLocation location);
        ICollection<PageElementLocation> GetContentPosition(string id);
    }

    public class PageElementLocation
    {
        public string Id { get; set; }
        
        public int PageNumber { get; set; }
        
        public float Width { get; set; }
        public float Height { get; set; }
        
        public float X { get; set; }
        public float Y { get; set; }
    }
}