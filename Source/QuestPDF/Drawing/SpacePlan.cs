using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    internal readonly struct SpacePlan
    {
        public readonly SpacePlanType Type;
        public readonly float Width;
        public readonly float Height;

        internal SpacePlan(SpacePlanType type, float width, float height)
        {
            Type = type;
            Width = width;
            Height = height;
        }

        internal static SpacePlan Wrap()
            => new(SpacePlanType.Wrap, 0, 0);

        internal static SpacePlan PartialRender(float width, float height)
            => new(SpacePlanType.PartialRender, width, height);

        internal static SpacePlan PartialRender(Size size)
            => PartialRender(size.Width, size.Height);

        internal static SpacePlan FullRender(float width, float height)
            => new(SpacePlanType.FullRender, width, height);

        internal static SpacePlan FullRender(Size size)
            => FullRender(size.Width, size.Height);

        public override string ToString()
            => Type switch
            {
                SpacePlanType.Wrap => Type.ToString(),
                _ => $"{Type} (Width: {Width:N3}, Height: {Height:N3})",
            };

        public static implicit operator Size(SpacePlan spacePlan)
            => new(spacePlan.Width, spacePlan.Height);
    }
}