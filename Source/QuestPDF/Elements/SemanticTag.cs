using System;
using System.Text;
using QuestPDF.Drawing;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Elements.Text;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SemanticTag : ContainerElement, ISemanticAware
{
    public SemanticTreeManager? SemanticTreeManager { get; set; }
    public SemanticTreeNode? SemanticTreeNode { get; private set; }

    public string TagType { get; set; }
    public string? Alt { get; set; }
    public string? Lang { get; set; }

    internal override void Draw(Size availableSpace)
    {
        var shouldIgnoreSemanticMeaning =
            Canvas.Is<DiscardDrawingCanvas>() ||
            SemanticTreeManager == null ||
            SemanticTreeManager.IsCurrentContentArtifact();
        
        if (shouldIgnoreSemanticMeaning)
        {
            Child?.Draw(availableSpace);
            return;       
        }
        
        RegisterCurrentSemanticNode();
        
        using var semanticScope = Canvas.StartSemanticScopeWithNodeId(SemanticTreeNode.NodeId);
        
        SemanticTreeManager.PushOnStack(SemanticTreeNode);
        Child?.Draw(availableSpace);
        SemanticTreeManager.PopStack();
    }

    internal void RegisterCurrentSemanticNode()
    {
        if (SemanticTreeNode != null)
            return;
        
        if (TagType is "H" or "H1" or "H2" or "H3" or "H4" or "H5" or "H6")
            UpdateHeaderText();
        
        if (TagType is "Link")
            UpdateDescriptionOfInnerLink();
        
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
                if (builder.Length > 0)
                    builder.Append(' ');
                
                builder.Append(textBlock.Text);
            }
            else if (element is ContainerElement container)
            {
                Traverse(builder, container.Child);
            }
            else
            {
                foreach (var child in element.GetChildren())
                    Traverse(builder, child);
            }
        }
    }
    
    private void UpdateDescriptionOfInnerLink()
    {
        if (string.IsNullOrWhiteSpace(Alt))
            return;
        
        var currentChild = Child;
        
        while (currentChild != null)
        {
            if (currentChild is Hyperlink hyperlink)
            {
                hyperlink.Description = Alt;
                return;
            }
            
            if (currentChild is SectionLink sectionLink)
            {
                sectionLink.Description = Alt;
                return;
            }
            
            currentChild = (currentChild as ContainerElement)?.Child;
        }
    }

    internal override string? GetCompanionHint()
    {
        var result = TagType;
        
        if (!string.IsNullOrWhiteSpace(Alt))
            result += $" ({Alt})";
        
        return result;
    }
    
    internal override string? GetCompanionSearchableContent() => TagType;
}