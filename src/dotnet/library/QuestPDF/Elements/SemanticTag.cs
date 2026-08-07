using System;
using System.Linq;
using System.Numerics;
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

        if (TagType is "Figure" or "Formula")
            UpdateBoundingBoxAttribute(availableSpace);

        using var semanticScope = Canvas.StartSemanticScopeWithNodeId(SemanticTreeNode.NodeId);
        
        SemanticTreeManager.PushOnStack(SemanticTreeNode);
        Child?.Draw(availableSpace);
        SemanticTreeManager.PopStack();
    }

    #region Bounding Box

    private SemanticTreeNode.Attribute? BoundingBoxAttribute { get; set; }
    private int? BoundingBoxPageNumber { get; set; }
    private bool IsBoundingBoxSuppressed { get; set; }

    /// <summary>
    /// Illustration elements (Figure, Formula) should provide the Layout/BBox attribute
    /// that describes the position of their content on the page (ISO 32000-1, Layout attributes).
    /// While veraPDF does not verify its presence, tools such as PAC (PDF Accessibility Checker) do.
    /// </summary>
    /// <remarks>
    /// The BBox attribute is only meaningful for elements rendered entirely on a single page.
    /// When content spans multiple pages (or is repeated on every page, e.g. inside a page header),
    /// the attribute is removed and no longer generated.
    /// </remarks>
    private void UpdateBoundingBoxAttribute(Size availableSpace)
    {
        // the semantic tree is consumed after the initial rendering phase;
        // values computed in later phases would never reach the output document
        if (!PageContext.IsInitialRenderingPhase)
            return;

        if (IsBoundingBoxSuppressed)
            return;

        if (BoundingBoxPageNumber != null && BoundingBoxPageNumber != PageContext.CurrentPage)
        {
            IsBoundingBoxSuppressed = true;

            if (BoundingBoxAttribute != null)
            {
                SemanticTreeNode?.Attributes.Remove(BoundingBoxAttribute);
                BoundingBoxAttribute = null;
            }

            return;
        }

        var pageSize = SemanticTreeManager?.CurrentPageSize ?? Size.Zero;

        if (pageSize.Height < Size.Epsilon)
            return;

        var contentSize = base.Measure(availableSpace);

        if (contentSize.Type is SpacePlanType.Empty or SpacePlanType.Wrap)
            return;

        // the BBox attribute stores an axis-aligned rectangle,
        // while the content may be translated, scaled or rotated by its parent elements
        var transform = Canvas.GetCurrentMatrix().ToMatrix4x4();

        var corners = new[]
        {
            Vector2.Transform(new Vector2(0, 0), transform),
            Vector2.Transform(new Vector2(contentSize.Width, 0), transform),
            Vector2.Transform(new Vector2(contentSize.Width, contentSize.Height), transform),
            Vector2.Transform(new Vector2(0, contentSize.Height), transform)
        };

        // convert from the canvas coordinate space (top-left origin, y-axis down)
        // to the PDF default user space (bottom-left origin, y-axis up)
        // using the [left, bottom, right, top] value order
        var boundingBox = new[]
        {
            corners.Min(x => x.X),
            pageSize.Height - corners.Max(x => x.Y),
            corners.Max(x => x.X),
            pageSize.Height - corners.Min(x => x.Y)
        };

        if (BoundingBoxAttribute == null)
        {
            BoundingBoxAttribute = new SemanticTreeNode.Attribute
            {
                Owner = "Layout",
                Name = "BBox",
                Value = boundingBox
            };

            SemanticTreeNode!.Attributes.Add(BoundingBoxAttribute);
        }
        else
        {
            BoundingBoxAttribute.Value = boundingBox;
        }

        BoundingBoxPageNumber = PageContext.CurrentPage;
    }

    #endregion

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