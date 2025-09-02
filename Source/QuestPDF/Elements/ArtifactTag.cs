using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class ArtifactTag : ContainerElement
{
    public int Id { get; set; }
    
    internal override void Draw(Size availableSpace)
    {
        Canvas.SetSemanticNodeId(Id);
        Child?.Draw(availableSpace);
    }
}