using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer;

internal record TreeNode<T>(T Value, ICollection<TreeNode<T>> Children);

internal static class TreeTraversal
{
    public static TreeNode<T> ExtractProxyOfType<T>(this Element root) where T : ElementProxy
    {
        return Traverse(root).FirstOrDefault();

        IEnumerable<TreeNode<T>> Traverse(Element element)
        {
            if (element is T proxy)
            {
                var result = new TreeNode<T>(proxy, new List<TreeNode<T>>());
                
                proxy
                    .Child!
                    .GetChildren()
                    .SelectMany(Traverse)
                    .ToList()
                    .ForEach(result.Children.Add);

                yield return result;
            }
            else
            {
                foreach (var treeNode in element.GetChildren().SelectMany(Traverse))
                    yield return treeNode;
            }
        }
    }
}