using System.Linq;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer;

internal static class PreviewerModelExtensions
{
    public static DocumentHierarchyElement ExtractHierarchy(this Element container)
    {
        var layoutTree = container.ExtractElementsOfType<LayoutProxy>().Single();
        return Traverse(layoutTree);
        
        DocumentHierarchyElement Traverse(TreeNode<LayoutProxy> node)
        {
            var layout = node.Value;
            
            if (layout.Child is Container or SnapshotRecorder or ElementProxy)
                return Traverse(node.Children.Single());
            
            var element = new DocumentHierarchyElement
            {
                ElementType = layout.Child.GetType().Name.PrettifyName(),
                
                Location = layout
                    .Snapshots
                    .Select(x => new PageLocation
                    {
                        PageNumber = x.PageNumber,
                        Position = new PagePosition
                        {
                            Left = x.Position.X,
                            Top = x.Position.Y
                        },
                        Size = new PageElementPosition
                        {
                            Width = x.Size.Width,
                            Height = x.Size.Height
                        }
                    })
                    .ToList(),
                
                IsSingleChildContainer = layout.Child is ContainerElement,
                Properties = layout
                    .Child
                    .GetElementConfiguration()
                    .Select(x => new ElementProperty
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
}