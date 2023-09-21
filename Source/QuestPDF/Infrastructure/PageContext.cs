using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestPDF.Infrastructure
{
    internal class PageContext : IPageContext
    {
        public int CurrentDocumentId { get; private set; }
        public int DocumentLength { get; private set; }
        public int CurrentPage { get; private set; }
        private List<DocumentLocation> Locations { get; } = new();

        internal void SetDocumentId(int id)
        {
            CurrentDocumentId = id;
        }
        
        internal void ResetPageNumber()
        {
            CurrentPage = 0;
        }
        
        internal void IncrementPageNumber()
        {
            CurrentPage++;
            DocumentLength = Math.Max(DocumentLength, CurrentPage);
        }
        
        public void SetSectionPage(string name)
        {
            var location = GetLocation(name);

            if (location == null)
            {
                location = new DocumentLocation
                {
                    DocumentId = CurrentDocumentId,
                    Name = name,
                    PageStart = CurrentPage,
                    PageEnd = CurrentPage
                };
                
                Locations.Add(location);
            }

            if (location.PageEnd < CurrentPage)
                location.PageEnd = CurrentPage;
        }

        public DocumentLocation? GetLocation(string name)
        {
            return Locations.Find(x => x.DocumentId == CurrentDocumentId && x.Name == name);
        }
        
        public string GetDocumentLocationName(string locationName)
        {
            return $"{CurrentDocumentId} | {locationName}";
        }
    }
}