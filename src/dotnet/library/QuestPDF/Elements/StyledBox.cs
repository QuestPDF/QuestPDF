using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements
{
    internal sealed class StyledBoxExtendedConfiguration
    {
        public float BorderRadiusTopLeft { get; set; }
        public float BorderRadiusTopRight { get; set; }
        public float BorderRadiusBottomLeft { get; set; }
        public float BorderRadiusBottomRight { get; set; }

        public bool HasRoundedCorners =>
            BorderRadiusTopLeft > 0 ||
            BorderRadiusTopRight > 0 ||
            BorderRadiusBottomLeft > 0 ||
            BorderRadiusBottomRight > 0;
        
        public bool HasUniformRoundedCorners =>
            BorderRadiusTopLeft == BorderRadiusTopRight &&
            BorderRadiusBottomLeft == BorderRadiusBottomRight &&
            BorderRadiusTopLeft == BorderRadiusBottomLeft;

        public float? BorderAlignment { get; set; } // 0 = inside, 0.5 = middle, 1 = outside

        public float EffectiveBorderAlignment => BorderAlignment ?? (HasRoundedCorners ? 0f : 0.5f);

        public Color[] BackgroundGradientColors { get; set; } = [];
        public float? BackgroundGradientAngle { get; set; }

        public Color[] BorderGradientColors { get; set; } = [];
        public float? BorderGradientAngle { get; set; }

        public BoxShadowStyle? Shadow { get; set; }
    }

    internal sealed class StyledBox : ContainerElement
    {
        public float BorderLeft { get; set; }
        public float BorderTop { get; set; }
        public float BorderRight { get; set; }
        public float BorderBottom { get; set; }

        public Color BackgroundColor { get; set; } = Colors.Transparent;
        public Color BorderColor { get; set; } = Colors.Transparent;

        // optimization: the vast majority of styled boxes use only a solid background and/or a uniform border;
        // rarely used settings live in a separate object so that typical instances stay small
        public StyledBoxExtendedConfiguration? ExtendedConfiguration { get; set; }

        private static readonly StyledBoxExtendedConfiguration DefaultExtendedConfiguration = new();

        public StyledBoxExtendedConfiguration GetOrCreateExtendedConfiguration()
        {
            return ExtendedConfiguration ??= new StyledBoxExtendedConfiguration();
        }

        private bool HasBorder =>
            BorderLeft > 0 ||
            BorderTop > 0 ||
            BorderRight > 0 ||
            BorderBottom > 0;

        private bool HasUniformBorder =>
            BorderLeft == BorderRight &&
            BorderTop == BorderBottom &&
            BorderLeft == BorderTop;

        internal override void Draw(LayoutSpace availableSpace)
        {
            // optimization: do not perform expensive calls
            if (Canvas is DiscardDrawingCanvas)
            {
                base.Draw(availableSpace);
                return;
            }

            if (ExtendedConfiguration == null && HasUniformBorder)
                DrawSimple(availableSpace);

            else
                DrawExtended(availableSpace);
        }

        // optimization: draws a solid background and/or a uniform middle-aligned border using only cached paints
        private void DrawSimple(LayoutSpace availableSpace)
        {
            if (BackgroundColor.Hex != Colors.Transparent.Hex)
            {
                using var semanticScope = Canvas.StartSemanticScopeWithNodeId(SkSemanticNodeSpecialId.BackgroundArtifact);

                using var backgroundPaint = SkPaintCache.GetSolidColor(BackgroundColor);
                Canvas.DrawRectangle(Position.Zero, availableSpace, backgroundPaint);
            }

            base.Draw(availableSpace);

            if (HasBorder && BorderColor.Hex != Colors.Transparent.Hex)
            {
                using var semanticScope = Canvas.StartSemanticScopeWithNodeId(SkSemanticNodeSpecialId.LayoutArtifact);

                using var borderPaint = SkPaintCache.GetStroke(BorderColor, BorderLeft);
                Canvas.DrawRectangle(Position.Zero, availableSpace, borderPaint);
            }
        }

        private void DrawExtended(LayoutSpace availableSpace)
        {
            var configuration = ExtendedConfiguration ?? DefaultExtendedConfiguration;

            using var backgroundPaint = GetPaint(availableSpace, BackgroundColor, configuration.BackgroundGradientColors, configuration.BackgroundGradientAngle);
            using var borderPaint = GetPaint(availableSpace, BorderColor, configuration.BorderGradientColors, configuration.BorderGradientAngle);

            var borderAlignment = configuration.EffectiveBorderAlignment;

            var contentRect = GetPrimaryBorderRect(availableSpace, configuration);
            var borderOuterRect = ExpandRoundedRectWithBorderThickness(contentRect, borderAlignment);
            var borderInnerRect = ExpandRoundedRectWithBorderThickness(contentRect, borderAlignment - 1f);

            if (configuration.Shadow != null)
            {
                var shadowRect = ExpandRoundedRect(contentRect, configuration.Shadow.Spread);

                var canvasShadow = new SkBoxShadow
                {
                    OffsetX = configuration.Shadow.OffsetX,
                    OffsetY = configuration.Shadow.OffsetY,
                    Blur = configuration.Shadow.Blur,
                    Color = configuration.Shadow.Color
                };
                
                using var semanticScope = Canvas.StartSemanticScopeWithNodeId(SkSemanticNodeSpecialId.BackgroundArtifact);
                Canvas.DrawShadow(shadowRect, canvasShadow);
            }

            if (configuration.HasRoundedCorners)
            {
                Canvas.Save();
                Canvas.ClipRoundedRectangle(contentRect);
            }

            if (backgroundPaint != null)
            {
                using var semanticScope = Canvas.StartSemanticScopeWithNodeId(SkSemanticNodeSpecialId.BackgroundArtifact);
                Canvas.DrawRectangle(Position.Zero, availableSpace, backgroundPaint);
            }
            
            base.Draw(availableSpace);
            
            if (configuration.HasRoundedCorners)
                Canvas.Restore();

            if (borderPaint != null)
            {
                using var semanticScope = Canvas.StartSemanticScopeWithNodeId(SkSemanticNodeSpecialId.LayoutArtifact);
                Canvas.DrawComplexBorder(borderInnerRect, borderOuterRect, borderPaint);
            }
        }

        private static (Position start, Position end) GetLinearGradientPositions(Size availableSpace, float angle)
        {
            if (angle == 0f)
                return (Position.Zero, new Position(availableSpace.Width, 0));
            
            if (angle == 90f)
                return (Position.Zero, new Position(0, availableSpace.Height));
            
            if (angle == 180f)
                return (new Position(availableSpace.Width, 0), new Position(0, 0));
            
            if (angle == 270f)
                return (new Position(0, availableSpace.Height), new Position(availableSpace.Width, availableSpace.Height));
            
            // other angles?
            var rectanglePoints = new[]
            {
                Position.Zero,
                new Position(availableSpace.Width, 0),
                new Position(availableSpace.Width, availableSpace.Height),
                new Position(0, availableSpace.Height)
            };
            
            var angleInRadians = Math.PI * angle / 180f;
            var linePoint = new Position(availableSpace.Width / 2f, availableSpace.Height / 2f);
            
            var projectedPoints = rectanglePoints
                .Select(point => ProjectPointOntoLine(linePoint, (float)angleInRadians, point))
                .ToArray();

            var start = projectedPoints.OrderBy(p => p.X).First();
            var end = projectedPoints.OrderByDescending(p => p.X).First();
            
            return (start, end);
            
            static Position ProjectPointOntoLine(Position linePoint, float lineAngleRadians, Position projectionPoint)
            {
                var dx = (float)Math.Cos(lineAngleRadians);
                var dy = (float)Math.Sin(lineAngleRadians);
        
                var vx = projectionPoint.X - linePoint.X;
                var vy = projectionPoint.Y - linePoint.Y;
        
                var t = vx * dx + vy * dy;
        
                return new Position(
                    linePoint.X + dx * t,
                    linePoint.Y + dy * t
                );
            }
        }
        
        private static SkPaint? GetPaint(Size availableSpace, Color solidColor, Color[] gradientColors, float? gradientAngle)
        {
            if (gradientColors.Length > 0)
            {
                var paint = new SkPaint();
                var gradientPoints = GetLinearGradientPositions(availableSpace, gradientAngle ?? 0);
                paint.SetLinearGradient(gradientPoints.start, gradientPoints.end, gradientColors);
                return paint;
            }

            if (solidColor.Hex == Colors.Transparent.Hex)
                return null;

            return SkPaintCache.GetSolidColor(solidColor);
        }
        
        private static SkRoundedRect GetPrimaryBorderRect(Size availableSpace, StyledBoxExtendedConfiguration configuration)
        {
            return new SkRoundedRect
            {
                Rect = new SkRect
                {
                    Left = 0,
                    Top = 0,
                    Right = availableSpace.Width,
                    Bottom = availableSpace.Height
                },
                TopLeftRadius = new SkPoint(configuration.BorderRadiusTopLeft, configuration.BorderRadiusTopLeft),
                TopRightRadius = new SkPoint(configuration.BorderRadiusTopRight, configuration.BorderRadiusTopRight),
                BottomLeftRadius = new SkPoint(configuration.BorderRadiusBottomLeft, configuration.BorderRadiusBottomLeft),
                BottomRightRadius = new SkPoint(configuration.BorderRadiusBottomRight, configuration.BorderRadiusBottomRight)
            };
        }

        private SkRoundedRect ExpandRoundedRectWithBorderThickness(SkRoundedRect primaryRect, float borderThicknessExpansionFactor)
        {
            return ExpandRoundedRect(
                primaryRect,
                borderThicknessExpansionFactor * BorderLeft,
                borderThicknessExpansionFactor * BorderTop,
                borderThicknessExpansionFactor * BorderRight,
                borderThicknessExpansionFactor * BorderBottom);
        }

        private static SkRoundedRect ExpandRoundedRect(SkRoundedRect rect, float all)
        {
            return ExpandRoundedRect(rect, all, all, all, all);
        }
        
        private static SkRoundedRect ExpandRoundedRect(SkRoundedRect input, float left, float top, float right, float bottom)
        {
            var rect = new SkRect
            {
                Left = input.Rect.Left - left,
                Top = input.Rect.Top - top,
                Right = input.Rect.Right + right,
                Bottom = input.Rect.Bottom + bottom
            };
            
            var hasRoundedCorners = 
                input.TopLeftRadius.X > 0 || 
                input.TopRightRadius.X > 0 || 
                input.BottomLeftRadius.X > 0 || 
                input.BottomRightRadius.X > 0;
            
            if (!hasRoundedCorners)
                return new SkRoundedRect { Rect = rect };
            
            return new SkRoundedRect
            {
                Rect = rect,
                TopLeftRadius = new SkPoint
                {
                    X = Math.Max(0, input.TopLeftRadius.X + left),
                    Y = Math.Max(0, input.TopLeftRadius.Y + top)
                },
                TopRightRadius = new SkPoint
                {
                    X = Math.Max(0, input.TopRightRadius.X + right),
                    Y = Math.Max(0, input.TopRightRadius.Y + top)
                },
                BottomLeftRadius = new SkPoint
                {
                    X = Math.Max(0, input.BottomLeftRadius.X + left),
                    Y = Math.Max(0, input.BottomLeftRadius.Y + bottom)
                },
                BottomRightRadius = new SkPoint
                {
                    X = Math.Max(0, input.BottomRightRadius.X + right),
                    Y = Math.Max(0, input.BottomRightRadius.Y + bottom)
                }
            };
        }

        internal IEnumerable<(string Type, string? Hint)> GetCompanionCustomContent()
        {
            var configuration = ExtendedConfiguration ?? DefaultExtendedConfiguration;

            // shadow
            if (configuration.Shadow != null)
                yield return ("Shadow", null);

            // rounded corners
            if (configuration.HasRoundedCorners)
            {
                if (configuration.HasUniformRoundedCorners)
                    yield return ("Border", $"R={configuration.BorderRadiusTopLeft}");
                else
                    yield return ("Border", $"TL={configuration.BorderRadiusTopLeft}   TR={configuration.BorderRadiusTopRight}   BL={configuration.BorderRadiusBottomLeft}   BR={configuration.BorderRadiusBottomRight}");
            }

            // border
            if (HasBorder)
            {
                var color = configuration.BorderGradientColors.Any() ? "gradient" : BorderColor.ToString();

                if (HasUniformBorder)
                    yield return ("Border", $"A={BorderLeft}   C={color}");
                else
                    yield return ("Border", $"L={BorderLeft}   T={BorderTop}   R={BorderRight}   B={BorderBottom}   C={color}");
            }

            // background
            if (configuration.BackgroundGradientColors.Length > 0)
                yield return ("Background", $"Gradient with {configuration.BackgroundGradientColors.Length} colors");

            else if (BackgroundColor.Hex != Colors.Transparent.Hex)
                yield return ("Background", BackgroundColor);
        }
    }
}
