using System.Collections.Generic;
using QuestPDF.Companion;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Infrastructure;
using Image = QuestPDF.Elements.Image;
using SvgImage = QuestPDF.Elements.SvgImage;

namespace QuestPDF.Drawing.Proxy;

internal sealed class LayoutProxy : ElementProxy
{
    public List<CompanionCommands.UpdateDocumentStructure.PageLocation> Snapshots { get; } = new();
    public List<CompanionCommands.UpdateDocumentStructure.LayoutErrorMeasurement> LayoutErrorMeasurements { get; } = new();

    public LayoutProxy(Element child)
    {
        Child = child;
    }
    
    internal override void Draw(Size availableSpace)
    {
        var size = ProvideIntrinsicSize() ? Child.Measure(availableSpace) : availableSpace;
        
        base.Draw(availableSpace);
        
        var canvas = Canvas as SkiaCanvasBase;
        
        if (canvas == null)
            return;
        
        var position = canvas.Canvas.GetCurrentTotalMatrix();

        Snapshots.Add(new CompanionCommands.UpdateDocumentStructure.PageLocation
        {
            PageNumber = PageContext.CurrentPage,
            Left = position.TranslateX,
            Top = position.TranslateY,
            Right = position.TranslateX + size.Width,
            Bottom = position.TranslateY + size.Height
        });

        bool ProvideIntrinsicSize()
        {
            // Image or DynamicImage or SvgImage or DynamicSvgImage should be excluded
            // They rely on the AspectRation component to provide true intrinsic size
            
            return Child is TextBlock or AspectRatio or Unconstrained;
        }
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
                
                LayoutErrorMeasurements.Add(new CompanionCommands.UpdateDocumentStructure.LayoutErrorMeasurement
                {
                    PageNumber = PageContext.CurrentPage,
                    AvailableSpace = new CompanionCommands.ElementSize
                    {
                        Width = overflowDebuggingProxy.AvailableSpace.Value.Width,
                        Height = overflowDebuggingProxy.AvailableSpace.Value.Height
                    },
                    MeasurementSize = new CompanionCommands.ElementSize
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