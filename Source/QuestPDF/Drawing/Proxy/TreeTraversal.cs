using System.Collections.Generic;
using System.Linq;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal class TreeNode<T>
{
    public T Value { get; }
    public TreeNode<T>? Parent { get; set; }
    public ICollection<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();
    
    public TreeNode(T Value)
    {
        this.Value = Value;
    }
}

internal static class TreeTraversal
{
    public static IEnumerable<TreeNode<T>> ExtractElementsOfType<T>(this Element element) where T : Element
    {
        if (element is T proxy)
        {
            var result = new TreeNode<T>(proxy);

            foreach (var treeNode in proxy.GetChildren().SelectMany(ExtractElementsOfType<T>))
            {
                result.Children.Add(treeNode);
                treeNode.Parent = result;
            }
                
            yield return result;
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
    
    public static ICollection<TreeNode<T>> ExtractAncestors<T>(this TreeNode<T> element)
    {
        var parent = element;
        var result = new List<TreeNode<T>>();
        
        while (parent is not null)
        {
            result.Add(parent);
            parent = parent.Parent;
        }

        return result;
    }
    
    public static TreeNode<OverflowDebuggingProxy>? FindElementOfType<TChild>(this TreeNode<OverflowDebuggingProxy> element)
    {
        TreeNode<OverflowDebuggingProxy> result = null;
        Traverse(element);
        return result;
        
        void Traverse(TreeNode<OverflowDebuggingProxy> currentElement)
        {
            if (currentElement.Value.Child is TChild)
            {
                result = currentElement;
                return;
            }

            foreach (var child in currentElement.Children)
                Traverse(child);
        }
    }
}