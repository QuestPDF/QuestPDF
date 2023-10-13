using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer.LayoutInspection;

internal sealed class DocumentInspectionElement
{
    public string ElementType { get; set; }
    public bool IsSingleChildContainer { get; set; }
    public bool IsContainer { get; set; }
    public IReadOnlyCollection<PageLocation> Location { get; set; }
    public IReadOnlyCollection<ElementProperty> Properties { get; set; }
    public ICollection<DocumentInspectionElement> Children { get; set; }
    
    internal class ElementProperty
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
    
    internal class PageLocation
    {
        public int PageNumber { get; set; }
        public ElementPosition Position { get; set; }
        public ElementSize Size { get; set; }
    }

    internal class ElementPosition
    {
        public float Left { get; set; }
        public float Top { get; set; }
    }
    
    internal class ElementSize
    {
        public float Width { get; set; }
        public float Height { get; set; }
    }
}