using QuestPDF.Fluent;

namespace QuestPDF.Infrastructure
{
    public enum AspectRatioOption
    {
        /// <summary>
        /// Adjusts content to occupy the full width available.
        /// </summary>
        /// <remarks>
        /// Used as the default setting in the library.
        /// </remarks>
        FitWidth,
        
        /// <summary>
        /// Adjusts content to fill the available height.
        /// </summary>
        /// <remarks>
        /// Often used with height-constraining elements such as: <see cref="ConstrainedExtensions.Height">Height</see>, <see cref="ConstrainedExtensions.MaxHeight">MaxHeight</see>, etc.
        /// </remarks>
        FitHeight,
        
        /// <summary>
        /// Adjusts content to fill the available area while maintaining its aspect ratio. This may result in the content fully occupying either the width or height, depending on its dimensions.
        /// </summary>
        /// <remarks>
        /// Often used with constraining elements such as: <see cref="ConstrainedExtensions.Width">Width</see>, <see cref="ConstrainedExtensions.MaxWidth">MaxWidth</see>, <see cref="ConstrainedExtensions.Height">Height</see>, <see cref="ConstrainedExtensions.MaxHeight">MaxHeight</see>, etc.
        /// </remarks>
        FitArea
    }
}