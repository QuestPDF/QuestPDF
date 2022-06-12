namespace QuestPDF.Infrastructure
{
    internal interface IContentDirectionAware
    {
        public ContentDirectionType ContentDirection { get; set; }
    }
}