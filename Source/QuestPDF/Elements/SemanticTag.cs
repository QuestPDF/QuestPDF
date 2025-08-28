using System;
using System.Text;
using QuestPDF.Elements.Text;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SemanticTag : ContainerElement
{
    public int Id { get; set; }
    public string TagType { get; set; }
    public string? Alt { get; set; }
    public string? Lang { get; set; }
    
    internal override void Draw(Size availableSpace)
    {
        Canvas.SetSemanticNodeId(Id);
        Child?.Draw(availableSpace);
    }
}