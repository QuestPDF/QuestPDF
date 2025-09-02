using System;
using System.Text;
using QuestPDF.Drawing;
using QuestPDF.Elements.Text;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SemanticTag : ContainerElement
{
    public SemanticTreeManager SemanticTreeManager { get; set; }

    public int Id { get; set; } = 0;
    public string TagType { get; set; }
    public string? Alt { get; set; }
    public string? Lang { get; set; }
    
    internal override void Draw(Size availableSpace)
    {
        if (Id == 0)
            Id = SemanticTreeManager.GetNextNodeId();

        SemanticTreeManager.AddNode(null);
        Canvas.SetSemanticNodeId(Id);
        Child?.Draw(availableSpace);
        SemanticTreeManager.UndoNesting();
    }
}