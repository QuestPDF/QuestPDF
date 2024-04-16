using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class MultiColumn : ContainerElement
{
    public int ColumnCount { get; set; } = 2;
    public bool MinimizeHeight { get; set; } = true;
    public float Spacing { get; set; }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        Child.InjectCanvas(new FreeCanvas());
        
        var originalState = GetState();
        return FindPerfectSpace();
        
        ICollection<(Element element, object state)> GetState()
        {
            var result = new List<(Element element, object state)>();
            
            Child.VisitChildren(x =>
            {
                if (x is IStateful stateful)
                    result.Add((x, stateful.CloneState()));
            });
            
            return result;
        }

        void SetState(ICollection<(Element element, object state)> state)
        {
            foreach (var (element, elementState) in state)
            {
                (element as IStateful).SetState(elementState);
            }
        }

        IEnumerable<SpacePlan> MeasureColumns(Size availableSpace)
        {
            var columnAvailableSpace = GetAvailableSpaceForColumn(availableSpace);
            
            foreach (var _ in Enumerable.Range(0, ColumnCount))
            {
                yield return Child.Measure(columnAvailableSpace);
                Child.Draw(columnAvailableSpace);
            }
            
            SetState(originalState);
        }
        
        SpacePlan FindPerfectSpace()
        {
            var defaultMeasurement = MeasureColumns(availableSpace);

            if (defaultMeasurement.First().Type is SpacePlanType.Wrap or SpacePlanType.NoContent)
                return defaultMeasurement.First();
            
            if (defaultMeasurement.Last().Type is SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(availableSpace);
            
            if (!MinimizeHeight)
                return SpacePlan.FullRender(availableSpace);

            var minHeight = 0f;
            var maxHeight = availableSpace.Height;
            
            foreach (var _ in Enumerable.Range(0, 8))
            {
                var middleHeight = (minHeight + maxHeight) / 2;
                var middleMeasurement = MeasureColumns(new Size(availableSpace.Width, middleHeight));
                
                if (middleMeasurement.Last().Type is SpacePlanType.NoContent or SpacePlanType.FullRender)
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
        var columnAvailableSpace = GetAvailableSpaceForColumn(availableSpace);
        Child.InjectCanvas(Canvas);
        
        Canvas.Save();
        
        foreach (var i in Enumerable.Range(0, ColumnCount))
        {
            var columnMeasurement = Child.Measure(columnAvailableSpace);
            
            Child.Draw(columnAvailableSpace);
            Canvas.Translate(new Position(columnAvailableSpace.Width + Spacing, 0));
            
            if (columnMeasurement.Type is SpacePlanType.NoContent or SpacePlanType.FullRender)
                break;
        }
        
        Canvas.Restore();
    }
}