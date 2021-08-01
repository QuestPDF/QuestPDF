using System.Collections.Generic;

namespace QuestPDF.Infrastructure
{
    public interface IPageContext
    {
        void SetLocationPage(string key);
        int GetLocationPage(string key);
        ICollection<string> GetRegisteredLocations();
    }
}