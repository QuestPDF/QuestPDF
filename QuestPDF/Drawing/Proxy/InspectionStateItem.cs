using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy
{
    internal class InspectionStateItem
    {
        public Element Element { get; internal set; }
        public Size Size { get; internal set; }
        public Position Position { get; internal set; }
    }

    internal class InspectionElementLocation
    {
        public int PageNumber { get; internal set; }
        public float Top { get; set; }
        public float Left { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
    
    internal class InspectionElement
    {
        public string Element { get; internal set; }
        public ICollection<InspectionElementLocation> Location { get; internal set; }
        public Dictionary<string, string> Properties { get; internal set; }
        public ICollection<InspectionElement> Children { get; set; }
    }
}