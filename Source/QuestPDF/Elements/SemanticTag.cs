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
        if (TagType is "H" or "H1" or "H2" or "H3" or "H4" or "H5" or "H6")
            UpdateHeaderText();
        
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
        Canvas.SetSemanticNodeId(0);
        SemanticTreeManager.PopStack();
    }

    private void UpdateHeaderText()
    {
        if (!string.IsNullOrWhiteSpace(Alt))
            return;
        
        var builder = new StringBuilder();
        Traverse(builder, Child);
        Alt = builder.ToString();
        
        static void Traverse(StringBuilder builder, Element element)
        {
            if (element is TextBlock textBlock)
            {
                builder.Append(textBlock.Text).Append(' ');
            }
            else if (element is ContainerElement container)
            {
                Traverse(builder, container);
            }
            else
            {
                foreach (var child in element.GetChildren())
                    Traverse(builder, child);
            }
        }
    }
}