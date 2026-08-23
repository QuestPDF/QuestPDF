using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.TestEngine;

internal static class SemanticTreeTestRunner
{
    public static void TestSemanticTree(this IDocument document, SemanticTreeNode? semanticTreeRootNode)
    {
        Settings.EnableCaching = false;
        Settings.EnableDebugging = false;

        var canvas = new SemanticAwareDocumentCanvas();
        var settings = new DocumentSettings { PDFA_Conformance = PDFA_Conformance.PDFA_3A };
        DocumentGenerator.RenderDocument(canvas, document, settings);

        CompareSemanticTrees(canvas.SemanticTree, semanticTreeRootNode);
    }

    private static void CompareSemanticTrees(SemanticTreeNode? actualRoot, SemanticTreeNode? expectedRoot)
    {
        if (expectedRoot == null && actualRoot == null)
            return;

        if (expectedRoot == null)
        {
            Assert.Fail($"Expected null but got node of type '{actualRoot?.Type}'");
            return;
        }

        if (actualRoot == null)
        {
            Assert.Fail($"Expected node of type '{expectedRoot.Type}' but got null");
            return;
        }
        
        var currentPath = new Stack<string>();
        
        try
        {
            Compare(actualRoot, expectedRoot);
        }
        catch
        {
            var pathText = string.Join(" -> ", currentPath.Reverse());
            Console.WriteLine("Problem location");
            Console.WriteLine(pathText);

            throw;
        }        

        void Compare(SemanticTreeNode actual, SemanticTreeNode expected)
        {
            if (!currentPath.Any())
                currentPath.Push(actual.Type);
            
            if (expected.NodeId != 0)
                Assert.That(actual.NodeId, Is.EqualTo(expected.NodeId), "NodeId mismatch");
            
            Assert.That(actual.Type, Is.EqualTo(expected.Type), "Type mismatch");
            Assert.That(actual.Alt, Is.EqualTo(expected.Alt), "Alt mismatch");
            Assert.That(actual.Lang, Is.EqualTo(expected.Lang), "Lang mismatch");

            CompareAttributes();
            CompareChildren();

            void CompareChildren()
            {
                var actualChildren = actual.Children ?? [];
                var expectedChildren = expected.Children ?? [];

                Assert.That(actualChildren.Count, Is.EqualTo(expectedChildren.Count), "Children count mismatch");

                var hasMultipleChildren = actualChildren.Count > 1;

                foreach (var (actualChild, expectedChild) in actualChildren.Zip(expectedChildren))
                {
                    var prefix = hasMultipleChildren ? $"{actualChildren.IndexOf(actualChild)}:" : "";
                    currentPath.Push(prefix + actualChild.Type);
                
                    Compare(actualChild, expectedChild);
                
                    currentPath.Pop();
                }
            }
            
            void CompareAttributes()
            {
                var actualAttributes = actual.Attributes ?? [];
                var expectedAttributes = expected.Attributes ?? [];

                Assert.That(actualAttributes.Count, Is.EqualTo(expectedAttributes.Count), "Attribute count mismatch");

                var actualList = actualAttributes.OrderBy(a => a.Owner).ThenBy(a => a.Name);
                var expectedList = expectedAttributes.OrderBy(a => a.Owner).ThenBy(a => a.Name);

                foreach (var (actualAttribute, expectedAttribute) in actualList.Zip(expectedList))
                {
                    Assert.That(actualAttribute.Owner, Is.EqualTo(expectedAttribute.Owner), "Attribute owner mismatch");
                    Assert.That(actualAttribute.Name, Is.EqualTo(expectedAttribute.Name), "Attribute name mismatch");

                    var valueMismatchMessage = $"Attribute value mismatch for '{expectedAttribute.Owner}:{expectedAttribute.Name}";

                    // layout-derived attributes (e.g. BBox) are subject to floating point rounding
                    if (expectedAttribute.Value is float[] expectedNumbers)
                        Assert.That(actualAttribute.Value, Is.EqualTo(expectedNumbers).Within(Size.Epsilon), valueMismatchMessage);
                    else
                        Assert.That(actualAttribute.Value, Is.EqualTo(expectedAttribute.Value), valueMismatchMessage);
                }
            }   
        }
    }
}

internal static class ExpectedSemanticTree
{
    public static SemanticTreeNode DocumentRoot(Action<SemanticTreeNode> configuration)
    {
        var root = new SemanticTreeNode
        {
            Type = "Document"
        };
        
        configuration(root);
        return root;
    }

    public static void Child(this SemanticTreeNode parent, string type, Action<SemanticTreeNode>? configuration = null)
    {
        var child = new SemanticTreeNode
        {
            Type = type
        };

        configuration?.Invoke(child);
        parent.AddChild(child);
    }
    
    public static SemanticTreeNode Id(this SemanticTreeNode node, int id)
    {
        node.NodeId = id;
        return node;
    }
    
    public static SemanticTreeNode Attribute(this SemanticTreeNode node, string owner, string name, object value)
    {
        var attribute = new SemanticTreeNode.Attribute
        {
            Owner = owner,
            Name = name,
            Value = value
        };
        
        node.AddAttribute(attribute);
        return node;
    }
    
    public static SemanticTreeNode Alt(this SemanticTreeNode node, string alt)
    {
        node.Alt = alt;
        return node;
    }
    
    public static SemanticTreeNode Lang(this SemanticTreeNode node, string lang)
    {
        node.Lang = lang;
        return node;
    }
}