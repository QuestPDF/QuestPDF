using System;
using System.IO;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using SvgImage = QuestPDF.Infrastructure.SvgImage;

namespace QuestPDF.Fluent;

/// <summary>
/// Generates an SVG image based on the given resolution.
/// </summary>
/// <param name="size">Desired resolution of the image in pixels.</param>
/// <returns>An SVG format compatible text.</returns>
public delegate string? GenerateDynamicSvgDelegate(Size size);

public class SvgImageDescriptor
{
    private Elements.SvgImage ImageElement { get; }
    private AspectRatio AspectRatioElement { get; }
    private float ImageAspectRatio { get; }

    internal SvgImageDescriptor(Elements.SvgImage imageElement, Elements.AspectRatio aspectRatioElement)
    {
        ImageElement = imageElement;
        AspectRatioElement = aspectRatioElement;
        ImageAspectRatio = ImageElement.Image.SkSvgImage.ViewBox.Width /  ImageElement.Image.SkSvgImage.ViewBox.Height;
    }
    
    /// <summary>
    /// Scales the image to fill the full width of its container. This is the default behavior.
    /// </summary>
    public SvgImageDescriptor FitWidth()
    {
        return SetAspectRatio(AspectRatioOption.FitWidth);
    }
    
    /// <summary>
    /// <para>The image stretches vertically to fit the full available height.</para>
    /// <para>Often used with height-constraining elements such as: <see cref="ConstrainedExtensions.Height">Height</see>, <see cref="ConstrainedExtensions.MaxHeight">MaxHeight</see>, etc.</para>
    /// </summary>
    public SvgImageDescriptor FitHeight()
    {
        return SetAspectRatio(AspectRatioOption.FitHeight);
    }
    
    /// <summary>
    /// Combines the FitWidth and FitHeight settings.
    /// The image resizes itself to utilize all available space, preserving its aspect ratio.
    /// It will either fill the width or height based on the container's dimensions.
    /// </summary>
    /// <remarks>
    /// An optimal and safe choice.
    /// </remarks>
    public SvgImageDescriptor FitArea()
    {
        return SetAspectRatio(AspectRatioOption.FitArea);
    }

    private SvgImageDescriptor SetAspectRatio(AspectRatioOption option)
    {
        AspectRatioElement.Ratio = ImageAspectRatio;
        AspectRatioElement.Option = option;
        return this;
    }
}

public static class SvgExtensions
{
    internal static void SvgPath(this IContainer container, string svgPath, string color)
    {
        container.Element(new SvgPath
        {
            Path = svgPath,
            FillColor = color
        });
    }
    
    /// <summary>
    /// Draws the SVG image loaded from a text.
    /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
    /// </summary>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="svg.remarks"]/*' />
    /// <param name="svg">
    /// Either a path to the SVG file or the SVG content itself.
    /// </param>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="svg.descriptor"]/*' />
    public static SvgImageDescriptor Svg(this IContainer container, string svg)
    {
        var isFile = Path.GetExtension(svg).Equals(".svg", StringComparison.OrdinalIgnoreCase);
        
        var image = isFile ? SvgImage.FromFile(svg) : SvgImage.FromText(svg);
        return container.Svg(image);
    }
    
    /// <summary>
    /// Draws the <see cref="Infrastructure.SvgImage" /> object. Allows to optimize the generation process.
    /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
    /// </summary>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="svg.remarks"]/*' />
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="svg.descriptor"]/*' />
    public static SvgImageDescriptor Svg(this IContainer parent, SvgImage image)
    {
        var imageElement = new QuestPDF.Elements.SvgImage
        {
            Image = image
        };

        var aspectRationElement = new AspectRatio
        {
            Child = imageElement
        };
            
        parent.Element(aspectRationElement);
        return new SvgImageDescriptor(imageElement, aspectRationElement).FitWidth();
    }
    
    /// <summary>
    /// Renders an SVG image of dynamic size dictated by the document layout constraints.
    /// </summary>
    /// <remarks>
    /// Ideal for integrating with other libraries, e.g. SkiaSharp or ScottPlot.
    /// </remarks>
    /// <param name="dynamicSvgSource">
    /// A delegate that requests an image of desired size.
    /// </param>
    public static void Svg(this IContainer element, GenerateDynamicSvgDelegate dynamicSvgSource)
    {
        var dynamicImage = new DynamicSvgImage
        {
            SvgSource = dynamicSvgSource
        };
            
        element.Element(dynamicImage);
    }
}