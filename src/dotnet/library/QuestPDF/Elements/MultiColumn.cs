using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements.Text;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal sealed class MultiColumnChildDrawingObserver : ElementProxy
{
    public bool HasBeenDrawn => ChildStateBeforeDrawingOperation != null;
    public object? ChildStateBeforeDrawingOperation { get; private set; }

    internal override void Draw(LayoutSpace availableSpace)
    {
        ChildStateBeforeDrawingOperation ??= (GetFirstElementChild() as IStateful).GetState();
        Child.Draw(availableSpace);
    }
    
    internal void ResetDrawingState()
    {
        ChildStateBeforeDrawingOperation = null;
    }

    internal void RestoreState()
    {
        (GetFirstElementChild() as IStateful)?.SetState(ChildStateBeforeDrawingOperation);
    }

    private Element GetFirstElementChild()
    {
        var child = Child;
        
        while (child is ElementProxy proxy)
            child = proxy.Child;

        return child;
    }
}

internal sealed class MultiColumn : Element, IPageContextAware, IContentDirectionAware, IDisposable
{
    // items
    internal Element Content { get; set; } = Empty.Instance;
    internal Element Spacer { get; set; } = Empty.Instance;
    
    // configuration
    public int ColumnCount { get; set; } = 2;
    public bool BalanceHeight { get; set; } = false;
    public float Spacing { get; set; }
    
    public IPageContext PageContext { get; set; }
    public ContentDirection ContentDirection { get; set; }

    // cache
    private ProxyDrawingCanvas ChildrenCanvas { get; } = new();
    private DiscardDrawingCanvas MeasurementCanvas { get; } = new();
    private List<TreeNode<MultiColumnChildDrawingObserver>> State { get; set; }

