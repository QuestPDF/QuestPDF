using System;
using System.IO;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Skia;

namespace QuestPDF.Infrastructure;

/// <summary>
/// Caches the SVG image in local memory for efficient reuse.
/// </summary>
/// <remarks>
/// This class is thread safe.
/// </remarks>
public class SvgImage
{
    internal SkSvgImage SkSvgImage { get; }
    
    private SvgImage(string content)
    {
        SkSvgImage = new SkSvgImage(content, SkResourceProvider.CurrentResourceProvider, FontManager.CurrentFontManager);    
    }

    ~SvgImage()
    {
        SkSvgImage?.Dispose();
    }
    
    /// <summary>
    /// Loads the SVG image from a file with specified path.
    /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
    /// </summary>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
    public static SvgImage FromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new DocumentComposeException($"Cannot load provided image, file not found: ${filePath}");
            
        var svg = File.ReadAllText(filePath);
        return new SvgImage(svg);
    }

    /// <summary>
    /// Loads the SVG image from a stream.
    /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
    /// </summary>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
    public static SvgImage FromText(string svg)
    {
        return new SvgImage(svg);
    }
}