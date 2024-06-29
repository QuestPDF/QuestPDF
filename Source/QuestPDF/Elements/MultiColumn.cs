using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class MultiColumnChildDrawingObserver : ContainerElement
{
    public bool HasBeenDrawn => ChildStateBeforeDrawingOperation != null;
    public object? ChildStateBeforeDrawingOperation { get; private set; }

    internal override void Draw(Size availableSpace)
    {
        ChildStateBeforeDrawingOperation ??= (Child as IStateful).GetState();
        
        Child.Draw(availableSpace);
    }
    
    internal void ResetDrawingState()
    {
        ChildStateBeforeDrawingOperation = null;
    }

    internal void RestoreState()
    {
        (Child as IStateful)?.SetState(ChildStateBeforeDrawingOperation);
    }
}

internal class MultiColumn : Element, IContentDirectionAware
{
    // items
    internal Element Content { get; set; } = Empty.Instance;
    internal Element Decoration { get; set; } = Empty.Instance;
    
    // configuration
    public int ColumnCount { get; set; } = 2;
    public bool BalanceHeight { get; set; } = false;
    public float Spacing { get; set; }
    
    public ContentDirection ContentDirection { get; set; }

    // cache
    private ProxyCanvas ChildrenCanvas { get; } = new();
    private TreeNode<MultiColumnChildDrawingObserver>[] State { get; set; }

    internal override void CreateProxy(Func<Element?, Element?> create)
    {
        Content = create(Content);
        Decoration = create(Decoration);
    }
    
    internal override IEnumerable<Element?> GetChildren()
    {
        yield return Content;
        yield return Decoration;
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
        
        if (Content.Canvas != ChildrenCanvas)
            Content.InjectDependencies(PageContext, ChildrenCanvas);
        
        ChildrenCanvas.Target = new FreeCanvas();
        
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
            var defaultMeasurement = MeasureColumns(availableSpace);

            if (defaultMeasurement.First().Type is SpacePlanType.Wrap or SpacePlanType.Empty)
                return defaultMeasurement.First();
            
            var maxHeight = defaultMeasurement.Max(x => x.Height);
            
            if (defaultMeasurement.Last().Type is SpacePlanType.PartialRender)
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
        var decorationAvailableSpace = new Size(Spacing, availableSpace.Height);

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
            
            var decorationMeasurement = Decoration.Measure(decorationAvailableSpace);

            if (i == ColumnCount || decorationMeasurement.Type is SpacePlanType.Wrap) 
                continue;
            
            var decorationOffset = GetTargetOffset(Spacing);
            
            Canvas.Translate(decorationOffset);
            Decoration.Draw(decorationAvailableSpace);
            Canvas.Translate(decorationOffset.Reverse());
                
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
}