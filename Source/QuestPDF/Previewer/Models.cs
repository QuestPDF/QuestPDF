using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    internal class DocumentHierarchyElement
    {
        public string ElementType { get; set; }
        public bool IsSingleChildContainer { get; set; }
        public List<PageLocation> Location { get; set; } = new();
        public List<ElementProperty> Properties { get; set; } = new();
        public List<DocumentHierarchyElement> Children { get; set; } = new();
    }

    internal class PageLocation
    {
        public int PageNumber { get; set; }
        public PagePosition Position { get; set; }
        public PageElementPosition Size { get; set; }
    }

    internal class PagePosition
    {
        public float Left { get; set; }
        public float Top { get; set; }
    }

    internal class PageElementPosition
    {
        public float Width { get; set; }
        public float Height { get; set; }
    }

    
    internal class ElementProperty
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}
