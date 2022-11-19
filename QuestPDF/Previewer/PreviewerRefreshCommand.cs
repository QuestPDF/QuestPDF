using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;

#if NETCOREAPP3_0_OR_GREATER

namespace QuestPDF.Previewer
{
    internal class PreviewerRefreshCommand
    {
        public ICollection<Page> Pages { get; set; }

        public class Page
        {
            public string Id { get; } = Guid.NewGuid().ToString("N");
            
            public float Width { get; set; }
            public float Height { get; set; }
        }
    }
}

#endif