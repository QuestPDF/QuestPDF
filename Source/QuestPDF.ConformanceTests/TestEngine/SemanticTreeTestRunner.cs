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

    private static void CompareSemanticTrees(SemanticTreeNode? actual, SemanticTreeNode? expected)
    {
        if (expected == null && actual == null)
            return;

        if (expected == null)
            Assert.Fail($"Expected null but got node of type '{actual?.Type}'");

        if (actual == null)
            Assert.Fail($"Expected node of type '{expected.Type}' but got null");

        Assert.That(actual.Type, Is.EqualTo(expected.Type), $"Node type mismatch");
        Assert.That(actual.Alt, Is.EqualTo(expected.Alt), $"Alt mismatch for node type '{expected.Type}'");
        Assert.That(actual.Lang, Is.EqualTo(expected.Lang), $"Lang mismatch for node type '{expected.Type}'");

        CompareAttributes(actual.Attributes, expected.Attributes, expected.Type);

        Assert.That(actual.Children.Count, Is.EqualTo(expected.Children.Count), $"Children count mismatch for node type '{expected.Type}'");

        foreach (var (actualChild, expectedChild) in actual.Children.Zip(expected.Children))
            CompareSemanticTrees(actualChild, expectedChild);
        
        static void CompareAttributes(ICollection<SemanticTreeNode.Attribute> actual, ICollection<SemanticTreeNode.Attribute> expected, string nodeType)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count), $"Attribute count mismatch for node type '{nodeType}'");

            var actualList = actual.OrderBy(a => a.Owner).ThenBy(a => a.Name).ToList();
            var expectedList = expected.OrderBy(a => a.Owner).ThenBy(a => a.Name).ToList();

            for (var i = 0; i < expectedList.Count; i++)
            {
                var actualAttr = actualList[i];
                var expectedAttr = expectedList[i];

                Assert.That(actualAttr.Owner, Is.EqualTo(expectedAttr.Owner), $"Attribute owner mismatch for node type '{nodeType}'");
                Assert.That(actualAttr.Name, Is.EqualTo(expectedAttr.Name), $"Attribute name mismatch for node type '{nodeType}'");
                Assert.That(actualAttr.Value, Is.EqualTo(expectedAttr.Value), $"Attribute value mismatch for '{expectedAttr.Owner}:{expectedAttr.Name}' in node type '{nodeType}'");
            }
        }
    }
}