using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing.Proxy;

internal class LayoutProxy : ElementProxy
{
    public List<PreviewerCommands.UpdateDocumentStructure.PageLocation> Snapshots { get; } = new();
    public List<PreviewerCommands.UpdateDocumentStructure.LayoutErrorMeasurement> LayoutErrorMeasurements { get; } = new();

    public LayoutProxy(Element child)
    {
        Child = child;
    }
    
    internal override void Draw(Size availableSpace)
    {
        base.Draw(availableSpace);
        
        var canvas = Canvas as SkiaCanvasBase;
        
        if (canvas == null)
            return;
        
        var position = canvas.Canvas.GetCurrentTotalMatrix();

        Snapshots.Add(new PreviewerCommands.UpdateDocumentStructure.PageLocation
        {
            PageNumber = PageContext.CurrentPage,
            Left = position.TranslateX,
            Top = position.TranslateY,
            Right = position.TranslateX + availableSpace.Width,
            Bottom = position.TranslateY + availableSpace.Height
        });
    }

    internal void CaptureLayoutErrorMeasurement()
    {
        var child = Child;
        
        while (true)
        {
            if (child is OverflowDebuggingProxy overflowDebuggingProxy)
            {
                if (overflowDebuggingProxy.AvailableSpace == null || overflowDebuggingProxy.SpacePlan == null)
                    break;
                
                LayoutErrorMeasurements.Add(new PreviewerCommands.UpdateDocumentStructure.LayoutErrorMeasurement
                {
                    PageNumber = PageContext.CurrentPage,
                    AvailableSpace = new PreviewerCommands.ElementSize
                    {
                        Width = overflowDebuggingProxy.AvailableSpace.Value.Width,
                        Height = overflowDebuggingProxy.AvailableSpace.Value.Height
                    },
                    MeasurementSize = new PreviewerCommands.ElementSize
                    {
                        Width = overflowDebuggingProxy.SpacePlan.Value.Width,
                        Height = overflowDebuggingProxy.SpacePlan.Value.Height
                    },
                    SpacePlanType = overflowDebuggingProxy.SpacePlan?.Type,
                    WrapReason = overflowDebuggingProxy.SpacePlan?.WrapReason,
                    IsLayoutErrorRootCause = overflowDebuggingProxy.Child.GetType() == typeof(LayoutOverflowVisualization)
                });
            }

            if (child is not ElementProxy proxy)
                break;
            
            child = proxy.Child;
        }
    }
}