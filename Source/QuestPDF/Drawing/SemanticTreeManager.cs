using System.Collections.Generic;

namespace QuestPDF.Drawing;

class SemanticTreeNode
{
    public int NodeId { get; set; }
    public string Type { get; set; } = "";
    public string? Alt { get; set; }
    public string? Lang { get; set; }
    public ICollection<SemanticTreeNode> Children { get; } = [];
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
}