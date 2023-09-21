using System.ComponentModel;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class DebugPointer : ContainerElement
    {
        public string Target { get; set; }
        public bool Highlight { get; set; }
    }
}