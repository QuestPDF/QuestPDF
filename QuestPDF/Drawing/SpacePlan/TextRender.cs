using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.SpacePlan
{
    internal class TextRender : FullRender
    {
        public float Ascent { get; set; }
        public float Descent { get; set; }
        
        public TextRender(float width, float height) : base(width, height)
        {
            
        }
    }
}