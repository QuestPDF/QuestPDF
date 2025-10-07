using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Fluent;

// TODO: is the Semantic* convention really needed? 
// Is it possibly to simply introduce new APis such as Part, Article, Division, Caption, Header, etc. ?
// Could some of semantic tags be applied automatically?

public static class SemanticExtensions
{
    internal static IContainer MarkRepeatedContentAsArtifact(this IContainer container, MarkRepeatedContentAsArtifact.PaginationType type)
    {
        return container.Element(new Elements.MarkRepeatedContentAsArtifact()
        {
            Type = type
        });
    }
    
    private static IContainer SemanticTag(this IContainer container, string type, string? alternativeText = null, string? language = null)
    {
        return container.Element(new Elements.SemanticTag
        {
            TagType = type, 
            Alt = alternativeText,
            Lang = language
        });
    }
    
    // Structure type "Document" - implementation not needed
    // The "Document" tag is required to be a root of the entire semantic tree
    // It is already implemented as such in the SemanticTreeManager class

    /// <summary>
    /// A large-scale division of a document.
    /// This type of element is appropriate for grouping articles or sections.
    /// </summary>
    public static IContainer SemanticPart(this IContainer container)
    {
        return container.SemanticTag("Part");
    }
    
    /// <summary>
    /// A relatively self-contained body of text constituting a single narrative or exposition.
    /// Articles should be disjoint; that is, they should not contain other articles as constituent elements.
    /// </summary>
    public static IContainer SemanticArticle(this IContainer container)
    {
        return container.SemanticTag("Art");
    }
    
    /// <summary>
    /// A container for grouping related content elements.
    /// For example, a section might contain a heading, several introductory paragraphs, and two or more other sections nested within it as subsections.
    /// </summary>
    public static IContainer SemanticSection(this IContainer container)
    {
        return container.SemanticTag("Sect");
    }
    
    /// <summary>
    /// A generic block-level element or group of elements.
    /// </summary>
    public static IContainer SemanticDivision(this IContainer container)
    {
        return container.SemanticTag("Div");
    }
    
    /// <summary>
    /// A portion of text consisting of one or more paragraphs attributed to someone other than the author of the surrounding text.
    /// </summary>
    public static IContainer SemanticBlockQuotation(this IContainer container)
    {
        return container.SemanticTag("BlockQuote");
    }
    
    /// <summary>
    /// A brief portion of text describing a table or figure.
    /// </summary>
    public static IContainer SemanticCaption(this IContainer container)
    {
        return container.SemanticTag("Caption");
    }
    
