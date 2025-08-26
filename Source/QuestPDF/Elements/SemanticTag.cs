using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SemanticTag : ContainerElement
{
    public int Id { get; set; }
    public string TagType { get; set; }
    public string? Alt { get; set; }
    public string? Lang { get; set; }
    
    internal override void Draw(Size availableSpace)
    {
        Console.WriteLine($"{TagType}: {Id}");
        Canvas.SetSemanticNodeId(Id);
        Child?.Draw(availableSpace);
    }
}