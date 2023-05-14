using System;
using System.Collections.Generic;
using QuestPDF.Previewer.Inspection;

namespace QuestPDF.Previewer;

internal sealed class IncompatibleVersionApiRequest
{
    
}

internal sealed class NotifyPresenceApiRequest
{
    public string ClientId { get; set; }
    public string LibraryVersion { get; set; }
    public bool IsDotnet6OrBeyond { get; set; }
    public bool IsDotnet3OrBeyond { get; set; }
    public bool IsExecutedInUnitTest { get; set; }
}

internal sealed class DocumentPreviewApiRequest
{
    public ICollection<PageSnapshot> PageSnapshots { get; set; }
    public InspectionElement DocumentHierarchy { get; set; }
        
    public class PageSnapshot
    {
        public string ResourceId { get; } = Guid.NewGuid().ToString("N");
        public int PageNumber { get; set; }
            
        public float Width { get; set; }
        public float Height { get; set; }
    }
}

internal sealed class GenericErrorApiRequest
{
    public GenericError? Error { get; set; }
}
    
internal sealed class LayoutErrorApiRequest
{
    public LayoutErrorTrace? Trace { get; set; }
}