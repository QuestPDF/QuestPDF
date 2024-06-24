using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    internal readonly struct SpacePlan
    {
        public readonly SpacePlanType Type;
        public readonly float Width;
        public readonly float Height;
        public readonly string? WrapReason;

        internal SpacePlan(SpacePlanType type, float width, float height, string? wrapReason = null)
        {
            Type = type;
            Width = width;
            Height = height;
            WrapReason = wrapReason;
        }

        internal static SpacePlan Empty() => new(SpacePlanType.Empty, 0, 0);
        
        internal static SpacePlan Wrap(string reason = null) => new(SpacePlanType.Wrap, 0, 0, reason);
        
        internal static SpacePlan PartialRender(float width, float height) => new(SpacePlanType.PartialRender, width, height);

        internal static SpacePlan PartialRender(Size size) => PartialRender(size.Width, size.Height);
        
        internal static SpacePlan FullRender(float width, float height) => new(SpacePlanType.FullRender, width, height);

        internal static SpacePlan FullRender(Size size) => FullRender(size.Width, size.Height);

        internal SpacePlan Forward()
        {
            if (Type == SpacePlanType.Wrap)
                return Wrap("Forwarded from child");
            
            return new SpacePlan(Type, Width, Height);
        }
        
        public override string ToString()
        {
            if (Type == SpacePlanType.Wrap)
                return Type.ToString();
            
            return $"{Type} (Width: {Width:N3}, Height: {Height:N3})";
        }

        public static implicit operator Size(SpacePlan spacePlan)
        {
            return new Size(spacePlan.Width, spacePlan.Height);
        }
    }
}