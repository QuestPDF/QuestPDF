using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Fluent;

public static class SemanticExtensions
{
    internal static IContainer SemanticTag(this IContainer container, string type, string? alternativeText = null, string? language = null)
    {
        return container.Element(new Elements.SemanticTag
        {
            TagType = type, 
            Alt = alternativeText,
            Lang = language
        });
    }

    /// <summary>
    /// Marks a self-contained body of text that forms a single narrative or exposition,
    /// such as a blog post, news story, or forum post.
    /// As a best practice, articles should not be nested within each other.
    /// </summary>
    public static IContainer SemanticArticle(this IContainer container)
    {
        return container.SemanticTag("Art");
    }
    
    /// <summary>
    /// Applies a 'Section' tag, grouping a set of related content.
    /// A section typically includes a heading (e.g., SemanticHeader2) and its corresponding content.
    /// Sections can be nested to create a hierarchical document structure.
    /// </summary>
    public static IContainer SemanticSection(this IContainer container)
    {
        return container.SemanticTag("Sect");
    }
    
    /// <summary>
    /// Marks a generic block-level container for grouping elements.
    /// It's often used when a more specific semantic tag (like 'Article' or 'Section') doesn't apply,
    /// serving as a general-purpose 'div', similar to its HTML counterpart.
    /// </summary>
    public static IContainer SemanticDivision(this IContainer container)
    {
        return container.SemanticTag("Div");
    }
    
    /// <summary>
    /// Designates a block of text that is a quotation, typically consisting of one or more paragraphs.
    /// This is for block-level quotes, as opposed to <see cref="SemanticQuote"/> which is for inline text.
    /// </summary>
    public static IContainer SemanticBlockQuotation(this IContainer container)
    {
        return container.SemanticTag("BlockQuote");
    }
    
    /// <summary>
    /// Identifies a brief portion of text that serves as a caption or description
    /// for a table, figure, or image. It should be placed near the element it describes.
    /// </summary>
    public static IContainer SemanticCaption(this IContainer container)
    {
        return container.SemanticTag("Caption");
    }
    
    /// <summary>
    /// Marks a section of the document as an index.
    /// This container typically holds a sequence of entries and references.
    /// </summary>
    public static IContainer SemanticIndex(this IContainer container)
    {
        return container.SemanticTag("Index");
    }
    
    /// <summary>
    /// Applies a language attribute to a container, specifying the natural language (e.g., 'en-US', 'es-ES') of its content.
    /// This is crucial for accessibility, enabling screen readers to use the correct pronunciation.
    /// </summary>
    /// <param name="language">The ISO 639 language code (e.g., 'en-US' or 'fr-FR') for the content.</param>
    public static IContainer SemanticLanguage(this IContainer container, string language)
    {
        return container.SemanticTag("NonStruct", language: language);
    }
    
    #region Table of Contents
    
    /// <summary>
    /// Marks a container as a Table of Contents (TOC).
    /// <para>
    /// A TOC should be composed of <see cref="SemanticTableOfContentsItem"/> elements.
    /// TOCs can be nested to represent a hierarchical document structure.
    /// </para>
    /// <para>This tag can also be used for lists of figures, lists of tables, or bibliographies.</para>
    /// </summary>
    public static IContainer SemanticTableOfContents(this IContainer container)
    {
        return container.SemanticTag("TOC");
    }
    
    /// <summary>
    /// Marks an individual item within a <see cref="SemanticTableOfContents"/>.
    /// This typically represents a single entry in the list.
    /// </summary>
    public static IContainer SemanticTableOfContentsItem(this IContainer container)
    {
        return container.SemanticTag("TOCI");
    }
    
    #endregion
    
    #region Headers
    
    private static IContainer SemanticHeader(this IContainer container, int level)
    {
        if (level < 1 || level > 6)
            throw new ArgumentOutOfRangeException(nameof(level), "Header level must be between 1 and 6.");

        return container.SemanticTag($"H{level}");
    }
    
    /// <summary>
    /// Marks the content as a level 1 heading (H1), the highest level in the document hierarchy.
    /// Headings are crucial for navigation and outlining the document's structure.
    /// </summary>
    public static IContainer SemanticHeader1(this IContainer container)
    {
        return container.SemanticHeader(1);
    }
    
    /// <summary>
    /// Marks the content as a level 2 heading (H2).
    /// </summary>
    public static IContainer SemanticHeader2(this IContainer container)
    {
        return container.SemanticHeader(2);
    }
    
    /// <summary>
    /// Marks the content as a level 3 heading (H3).
    /// </summary>
    public static IContainer SemanticHeader3(this IContainer container)
    {
        return container.SemanticHeader(3);
    }
    
    /// <summary>
    /// Marks the content as a level 4 heading (H4).
    /// </summary>
    public static IContainer SemanticHeader4(this IContainer container)
    {
        return container.SemanticHeader(4);
    }
    
