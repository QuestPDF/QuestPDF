using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing.Proxy;

internal static class Helpers
{
    public static void ApplyInfiniteLayoutDebugging(this Container container)
    {
        container.VisitChildren(x =>
        {
            x.CreateProxy(y => y is ElementProxy ? y : new OverflowDebuggingProxy(y));
        });
    }
    
    public static void ApplyOverlayDebugging(this TreeNode<OverflowDebuggingProxy> hierarchyRoot)
    {
        Traverse(hierarchyRoot);
        
        void Traverse(TreeNode<OverflowDebuggingProxy> parent)
        {
            if (parent.Value.SpacePlanType == null)
                return;
            
            if (parent.Value.SpacePlanType == SpacePlanType.FullRender)
                return;

            var hasWraps = parent.Children.Any(x => x.Value.SpacePlanType == SpacePlanType.Wrap);
            var hasPartialRenders = parent.Children.Any(x => x.Value.SpacePlanType == SpacePlanType.PartialRender);

            if (hasWraps || hasPartialRenders)
            {
                foreach (var child in parent.Children)
                    Traverse(child);
            }
            else
            {
                parent.Value.CreateProxy(x => new ContentOverflowDebugArea { Child = x });
            }
        }
    }

    public static void RemoveExistingProxies(this Container content)
    {
        content.VisitChildren(x =>
        {
            x.CreateProxy(y => y is ElementProxy proxy ? proxy.Child : y);
        });
    }
}