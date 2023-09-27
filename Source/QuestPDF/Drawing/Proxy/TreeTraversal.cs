using System.Collections.Generic;
using System.Linq;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal class TreeNode<T>
{
    public T Value { get; }
    public ICollection<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();
    
    public TreeNode(T Value)
    {
        this.Value = Value;
    }
}

internal static class TreeTraversal
{
    public static TreeNode<T> ExtractProxyOfType<T>(this Element root) where T : ElementProxy
    {
        return Traverse(root).FirstOrDefault();

        IEnumerable<TreeNode<T>> Traverse(Element element)
        {
            if (element is T proxy)
            {
                var result = new TreeNode<T>(proxy);
                
                foreach (var treeNode in proxy.Child!.GetChildren().SelectMany(Traverse))
                    result.Children.Add(treeNode);
                
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