using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Companion;

internal static class CompanionModelHelpers
{
    internal static CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement ExtractHierarchy(this Element container)
    {
        var layoutTree = container.ExtractElementsOfType<LayoutProxy>().Single();
        return Traverse(layoutTree);
        
        CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement Traverse(TreeNode<LayoutProxy> node)
        {
            var layout = node.Value;
            var child = layout.Child;

            while (child is ElementProxy elementProxy)
                child = elementProxy.Child;

            if (child is Container)
                return Traverse(node.Children.Single());
            
            var element = new CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement
            {
                Element = child,
                
                ElementType = child.GetType().Name,
                Hint = child.GetCompanionHint(),
                SearchableContent = child.GetCompanionSearchableContent(),
                
                PageLocations = layout.Snapshots,
                SourceCodeDeclarationPath = GetSourceCodePath(child.CodeLocation),
                LayoutErrorMeasurements = layout.LayoutErrorMeasurements,
                
                IsSingleChildContainer = child is ContainerElement,
                Properties = child.GetCompanionProperties()?.Select(x => new CompanionCommands.ElementProperty { Label = x.Key, Value = x.Value }).ToList() ?? [],
                
                Children = node.Children.Select(Traverse).ToList()
            };

            return element;
        }
    }

    private static CompanionCommands.UpdateDocumentStructure.SourceCodePath? GetSourceCodePath(SourceCodePath? path)
    {
        if (path == null)
            return null;
        
        return new CompanionCommands.UpdateDocumentStructure.SourceCodePath
        {
            FilePath = path.Value.FilePath,
            LineNumber = path.Value.LineNumber
        };
    }
    
    #if NET6_0_OR_GREATER
    
    internal static CompanionCommands.ShowGenericException.StackFrame[] ParseStackTrace(this string stackTrace)
    {
        var lines = stackTrace.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
        
        var frames = new List<CompanionCommands.ShowGenericException.StackFrame>();

        foreach (string line in lines)
        {
            var fullMatch = Regex.Match(line, @"at\s+(?<codeLocation>.+)\s+in\s(?<fileName>.+)\s*:line\s(?<lineNumber>\d+)");
            var codeOnlyMatch = Regex.Match(line, @"at\s+(?<codeLocation>.+)");

            if (fullMatch.Success)
            {
                frames.Add(new CompanionCommands.ShowGenericException.StackFrame
                {
                    CodeLocation = fullMatch.Groups["codeLocation"].Value,
                    FileName = fullMatch.Groups["fileName"].Value,
                    LineNumber = int.Parse(fullMatch.Groups["lineNumber"].Value)
                });
            }
            else if (codeOnlyMatch.Success)
            {
                frames.Add(new CompanionCommands.ShowGenericException.StackFrame
                {
                    CodeLocation = codeOnlyMatch.Groups["codeLocation"].Value
                });
            }
        }

        return frames.ToArray();
    }

    #endif
    
    internal static CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement ImproveHierarchyStructure(this CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement root)
    {
        var document = FindDocumentStructurePointersThat(root, x => x == DocumentStructureTypes.Document).Single();
        document.IsSingleChildContainer = false;

        var pages = FindDocumentStructurePointersThat(document, x => x == DocumentStructureTypes.Page).ToList();

        foreach (var page in pages)
        {
            page.IsSingleChildContainer = false;
            page.Children = FindDocumentStructurePointersThat(page, x => x is not (DocumentStructureTypes.Document or DocumentStructureTypes.Page)).ToList();
        }
        
        document.Children = pages;

        if (pages.Count == 1)
            document.Children = pages.Single().Children;

        return document;

        ICollection<CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement> FindDocumentStructurePointersThat(CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement root, Predicate<DocumentStructureTypes> predicate)
        {
            var result = new List<CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement>();
            Traverse(root);
            return result;

            void Traverse(CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement element)
            {
                if (element.Element is DebugPointer { Type: DebugPointerType.DocumentStructure } debugPointer && Enum.TryParse<DocumentStructureTypes>(debugPointer.Label, out var structureType) && predicate(structureType))
                {
                    result.Add(element);
                    return;
                }
                
                foreach (var child in element.Children)
                    Traverse(child);
            }
        }
    }
}