using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;

#if NET6_0_OR_GREATER

namespace QuestPDF.Previewer
{
    internal class PreviewerRefreshCommand
    {
        public ICollection<Page> Pages { get; set; }

        public class Page
        {
            public string Id { get; }
            
            public float Width { get; init; }
            public float Height { get; init; }

            public Page()
            {
                Id = Guid.NewGuid().ToString("N");
            }
        }
    }
}

#endif