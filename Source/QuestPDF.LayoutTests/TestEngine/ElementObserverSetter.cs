using QuestPDF.Helpers;

namespace QuestPDF.LayoutTests.TestEngine;

internal class ElementObserverSetter : ContainerElement
{
    public required DrawingRecorder Recorder { get; init; }
    
    internal override void Draw(Size availableSpace)
    {
        this.VisitChildren(x =>
        {
            if (x is ElementObserver observer)
                observer.DrawingRecorder = Recorder;
        });
        
        base.Draw(availableSpace);
    }
}