using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal sealed class TreeNode<T>
{
    public T Value { get; }
    public TreeNode<T>? Parent { get; private set; }
    
    private List<TreeNode<T>>? ChildrenStore { get; set; }
    private static readonly List<TreeNode<T>> EmptyChildrenList = [];
    public IReadOnlyList<TreeNode<T>> Children => ChildrenStore ?? EmptyChildrenList;

    public TreeNode(T value)
    {
        Value = value;
    }

    public void AddChild(TreeNode<T> child)
    {
        ChildrenStore ??= new List<TreeNode<T>>();
        ChildrenStore.Add(child);
        child.Parent = this;
    }
}

internal static class TreeTraversal
{
    public static List<TreeNode<T>> ExtractElementsOfType<T>(this Element root) where T : ContainerElement
    {
        var rootNodes = new List<TreeNode<T>>();
        Traverse(root, parent: null);
        return rootNodes;
        
        void Traverse(Element element, TreeNode<T>? parent)
        {
            if (element is T proxy)
            {
                var node = new TreeNode<T>(proxy);

                if (parent == null)
                    rootNodes.Add(node);
                else
                    parent.AddChild(node);

                Traverse(proxy.Child, node);
            }
            else if (element is ContainerElement containerElement)
            {
                Traverse(containerElement.Child, parent);
            }
            else
            {
                var children = element.GetChildren();

                for (var i = 0; i < children.Count; i++)
                    Traverse(children[i], parent);
            }
        }
    }
    
    public static IEnumerable<TreeNode<T>> Flatten<T>(this TreeNode<T> element) where T : Element
    {
        yield return element;

        foreach (var child in element.Children)
            foreach (var innerChild in Flatten(child))
                yield return innerChild;
    }
    
    public static IEnumerable<TreeNode<T>> ExtractAncestors<T>(this TreeNode<T> node)
    {
        while (true)
        {
            node = node.Parent;
            
            if (node is null)
                yield break;

            yield return node;
        }
    }
}