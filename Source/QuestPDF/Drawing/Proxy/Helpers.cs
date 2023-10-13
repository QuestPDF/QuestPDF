using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer.LayoutInspection;

namespace QuestPDF.Drawing.Proxy;

internal static class Helpers
{
    public static void ApplyLayoutDebugging(this Element container)
    {
        container.VisitChildren(x =>
        {
            x.CreateProxy(y => y is ElementProxy ? y : new LayoutDebuggingProxy(y));
        });
    }
    
    public static void ResetLayoutOverflowDetection(this TreeNode<LayoutDebuggingProxy> hierarchyRoot)
    {
        Traverse(hierarchyRoot);
        
        void Traverse(TreeNode<LayoutDebuggingProxy> parent)
        {
            if (parent.Value.CurrentSpacePlanType == null)
                return;

            parent.Value.CurrentSpacePlanType = null;

            foreach (var child in parent.Children)
                Traverse(child);
        }
    }
    
    public static void ApplyLayoutOverflowVisualization(this TreeNode<LayoutDebuggingProxy> hierarchyRoot)
    {
        Traverse(hierarchyRoot);
        
        void Traverse(TreeNode<LayoutDebuggingProxy> parent)
        {
            if (parent.Value.CurrentSpacePlanType == null)
                return;
            
            if (parent.Value.CurrentSpacePlanType == SpacePlanType.FullRender)
                return;
            
            var childrenWithWraps = parent.Children.Where(x => x.Value.CurrentSpacePlanType is SpacePlanType.Wrap).ToList();
            var childrenWithPartialRenders = parent.Children.Where(x => x.Value.CurrentSpacePlanType is SpacePlanType.PartialRender).ToList();

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
            var elementName = (parent as InspectorPointer)?.Label ?? parent.GetType().Name;
            
            result.AppendLine();
            result.Append(indentationCache[indentationLevel]);
            result.Append(elementName);

            indentationLevel++;
            
            foreach (var child in parent.GetChildren())
                Traverse(child);

            indentationLevel--;
        }
    }
    
    public static IReadOnlyCollection<DocumentInspectionElement.ElementProperty> GetElementConfiguration(this IElement element)
    {
        return element
            .GetType()
            .GetProperties()
            .Select(x => new
            {
                Property = x.Name.PrettifyName(),
                Value = x.GetValue(element)
            })
            .Where(x => !(x.Value is IElement))
            .Where(x => x.Value is string || !(x.Value is IEnumerable) || x.Value.GetType().IsEnum)
            .Where(x => !(x.Value is TextStyle))
            .Select(x => new DocumentInspectionElement.ElementProperty
            {
                Label = x.Property,
                Value = FormatValue(x.Value)
            })
            .ToList();

        string FormatValue(object value)
        {
            const int maxLength = 250;
                
            var text = value?.ToString() ?? "-";

            if (text.Length < maxLength)
                return text;

            var omittedLength = text.Length - maxLength; 
            return text.AsSpan(0, maxLength).ToString() + $" (+{omittedLength} characters)";
        }
    }
    
    public static DocumentInspectionElement ToDocumentInspectionHierarchy(this TreeNode<LayoutDebuggingProxy> hierarchyRoot)
    {
        return Traverse(hierarchyRoot);
        
        DocumentInspectionElement Traverse(TreeNode<LayoutDebuggingProxy> parent)
        {
            var proxy = parent.Value;
            var element = proxy.Child;

            // do not include Container elements in the final output
            if (element.GetType() == typeof(Container))
                return Traverse(parent.Children.First());

            var locations = proxy
                .DrawOperations
                .Select(o => new DocumentInspectionElement.PageLocation
                {
                    PageNumber = o.PageNumber,
                    Size = new DocumentInspectionElement.ElementSize()
                    {
                        Width = o.Size.Width,
                        Height = o.Size.Height
                    },
                    Position = new DocumentInspectionElement.ElementPosition
                    {
                        Left = o.Position.X,
                        Top = o.Position.Y
                    }
                })
                .ToList();
            
            return new DocumentInspectionElement
            {
                ElementType = element.GetType().Name.PrettifyName(),
                IsSingleChildContainer = element is ContainerElement,
                IsContainer = element is IContainer,
                Location = locations,
                Properties = element.GetElementConfiguration().ToList(),
                Children = parent.Children.Select(Traverse).ToList()
            };
        }
    }
}