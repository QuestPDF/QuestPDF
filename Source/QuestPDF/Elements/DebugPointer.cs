namespace QuestPDF.Elements
{
    internal enum DebugPointerType
    {
        LayoutStructure,
        Component,
        UserDefined
    }
    
    internal sealed class DebugPointer : Container
    {
        public DebugPointerType Type { get; set; }
        public string Label { get; set; }
    }
}