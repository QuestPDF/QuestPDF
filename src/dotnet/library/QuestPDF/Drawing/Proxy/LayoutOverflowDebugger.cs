using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal sealed record LayoutOverflowRootCause(Element[] Ancestors, TreeNode<OverflowDebuggingProxy> Layout)
{
    
}

internal sealed record LayoutOverflowDebugger(IPageContext PageContext, IDocumentCanvas Canvas, ContainerElement Content)
{
    private bool IsInitialized { get; set; }
    public bool IsRecording { get; private set; }
    private TreeNode<OverflowDebuggingProxy>? OverflowDebuggingProxies { get; set; }

    public void DebugLayoutAndFix()
    {
        Initialize();
        RecordLayout();
        FixLayout();
    }
    
    private void Initialize()
    {
        if (IsInitialized)
            return;
        
        Content.RemoveExistingProxiesOfType<SnapshotCacheRecorderProxy>();

        Content.VisitChildren(x => x.CreateProxy(y =>
        {
            if (y is ElementProxy)
                return y;

            return new OverflowDebuggingProxy(y, this);
        }));
        
        OverflowDebuggingProxies = Content
            .ExtractElementsOfType<OverflowDebuggingProxy>()
            .Single();
        
        IsInitialized = true;
    }

    private void RecordLayout()
    {
        IsRecording = true;
        OverflowDebuggingProxies?.Visit(x => x.Value.Reset());
        Content.Measure(Size.Max);
        IsRecording = false;
    }
    
    private void FixLayout()
    {
        OverflowDebuggingProxies?.TryToFixTheLayoutOverflowIssue();
        Content.VisitChildren(x => (x as LayoutProxy)?.CaptureLayoutErrorMeasurement());
        
        Content.ApplyContentDirection();
        Content.InjectDependencies(PageContext, Canvas.GetDrawingCanvas());
    }

    public LayoutOverflowRootCause? TryFindRootCause()
    {
        if (OverflowDebuggingProxies == null)
            return null;
        
        try
        {
            var rootCause = OverflowDebuggingProxies.FindLayoutOverflowVisualizationNodes().First();
                
            var ancestors = rootCause
                .ExtractAncestors()
                .Select(x => x.Value.Child)
                .Where(x => x is DebugPointer or SourceCodePointer)
                .Reverse()
                .ToArray();

            var layout = rootCause
                .ExtractAncestors()
                .First(x => x.Value.Child is SourceCodePointer or DebugPointer)
                .Children
                .First();

            return new LayoutOverflowRootCause(ancestors, layout);
        }
        catch
        {
            return null;
        }
    }
}