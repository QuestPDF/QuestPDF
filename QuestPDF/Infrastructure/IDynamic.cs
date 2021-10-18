using QuestPDF.Elements;

namespace QuestPDF.Infrastructure
{
    public interface IDynamic
    {
        void Reset();
        bool Compose(DynamicContext context, IContainer container);
    }
}