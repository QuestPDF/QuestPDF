namespace QuestPDF.Elements.Table
{
    internal sealed class TableColumnDefinition
    {
        public float ConstantSize { get; }
        public float RelativeSize { get; }

        internal float Width { get; set; }

        public bool AllowShrink { get; set; }
        public bool AllowGrow { get; set; }

        public TableColumnDefinition(float constantSize, float relativeSize, bool allowShrink, bool allowGrow)
        {
            ConstantSize = constantSize;
            RelativeSize = relativeSize;
            AllowShrink = allowShrink;
            AllowGrow = allowGrow;
        }
    }
}