    /// <summary>
    /// A sequence of entries containing identifying text accompanied by reference elements that point out occurrences of the specified text in the main body of a document.
    /// </summary>
    public static IContainer SemanticIndex(this IContainer container, string language)
    {
        return container.SemanticTag("Index", language: language);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static IContainer SemanticLanguage(this IContainer container, string language)
    {
        return container.SemanticTag("NonStruct", language: language);
    }
    
    #region Table of Contents
    
    /// <summary>
    /// <para>A list made up of table of contents item entries (SemanticTableOfContentsItem) and/or other nested table of contents entries (SemanticTableOfContents).</para>
    /// <para>A SemanticTableOfContents entry that includes only SemanticTableOfContentsItem entries represents a flat hierarchy.</para>
    /// <para>A SemanticTableOfContents entry that includes other nested SemanticTableOfContents entries (and possibly SemanticTableOfContentsItem entries) represents a more complex hierarchy.</para>
    /// <para>Ideally, the hierarchy of a top-level SemanticTableOfContents entry reflects the structure of the main body of the document.</para>
    /// <para>Lists of figures and tables, as well as bibliographies, can be treated as  tables of contents for purposes of the standard structure types.</para>
    /// </summary>
    public static IContainer SemanticTableOfContents(this IContainer container)
    {
        return container.SemanticTag("TOC");
    }
    
    /// <summary>
    /// An individual member of a table of contents.
    /// </summary>
    public static IContainer SemanticTableOfContentsItem(this IContainer container)
    {
        return container.SemanticTag("TOCI");
    }
    
    #endregion
    
    #region Headers
    
    /// <summary>
    /// A label for a subdivision of a document's content.
    /// It should be the first child of the division that it heads.
    /// </summary>
    public static IContainer SemanticHeader(this IContainer container)
    {
        return container.SemanticTag("H");
    }
    
    private static IContainer SemanticHeader(this IContainer container, int level)
    {
        if (level < 1 || level > 6)
            throw new ArgumentOutOfRangeException(nameof(level), "Header level must be between 1 and 6.");

        return container.SemanticTag($"H{level}");
    }
    
    /// <summary>
    /// A label for a subdivision of a document's content. It should be the first child of the division that it heads.
    /// A level 1 header - the highest level of heading.
    /// </summary>
    public static IContainer SemanticHeader1(this IContainer container)
    {
        return container.SemanticHeader(1);
    }
    
    /// <summary>
    /// A label for a subdivision of a document's content. It should be the first child of the division that it heads.
    /// A level 2 header.
    /// </summary>
    public static IContainer SemanticHeader2(this IContainer container)
    {
        return container.SemanticHeader(2);
    }
    
    /// <summary>
    /// A label for a subdivision of a document's content. It should be the first child of the division that it heads.
    /// A level 3 header.
    /// </summary>
    public static IContainer SemanticHeader3(this IContainer container)
    {
        return container.SemanticHeader(3);
    }
    
    /// <summary>
    /// A label for a subdivision of a document's content. It should be the first child of the division that it heads.
    /// A level 4 header.
    /// </summary>
    public static IContainer SemanticHeader4(this IContainer container)
    {
        return container.SemanticHeader(4);
    }
    
    /// <summary>
    /// A label for a subdivision of a document's content. It should be the first child of the division that it heads.
    /// A level 5 header.
    /// </summary>
    public static IContainer SemanticHeader5(this IContainer container)
    {
        return container.SemanticHeader(5);
    }
    
    /// <summary>
    /// A label for a subdivision of a document's content. It should be the first child of the division that it heads.
    /// A level 6 header - the lowest level of heading.
    /// </summary>
    public static IContainer SemanticHeader6(this IContainer container)
    {
        return container.SemanticHeader(6);
    }
    
    #endregion
    
    /// <summary>
    /// A low-level division of text.
    /// </summary>
    public static IContainer SemanticParagraph(this IContainer container)
    {
        return container.SemanticTag("P");
    }
    
    #region Lists
    
    /// <summary>
    /// A sequence of items of like meaning and importance.
    /// Its immediate children should be an optional SemanticCaption followed by one or more list items SemanticListItem.
    /// </summary>
    public static IContainer SemanticList(this IContainer container)
    {
        return container.SemanticTag("L");
    }

    /// <summary>
    /// An individual member of a list.
    /// Its children may be one or more SemanticListLabel, SemanticListItemBody, or both.
    /// </summary>
    public static IContainer SemanticListItem(this IContainer container)
    {
        return container.SemanticTag("LI");
    }
    
    /// <summary>
    /// A name or number that distinguishes a given item from others in the same list or other group of like items.
    /// 
    /// <remarks>
    /// In a dictionary list, for example, it contains the term being defined; in a bulleted or numbered list, it contains the bullet character or the number of the list item and associated punctuation.
    /// </remarks>
    /// </summary>
    public static IContainer SemanticListLabel(this IContainer container)
    {
        return container.SemanticTag("Lbl");
    }
    
    /// <summary>
    /// The descriptive content of a list item.
    /// 
    /// <remarks>
    /// In a dictionary list, for example, it contains the definition of the term. It may contain more sophisticated content.
    /// </remarks>
    /// </summary>
    public static IContainer SemanticListItemBody(this IContainer container)
    {
        return container.SemanticTag("LBody");
    }
    
    #endregion
    
    #region Table
    
    /// <summary>
    /// A two-dimensional layout of rectangular data cells, possibly having a complex substructure.
    /// It contains either one or more table rows SemanticTableRow as children; or an optional table head SemanticTableHeader followed by one or more table body elements SemanticTableBody and an optional table footer SemanticTableFooter.
    /// In addition, a table may have a SemanticCaption as its first or last child.
    /// </summary>
    public static IContainer SemanticTable(this IContainer container)
    {
        return container.SemanticTag("Table");
    }
    
    /// <summary>
    ///  A row of headings or data in a table. It may contain table header cells and table data cells (structure types TH and TD).
    /// </summary>
    public static IContainer SemanticTableRow(this IContainer container)
    {
        return container.SemanticTag("TR");
    }
    
    /// <summary>
    /// A table cell containing header text describing one or more rows or columns of the table.
    /// </summary>
    public static IContainer SemanticTableHeaderCell(this IContainer container)
    {
        return container.SemanticTag("TH");
    }
    
    /// <summary>
    /// A table cell containing data that is part of the table's content.
    /// </summary>
    public static IContainer SemanticTableDataCell(this IContainer container)
    {
        return container.SemanticTag("TD");
    }
    
    /// <summary>
    /// A group of rows that constitute the header of a table.
    /// If the table is split across multiple pages, these rows may be redrawn at the top of each table fragment (although there is only one SemanticTableHeader element).
    /// </summary>
    public static IContainer SemanticTableHeader(this IContainer container)
    {
        return container.SemanticTag("THead");
    }
    
    /// <summary>
    /// A group of rows that constitute the main body portion of a table.
    /// If the table is split across multiple pages, the body area may be broken apart on a row boundary.
    /// A table may have multiple SemanticTableBody elements to allow for the drawing of a border or background for a set of rows.
    /// </summary>
    public static IContainer SemanticTableBody(this IContainer container)
    {
        return container.SemanticTag("TBody");
    }

    /// <summary>
    /// A group of rows that constitute the footer of a table.
    /// If the table is split across multiple pages, these rows may be redrawn at the bottom of each table fragment (although there is only one SemanticTableFooter element).
    /// </summary>
    public static IContainer SemanticTableFooter(this IContainer container)
    {
        return container.SemanticTag("TFoot");
    }
    
    #endregion
    
    #region Inline Elements
    
    /// <summary>
    /// A generic inline portion of text having no particular inherent characteristics.
    /// It can be used, for example, to delimit a range of text with a given set of styling attributes.
    /// </summary>
    public static IContainer SemanticSpan(this IContainer container, string? alternativeText = null)
    {
        return container.SemanticTag("Span", alternativeText);
    }
    
    /// <summary>
    /// An inline portion of text attributed to someone other than the author of the surrounding text.
    /// The quoted text should be contained inline within a single paragraph.
    /// This differs from the block-level element SemanticBlockQuotation, which consists of one or more complete paragraphs (or other elements presented as if they were complete paragraphs).
    /// </summary>
    public static IContainer SemanticQuote(this IContainer container)
    {
        return container.SemanticTag("Quote");
    }

    /// <summary>
    /// A fragment of computer program text.
    /// </summary>
    public static IContainer SemanticCode(this IContainer container)
    {
        return container.SemanticTag("Code");
    }

    /// <summary>
    /// Applies the semantic "Link" tag to the specified container.
    /// This is used to signify that the content represents a link or hyperlink within a document.
    /// </summary>
    public static IContainer SemanticLink(this IContainer container, string alternativeText)
    {
        return container.SemanticTag("Link", alternativeText: alternativeText);
    }
    
    #endregion
    
    #region Illustration Elements
    
    /// <summary>
    /// An item of graphical content.
    /// </summary>
    public static IContainer SemanticFigure(this IContainer container, string alternativeText)
    {
        return container.SemanticTag("Figure", alternativeText: alternativeText);
    }
    
    /// <summary>
    /// An alias for a SemanticFigure.
    /// </summary>
    public static IContainer SemanticImage(this IContainer container, string alternativeText)
    {
        return container.SemanticFigure(alternativeText);
    }
    
    /// <summary>
    /// A mathematical formula.
    ///
    /// This structure type is useful only for identifying an entire content element as a formula.
    /// No standard structure types are defined for identifying individual components within the formula.
    /// From a formatting standpoint, the formula shall be treated similarly to a figure.
    /// </summary>
    public static IContainer SemanticFormula(this IContainer container, string alternativeText)
    {
        return container.SemanticTag("Formula", alternativeText: alternativeText);
    }
    
    #endregion
}