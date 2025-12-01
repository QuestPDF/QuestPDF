using System.Collections.Generic;

namespace QuestPDF.Interop.Generators;

public class NativeInteropMethodTemplateModel
{
    public string NativeName { get; set; }
    public string ManagedName { get; set; }
    public string ApiName { get; set; }
    public IEnumerable<string> MethodParameters { get; set; }
    public bool IsStaticMethod { get; set; }
    public string TargetObjectName { get; set; }
    public string TargetObjectType { get; set; }
    public string TargetObjectParameterName { get; set; }
    public IEnumerable<string> TargetMethodParameters { get; set; }
    public string ReturnType { get; set; }
    public string ResultTransformFunction { get; set; }
    public bool ShouldFreeTarget { get; set; }
}