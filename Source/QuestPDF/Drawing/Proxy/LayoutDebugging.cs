using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal static class LayoutDebugging
{
    internal static SpacePlan TryMeasureWithOverflow(this Element element, Size availableSpace)
    {
        return TryVerticalOverflow()
               ?? TryHorizontalOverflow() 
               ?? TryUnconstrainedOverflow()
               ?? SpacePlan.Wrap("Extending the available space does not allow the child to fit on the page.");

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
    
    public static void TryToFixTheLayoutOverflowIssue(this TreeNode<OverflowDebuggingProxy> hierarchyRoot)
    {
        Traverse(hierarchyRoot);
        
        void Traverse(TreeNode<OverflowDebuggingProxy> element)
        {
            if (element.Value.Child is DebugPointer or SourceCodePointer or Container)
            {
                Traverse(element.Children.First());
                return;
            }
            
            if (element.Value.AvailableSpace is null)
                return;
            
            // element was not part of the current layout measurement,
            // it could not impact the process
            if (element.Value.SpacePlan is null)
                return;
            
            // element is empty,
            // it could not impact the process
            if (element.Value.SpacePlan?.Type is SpacePlanType.Empty)
                return;
            
            // element renders fully,
            // it could not impact the process
            if (element.Value.SpacePlan?.Type is SpacePlanType.FullRender)
                return;

            // when the current element is partially rendering, it likely has no issues,
            // however, in certain cases, it may contain a child that is a root cause
            if (element.Value.SpacePlan?.Type is SpacePlanType.PartialRender)
            {
                foreach (var child in element.Children)
                    Traverse(child);
                
                return;
            }
            
            // all the code below relates to element that is wrapping,
            // it could be a root cause, or contain a child (even deeply nested) that is the root cause
            
            // strategy:
            // the current element does not contain any wrapping children, no obvious root causes,
            // if it renders at least partially with extended space, it is a layout root cause
            if (element.Children.All(x => x.Value.SpacePlan?.Type is not SpacePlanType.Wrap) && MeasureElementWithExtendedSpace() is not SpacePlanType.Wrap)
            {
                element.Value.CreateProxy(x => new LayoutOverflowVisualization { Child = x });
                return;
            }

            // strategy:
            // the current element contains wrapping children, they are likely the root cause,
            // traverse them and attempt to fix them
            foreach (var child in element.Children.Where(x => x.Value.SpacePlan?.Type is SpacePlanType.Wrap).ToList())
                Traverse(child);

            // check if fixing wrapping children resolved the issue under original constraints
            if (MeasureElementWithOriginalSpace() is not SpacePlanType.Wrap)
                return;

            // fixing wrapping children was not sufficient under original constraints;
            // if this element fits with extended space, it is also a root cause
            if (MeasureElementWithExtendedSpace() is not SpacePlanType.Wrap)
            {
                element.Value.CreateProxy(x => new LayoutOverflowVisualization { Child = x });
                return;
            }

            // strategy:
            // the current element has layout issues but no obvious/trivial root causes,
            // possibly the problem is in nested children of partial rendering children
            foreach (var child in element.Children.Where(x => x.Value.SpacePlan?.Type is SpacePlanType.PartialRender).ToList())
                Traverse(child);

            // check if fixing partial children resolved the issue under original constraints
            if (MeasureElementWithOriginalSpace() is not SpacePlanType.Wrap)
                return;

            // none of the attempts above have fixed the layout issue,
            // the element itself is the root cause
            element.Value.CreateProxy(x => new LayoutOverflowVisualization { Child = x });

            SpacePlanType MeasureElementWithExtendedSpace()
            {
                return element.Value.TryMeasureWithOverflow(element.Value.AvailableSpace!.Value).Type;
            }

            SpacePlanType MeasureElementWithOriginalSpace()
            {
                return element.Value.Measure(element.Value.AvailableSpace!.Value).Type;
            }
        }
    }

    public static void RemoveExistingProxies(this Element content)
    {
        content.RemoveExistingProxiesOfType<ElementProxy>();
    }
    
    public static void RemoveExistingProxiesOfType<TProxy>(this Element content) where TProxy : ElementProxy
    {
        content.VisitChildren(x =>
        {
            x.CreateProxy(y =>
            {
                if (y is not TProxy proxy)
                    return y;
                
                (proxy as IDisposable)?.Dispose();
                return proxy.Child;
            });
        });
    }

    public static void StopMeasuring(this TreeNode<OverflowDebuggingProxy> parent)
    {
        parent.Value.StopMeasuring();
            
        foreach (var child in parent.Children)
            StopMeasuring(child);
    }
    
    public static IEnumerable<TreeNode<OverflowDebuggingProxy>> FindLayoutOverflowVisualizationNodes(this TreeNode<OverflowDebuggingProxy> rootNode)
    {
        var result = new List<TreeNode<OverflowDebuggingProxy>>();
        Traverse(rootNode);
        return result;
        
        void Traverse(TreeNode<OverflowDebuggingProxy> node)
        {
            if (node.Value.Child is LayoutOverflowVisualization)
                result.Add(node);

            foreach (var child in node.Children)
                Traverse(child);
        }
    }
    
    public static string FormatAncestors(this IEnumerable<Element> ancestors)
    {
        var result = new StringBuilder();
        
        foreach (var ancestor in ancestors)
            Format(ancestor);
        
        return result.ToString();

        void Format(Element node)
        {
            if (node is DebugPointer debugPointer)
            {
                result.AppendLine($"-> {debugPointer.Label}");
            }
            else if (node is SourceCodePointer sourceCodePointer)
            {
                result.AppendLine($"-> In method:   {sourceCodePointer.MethodName}");
                result.AppendLine($"   Called from: {sourceCodePointer.CalledFrom}");
                result.AppendLine($"   Source path: {sourceCodePointer.FilePath}");
                result.AppendLine($"   Line number: {sourceCodePointer.LineNumber}");
            }
            else
            {
                
            }
            
            result.AppendLine();
        }
    }
    
    public static string FormatLayoutSubtree(this TreeNode<OverflowDebuggingProxy> root)
    {
        var indentationCache = Enumerable.Range(0, 128).Select(x => x * 3).Select(x => new string(' ', x)).ToArray();
        
        var indentationLevel = 0;
        var result = new StringBuilder();
        
        Traverse(root);
        
        return result.ToString();

        void Traverse(TreeNode<OverflowDebuggingProxy> parent)
        {
            var proxy = parent.Value;

            if (proxy.Child is Container)
            {
                Traverse(parent.Children.First());
                return;
            }
            
            var indent = indentationCache[indentationLevel];
            
            foreach (var content in Format(proxy))
                result.AppendLine($"{indent}{content}");
            
            result.AppendLine();
            result.AppendLine();
            
            if (proxy.AvailableSpace is null || proxy.SpacePlan is null)
                return;

            indentationLevel++;
            
            foreach (var child in parent.Children)
                Traverse(child);

            indentationLevel--;
        }

        static IEnumerable<string> Format(OverflowDebuggingProxy proxy)
        {
            var child = proxy.Child;
            
            if (child is LayoutOverflowVisualization layoutOverflowVisualization)
                child = layoutOverflowVisualization.Child;

            var title = GetTitle();
            yield return title;
            
            yield return new string('=', title.Length + 1);

            if (proxy is { AvailableSpace: not null, SpacePlan: not null })
            {
                yield return $"Available Space: {proxy.AvailableSpace}";
                yield return $"Space Plan: {proxy.SpacePlan}";
                
                if (proxy.SpacePlan?.Type == SpacePlanType.Wrap)
                    yield return "Wrap Reason: " + (proxy.SpacePlan?.WrapReason ?? "Unknown");
                
                yield return new string('-', title.Length + 1);
            }

            if (child is StyledBox styledBox)
            {
                foreach (var valueTuple in styledBox.GetCompanionCustomContent())
                    yield return $"{valueTuple.Item1}: {valueTuple.Item2}";
            }
            else
            {
                yield return $"Configuration: {child.GetCompanionHint() ?? "-"}";
            }
            
            
            string GetTitle()
            {
                var elementName = child.GetType().Name;
                
                if (proxy.Child is LayoutOverflowVisualization)
                    return $"🚨 {elementName} 🚨";
                
                var indicator = proxy.SpacePlan?.Type switch
                {
                    SpacePlanType.Wrap => "🔴",
                    SpacePlanType.PartialRender => "🟡",
                    SpacePlanType.FullRender => "🟢",
                    SpacePlanType.Empty => "🟢",
                    _ => "⚪️"
                };
                
                return $"{indicator} {elementName}";
            }
        }
        
    }

    public const string LayoutVisualizationLegend =
        "Legend: \n" +
        "🚨 - Element that is likely the root cause of the layout issue based on library heuristics and prediction. \n" +
        "🔴 - Element that cannot be drawn due to the provided layout constraints. This element likely causes the layout issue, or one of its descendant children is responsible for the problem. \n" +
        "🟡 - Element that can be partially drawn on the page and will also be rendered on the consecutive page. In more complex layouts, this element may also cause issues or contain a child that is the actual root cause.\n" +
        "🟢 - Element that is successfully and completely drawn on the page.\n" +
        "⚪️ - Element that has not been drawn on the faulty page. Its children are omitted.\n";
}