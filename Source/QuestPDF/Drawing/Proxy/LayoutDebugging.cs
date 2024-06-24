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
    
    public static void ApplyLayoutOverflowVisualization(this TreeNode<OverflowDebuggingProxy> hierarchyRoot)
    {
        Traverse(hierarchyRoot);
        
        void Traverse(TreeNode<OverflowDebuggingProxy> element)
        {
            if (element.Value.MeasurementSize is null)
                return;
            
            // element was not part of the current layout measurement,
            // it could not impact the process
            if (element.Value.SpacePlan is null)
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
            
            // strategy
            // the current does not contain any wrapping elements, no obvious root causes,
            // if it renders fully with extended space, it is a layout root cause
            if (element.Children.All(x => x.Value.SpacePlan?.Type is not SpacePlanType.Wrap) && MeasureElementWithExtendedSpace() is SpacePlanType.FullRender)
            {
                // so apply the layout overflow proxy
                element.Value.CreateProxy(x => new LayoutOverflowVisualization { Child = x });
                return;
            }

            // strategy:
            // the current contains wrapping children, they are likely the root cause,
            // traverse them and attempt to fix them
            foreach (var child in element.Children.Where(x => x.Value.SpacePlan?.Type is SpacePlanType.Wrap))
                Traverse(child);
                
            // check if fixing wrapping children helped
            if (MeasureElementWithExtendedSpace() is not SpacePlanType.Wrap)
                return;

            // strategy:
            // the current has layout issues but no obvious/trivial root causes
            // possibly the problem is in nested children of partial rendering children
            foreach (var child in element.Children.Where(x => x.Value.SpacePlan?.Type is SpacePlanType.PartialRender))
                Traverse(child);
                
            // check if fixing partial children helped
            if (MeasureElementWithExtendedSpace() is not SpacePlanType.Wrap)
                return;
            
            // none of the attempts above have fixed the layout issue
            // the element itself is the root cause
            element.Value.CreateProxy(x => new LayoutOverflowVisualization { Child = x });

            SpacePlanType MeasureElementWithExtendedSpace()
            {
                return element.Value.TryMeasureWithOverflow(element.Value.MeasurementSize.Value).Type;
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
    
    public static string FormatAncestors(this IEnumerable<OverflowDebuggingProxy> ancestors)
    {
        var result = new StringBuilder();
        
        foreach (var ancestor in ancestors)
            Format(ancestor);
        
        return result.ToString();

        void Format(OverflowDebuggingProxy node)
        {
            if (node.Child is DebugPointer debugPointer)
            {
                result.AppendLine($"-> {debugPointer.Label}");
            }
            else if (node.Child is SourceCodePointer sourceCodePointer)
            {
                result.AppendLine($"-> In method:   {sourceCodePointer.HandlerName}");
                result.AppendLine($"   Called from: {sourceCodePointer.ParentName}");
                result.AppendLine($"   Source path: {sourceCodePointer.SourceFilePath}");
                result.AppendLine($"   Line number: {sourceCodePointer.SourceLineNumber}");
            }
            else
            {
                return;
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
            
            if (proxy.MeasurementSize is null || proxy.SpacePlan is null)
                return;
            
            var indent = indentationCache[indentationLevel];
            
            foreach (var content in Format(proxy))
                result.AppendLine($"{indent}{content}");
            
            result.AppendLine();
            result.AppendLine();
            
            if (!proxy.SpacePlan.HasValue)
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
            
            yield return $"Available Space: {proxy.MeasurementSize}";
            yield return $"Space Plan: {proxy.SpacePlan}";

            if (proxy.SpacePlan?.Type == SpacePlanType.Wrap)
                yield return "Wrap Reason: " + (proxy.SpacePlan?.WrapReason ?? "Unknown");
            
            yield return new string('-', title.Length + 1);
            
            foreach (var configuration in GetElementConfiguration(child))
                yield return $"{configuration}";
            
            string GetTitle()
            {
                var elementName = child.GetType().Name;
                
                if (proxy.Child is LayoutOverflowVisualization)
                    return $"🚨 {elementName} 🚨";
                
                var indicator = proxy.SpacePlan.Value.Type switch
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
        
        static IEnumerable<string> GetElementConfiguration(IElement element)
        {
            if (element is DebugPointer)
                return Enumerable.Empty<string>();
                
            return element
                .GetType()
                .GetProperties()
                .Select(x => new
                {
                    Property = x.Name.PrettifyName(),
                    Value = x.GetValue(element)
                })
                .Where(x => !(x.Value is IElement))
                .Where(x => x.Value is string || !(x.Value is IEnumerable))
                .Where(x => !(x.Value is TextStyle))
                .Select(x => $"{x.Property}: {FormatValue(x.Value)}");

            string FormatValue(object value)
            {
                const int maxLength = 100;
                    
                var text = value?.ToString() ?? "-";

                if (text.Length < maxLength)
                    return text;

                return text.Substring(0, maxLength) + "...";
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