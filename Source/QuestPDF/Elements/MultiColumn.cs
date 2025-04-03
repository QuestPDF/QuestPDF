using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    internal override void Draw(Size availableSpace)
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

internal sealed class MultiColumn : Element, IContentDirectionAware, IDisposable
{
    // items
    internal Element Content { get; set; } = Empty.Instance;
    internal Element Spacer { get; set; } = Empty.Instance;
    
    // configuration
    public int ColumnCount { get; set; } = 2;
    public bool BalanceHeight { get; set; } = false;
    public float Spacing { get; set; }
    
    public ContentDirection ContentDirection { get; set; }

    // cache
    private ProxyDrawingCanvas ChildrenCanvas { get; } = new();
    private TreeNode<MultiColumnChildDrawingObserver>[] State { get; set; }

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
    
    internal override IEnumerable<Element?> GetChildren()
    {
        yield return Content;
        yield return Spacer;
    }
    
    private void BuildState()
    {
        if (State != null)
            return;
        
        this.VisitChildren(child =>
        {
            child.CreateProxy(x => x is IStateful ? new MultiColumnChildDrawingObserver { Child = x } : x);
        });
        
        State = this.ExtractElementsOfType<MultiColumnChildDrawingObserver>().ToArray();
    }

    internal override SpacePlan Measure(Size availableSpace)
    {
        BuildState();
        OptimizeTextCacheBehavior();
        
        if (Content.Canvas != ChildrenCanvas)
            Content.InjectDependencies(PageContext, ChildrenCanvas);
        
        ChildrenCanvas.Target = new FreeDrawingCanvas();
        
        return FindPerfectSpace();

        IEnumerable<SpacePlan> MeasureColumns(Size availableSpace)
        {
            var columnAvailableSpace = GetAvailableSpaceForColumn(availableSpace);
            
            foreach (var _ in Enumerable.Range(0, ColumnCount))
            {
                yield return Content.Measure(columnAvailableSpace);
                Content.Draw(columnAvailableSpace);
            }
            
            ResetObserverState(restoreChildState: true);
        }
        
        SpacePlan FindPerfectSpace()
        {
            var defaultMeasurement = MeasureColumns(availableSpace).ToArray();

            if (defaultMeasurement.First().Type is SpacePlanType.Empty or SpacePlanType.Wrap)
                return defaultMeasurement.First();
            
            var maxHeight = defaultMeasurement.Max(x => x.Height);
            
            if (defaultMeasurement.Last().Type is SpacePlanType.PartialRender or SpacePlanType.Wrap)
                return SpacePlan.PartialRender(availableSpace.Width, maxHeight);
            
            if (!BalanceHeight)
                return SpacePlan.FullRender(availableSpace.Width, maxHeight);

            var minHeight = 0f;
            maxHeight = availableSpace.Height;
            
            foreach (var _ in Enumerable.Range(0, 8))
            {
                var middleHeight = (minHeight + maxHeight) / 2;
                var middleMeasurement = MeasureColumns(new Size(availableSpace.Width, middleHeight));
                
                if (middleMeasurement.Last().Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                    maxHeight = middleHeight;
                
                else
                    minHeight = middleHeight;
            }
            
            return SpacePlan.FullRender(new Size(availableSpace.Width, maxHeight));
        }
    }

    Size GetAvailableSpaceForColumn(Size totalSpace)
    {
        var columnWidth = (totalSpace.Width - Spacing * (ColumnCount - 1)) / ColumnCount;
        return new Size(columnWidth, totalSpace.Height);
    }
    
    internal override void Draw(Size availableSpace)
    {
        var contentAvailableSpace = GetAvailableSpaceForColumn(availableSpace);
        var spacerAvailableSpace = new Size(Spacing, availableSpace.Height);

        var horizontalOffset = 0f;
        ChildrenCanvas.Target = Canvas;

        foreach (var i in Enumerable.Range(1, ColumnCount))
        {
            var contentMeasurement = Content.Measure(contentAvailableSpace);
            var targetColumnSize = new Size(contentAvailableSpace.Width, contentMeasurement.Height);

            var contentOffset = GetTargetOffset(targetColumnSize.Width);
            
            Canvas.Translate(contentOffset);
            Content.Draw(targetColumnSize);
            Canvas.Translate(contentOffset.Reverse());
            
            horizontalOffset += contentAvailableSpace.Width;
            
            if (contentMeasurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                break;
            
            var spacerMeasurement = Spacer.Measure(spacerAvailableSpace);

            if (i == ColumnCount || spacerMeasurement.Type is SpacePlanType.Wrap) 
                continue;
            
            var spacerOffset = GetTargetOffset(Spacing);
            
            Canvas.Translate(spacerOffset);
            Spacer.Draw(spacerAvailableSpace);
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
                
            foreach (var child in node.Children)
                Traverse(child);
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