using System.Collections.Generic;
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
        
        internal override string? GetCompanionSearchableContent() => Label;
        
        internal override IEnumerable<KeyValuePair<string, string>>? GetCompanionProperties()
        {
            yield return new("Type", Type.ToString());
            yield return new("Label", Label);
        }
    }
}