using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestPDF.Infrastructure
{
    internal class PageContext : IPageContext
    {
        public const string DocumentLocation = "document";

        private List<DocumentLocation> Locations { get; } = new();
        public int CurrentPage { get; private set; }

        internal void SetPageNumber(int number)
        {
            CurrentPage = number;
            SetSectionPage(DocumentLocation);
        }

        public void SetSectionPage(string name)
        {
            var location = GetLocation(name);

            if (location == null)
            {
                location = new DocumentLocation
                {
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
            return Locations.FirstOrDefault(x => x.Name == name);
        }

        public int? BeginPageNumberOfSection(string locationName)
        {
            return GetLocation(locationName)?.PageStart;
        }

        public int? EndPageNumberOfSection(string locationName)
        {
            return GetLocation(locationName)?.PageEnd;
        }

        public int? PageNumberWithinSection(string locationName)
        {
            return CurrentPage + 1 - GetLocation(locationName)?.PageStart;
        }

        public int? TotalPagesWithinSection(string locationName)
        {
            return GetLocation(locationName)?.Length;
        }
    }
}