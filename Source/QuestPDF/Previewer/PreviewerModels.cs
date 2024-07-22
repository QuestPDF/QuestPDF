using System;
using System.Collections.Generic;

namespace QuestPDF.Previewer;

#if NET6_0_OR_GREATER



class PageSnapshotIndex
{
    public int PageIndex { get; set; }
    public int ZoomLevel { get; set; }

    public override string ToString() => $"{ZoomLevel}/{PageIndex}";
}

#endif