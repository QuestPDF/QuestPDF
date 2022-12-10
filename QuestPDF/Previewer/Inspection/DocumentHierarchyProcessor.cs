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
    internal static InspectionElement ExtractDocumentHierarchy(this Container content)
    {
        var proxies = content.ExtractProxyOfType<InspectionProxy>();
        return Map(proxies);

        InspectionElement Map(TreeNode<InspectionProxy> treeNode)
        {
            var proxy = treeNode.Value;
            var element = proxy.Child;
                
            var locations = proxy
                .Statistics
                .Keys
                .Select(x =>
                {
                    var statistics = proxy.Statistics[x];
                    
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
                ElementType = element.GetType().Name,
                IsSingleChildContainer = element is ContainerElement,
                Location = locations,
                Properties = element.GetElementConfiguration().ToList(),
                Children = treeNode.Children.Select(Map).ToList()
            };
        }
    }
}