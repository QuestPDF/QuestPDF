using System.Linq;
using System.Text;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal static class Helpers
{
    public static void ApplyLayoutOverflowDetection(this Element container)
    {
        container.VisitChildren(x =>
        {
            x.CreateProxy(y => y is ElementProxy ? y : new OverflowDebuggingProxy(y));
        });
    }
    
    public static void ApplyLayoutOverflowVisualization(this TreeNode<OverflowDebuggingProxy> hierarchyRoot)
    {
        Traverse(hierarchyRoot);
        
        void Traverse(TreeNode<OverflowDebuggingProxy> parent)
        {
            switch (parent.Value.SpacePlanType)
            {
                case null:
                    return;
                case SpacePlanType.FullRender:
                    return;
            }

            var childrenWithWraps = parent.Children.Where(x => x.Value.SpacePlanType is SpacePlanType.Wrap).ToList();
            var childrenWithPartialRenders = parent.Children.Where(x => x.Value.SpacePlanType is SpacePlanType.PartialRender).ToList();

            if (childrenWithWraps.Any())
            {
                childrenWithWraps.ForEach(Traverse);
            }
            else if (childrenWithPartialRenders.Any())
            {
                childrenWithPartialRenders.ForEach(Traverse);
            }
            else
            {
                parent.Value.CreateProxy(x => new LayoutOverflowVisualization { Child = x });
            }
        }
    }

    public static void RemoveExistingProxies(this Element content)
    {
        content.VisitChildren(x =>
        {
            x.CreateProxy(y => y is ElementProxy proxy ? proxy.Child : y);
        });
    }

    public static string HierarchyToString(this Element root)
    {
        var indentationCache = Enumerable.Range(0, 128).Select(x => new string(' ', x)).ToArray();
        
        var indentationLevel = 0;
        var result = new StringBuilder();
        
        Traverse(root);

        return result.ToString();
        
        void Traverse(Element parent)
        {
            var elementName = (parent as DebugPointer)?.Target ?? parent.GetType().Name;
            
            result.AppendLine();
            result.Append(indentationCache[indentationLevel]);
            result.Append(elementName);

            indentationLevel++;
            
            foreach (var child in parent.GetChildren())
                Traverse(child);

            indentationLevel--;
        }
    }
}