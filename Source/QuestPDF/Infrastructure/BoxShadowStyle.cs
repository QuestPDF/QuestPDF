using QuestPDF.Helpers;

namespace QuestPDF.Infrastructure;

/// <summary>
/// Represents the visual styling properties for a box shadow effect.
/// </summary>
public sealed class BoxShadowStyle
{
    /// <summary>
    /// Gets or sets the horizontal offset of the shadow in pixels.
    /// Positive values move the shadow to the right, negative values move it to the left.
    /// </summary>
    public float OffsetX { get; set; }
    
    /// <summary>
    /// Gets or sets the vertical offset of the shadow in pixels.
    /// Positive values move the shadow downward, negative values move it upward.
    /// </summary>
    public float OffsetY { get; set; }
    
    /// <summary>
    /// Gets or sets the spread radius of the shadow in pixels.
    /// Positive values cause the shadow to expand, negative values cause it to contract.
    /// </summary>
    public float Spread { get; set; }
    
    /// <summary>
    /// Gets or sets the blur radius of the shadow in pixels.
    /// Higher values produce a more diffused shadow with softer edges.
    /// A value of 0 results in a sharp, unblurred shadow.
    /// </summary>
    /// <remarks>
    /// Values different from 0 may significantly impact performance and enlarge the output file size.
    /// Use with caution, especially in large documents or when rendering complex shadows.
    /// </remarks>
    public float Blur { get; set; }
    
    /// <summary>
    /// Gets or sets the color of the shadow.
    /// </summary>
    public Color Color { get; set; } = Colors.Black;
}