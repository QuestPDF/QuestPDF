namespace QuestPDF.Infrastructure
{
    public enum ImageCompressionQuality
    {
        /// <summary>
        /// JPEG format with compression set to 100 out of 100
        /// </summary>
        Best,

        /// <summary>
        /// JPEG format with compression set to 90 out of 100
        /// </summary>
        VeryHigh,

        /// <summary>
        /// JPEG format with compression set to 80 out of 100
        /// </summary>
        High,

        /// <summary>
        /// JPEG format with compression set to 60 out of 100
        /// </summary>
        Medium,

        /// <summary>
        /// JPEG format with compression set to 40 out of 100
        /// </summary>
        Low,

        /// <summary>
        /// JPEG format with compression set to 20 out of 100
        /// </summary>
        VeryLow
    }
}