using System;
using System.Text;
using QuestPDF.Drawing;
using QuestPDF.Elements.Text;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SemanticTag : ContainerElement
{
    public SemanticTreeManager SemanticTreeManager { get; set; }
    public SemanticTreeNode? SemanticTreeNode { get; set; }

    public string TagType { get; set; }
    public string? Alt { get; set; }
    public string? Lang { get; set; }

    internal override void Draw(Size availableSpace)
    {
        if (SemanticTreeNode == null)
        {
            var id = SemanticTreeManager.GetNextNodeId();
            
            SemanticTreeNode = new SemanticTreeNode
            {
                NodeId = id,
                Type = TagType,
                Alt = Alt,
                Lang = Lang
            };
            
            SemanticTreeManager.AddNode(SemanticTreeNode);
        }
        
        SemanticTreeManager.PushOnStack(SemanticTreeNode);
        Canvas.SetSemanticNodeId(SemanticTreeNode.NodeId);
        Child?.Draw(availableSpace);
        SemanticTreeManager.PopStack();
    }
}