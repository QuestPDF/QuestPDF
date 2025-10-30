using System.Collections.Generic;

namespace QuestPDF.Drawing;

internal class SemanticTreeNode
{
    public int NodeId { get; set; }
    public string Type { get; set; } = "";
    public string? Alt { get; set; }
    public string? Lang { get; set; }
    public IList<SemanticTreeNode> Children { get; } = [];
    public ICollection<Attribute> Attributes { get; } = [];

    public class Attribute
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
    }
}

class SemanticTreeManager
{
    private int CurrentNodeId { get; set; }
    private SemanticTreeNode? Root { get; set; }
    private Stack<SemanticTreeNode> Stack { get; set; } = [];

    public SemanticTreeManager()
    {
        PopulateWithTopLevelNode();
    }

    private void PopulateWithTopLevelNode()
    {
        AddNode(new SemanticTreeNode
        {
            NodeId = GetNextNodeId(),
            Type = "Document"
        });
    }
    
    public int GetNextNodeId()
    {
        CurrentNodeId++;
        return CurrentNodeId;
    }
    
    public void AddNode(SemanticTreeNode node)
    {
        if (Root == null)
        {
            Root = node;
            Stack.Push(node);
            return;
        }
        
        Stack.Peek()?.Children.Add(node);
    }
    
    public void PushOnStack(SemanticTreeNode node)
    {
        Stack.Push(node);
    }
    
    public void PopStack()
    {
        Stack.Pop();
    }
    
    public SemanticTreeNode PeekStack()
    {
        return Stack.Peek();
    }
    
    public void Reset()
    {
        CurrentNodeId = 0;
        Root = null;
        Stack.Clear();
    }

    public SemanticTreeNode? GetSemanticTree()
    {
        return Root;
    }
    
    #region Artifacts
    
    private int ArtifactNestingLevel { get; set; } = 0;
    
    public void BeginArtifactContent()
    {
        ArtifactNestingLevel++;
    }
    
    public void EndArtifactContent()
    {
        ArtifactNestingLevel--;
    }
    
    public bool IsCurrentContentArtifact()
    {
        return ArtifactNestingLevel > 0;
    }
    
    #endregion
}