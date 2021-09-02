using System;
using System.Collections.Generic;

namespace QuestPDF.Infrastructure
{
    public class PageContext : IPageContext
    {
        public const string CurrentPageSlot = "currentPage";
        public const string TotalPagesSlot = "totalPages";
        
        private Dictionary<string, int> Locations { get; } = new Dictionary<string, int>();
        private int PageNumber { get; set; }

        internal void SetPageNumber(int number)
        {
            PageNumber = number;
            Locations[CurrentPageSlot] = number;

            if (!Locations.ContainsKey(TotalPagesSlot) || Locations[TotalPagesSlot] < number)
                Locations[TotalPagesSlot] = number;
        }
        
        public void SetLocationPage(string key)
        {
            if (Locations.ContainsKey(key))
                return;
            
            Locations[key] = PageNumber;
        }

        public int GetLocationPage(string key)
        {
            if (!Locations.ContainsKey(key))
                throw new ArgumentException($"The location '{key}' does not exists.");
            
            return Locations[key];
        }

        public ICollection<string> GetRegisteredLocations()
        {
            return Locations.Keys;
        }
    }
}