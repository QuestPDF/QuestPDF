using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class ArtifactTag : ContainerElement, ISemanticAware
{
    public SemanticTreeManager? SemanticTreeManager { get; set; }
    
    public int Id { get; set; }
    
    internal override void Draw(Size availableSpace)
    {
        if (SemanticTreeManager == null)
        {
            base.Draw(availableSpace);
            return;       
        }
        
        using var semanticScope = Canvas.StartSemanticScopeWithNodeId(Id);
        
        SemanticTreeManager.BeginArtifactContent();
        Child?.Draw(availableSpace);
        SemanticTreeManager.EndArtifactContent();
    }
}