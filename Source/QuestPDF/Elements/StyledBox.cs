using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements
{
    internal sealed class StyledBox : ContainerElement
    {
        public float BorderLeft { get; set; }
        public float BorderTop { get; set; }
        public float BorderRight { get; set; }
        public float BorderBottom { get; set; }

        private bool HasBorder =>
            BorderLeft > 0 || 
            BorderTop > 0 || 
            BorderRight > 0 || 
            BorderBottom > 0;
        
        private bool HasFullBorder =>
            BorderLeft > 0 && 
            BorderTop > 0 && 
            BorderRight > 0 && 
            BorderBottom > 0;

        private bool HasUniformBorder =>
            BorderLeft == BorderRight && 
            BorderTop == BorderBottom && 
            BorderLeft == BorderTop;
    
        public float BorderRadiusTopLeft { get; set; }
        public float BorderRadiusTopRight { get; set; }
        public float BorderRadiusBottomLeft { get; set; }
        public float BorderRadiusBottomRight { get; set; }

        private bool HasRoundedCorners =>
            BorderRadiusTopLeft > 0 || 
            BorderRadiusTopRight > 0 || 
            BorderRadiusBottomLeft > 0 || 
            BorderRadiusBottomRight > 0;
        
        private bool HasUniformRoundedCorners =>
            BorderRadiusTopLeft == BorderRadiusTopRight && 
            BorderRadiusBottomLeft == BorderRadiusBottomRight && 
            BorderRadiusTopLeft == BorderRadiusBottomLeft;
  
        public float? BorderAlignment { get; set; } // 0 = inset, 1 = outset
    
        public Color BackgroundColor { get; set; } = Colors.Transparent;
        public Color[] BackgroundGradientColors { get; set; } = [];
        public float? BackgroundGradientAngle { get; set; }

        public Color BorderColor { get; set; } = Colors.Transparent;
        public Color[] BorderGradientColors { get; set; } = [];
        public float? BorderGradientAngle { get; set; }

        private bool HasSimpleStyle => BackgroundGradientColors.Length == 0 && BorderGradientColors.Length == 0;
        
        public BoxShadowStyle? Shadow { get; set; }
        
        private void AdjustBorderAlignment()
        {
            if (BorderAlignment != null) 
                return;

            var shouldHaveInsetBorder = HasRoundedCorners;
            BorderAlignment = shouldHaveInsetBorder ? 0f : 0.5f;
        }
        
        internal override void Draw(Size availableSpace)
        {
            AdjustBorderAlignment();

            // optimization: do not perform expensive calls
            if (Canvas is FreeDrawingCanvas)
            {
                base.Draw(availableSpace);
                return;
            }
            
            using var backgroundPaint = GetBackgroundPaint(availableSpace);
            using var borderPaint = GetBorderPaint(availableSpace);
            
            if (HasFullBorder && HasUniformBorder && !HasRoundedCorners && HasSimpleStyle && BorderAlignment == 0.5f && Shadow == null)
            {
                // optimization: draw a simple rectangle with border
                if (backgroundPaint != null)
                    Canvas.DrawRectangle(Position.Zero, availableSpace, backgroundPaint);
                
                base.Draw(availableSpace);
                
                if (borderPaint != null)
                {
                    borderPaint.SetStroke(BorderLeft);
                    Canvas.DrawRectangle(Position.Zero, availableSpace, borderPaint);
                }
                
                return;
            }
            
            var contentRect = GetPrimaryBorderRect(availableSpace);
            var borderOuterRect = GetOuterRect(availableSpace);
            var borderInnerRect = GetInnerRect(availableSpace);

            if (Shadow != null)
            {
                var shadowRect = ExpandRoundedRect(contentRect, Shadow.Spread);

                var canvasShadow = new SkBoxShadow
                {
                    OffsetX = Shadow.OffsetX,
                    OffsetY = Shadow.OffsetY,
                    Blur = Shadow.Blur,
                    Color = Shadow.Color
                };
                
                Canvas.DrawShadow(shadowRect, canvasShadow);
            }

            if (HasRoundedCorners)
            {
                Canvas.Save();
                Canvas.ClipRoundedRectangle(contentRect);
            }

            if (backgroundPaint != null)
                Canvas.DrawRectangle(Position.Zero, availableSpace, backgroundPaint);
            
            base.Draw(availableSpace);
            
            if (HasRoundedCorners)
                Canvas.Restore();

            if (borderPaint != null)
                Canvas.DrawComplexBorder(borderInnerRect, borderOuterRect, borderPaint);
        }

        private (Position start, Position end) GetLinearGradientPositions(Size availableSpace, float angle)
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
        
        private SkPaint? GetBorderPaint(Size availableSpace)
        {
            if (BorderGradientColors.Length > 0)
            {
                var paint = new SkPaint();
                var gradientPoints = GetLinearGradientPositions(availableSpace, BorderGradientAngle ?? 0);
                paint.SetLinearGradient(gradientPoints.start, gradientPoints.end, BorderGradientColors);
                return paint;
            }
            
            if (BorderColor.Hex != Colors.Transparent.Hex)
            {
                var paint = new SkPaint();
                paint.SetSolidColor(BorderColor);
                return paint;
            }

            return null;
        }
        
        private SkPaint? GetBackgroundPaint(Size availableSpace)
        {
            if (BackgroundGradientColors.Length > 0)
            {
                var paint = new SkPaint();
                var gradientPoints = GetLinearGradientPositions(availableSpace, BackgroundGradientAngle ?? 0);
                paint.SetLinearGradient(gradientPoints.start, gradientPoints.end, BackgroundGradientColors);
                return paint;
            }
            
            if (BackgroundColor.Hex != Colors.Transparent.Hex)
            {
                var paint = new SkPaint();
                paint.SetSolidColor(BackgroundColor);
                return paint;
            }

            return null;
        }
        
        private SkRoundedRect GetPrimaryBorderRect(Size availableSpace)
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
                TopLeftRadius = new SkPoint(BorderRadiusTopLeft, BorderRadiusTopLeft),
                TopRightRadius = new SkPoint(BorderRadiusTopRight, BorderRadiusTopRight),
                BottomLeftRadius = new SkPoint(BorderRadiusBottomLeft, BorderRadiusBottomLeft),
                BottomRightRadius = new SkPoint(BorderRadiusBottomRight, BorderRadiusBottomRight)
            };
        }
        
        private SkRoundedRect GetBorderRectExpandedWithBorderThickness(Size availableSpace, float borderThicknessExpansionFactor)
        {
            var primaryRect = GetPrimaryBorderRect(availableSpace);
            
            return ExpandRoundedRect(
                primaryRect, 
                borderThicknessExpansionFactor * BorderLeft,
                borderThicknessExpansionFactor * BorderTop,
                borderThicknessExpansionFactor * BorderRight,
                borderThicknessExpansionFactor * BorderBottom);
        }
        
        private SkRoundedRect GetOuterRect(Size availableSpace)
        {
            return GetBorderRectExpandedWithBorderThickness(availableSpace, BorderAlignment!.Value);
        }
        
        private SkRoundedRect GetInnerRect(Size availableSpace)
        {
            return GetBorderRectExpandedWithBorderThickness(availableSpace, BorderAlignment!.Value - 1f);
        }

        private SkRoundedRect ExpandRoundedRect(SkRoundedRect rect, float all)
        {
            return ExpandRoundedRect(rect, all, all, all, all);
        }
        
        private SkRoundedRect ExpandRoundedRect(SkRoundedRect input, float left, float top, float right, float bottom)
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
            // shadow
            if (Shadow != null)
                yield return ("Shadow", null);

            // rounded corners
            if (HasRoundedCorners)
            {
                if (HasUniformRoundedCorners)
                    yield return ("Border", $"R={BorderRadiusTopLeft}");
                else
                    yield return ("Border", $"TL={BorderRadiusTopLeft}   TR={BorderRadiusTopRight}   BL={BorderRadiusBottomLeft}   BR={BorderRadiusBottomRight}");
            }

            // border
            if (HasBorder)
            {
                var color = BorderGradientColors.Any() ? "gradient" : BorderColor.ToString();
                
                if (HasUniformBorder)
                    yield return ("Border", $"A={BorderLeft}   C={color}");
                else
                    yield return ("Border", $"L={BorderLeft}   T={BorderTop}   R={BorderRight}   B={BorderBottom}   C={color}");
            }

            // background
            if (BackgroundGradientColors.Length > 0)
                yield return ("Background", $"Gradient with {BackgroundGradientColors.Length} colors");

            else if (BackgroundColor.Hex != Colors.Transparent.Hex)
                yield return ("Background", BackgroundColor);
        }
    }
}