namespace QuestPDF.Elements
{
    internal enum InspectorPointerType
    {
        PageStructure, // bookmark
        Section, // bookmark
        Method, // code
        Component, // widgets
        Custom // star
    }
    
    internal sealed class InspectorPointer : Container
    {
        public InspectorPointerType Type { get; set; }
        public string Label { get; set; }
    }
}