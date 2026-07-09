using QuestPDF.Drawing;
using QuestPDF.Helpers;

namespace QuestPDF.LayoutTests.TestEngine;

internal class ElementObserverSetter : ContainerElement
{
    public required DrawingRecorder Recorder { get; init; }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        SetRecorderOnChildren();
        return base.Measure(availableSpace);
    }
    
    internal override void Draw(Size availableSpace)
    {
        SetRecorderOnChildren();
        base.Draw(availableSpace);
    }
    
    private void SetRecorderOnChildren()
    {
        this.VisitChildren(x =>
        {
            if (x is ElementObserver observer)
                observer.DrawingRecorder = Recorder;
        });
    }
}