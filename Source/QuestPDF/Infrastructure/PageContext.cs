using System;
using System.Collections.Generic;

namespace QuestPDF.Infrastructure
{
    internal sealed class PageContext : IPageContext
    {
        public bool IsInitialRenderingPhase { get; private set; } = true;
        public int DocumentLength { get; private set; }
        private List<DocumentLocation> Locations { get; } = new();
        
        public int CurrentDocumentId { get; private set; }
        public int CurrentPage { get; private set; }

        internal void SetDocumentId(int id)
        {
            CurrentDocumentId = id;
        }
        
        internal void ProceedToNextRenderingPhase()
        {
            IsInitialRenderingPhase = false;
            CurrentPage = 0;
        }
        
        internal void DecrementPageNumber()
        {
            CurrentPage--;
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