using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuestPDF.Drawing;

namespace QuestPDF.Infrastructure
{
    internal abstract class Element : IElement
    {
        internal IPageContext PageContext { get; set; }
        internal IDrawingCanvas Canvas { get; set; }
        internal SourceCodePath? CodeLocation { get; set; }
        
        internal virtual IEnumerable<Element?> GetChildren()
        {
            yield break;
        }

        internal virtual void CreateProxy(Func<Element?, Element?> create)
        {
            
        }
        
        internal abstract SpacePlan Measure(Size availableSpace);
        internal abstract void Draw(Size availableSpace);

        internal virtual string? GetCompanionHint() => null;
        internal virtual string? GetCompanionSearchableContent() => null;
        internal virtual IEnumerable<KeyValuePair<string, string>>? GetCompanionProperties() => null;
    }
}