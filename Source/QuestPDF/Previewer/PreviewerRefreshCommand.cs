using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer.LayoutInspection;

#if NET6_0_OR_GREATER

namespace QuestPDF.Previewer
{
    internal class PreviewerRefreshCommand
    {
        public DocumentInspectionElement DocumentHierarchy { get; set; }
        public ICollection<Page> Pages { get; set; }

        public class Page
        {
            public string Id { get; } = Guid.NewGuid().ToString("N");
            
            public float Width { get; init; }
            public float Height { get; init; }
        }
    }
}

#endif