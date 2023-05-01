namespace QuestPDF.Infrastructure
{
    public enum ImageQuality
    {
        /// <summary>
        /// PNG format with alpha support
        /// </summary>
        Lossless = 101,
        
        /// <summary>
        /// JPEG format with compression set to 100 out of 100
        /// </summary>
        Max = 100,
        
        /// <summary>
        /// JPEG format with compression set to 90 out of 100
        /// </summary>
        VeryHigh = 90,
        
        /// <summary>
        /// JPEG format with compression set to 80 out of 100
        /// </summary>
        High = 80,
        
        /// <summary>
        /// JPEG format with compression set to 60 out of 100
        /// </summary>
        Medium = 60,

        /// <summary>
        /// JPEG format with compression set to 40 out of 100
        /// </summary>
        Low = 40,
        
        /// <summary>
        /// JPEG format with compression set to 20 out of 100
        /// </summary>
        VeryLow = 20
    }
}