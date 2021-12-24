namespace QuestPDF.Elements.Table
{
    internal class TableColumnDefinition
    {
        public float ConstantSize { get;  }
        public float RelativeSize { get; }

        internal float Width { get; set; }

        public TableColumnDefinition(float constantSize, float relativeSize)
        {
            ConstantSize = constantSize;
            RelativeSize = relativeSize;
        }
    }
}