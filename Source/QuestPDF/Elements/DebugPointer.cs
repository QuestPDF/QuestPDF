using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal enum DocumentStructureTypes
    {
        Document,
        Page,
        Background,
        Foreground,
        Header,
        Content,
        Footer
    }
    
    internal enum DebugPointerType
    {
        DocumentStructure,
        ElementStructure,
        Component,
        Section,
        Dynamic,
        UserDefined
    }
    
    internal sealed class DebugPointer : ContainerElement
    {
        public DebugPointerType Type { get; set; }
        public string Label { get; set; }
    }
}