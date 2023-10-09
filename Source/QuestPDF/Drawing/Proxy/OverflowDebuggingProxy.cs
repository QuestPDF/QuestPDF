using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal class OverflowDebuggingProxy : ElementProxy
{
    public SpacePlanType? SpacePlanType { get; private set; }

    public OverflowDebuggingProxy(Element child)
    {
        Child = child;
    }

    internal override SpacePlan Measure(Size availableSpace)
    {
        var spacePlan = Child.Measure(availableSpace);
        SpacePlanType = spacePlan.Type;
        return spacePlan;
    }
}