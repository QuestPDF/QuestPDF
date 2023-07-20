using System.Collections.Generic;

namespace QuestPDF.Infrastructure
{
    internal class DocumentLocation
    {
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public int PageStart { get; set; }
        public int PageEnd { get; set; }
        public int Length => PageEnd - PageStart + 1;
    }
    
    internal interface IPageContext
    {
        int DocumentLength { get; }
        int CurrentPage { get; }
        void SetSectionPage(string name);
        DocumentLocation? GetLocation(string name);
        string GetDocumentLocationName(string locationName);
    }
}