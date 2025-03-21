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
public sealed class SvgImage : IDisposable
{
    internal SkSvgImage SkSvgImage { get; }
    internal bool IsShared { get; set; } = true;
    
    private SvgImage(string content)
    {
        SkSvgImage = new SkSvgImage(content, SkResourceProvider.CurrentResourceProvider, FontManager.CurrentFontManager);    
    }

    ~SvgImage()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
        
    public void Dispose()
    {
        SkSvgImage?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Loads the SVG image from a file with specified path.
    /// <a href="https://www.questpdf.com/api-reference/image/svg.html">Learn more</a>
    /// </summary>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
    public static SvgImage FromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            var fallbackPath = Path.Combine(Helpers.Helpers.ApplicationFilesPath, filePath);
                
            if (!File.Exists(fallbackPath))
                throw new DocumentComposeException($"Cannot load provided image, file not found: ${filePath}");
                
            filePath = fallbackPath;
        }
            
        var svg = File.ReadAllText(filePath);
        return new SvgImage(svg);
    }

    /// <summary>
    /// Loads the SVG image from a stream.
    /// <a href="https://www.questpdf.com/api-reference/image/svg.html">Learn more</a>
    /// </summary>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
    public static SvgImage FromText(string svg)
    {
        return new SvgImage(svg);
    }
}