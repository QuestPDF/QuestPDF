namespace QuestPDF.Infrastructure
{
    public enum ContentDirection
    {
        /// <summary>
        /// Sets the left-to-right (LTR) content direction.
        /// <a href="https://www.questpdf.com/api-reference/content-direction.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="contentDirection.ltr.remarks"]/*' />
        LeftToRight,
        
        /// <summary>
        /// Sets the right-to-left (RTL) content direction.
        /// <a href="https://www.questpdf.com/api-reference/content-direction.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="contentDirection.rtl.remarks"]/*' />
        RightToLeft
    }
}