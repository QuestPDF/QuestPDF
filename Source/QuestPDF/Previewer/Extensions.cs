using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer;

internal static class PreviewerModelExtensions
{
    internal static PreviewerCommands.UpdateDocumentStructure.DocumentHierarchyElement ExtractHierarchy(this Element container)
    {
        var layoutTree = container.ExtractElementsOfType<LayoutProxy>().Single();
        return Traverse(layoutTree);
        
        PreviewerCommands.UpdateDocumentStructure.DocumentHierarchyElement Traverse(TreeNode<LayoutProxy> node)
        {
            var layout = node.Value;
            
            if (layout.Child is Container or SnapshotRecorder or ElementProxy)
                return Traverse(node.Children.Single());
            
            var element = new PreviewerCommands.UpdateDocumentStructure.DocumentHierarchyElement
            {
                ElementType = layout.Child.GetType().Name.PrettifyName(),
                
                PageLocations = layout.Snapshots,
                
                IsSingleChildContainer = layout.Child is ContainerElement,
                Properties = layout
                    .Child
                    .GetElementConfiguration()
                    .Select(x => new PreviewerCommands.ElementProperty
                    {
                        Label = x.Property,
                        Value = x.Value
                    })
                    .ToList(),
                
                Children = node.Children.Select(Traverse).ToList()
            };

            return element;
        }
    }
    
    internal static ICollection<PreviewerCommands.ShowLayoutError.Ancestor> MapLayoutErrorAncestors(this ICollection<Element> ancestors)
    {
        return ancestors
            .Select(x => new PreviewerCommands.ShowLayoutError.Ancestor
            {
                ElementType = x.GetType().Name.PrettifyName(),
                Properties = x.GetElementProperties(),
            })
            .ToArray();
    }
        
    internal static PreviewerCommands.ShowLayoutError.LayoutErrorElement MapLayoutErrorHierarchy(this TreeNode<OverflowDebuggingProxy> node)
    {
        var layoutElement = node.Value.Child;
        var layoutMetrics = node.Value;

        if (layoutMetrics.Child is Container or SnapshotRecorder or ElementProxy or LayoutOverflowVisualization)
            return MapLayoutErrorHierarchy(node.Children.Single());

        var isAssessed = layoutMetrics.SpacePlan is not null;
            
        return new PreviewerCommands.ShowLayoutError.LayoutErrorElement
        {
            ElementType = layoutElement.GetType().Name.PrettifyName(),
                
            IsSingleChildContainer = layoutMetrics.Child is ContainerElement,
            Properties = layoutElement.GetElementProperties(),
                
            Children = isAssessed 
                ? node.Children.Select(MapLayoutErrorHierarchy).ToList() 
                : ArraySegment<PreviewerCommands.ShowLayoutError.LayoutErrorElement>.Empty,
                
            AvailableSpace = layoutMetrics.AvailableSpace?.Map(),
            MeasurementSize = ((Size?)layoutMetrics.SpacePlan)?.Map(),
            SpacePlanType = layoutMetrics.SpacePlan?.Type,
            WrapReason = layoutMetrics.SpacePlan?.WrapReason
        };
    }
    
    private static ICollection<PreviewerCommands.ElementProperty> GetElementProperties(this Element element)
    {
        return element
            .GetElementConfiguration()
            .Select(x => new PreviewerCommands.ElementProperty { Label = x.Property, Value = x.Value })
            .ToArray();
    }

    private static PreviewerCommands.ElementSize Map(this Size element)
    {
        return new PreviewerCommands.ElementSize
        {
            Width = element.Width,
            Height = element.Height
        };
    }
    
    internal static PreviewerCommands.ShowGenericException.StackFrame[] ParseStackTrace(this string stackTrace)
    {
        var lines = stackTrace.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
        
        var frames = new List<PreviewerCommands.ShowGenericException.StackFrame>();

        foreach (string line in lines)
        {
            var match = Regex.Match(line, @"at\s+([\w\.]+).*?\s+in\s+(.*?):line\s+(\d+)");
            
            if (match.Success)
            {
                frames.Add(new PreviewerCommands.ShowGenericException.StackFrame
                {
                    MethodName = match.Groups[1].Value,
                    FileName = match.Groups[2].Value,
                    LineNumber = int.Parse(match.Groups[3].Value)
                });
            }
        }

        return frames.ToArray();
    }
}