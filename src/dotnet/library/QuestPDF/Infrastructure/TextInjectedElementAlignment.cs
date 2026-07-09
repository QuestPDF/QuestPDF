namespace QuestPDF.Infrastructure;

public enum TextInjectedElementAlignment
{
    /// <summary>
    /// Aligns the bottom edge of the injected element with the text baseline. The injected element sits on top of the baseline.
    /// </summary>
    AboveBaseline,

    /// <summary>
    /// Aligns the top edge of the injected element with the text baseline. The injected element hangs below the baseline.
    /// </summary>
    BelowBaseline,

    /// <summary>
    /// Aligns the top edge of the injected element with the top edge of the font. If the injected element is very tall, the extra space will hang from the top and extend through the bottom of the line.
    /// </summary>
    Top,

    /// <summary>
    /// Aligns the bottom edge of the injected element with the top edge of the font. If the injected element is very tall, the extra space will rise from the bottom and extend through the top of the line.
    /// </summary>
    Bottom,

    /// <summary>
    /// Aligns the middle of the injected element with the middle of the text. If the injected element is very tall, the extra space will grow equally from the top and bottom of the line.
    /// </summary>
    Middle
}