namespace QuestPDF.Infrastructure
{
    /// <summary>
    /// The QuestPDF library is available under a hybrid license favorable to both community and business users.
    /// For a comprehensive overview, please visit the <a href="https://www.questpdf.com/license">License and Pricing details webpage</a>.
    /// </summary>
    public enum LicenseType
    {
        /// <summary>
        /// For evaluating QuestPDF before choosing a license. Not permitted in production.
        /// <a href="https://www.questpdf.com/license">Learn more</a>
        /// </summary>
        Evaluation,

        /// <summary>
        /// Free license for individuals, non-profits, open-source projects, and organizations with less than $1M USD annual gross revenue.
        /// <a href="https://www.questpdf.com/license">Learn more</a>
        /// </summary>
        Community,

        /// <summary>
        /// Paid license covering a single company or individual, with unlimited developers.
        /// <a href="https://www.questpdf.com/license">Learn more</a>
        /// </summary>
        Professional,

        /// <summary>
        /// Paid license covering a company and its affiliates, with unlimited developers and prioritized support.
        /// <a href="https://www.questpdf.com/license">Learn more</a>
        /// </summary>
        Enterprise
    }
} 