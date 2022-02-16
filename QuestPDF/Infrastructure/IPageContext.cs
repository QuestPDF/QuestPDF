using System.Collections.Generic;

namespace QuestPDF.Infrastructure
{
    internal class DocumentLocation
    {
        public string Name { get; set; }
        public int PageStart { get; set; }
        public int PageEnd { get; set; }
        public int Length => PageEnd - PageStart + 1;
    }
    
    internal interface IPageContext
    {
        int CurrentPage { get; }
        void SetLocationPage(string name);
        DocumentLocation? GetLocation(string name);
    }
}