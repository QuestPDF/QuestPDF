using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    static internal class PreviewerCommands
    {
        internal class UpdateDocumentStructure
        {
            public bool DocumentContentHasLayoutOverflowIssues { get; set; }
            public ICollection<PageSize> Pages { get; set; }
            public DocumentHierarchyElement Hierarchy { get; set; }
    
            public class PageSize
            {
                public float Width { get; set; }
                public float Height { get; set; }
            }
            
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
        
        internal class ProvideRenderedDocumentPage
        {
            public ICollection<RenderedPage> Pages { get; set; }

            internal class RenderedPage
            {
                public int PageIndex { get; set; }
                public int ZoomLevel { get; set; }
                public string ImageData { get; set; } // base64
            }
        }
        
        internal class ShowGenericException
        {
            public GenericExceptionDetails Exception { get; set; }
            
            internal class GenericExceptionDetails
            {
                public string Type { get; set; }
                public string Message { get; set; }
                public string? StackTrace { get; set; }
                public GenericExceptionDetails? InnerException { get; set; }
            }
        }

        internal class ShowLayoutError
        {
            
        }
    }

    static internal class PreviewerResponses
    {
        internal class GetApiVersion
        {
            public int Version { get; set; }
        }
    }
}
