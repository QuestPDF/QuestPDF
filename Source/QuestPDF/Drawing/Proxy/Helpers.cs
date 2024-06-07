using System;
using System.Collections;
using System.Collections.Generic;
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
            if (element.Value.MeasurementSize is null)
                return;
            
            // before assessing the element,
            // reset layout state by measuring the element with original space
            // in case when parent has altered the layout state with different overflow test
            element.Value.Measure(element.Value.MeasurementSize.Value);
            
            // element was not part of the current layout measurement,
            // it could not impact the process
            if (element.Value.SpacePlan is null)
                return;
            
            // element renders fully,
            // it could not impact the process
            if (element.Value.SpacePlan?.Type is SpacePlanType.FullRender)
                return;

            // when element is partially rendering, it likely has no issues,
            // however, in certain cases, it may contain a child that is a root cause
            if (element.Value.SpacePlan?.Type is SpacePlanType.PartialRender)
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
            if (element.Children.All(x => x.Value.SpacePlan?.Type is not SpacePlanType.Wrap) && MeasureElementWithExtendedSpace() is SpacePlanType.FullRender)
            {
                // so apply the layout overflow proxy
                element.Value.CreateProxy(x => new LayoutOverflowVisualization { Child = x });
                return;
            }

            // every time a measurement is made, the layout state is mutated
            // the previous strategy could modify the layout state
            // reset layout state by measuring the element with original space
            element.Value.Measure(element.Value.MeasurementSize.Value); 
            
            // strategy:
            // element contains wrapping children, they are likely the root cause,
            // traverse them and attempt to fix them
            foreach (var child in element.Children.Where(x => x.Value.SpacePlan?.Type is SpacePlanType.Wrap))
                Traverse(child);
                
            // check if fixing wrapping children helped
            if (MeasureElementWithExtendedSpace() is not SpacePlanType.Wrap)
                return;

            // reset layout state by measuring the element with original space
            element.Value.Measure(element.Value.MeasurementSize.Value); // reset state
            
            // strategy:
            // element has layout issues but no obvious/trivial root causes
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

    public static void CaptureOriginalValues(this TreeNode<OverflowDebuggingProxy> parent)
    {
        parent.Value.CaptureOriginalValues();
            
        foreach (var child in parent.Children)
            CaptureOriginalValues(child);
    }

    public static TreeNode<OverflowDebuggingProxy>? FindLayoutOverflowVisualization(this TreeNode<OverflowDebuggingProxy> element)
    {
        TreeNode<OverflowDebuggingProxy> result = null;
        Traverse(element);
        return result;
        
        void Traverse(TreeNode<OverflowDebuggingProxy> currentElement)
        {
            if (currentElement.Value.Child is LayoutOverflowVisualization)
            {
                result = currentElement;
                return;
            }

            foreach (var child in currentElement.Children)
                Traverse(child);
        }
    }
    
    public static ICollection<TreeNode<OverflowDebuggingProxy>> ExtractAncestors(this TreeNode<OverflowDebuggingProxy> element)
    {
        var parent = element;
        var result = new List<TreeNode<OverflowDebuggingProxy>>();
        
        while (parent is not null)
        {
            result.Add(parent);
            parent = parent.Parent;
        }

        return result;
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
            
            if (proxy.OriginalMeasurementSize is null || proxy.OriginalSpacePlan is null)
                return;
            
            var indent = indentationCache[indentationLevel];
            
            foreach (var content in Format(proxy))
                result.AppendLine($"{indent} {content}");
            
            result.AppendLine();
            result.AppendLine();
            
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
            
            yield return $"{SpacePlanDot()} {child.GetType().Name}";
            
            yield return new string('=', child.GetType().Name.Length + 4);
            
            yield return $"Available Space: {proxy.OriginalMeasurementSize}";
            yield return $"Space Plan: {proxy.OriginalSpacePlan}";
            
            yield return new string('-', child.GetType().Name.Length + 4);
            
            foreach (var configuration in GetElementConfiguration(child))
                yield return $"{configuration}";
            
            string SpacePlanDot()
            {
                return proxy.OriginalSpacePlan.Value.Type switch
                {
                    SpacePlanType.Wrap => "🔴",
                    SpacePlanType.PartialRender => "🟡",
                    SpacePlanType.FullRender => "🟢",
                    _ => "-"
                };
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
                //return text.AsSpan(0, maxLength).ToString() + "...";
            }
        }
    }
}