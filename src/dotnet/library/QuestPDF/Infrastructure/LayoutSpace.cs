using System;

namespace QuestPDF.Infrastructure
{
    /// <summary>
    /// Describes what the length offered along one axis means to the measured element.
    /// </summary>
    internal enum LayoutAxisMode
    {
        /// <summary>
        /// The offered length is the largest the element can ever receive: no later page is wider,
        /// every later page is exactly as tall, and the enclosing box is fixed. A shortfall cannot be resolved
        /// by moving the content, so this is the only mode in which an element may adapt its own configuration.
        /// </summary>
        Final,

        /// <summary>
        /// More space may still arrive: the element is placed below other content and would receive
        /// an entire page if moved, or the offered length is a candidate in a search. The element describes
        /// itself exactly as configured and reports a shortfall as usual. This is the only mode in which
        /// a pagination hint, such as ShowEntire, requests a move: anywhere else the move could not buy more space.
        /// </summary>
        Flowing,

        /// <summary>
        /// The element is asked what it needs, rather than told what it may take. It describes itself
        /// exactly as configured; the length offered is only an upper bound for the answer.
        /// </summary>
        Query
    }

    /// <summary>
    /// The space offered to an element: how much, and what that amount means along each axis.
    /// </summary>
    /// <remarks>
    /// <para>Containers derive the space of their children from their own, in the same way while measuring
    /// and while drawing, so that both operations reach identical decisions and share measurement caches.
    /// Forwarding is the default: <see cref="With(Size)"/> keeps the axis modes, and only a container
    /// that changes what an axis means has to say so.</para>
    /// </remarks>
    internal readonly struct LayoutSpace : IEquatable<LayoutSpace>
    {
        public readonly Size Size;
        public readonly LayoutAxisMode WidthMode;
        public readonly LayoutAxisMode HeightMode;

        public float Width => Size.Width;
        public float Height => Size.Height;

        public bool IsWidthFinal => WidthMode == LayoutAxisMode.Final;
        public bool IsHeightFinal => HeightMode == LayoutAxisMode.Final;

        public LayoutSpace(Size size, LayoutAxisMode widthMode, LayoutAxisMode heightMode)
        {
            Size = size;
            WidthMode = widthMode;
            HeightMode = heightMode;
        }

        /// <summary>A question about the natural size of the element.</summary>
        public static LayoutSpace Query(Size size) => new(size, LayoutAxisMode.Query, LayoutAxisMode.Query);

        /// <summary>A candidate area in a search, e.g. one scale tried by ScaleToFit.</summary>
        public static LayoutSpace Candidate(Size size) => new(size, LayoutAxisMode.Flowing, LayoutAxisMode.Flowing);

        /// <summary>A fixed box that the element can never outgrow.</summary>
        public static LayoutSpace Final(Size size) => new(size, LayoutAxisMode.Final, LayoutAxisMode.Final);

        /// <summary>Changes the amount of space while keeping what it means. This is how containers forward space to their children.</summary>
        public LayoutSpace With(Size size) => new(size, WidthMode, HeightMode);
        public LayoutSpace With(float width, float height) => new(new Size(width, height), WidthMode, HeightMode);

        public LayoutSpace WithWidthMode(LayoutAxisMode mode) => new(Size, mode, HeightMode);
        public LayoutSpace WithHeightMode(LayoutAxisMode mode) => new(Size, WidthMode, mode);

        /// <summary>
        /// Declares that the element is offered less along this axis than a fresh page would offer it,
        /// so a shortfall may still be resolved by moving the content. A box that is not final stays as it is.
        /// </summary>
        public LayoutSpace WithFlowingWidth() => IsWidthFinal ? WithWidthMode(LayoutAxisMode.Flowing) : this;
        public LayoutSpace WithFlowingHeight() => IsHeightFinal ? WithHeightMode(LayoutAxisMode.Flowing) : this;

        /// <summary>Swaps the axes, for content laid out with a quarter turn.</summary>
        public LayoutSpace Transposed => new(new Size(Size.Height, Size.Width), HeightMode, WidthMode);

        public static implicit operator Size(LayoutSpace space) => space.Size;

        public bool Equals(LayoutSpace other)
        {
            return Size.Width == other.Size.Width && Size.Height == other.Size.Height && WidthMode == other.WidthMode && HeightMode == other.HeightMode;
        }

        public override bool Equals(object? obj) => obj is LayoutSpace other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Size.Width.GetHashCode();
                hash = hash * 31 + Size.Height.GetHashCode();
                hash = hash * 31 + (int)WidthMode;
                hash = hash * 31 + (int)HeightMode;
                return hash;
            }
        }

        public override string ToString() => $"{Size} W:{WidthMode} H:{HeightMode}";
    }
}
