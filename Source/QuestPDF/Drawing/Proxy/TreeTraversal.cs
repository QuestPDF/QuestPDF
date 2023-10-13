using System.Collections.Generic;
using System.Linq;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal class TreeNode<T>
{
    public T Value { get; }
    public IEnumerable<TreeNode<T>> Children { get; }
    
    public TreeNode(T Value, IEnumerable<TreeNode<T>> children)
    {
        this.Value = Value;
        this.Children = children;
    }
}

internal static class TreeTraversal
{
    public static IEnumerable<TreeNode<T>> ExtractElementsOfType<T>(this Element element) where T : Element
    {
        if (element is T proxy)
        {
            var children = proxy.GetChildren().SelectMany(ExtractElementsOfType<T>);
            yield return new TreeNode<T>(proxy, children);
        }
        else
        {
            foreach (var treeNode in element.GetChildren().SelectMany(ExtractElementsOfType<T>))
                yield return treeNode;
        }
    }
    
    public static IEnumerable<TreeNode<T>> Flatten<T>(this TreeNode<T> element) where T : Element
    {
        yield return element;

        foreach (var child in element.Children)
            foreach (var innerChild in Flatten(child))
                yield return innerChild;
    }
}