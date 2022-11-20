using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer.Inspection;

public static class DocumentHierarchyProcessor
{
    internal static InspectionElement ExtractDocumentHierarchy(Element container)
    {
        return Traverse(container);
        
        InspectionElement? Traverse(Element item)
        {
            InspectionElement? result = null;
            Element currentItem = item;
            
            while (true)
            {
                if (currentItem is InspectionProxy proxy)
                {
                    if (proxy.Child.GetType() == typeof(Container))
                    {
                        currentItem = proxy.Child;
                        continue;
                    }
                    
                    var statistics = GetInspectionElement(proxy);

                    if (statistics == null)
                        return null;
                    
                    if (result == null)
                    {
                        result = statistics;
                    }
                    else
                    {
                        result.Children.Add(statistics);
                    }

                    currentItem = proxy.Child;
                }
                else
                {
                    var children = currentItem.GetChildren().ToList();

                    if (children.Count == 0)
                    {
                        return result;
                    }
                    else if (children.Count == 1)
                    {
                        currentItem = children.First();
                        continue;
                    }
                    else
                    {
                        children
                            .Select(Traverse)
                            .Where(x => x != null)
                            .ToList()
                            .ForEach(result.Children.Add);

                        return result;
                    }
                }
            }
        }

        static InspectionElement? GetInspectionElement(InspectionProxy inspectionProxy)
        {
            var locations = inspectionProxy
                .Statistics
                .Keys
                .Select(x =>
                {
                    var statistics = inspectionProxy.Statistics[x];
                    
                    return new InspectionElementLocation
                    {
                        PageNumber = x,
                        Position = new MyPosition
                        {
                            Top = statistics.Position.Y,
                            Left = statistics.Position.X
                        },
                        Size = new Size
                        {
                            Width = statistics.Size.Width,
                            Height = statistics.Size.Height
                        }
                    };
                })
                .ToList();
            
            return new InspectionElement
            {
                ElementType = inspectionProxy.Child.GetType().Name,
                Location = locations,
                Properties = GetElementConfiguration(inspectionProxy.Child),
                Children = new List<InspectionElement>()
            };
        }
        
        static IReadOnlyCollection<DocumentElementProperty> GetElementConfiguration(IElement element)
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
                .Where(x => x.Value is string || !(x.Value is IEnumerable))
                .Where(x => !(x.Value is TextStyle))
                .Select(x => new DocumentElementProperty
                {
                    Label = x.Property,
                    Value = FormatValue(x.Value)
                })
                .ToList();

            string FormatValue(object value)
            {
                const int maxLength = 100;
                
                var text = value?.ToString() ?? "-";

                if (text.Length < maxLength)
                    return text;

                return text.AsSpan(0, maxLength).ToString() + "...";
            }
        }
    }
}