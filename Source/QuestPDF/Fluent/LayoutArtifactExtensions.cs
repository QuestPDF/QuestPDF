using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Fluent;

public static class LayoutArtifactExtensions
{
    private static IContainer Artifact(this IContainer container, int nodeId)
    {
        return container.Element(new Elements.LayoutArtifact
        {
            Id = nodeId
        });
    }
    
    /// <summary>
    /// Marks the container content as a generic PDF artifact.
    /// Generic artifacts are non-structural elements that don't fit into other specific categories.
    /// </summary>
    public static IContainer ArtifactOther(this IContainer container)
    {
        return container.Artifact(SkSemanticNodeSpecialId.OtherArtifact);
    }

    /// <summary>
    /// Marks the container content as a pagination artifact.
    /// Used for page-related content that isn't part of the document's logical structure.
    /// </summary>
    public static IContainer ArtifactPagination(this IContainer container)
    {
        return container.Artifact(SkSemanticNodeSpecialId.PaginationArtifact);
    }

    /// <summary>
    /// Marks the container content as a page header artifact.
    /// Used for repeating header content that appears at the top of pages.
    /// </summary>
    public static IContainer ArtifactPaginationHeader(this IContainer container)
    {
        return container.Artifact(SkSemanticNodeSpecialId.PaginationHeaderArtifact);
    }

    /// <summary>
    /// Marks the container content as a page footer artifact.
    /// Used for repeating footer content that appears at the bottom of pages.
    /// </summary>
    public static IContainer ArtifactPaginationFooter(this IContainer container)
    {
        return container.Artifact(SkSemanticNodeSpecialId.PaginationFooterArtifact);
    }

    /// <summary>
    /// Marks the container content as a watermark artifact.
    /// Used for watermark content that overlays the page but isn't part of the document structure.
    /// </summary>
    public static IContainer ArtifactPaginationWatermark(this IContainer container)
    {
        return container.Artifact(SkSemanticNodeSpecialId.PaginationWatermarkArtifact);
    }

    /// <summary>
    /// Marks the container content as a layout artifact.
    /// Used for decorative or layout-related content that doesn't convey document meaning.
    /// </summary>
    public static IContainer ArtifactLayout(this IContainer container)
    {
        return container.Artifact(SkSemanticNodeSpecialId.LayoutArtifact);
    }

    /// <summary>
    /// Marks the container content as a page artifact.
    /// Used for page-level decorative elements that aren't part of the document's logical structure.
    /// </summary>
    public static IContainer ArtifactPage(this IContainer container)
    {
        return container.Artifact(SkSemanticNodeSpecialId.PageArtifact);
    }

    /// <summary>
    /// Marks the container content as a background artifact.
    /// Used for background graphics or shading that doesn't convey document meaning.
    /// </summary>
    public static IContainer ArtifactBackground(this IContainer container)
    {
        return container.Artifact(SkSemanticNodeSpecialId.BackgroundArtifact);
    }
}