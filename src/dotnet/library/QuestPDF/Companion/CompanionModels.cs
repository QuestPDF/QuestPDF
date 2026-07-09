using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Companion
{
    sealed class PageSnapshotIndex
    {
        public int PageIndex { get; set; }
        public int ZoomLevel { get; set; }

        public override string ToString() => $"{ZoomLevel}/{PageIndex}";
    }
    
    static internal class CompanionCommands
    {
        internal sealed class Notify
        {
            private static readonly string CurrentClientId = Guid.NewGuid().ToString();
            
            public string ClientId => CurrentClientId;
            public LicenseType License => Settings.License ?? LicenseType.Community;
        }
        
        internal sealed class UpdateDocumentStructure
        {
            public bool IsDocumentHotReloaded { get; set; }
            public ICollection<PageSize> Pages { get; set; }
            public DocumentHierarchyElement Hierarchy { get; set; }
    
            public sealed class PageSize
            {
                public float Width { get; set; }
                public float Height { get; set; }
            }
            
            internal sealed record DocumentHierarchyElement
            {
                internal Element Element { get; set; }
                
                public string ElementType { get; set; }
                public string? Hint { get; set; }
                public string? SearchableContent { get; set; }
                public bool IsSingleChildContainer { get; set; }
                public ICollection<PageLocation> PageLocations { get; set; }
                public ICollection<LayoutErrorMeasurement> LayoutErrorMeasurements { get; set; }
                public ICollection<ElementProperty> Properties { get; set; }
                public SourceCodePath? SourceCodeDeclarationPath { get; set; }
                public ICollection<DocumentHierarchyElement> Children { get; set; }
            }

            internal sealed class PageLocation
            {
                public int PageNumber { get; init; }
                public float Left { get; init; }
                public float Top { get; init; }
                public float Right { get; init; }
                public float Bottom { get; init; }
            }

            internal sealed class LayoutErrorMeasurement
            {
                public int PageNumber { get; set; }
                public ElementSize? AvailableSpace { get; set; }
                public ElementSize? MeasurementSize { get; set; }
                public SpacePlanType? SpacePlanType { get; set; }
                public string? WrapReason { get; set; }
                public bool IsLayoutErrorRootCause { get; set; }
            }
            
            internal sealed class SourceCodePath
            {
                public string FilePath { get; set; }
                public int LineNumber { get; set; }
            }
        }
        
        internal sealed class ProvideRenderedDocumentPage
        {
            public ICollection<RenderedPage> Pages { get; set; }

            internal sealed class RenderedPage
            {
                public int PageIndex { get; set; }
                public int ZoomLevel { get; set; }
                public string ImageData { get; set; } // base64
            }
        }
        
        internal sealed class ShowGenericException
        {
            public GenericExceptionDetails Exception { get; set; }
            
            internal sealed class GenericExceptionDetails
            {
                public string Type { get; set; }
                public string Message { get; set; }
                public ICollection<StackFrame> StackTrace { get; set; }
                public GenericExceptionDetails? InnerException { get; set; }
            }
            
            internal sealed class StackFrame
            {
                public string CodeLocation { get; set; }
                public string? FileName { get; set; }
                public int? LineNumber { get; set; }
            }
        }

        internal sealed class ElementSize
        {
            public float Width { get; set; }
            public float Height { get; set; }
        }
        
        internal sealed class ElementProperty
        {
            public string Label { get; set; }
            public string Value { get; set; }
        }

        internal sealed class GetVersionCommandResponse
        {
            public ICollection<int> SupportedVersions { get; set; }
        }
    }
}
