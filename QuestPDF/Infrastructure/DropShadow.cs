namespace QuestPDF.Infrastructure
{
    public class DropShadow
    {
        public readonly float OffsetX;
        public readonly float OffsetY;
        public readonly float BlurX;
        public readonly float BlurY;
        public readonly string Color;
        public readonly object Key;

        public DropShadow(float offsetX, float offsetY, float blurX, float blurY, string color)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            BlurX = blurX;
            BlurY = blurY;
            Color = color;
            Key = (OffsetX, OffsetY, BlurX, BlurY, Color);
        }
    }
}
