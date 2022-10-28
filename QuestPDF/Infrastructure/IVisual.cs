namespace QuestPDF.Infrastructure
{
    internal interface IVisual
    {
        public bool IsRendered { get; set; }
        public bool RepeatContent { get; set; }
    }
}