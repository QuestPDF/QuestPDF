using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer.Inspection;

namespace QuestPDF.Previewer;

internal sealed class DocumentElementProperty
{
    public string Label { get; set; }
    public string Value { get; set; }
}

internal sealed class Size
{
    public float Width { get; set; }
    public float Height { get; set; }
}

internal sealed class SpacePlan
{
    public float Width { get; set; }
    public float Height { get; set; }
    public SpacePlanType Type { get; set; }
}



internal sealed class LayoutErrorTrace
{
    public string ElementType { get; set; }
    public bool IsSingleChildContainer { get; set; }
    public IReadOnlyCollection<DocumentElementProperty> ElementProperties { get; set; }
    public IReadOnlyCollection<LayoutErrorMeasurement> Measurements { get; set; }
    public IReadOnlyCollection<LayoutErrorTrace> Children { get; set; }
}

internal sealed class LayoutErrorMeasurement
{
    public Size AvailableSpace { get; set; }
    public SpacePlan SpacePlan { get; set; }
}






    
internal sealed class GenericError
{
    public string ExceptionType { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public GenericError? InnerException { get; set; }
}






