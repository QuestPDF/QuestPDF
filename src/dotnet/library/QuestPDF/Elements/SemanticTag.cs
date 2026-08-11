using System;
using System.Linq;
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
        
        CaptureBoundingBoxIfFigureTag(availableSpace);

        SemanticTreeManager.PushOnStack(SemanticTreeNode);
        Child?.Draw(availableSpace);
        SemanticTreeManager.PopStack();
    }

    internal void RegisterCurrentSemanticNode()
    {
        if (SemanticTreeNode != null)
            return;

        UpdateTitleIfHeaderTag();
        UpdateDescriptionIfLinkTag();
        
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

    private void UpdateTitleIfHeaderTag()
    {
        if (TagType is not ("H" or "H1" or "H2" or "H3" or "H4" or "H5" or "H6"))
            return;
        
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
    
    private void UpdateDescriptionIfLinkTag()
    {
        if (TagType is not "Link")
            return;
        
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
    
    private void CaptureBoundingBoxIfFigureTag(Size availableSpace)
    {
        if (TagType is not ("Figure" or "Formula"))
            return;
        
        if (SemanticTreeNode == null)
            return;
        
        if (!PageContext.IsInitialRenderingPhase)
            return;

        if (PageContext.CurrentPageSize == null)
            return;

        if (SemanticTreeNode.Attributes.Any(x => x is { Owner: "Layout", Name: "BBox" }))
            return;
        
        if (base.Measure(availableSpace).Type is not SpacePlanType.FullRender)
            return;
        
        var bounds = Canvas.GetCurrentMatrix().GetTransformedBoundingBox(availableSpace);
        var pageHeight = PageContext.CurrentPageSize.Value.Height;

        // the PDF coordinate space starts at the bottom-left corner of the page
        var boundingBox = new[]
        {
            (float)Math.Floor(bounds.Left),
            (float)Math.Floor(pageHeight - bounds.Bottom),
            (float)Math.Ceiling(bounds.Right),
            (float)Math.Ceiling(pageHeight - bounds.Top)
        };

        SemanticTreeNode.Attributes.Add(new SemanticTreeNode.Attribute
        {
            Owner = "Layout",
            Name = "BBox",
            Value = boundingBox
        });
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