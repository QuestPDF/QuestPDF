namespace QuestPDF.Infrastructure
{
    public enum ImageCompressionQuality
    {
        /// <summary>
        /// JPEG format with target quality set to 100 out of 100
        /// </summary>
        Best,

        /// <summary>
        /// JPEG format with target quality set to 90 out of 100
        /// </summary>
        VeryHigh,

        /// <summary>
        /// JPEG format with target quality set to 75 out of 100
        /// </summary>
        High,

        /// <summary>
        /// JPEG format with target quality set to 50 out of 100
        /// </summary>
        Medium,

        /// <summary>
        /// JPEG format with target quality set to 25 out of 100
        /// </summary>
        Low,

        /// <summary>
        /// JPEG format with target quality set to 10 out of 100
        /// </summary>
        VeryLow
    }
}