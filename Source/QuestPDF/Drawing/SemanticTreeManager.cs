using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;

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
    
    #region State

    public class StateSnapshot
    {
        internal int CurrentNodeId { get; init; }
    }
    
    public StateSnapshot GetState()
    {
        return new StateSnapshot
        {
            CurrentNodeId = CurrentNodeId
        };
    }
    
    public void SetState(StateSnapshot state)
    {
        CurrentNodeId = state.CurrentNodeId;
    }
    
    #endregion
}

class SemanticTreeSnapshots(SemanticTreeManager? semanticTreeManager, IPageContext pageContext)
{
    private IList<SemanticTreeManager.StateSnapshot> Snapshots { get; } = [];

    public SemanticTreeSnapshotScope? StartSemanticStateScope(int index)
    {
        if (semanticTreeManager == null)
            return null;
        
        var originalSemanticState = semanticTreeManager.GetState();
        
        if (index >= Snapshots.Count)
        {
            Snapshots.Add(originalSemanticState);
        }
        else
        {
            var snapshot = Snapshots[index];
            semanticTreeManager.SetState(snapshot);
        }
        
        return new SemanticTreeSnapshotScope(() =>
        {
            if (pageContext.IsInitialRenderingPhase)
                return;
                
            semanticTreeManager.SetState(originalSemanticState);
        });
    }

    public class SemanticTreeSnapshotScope(Action resetState) : IDisposable
    {
        public void Dispose()
        {
            resetState();
            GC.SuppressFinalize(this);
        }
    }
}

internal readonly ref struct SemanticScope : IDisposable
{
    private IDrawingCanvas DrawingCanvas { get; }
    private int OriginalSemanticNodeId { get; }

    public SemanticScope(IDrawingCanvas drawingCanvas, int nodeId)
    {
        DrawingCanvas = drawingCanvas;
        OriginalSemanticNodeId = drawingCanvas.GetSemanticNodeId();
        DrawingCanvas.SetSemanticNodeId(nodeId);
    }
    
    public void Dispose()
    {
        DrawingCanvas.SetSemanticNodeId(OriginalSemanticNodeId);
    }
}

internal static class SemanticCanvasExtensions
{
    public static SemanticScope StartSemanticScopeWithNodeId(this IDrawingCanvas canvas, int nodeId)
    {
        return new SemanticScope(canvas, nodeId);
    }
}