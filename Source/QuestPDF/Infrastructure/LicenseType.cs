namespace QuestPDF.Infrastructure
{
    public enum LicenseType
    {
        /// <summary>
        /// <para>We love and highly appreciate the .NET Community and therefore the vast majority of users are welcome to use the library under the QuestPDF Community MIT License, without any limitations, even for commercial usage. Please kindly check if you are eligible to use this license.</para>
        /// <para>License comparison: http://www.questpdf.com/pricing.html#license</para>
        /// <para>License link: https://www.questpdf.com/license-community.html</para>
        /// </summary>
        Community,
        
        /// <summary>
        /// <para>You must purchase the QuestPDF Professional license, if you are consuming the QuestPDF library as a Direct Package Dependency for usage in a Closed Source software in the capacity of a for-profit company/individual with more than 1M USD annual gross revenue, <c>and there are up to 10 developers</c>.</para>
        /// <para>If the number of developers is more than 10, you must purchase the QuestPDF Enterprise license.</para>
        /// <para>Before making a license purchase, please evaluate the library in a non-production environment.</para>
        /// <para>License comparison: http://www.questpdf.com/pricing.html#license</para>
        /// <para>License link: https://www.questpdf.com/license-commercial.html</para>
        /// </summary>
        Professional,
        
        /// <summary>
        /// <para>You must purchase the QuestPDF Professional license, if you are consuming the QuestPDF library as a Direct Package Dependency for usage in a Closed Source software in the capacity of a for-profit company/individual with more than 1M USD annual gross revenue.</para>
        /// <para>If there are less than 10 developers, you are eligible to use the cheaper QuestPDF Professional License.</para>
        /// <para>Before making a license purchase, please evaluate the library in a non-production environment.</para>
        /// <para>License comparison: http://www.questpdf.com/pricing.html#license</para>
        /// <para>License link: https://www.questpdf.com/license-commercial.html</para>
        /// </summary>
        Enterprise
    }
}