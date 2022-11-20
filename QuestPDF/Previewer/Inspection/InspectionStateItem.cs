using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer.Inspection
{
    internal class InspectionStateItem
    {
        public Element Element { get; internal set; }
        public Infrastructure.Size Size { get; internal set; }
        public Position Position { get; internal set; }
    }

    internal class MyPosition
    {
        public float Top { get; set; }
        public float Left { get; set; }
    }
    
    internal class InspectionElementLocation
    {
        public int PageNumber { get; internal set; }
        public MyPosition Position { get; internal set; }
        public Size Size { get; internal set; }
    }
    
    internal class InspectionElement
    {
        public string ElementType { get; internal set; }
        public IReadOnlyCollection<InspectionElementLocation> Location { get; internal set; }
        public IReadOnlyCollection<DocumentElementProperty> Properties { get; internal set; }
        public ICollection<InspectionElement> Children { get; set; }
    }
}