    /// <summary>
    /// Marks the content as a level 5 heading (H5).
    /// </summary>
    public static IContainer SemanticHeader5(this IContainer container)
    {
        return container.SemanticHeader(5);
    }
    
    /// <summary>
    /// Marks the content as a level 6 heading (H6), the lowest level in the document hierarchy.
    /// </summary>
    public static IContainer SemanticHeader6(this IContainer container)
    {
        return container.SemanticHeader(6);
    }
    
    #endregion
    
    /// <summary>
    /// Marks a container as a paragraph.
    /// This is one of the most common block-level tags for organizing text content.
    /// </summary>
    public static IContainer SemanticParagraph(this IContainer container)
    {
        return container.SemanticTag("P");
    }
    
    #region Lists
    
    /// <summary>
    /// Marks a container as a list.
    /// Its direct children should be one or more <see cref="SemanticListItem"/> elements.
    /// A <see cref="SemanticCaption"/> can also be included as an optional first child.
    /// </summary>
    public static IContainer SemanticList(this IContainer container)
    {
        return container.SemanticTag("L");
    }

    /// <summary>
    /// Marks an individual item within a <see cref="SemanticList"/>.
    /// Its children should typically be a <see cref="SemanticListLabel"/> (e.g., the bullet or number) and/or a <see cref="SemanticListItemBody"/> (the content).
    /// </summary>
    public static IContainer SemanticListItem(this IContainer container)
    {
        return container.SemanticTag("LI");
    }
    
    /// <summary>
    /// Marks the label of a list item.
    /// This container holds the bullet, number (e.g., '1.'), or term (in a definition list) that identifies the list item.
    /// </summary>
    public static IContainer SemanticListLabel(this IContainer container)
    {
        return container.SemanticTag("Lbl");
    }
    
    /// <summary>
    /// Marks the body or descriptive content of a <see cref="SemanticListItem"/>.
    /// This contains the main text or content associated with the list item's label.
    /// </summary>
    public static IContainer SemanticListItemBody(this IContainer container)
    {
        return container.SemanticTag("LBody");
    }
    
    #endregion
    
    #region Table
    
    /// <summary>
    /// Marks a container as a table.
    /// The library automatically automatically tags headers, rows, cells, etc.
    /// </summary>
    public static IContainer SemanticTable(this IContainer container)
    {
        return container.SemanticTag("Table");
    }
    
    #endregion
    
    #region Inline Elements
    
    /// <summary>
    /// Marks a generic inline portion of text (Span).
    /// This is useful for grouping inline elements or applying styling, similar to an HTML &lt;span&gt;.
    /// </summary>
    /// <param name="alternativeText">Optional alternative text, often used to provide an expansion for an abbreviation or other supplementary information.</param>
    public static IContainer SemanticSpan(this IContainer container, string? alternativeText = null)
    {
        return container.SemanticTag("Span", alternativeText);
    }
    
    /// <summary>
    /// Marks an inline portion of text as a quote.
    /// This differs from <see cref="SemanticBlockQuotation"/>, which is intended for block-level content (one or more paragraphs).
    /// </summary>
    public static IContainer SemanticQuote(this IContainer container)
    {
        return container.SemanticTag("Quote");
    }

    /// <summary>
    /// Marks a fragment of text as computer code.
    /// </summary>
    public static IContainer SemanticCode(this IContainer container)
    {
        return container.SemanticTag("Code");
    }

    /// <summary>
    /// Marks the content as a hyperlink (Link).
    /// </summary>
    /// <param name="alternativeText">Alternative text describing the link's purpose or destination. This is essential for screen readers.</param>
    public static IContainer SemanticLink(this IContainer container, string alternativeText)
    {
        return container.SemanticTag("Link", alternativeText: alternativeText);
    }
    
    #endregion
    
    #region Illustration Elements
    
    /// <summary>
    /// Marks a container as a figure, which is an item of graphical content like a chart, diagram, or photograph.
    /// </summary>
    /// <param name="alternativeText">A textual description of the figure, read by screen readers. This is essential for accessibility.</param>
    public static IContainer SemanticFigure(this IContainer container, string alternativeText)
    {
        return container.SemanticTag("Figure", alternativeText: alternativeText);
    }
    
    /// <summary>
    /// An alias for <see cref="SemanticFigure"/>. Marks the content as an image.
    /// </summary>
    /// <param name="alternativeText">A textual description of the image, read by screen readers. This is essential for accessibility.</param>
    public static IContainer SemanticImage(this IContainer container, string alternativeText)
    {
        return container.SemanticFigure(alternativeText);
    }
    
    /// <summary>
    /// Marks the content as a mathematical formula.
    /// From a structural and accessibility standpoint, it is treated similarly to a figure.
    /// </summary>
    public static IContainer SemanticFormula(this IContainer container, string alternativeText)
    {
        return container.SemanticTag("Formula", alternativeText: alternativeText);
    }
    
    #endregion
}