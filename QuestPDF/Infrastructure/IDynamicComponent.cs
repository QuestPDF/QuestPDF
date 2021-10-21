using QuestPDF.Elements;

namespace QuestPDF.Infrastructure
{
    public interface IDynamicComponent
    {
        void Compose(DynamicContext context, IDynamicContainer container);
    }
}