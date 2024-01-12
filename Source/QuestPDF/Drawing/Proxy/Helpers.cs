using System.Linq;
using System.Text;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal static class Helpers
{
    internal static Size? TryMeasureWithOverflow(this Element element, Size availableSpace)
    {
        return TryVerticalOverflow()
               ?? TryHorizontalOverflow() 
               ?? TryUnconstrainedOverflow();

        Size? TryOverflow(Size targetSpace)
        {
            var contentSize = element.Measure(targetSpace);
            return contentSize.Type == SpacePlanType.Wrap ? null : contentSize;
        }
    
        Size? TryVerticalOverflow()
        {
            var overflowSpace = new Size(availableSpace.Width, Size.Infinity);
            return TryOverflow(overflowSpace);
        }
    
        Size? TryHorizontalOverflow()
        {
            var overflowSpace = new Size(Size.Infinity, availableSpace.Height);
            return TryOverflow(overflowSpace);
        }
    
        Size? TryUnconstrainedOverflow()
        {
            var overflowSpace = new Size(Size.Infinity, Size.Infinity);
            return TryOverflow(overflowSpace);
        }
    }
    
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
        
        void Traverse(TreeNode<OverflowDebuggingProxy> element)
        {
            if (element.Value.SpacePlanType is null or SpacePlanType.FullRender)
                return;

            if (element.Value.SpacePlanType is SpacePlanType.PartialRender)
            {
                foreach (var child in element.Children)
                    Traverse(child);
                
                return;
            }
            
            // strategy: element wrap can be caused by any child that returned wrap
            if (element.Children.Any(x => x.Value.SpacePlanType == SpacePlanType.Wrap))
            {
                if (TryFixChildrenOfType(SpacePlanType.Wrap))
                    return;
            }
 
            // strategy: there could be more complex inner/hidden layout constraint issue,
            // if element cannot be successfully drawn on infinite canvas
            if (element.Value.TryMeasureWithOverflow(element.Value.MeasurementSize) == null)
            {
                if (TryFixChildrenOfType(SpacePlanType.PartialRender))
                    return;
            }
  
            // fixing children does not help, fix the element itself
            element.Value.RemoveExistingProxies();
            element.Value.CreateProxy(x => new LayoutOverflowVisualization { Child = x });

            bool TryFixChildrenOfType(SpacePlanType spacePlanType)
            {
                var suspectedChildren = element.Children.Where(x => x.Value.SpacePlanType == spacePlanType);

                if (!suspectedChildren.Any())
                    return false;

                foreach (var child in suspectedChildren)
                    Traverse(child);

                return element.Value.TryMeasureWithOverflow(element.Value.MeasurementSize).HasValue;
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