    ~MultiColumn()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        ChildrenCanvas?.Dispose();
        GC.SuppressFinalize(this);
    }
    
    internal override void CreateProxy(Func<Element?, Element?> create)
    {
        Content = create(Content);
        Spacer = create(Spacer);
    }
    
    internal override IReadOnlyList<Element?> GetChildren()
    {
        return [Content, Spacer];
    }
    
    private void BuildState()
    {
        if (State != null)
            return;
        
        this.VisitChildren(child =>
        {
            child.CreateProxy(x => x is IStateful ? new MultiColumnChildDrawingObserver { Child = x } : x);
        });
        
        State = this.ExtractElementsOfType<MultiColumnChildDrawingObserver>();
    }

    internal override SpacePlan Measure(LayoutSpace availableSpace)
    {
        BuildState();
        OptimizeTextCacheBehavior();
        
        if (Content.Canvas != ChildrenCanvas)
            Content.InjectDependencies(PageContext, ChildrenCanvas);
        
        ChildrenCanvas.Target = MeasurementCanvas;
        
        return FindPerfectSpace();
        
        SpacePlan FindPerfectSpace()
        {
            // the entire height is the most any column will ever receive, so the content may adapt to it
            var defaultMeasurement = MeasureColumns(GetContentSpace(availableSpace, isCandidate: false));

            if (defaultMeasurement.First.Type is SpacePlanType.Empty or SpacePlanType.Wrap)
                return defaultMeasurement.First;
            
            if (defaultMeasurement.Last.Type is SpacePlanType.PartialRender or SpacePlanType.Wrap)
                return SpacePlan.PartialRender(availableSpace.Width, defaultMeasurement.MaxHeight);
            
            if (!BalanceHeight)
                return SpacePlan.FullRender(availableSpace.Width, defaultMeasurement.MaxHeight);

            var minHeight = 0f;
            var maxHeight = availableSpace.Height;
            
            for (var i = 0; i < 8; i++)
            {
                var middleHeight = (minHeight + maxHeight) / 2;
                var middleSpace = GetContentSpace(availableSpace.With(availableSpace.Width, middleHeight), isCandidate: true);
                var middleMeasurement = MeasureColumns(middleSpace);
                
                if (middleMeasurement.Last.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                    maxHeight = middleHeight;
                
                else
                    minHeight = middleHeight;
            }
            
            return SpacePlan.FullRender(new Size(availableSpace.Width, maxHeight));
        }
    }

    /// <summary>
    /// Lays the content out column after column on the measurement canvas, then restores its state.
    /// </summary>
    private (SpacePlan First, SpacePlan Last, float MaxHeight) MeasureColumns(LayoutSpace contentSpace)
    {
        var first = default(SpacePlan);
        var last = default(SpacePlan);
        var maxHeight = 0f;

        for (var i = 0; i < ColumnCount; i++)
        {
            var measurement = Content.Measure(contentSpace);
            Content.Draw(contentSpace);

            if (i == 0)
                first = measurement;

            last = measurement;
            maxHeight = Math.Max(maxHeight, measurement.Height);
        }
        
        ResetObserverState(restoreChildState: true);
        return (first, last, maxHeight);
    }

    private Size GetAvailableSpaceForColumn(Size totalSpace)
    {
        var columnWidth = (totalSpace.Width - Spacing * (ColumnCount - 1)) / ColumnCount;
        return new Size(columnWidth, totalSpace.Height);
    }

    /// <summary>
    /// Every column is exactly as wide and as tall as the others, so the content inherits the constraints
    /// of the whole element. A height tried while balancing is a candidate in a search rather than an allocation,
    /// and the content has to describe itself as configured at every candidate.
    /// </summary>
    private LayoutSpace GetContentSpace(LayoutSpace availableSpace, bool isCandidate)
    {
        var contentSpace = availableSpace.With(GetAvailableSpaceForColumn(availableSpace));
        return isCandidate ? contentSpace.WithFlowingHeight() : contentSpace;
    }

    /// <summary>
    /// A balanced height was found with candidate offers, and the content is drawn with the same offer
    /// so that it lands in the same columns. When no candidate held the content, the element is as tall
    /// as it was allowed to be and the content fits only by adapting to that height, so it is drawn with
    /// the allocation it was measured with.
    /// </summary>
    private LayoutSpace GetDrawingContentSpace(LayoutSpace availableSpace)
    {
        var allocation = GetContentSpace(availableSpace, isCandidate: false);
        
        if (!BalanceHeight)
            return allocation;
        
        var candidate = GetContentSpace(availableSpace, isCandidate: true);
        
        if (candidate.Equals(allocation))
            return allocation;

        ChildrenCanvas.Target = MeasurementCanvas;
        var measurement = MeasureColumns(candidate);
        
        return measurement.Last.Type is SpacePlanType.Empty or SpacePlanType.FullRender
            ? candidate
            : allocation;
    }
    
    internal override void Draw(LayoutSpace availableSpace)
    {
        var contentAvailableSpace = GetAvailableSpaceForColumn(availableSpace);
        var spacerAvailableSpace = new Size(Spacing, availableSpace.Height);
        
        var contentSpace = GetDrawingContentSpace(availableSpace);
        var spacerSpace = availableSpace.With(spacerAvailableSpace).WithWidthMode(LayoutAxisMode.Final);

        var horizontalOffset = 0f;
        ChildrenCanvas.Target = Canvas;
        
        for (var i = 1; i <= ColumnCount; i++)
        {
            var contentMeasurement = Content.Measure(contentSpace);
            var targetColumnSize = new Size(contentAvailableSpace.Width, contentMeasurement.Height);

            var contentOffset = GetTargetOffset(targetColumnSize.Width);
            
            Canvas.Translate(contentOffset);
            Content.Draw(contentSpace.With(targetColumnSize));
            Canvas.Translate(contentOffset.Reverse());
            
            horizontalOffset += contentAvailableSpace.Width;
            
            if (contentMeasurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                break;
            
            var spacerMeasurement = Spacer.Measure(spacerSpace);

            if (i == ColumnCount || spacerMeasurement.Type is SpacePlanType.Wrap) 
                continue;
            
            var spacerOffset = GetTargetOffset(Spacing);
            
            Canvas.Translate(spacerOffset);
            Spacer.Draw(spacerSpace);
            Canvas.Translate(spacerOffset.Reverse());
                
            horizontalOffset += Spacing;
        }
        
        ResetObserverState(restoreChildState: false);

        Position GetTargetOffset(float contentWidth)
        {
            return ContentDirection == ContentDirection.LeftToRight
                ? new Position(horizontalOffset, 0)
                : new Position(availableSpace.Width - horizontalOffset - contentWidth, 0);
        }
    }
    
    void ResetObserverState(bool restoreChildState)
    {
        foreach (var node in State)
            Traverse(node);
            
        void Traverse(TreeNode<MultiColumnChildDrawingObserver> node)
        {
            var observer = node.Value;

            if (!observer.HasBeenDrawn)
                return;

            if (restoreChildState)
                observer.RestoreState();
            
            observer.ResetDrawingState();
                
            for (var i = 0; i < node.Children.Count; i++)
                Traverse(node.Children[i]);
        }
    }
    
    #region Text Optimization

    private bool IsTextOptimizationExecuted { get; set; } = false;
    
    /// <summary>
    /// The TextBlock element uses SkParagraph cache to enhance rendering speed.
    /// This cache uses a significant amount of memory and is cleared after FullRender.
    /// However, the MultiColumn element uses a sophisticated measuring algorithm,
    /// and may force the Text element to measure/render multiple times per page.
    /// To avoid performance issues, the TextBlock element should keep its cache.
    /// </summary>
    private void OptimizeTextCacheBehavior()
    {
        if (IsTextOptimizationExecuted)
            return;
        
        IsTextOptimizationExecuted = true;
        
        Content.VisitChildren(x =>
        {
            if (x is TextBlock text)
                text.ClearInternalCacheAfterFullRender = false;
        });
    }
    
    #endregion
}