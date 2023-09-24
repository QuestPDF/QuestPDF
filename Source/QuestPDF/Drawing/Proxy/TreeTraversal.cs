using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer;

internal class TreeNode<T>
{
    public T Value { get; }
    public ICollection<TreeNode<T>> Children { get; }
    
    public TreeNode(T Value, ICollection<TreeNode<T>> Children)
    {
        this.Value = Value;
        this.Children = Children;
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
                var result = new TreeNode<T>(proxy, new List<TreeNode<T>>());
                
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