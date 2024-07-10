using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    internal class DocumentHierarchyElement
    {
        public string ElementType { get; set; }
        public bool IsSingleChildContainer { get; set; }
        public List<PageLocation> PageLocations { get; set; } = new();
        public List<ElementProperty> Properties { get; set; } = new();
        public List<DocumentHierarchyElement> Children { get; set; } = new();
    }

    internal class PageLocation
    {
        public int PageNumber { get; init; }
        public float Left { get; init; }
        public float Top { get; init; }
        public float Right { get; init; }
        public float Bottom { get; init; }
    }
    
    internal class ElementProperty
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}
