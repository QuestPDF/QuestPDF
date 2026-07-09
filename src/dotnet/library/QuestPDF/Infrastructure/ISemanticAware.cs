using QuestPDF.Drawing;

namespace QuestPDF.Infrastructure;

internal interface ISemanticAware
{
    public SemanticTreeManager? SemanticTreeManager { get; set; }
}