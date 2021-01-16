using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Container : ContainerElement
    {
        internal Container()
        {
            Child = new Empty();
        }
    }
}