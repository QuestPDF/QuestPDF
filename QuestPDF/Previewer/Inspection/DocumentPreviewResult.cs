using System.Collections.Generic;
using QuestPDF.Drawing;

namespace QuestPDF.Previewer.Inspection;

internal class DocumentPreviewResult
{
    public IList<PreviewerPicture> PageSnapshots { get; set; }
    public InspectionElement DocumentHierarchy { get; set; }
}