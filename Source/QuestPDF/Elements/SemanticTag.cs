using System;
using System.Text;
using QuestPDF.Elements.Text;
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
        Canvas.SetSemanticNodeId(Id);
        Child?.Draw(availableSpace);
    }

    internal void UpdateAlternativeText()
    {
        if (!string.IsNullOrWhiteSpace(Alt))
            return;
        
        var builder = new StringBuilder();
        Traverse(builder, Child);
        Alt = builder.ToString();
        
        static void Traverse(StringBuilder builder, Element element)
        {
            if (element is TextBlock textBlock)
            {
                builder.Append(textBlock.Text).Append(' ');
            }
            else if (element is ContainerElement container)
            {
                Traverse(builder, container);
            }
            else
            {
                foreach (var child in element.GetChildren())
                    Traverse(builder, child);
            }
        }
    }
}