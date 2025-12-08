using System.Collections.Generic;
using QuestPDF.Companion;
using QuestPDF.Drawing.DrawingCanvases;
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

        if (Canvas is FreeDrawingCanvas)
            return;
        
        var matrix = Canvas.GetCurrentMatrix();
        
        Snapshots.Add(new CompanionCommands.UpdateDocumentStructure.PageLocation
        {
            PageNumber = PageContext.CurrentPage,
            Left = matrix.TranslateX,
            Top = matrix.TranslateY,
            Right = matrix.TranslateX + size.Width,
            Bottom = matrix.TranslateY + size.Height
        });

        bool ProvideIntrinsicSize()
        {
            // Image or DynamicImage or SvgImage or DynamicSvgImage should be excluded
            // They rely on the AspectRation component to provide true intrinsic size
            
            return Child is TextBlock or AspectRatio or Unconstrained or SemanticTag or ArtifactTag;
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