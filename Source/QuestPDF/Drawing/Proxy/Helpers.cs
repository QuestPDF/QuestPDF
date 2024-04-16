using System.Linq;
using System.Text;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal static class Helpers
{
    internal static SpacePlan TryMeasureWithOverflow(this Element element, Size availableSpace)
    {
        return TryVerticalOverflow()
               ?? TryHorizontalOverflow() 
               ?? TryUnconstrainedOverflow()
               ?? SpacePlan.Wrap();

        SpacePlan? TryOverflow(Size targetSpace)
        {
            var contentSize = element.Measure(targetSpace);
            return contentSize.Type == SpacePlanType.Wrap ? null : contentSize;
        }
    
        SpacePlan? TryVerticalOverflow()
        {
            var overflowSpace = new Size(availableSpace.Width, Size.Infinity);
            return TryOverflow(overflowSpace);
        }
    
        SpacePlan? TryHorizontalOverflow()
        {
            var overflowSpace = new Size(Size.Infinity, availableSpace.Height);
            return TryOverflow(overflowSpace);
        }
    
        SpacePlan? TryUnconstrainedOverflow()
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
            // before assessing the element,
            // reset layout state by measuring the element with original space
            // in case when parent has altered the layout state with different overflow test
            element.Value.Measure(element.Value.MeasurementSize);
            
            // element was not part of the current layout measurement,
            // it could not impact the process
            if (element.Value.SpacePlanType is null)
                return;
            
            // element renders fully,
            // it could not impact the process
            if (element.Value.SpacePlanType is SpacePlanType.FullRender)
                return;

            // when element is partially rendering, it likely has no issues,
            // however, in certain cases, it may contain a child that is a root cause
            if (element.Value.SpacePlanType is SpacePlanType.PartialRender)
            {
                foreach (var child in element.Children)
                    Traverse(child);
                
                return;
            }
            
            // all of the code below relates to element that is wrapping,
            // it could be root cause, or contain a child (even deeply nested) that is the root cause
            
            // strategy
            // element does not contain any wrapping elements, no obvious root causes,
            // if it renders fully with extended space, it is a layout root cause
            if (element.Children.All(x => x.Value.SpacePlanType is not SpacePlanType.Wrap) && MeasureElementWithExtendedSpace() is SpacePlanType.NoContent or SpacePlanType.FullRender)
            {
                // so apply the layout overflow proxy
                element.Value.CreateProxy(x => new LayoutOverflowVisualization { Child = x });
                return;
            }

            // every time a measurement is made, the layout state is mutated
            // the previous strategy could modify the layout state
            // reset layout state by measuring the element with original space
            element.Value.Measure(element.Value.MeasurementSize); 
            
            // strategy:
            // element contains wrapping children, they are likely the root cause,
            // traverse them and attempt to fix them
            foreach (var child in element.Children.Where(x => x.Value.SpacePlanType is SpacePlanType.Wrap))
                Traverse(child);
                
            // check if fixing wrapping children helped
            if (MeasureElementWithExtendedSpace() is not SpacePlanType.Wrap)
                return;

            // reset layout state by measuring the element with original space
            element.Value.Measure(element.Value.MeasurementSize); // reset state
            
            // strategy:
            // element has layout issues but no obvious/trivial root causes
            // possibly the problem is in nested children of partial rendering children
            foreach (var child in element.Children.Where(x => x.Value.SpacePlanType is SpacePlanType.PartialRender))
                Traverse(child);
                
            // check if fixing partial children helped
            if (MeasureElementWithExtendedSpace() is not SpacePlanType.Wrap)
                return;
            
            // none of the attempts above have fixed the layout issue
            // the element itself is the root cause
            element.Value.CreateProxy(x => new LayoutOverflowVisualization { Child = x });

            SpacePlanType MeasureElementWithExtendedSpace()
            {
                return element.Value.TryMeasureWithOverflow(element.Value.MeasurementSize).Type;
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