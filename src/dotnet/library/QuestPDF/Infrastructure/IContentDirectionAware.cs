namespace QuestPDF.Infrastructure
{
    internal interface IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
    }
}