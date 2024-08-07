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
            var child = layout.Child;
            
            if (child is Container)
                return Traverse(node.Children.Single());
            
            while (child is SnapshotRecorder or ElementProxy)
                child = child.GetChildren().Single();
            
            var element = new PreviewerCommands.UpdateDocumentStructure.DocumentHierarchyElement
            {
                ElementType = child.GetType().Name.PrettifyName(),
                
                PageLocations = layout.Snapshots,
                LayoutErrorMeasurements = layout.LayoutErrorMeasurements,
                
                IsSingleChildContainer = child is ContainerElement,
                Properties = child
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
            .Select(Map)
            .ToArray();
        
        PreviewerCommands.ShowLayoutError.Ancestor Map(Element element)
        {
            if (element is SourceCodePointer sourceCodePointer)
            {
                return new PreviewerCommands.ShowLayoutError.Ancestor()
                {
                    Type = PreviewerCommands.ShowLayoutError.AncestorType.MethodInvocation,
                    Title = sourceCodePointer.MethodName,
                    FileName = sourceCodePointer.SourceFilePath,
                    LineNumber = sourceCodePointer.SourceLineNumber
                };
            }
            
            if (element is DebugPointer debugPointer)
            {
                return new PreviewerCommands.ShowLayoutError.Ancestor
                {
                    Type = MapDebugPointerType(debugPointer.Type),
                    Title = debugPointer.Label
                };
            }

            throw new Exception($"Unknown ancestor type: {element.GetType()}");
            
            PreviewerCommands.ShowLayoutError.AncestorType MapDebugPointerType(DebugPointerType type)
            {
                return type switch
                {
                    DebugPointerType.DocumentStructure => PreviewerCommands.ShowLayoutError.AncestorType.DocumentStructure,
                    DebugPointerType.ElementStructure => PreviewerCommands.ShowLayoutError.AncestorType.ElementStructure,
                    DebugPointerType.Component => PreviewerCommands.ShowLayoutError.AncestorType.Component,
                    DebugPointerType.Section => PreviewerCommands.ShowLayoutError.AncestorType.Section,
                    DebugPointerType.Dynamic => PreviewerCommands.ShowLayoutError.AncestorType.Dynamic,
                    DebugPointerType.UserDefined => PreviewerCommands.ShowLayoutError.AncestorType.UserDefined,
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };
            }
        }
    }
        
    internal static PreviewerCommands.ShowLayoutError.LayoutErrorElement MapLayoutErrorHierarchy(this TreeNode<OverflowDebuggingProxy> node)
    {
        var layoutElement = node.Value.Child;
        var layoutMetrics = node.Value;

        if (layoutElement is Container)
            return MapLayoutErrorHierarchy(node.Children.Single());

        if (layoutElement is SnapshotRecorder or ElementProxy or LayoutOverflowVisualization)
            layoutElement = layoutElement.GetChildren().Single();
                
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
            WrapReason = layoutMetrics.SpacePlan?.WrapReason,
            IsLayoutErrorRootCause = node.Value.Child is LayoutOverflowVisualization
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
            var match = Regex.Match(line, @"at\s+(.+)\sin\s(.+):line\s(\d+)");
            
            if (match.Success)
            {
                var locationParts = match.Groups[1].Value.Split('.');
                
                frames.Add(new PreviewerCommands.ShowGenericException.StackFrame
                {
                    Namespace = string.Join(".", locationParts.SkipLast(2)),
                    MethodName = string.Join(".", locationParts.TakeLast(2)),
                    FileName = match.Groups[2].Value,
                    LineNumber = int.Parse(match.Groups[3].Value)
                });
            }
        }

        return frames.ToArray();
    }
    
    internal static PreviewerCommands.UpdateDocumentStructure.DocumentHierarchyElement ImproveHierarchyStructure(this PreviewerCommands.UpdateDocumentStructure.DocumentHierarchyElement root)
    {
        var debugPointerName = nameof(DebugPointer).PrettifyName();
        
        var pointers = new Dictionary<DocumentStructureTypes, PreviewerCommands.UpdateDocumentStructure.DocumentHierarchyElement>();
        FindDebugPointers(root);
        
        var document = pointers[DocumentStructureTypes.Document];
        document.IsSingleChildContainer = false;
        
        document.Children = pointers
            .Where(x => x.Key != DocumentStructureTypes.Document)
            .Select(x => x.Value)
            .ToList();
        
        return document;
        
        PreviewerCommands.UpdateDocumentStructure.DocumentHierarchyElement? FindDebugPointers(PreviewerCommands.UpdateDocumentStructure.DocumentHierarchyElement element)
        {
            if (element.ElementType == debugPointerName)
            {
                var type = element.Properties.Single(x => x.Label == "Type").Value;
                
                if (type == DebugPointerType.DocumentStructure.ToString())
                {
                    var structureType = element.Properties.Single(x => x.Label == "Label").Value;
                    pointers[(DocumentStructureTypes)Enum.Parse(typeof(DocumentStructureTypes), structureType)] = element;
                }
            }

            foreach (var child in element.Children)
            {
                var result = FindDebugPointers(child);
                
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}