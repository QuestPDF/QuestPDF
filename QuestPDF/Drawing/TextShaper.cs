using System;
using System.Linq;
using HarfBuzzSharp;
using QuestPDF.Infrastructure;
using SkiaSharp;
using Buffer = HarfBuzzSharp.Buffer;

namespace QuestPDF.Drawing
{
    internal class TextShaper
    {
        public const int FontShapingScale = 512;
        
        private TextStyle TextStyle { get; }

        private SKFont Font => TextStyle.ToFont();
        private Font ShaperFont => TextStyle.ToShaperFont();
        private SKPaint Paint => TextStyle.ToPaint();

        public TextShaper(TextStyle textStyle)
        {
            TextStyle = textStyle;
        }

        public TextShapingResult Shape(string text)
        {
            using var buffer = new Buffer();
            
            PopulateBufferWithText(buffer, text);
            buffer.GuessSegmentProperties();

            if (TextStyle.Direction == TextDirection.LeftToRight)
                buffer.Direction = Direction.LeftToRight;
            
            if (TextStyle.Direction == TextDirection.RightToLeft)
                buffer.Direction = Direction.RightToLeft;
            
            ShaperFont.Shape(buffer);
            
            var length = buffer.Length;
            var glyphInfos = buffer.GlyphInfos;
            var glyphPositions = buffer.GlyphPositions;
            
            var scaleY = Paint.TextSize / FontShapingScale;
            var scaleX = scaleY * Paint.TextScaleX;
            
            var xOffset = 0f;
            var yOffset = 0f;
            
            // used for letter spacing calculation
            var lastCluster = glyphInfos.LastOrDefault().Cluster;

            var glyphs = new ShapedGlyph[length];
            for (var i = 0; i < length; i++)
            {
                // letter spacing should be applied between glyph clusters, not between individual glyphs,
                // different cluster id indicates the end of the glyph cluster
                if (lastCluster != glyphInfos[i].Cluster)
                {
                    lastCluster = glyphInfos[i].Cluster;
                    xOffset += TextStyle.LetterSpacing ?? 0;
                }

                glyphs[i] = new ShapedGlyph
                {
                    Codepoint = (ushort)glyphInfos[i].Codepoint,
                    Position = new SKPoint(xOffset + glyphPositions[i].XOffset * scaleX, yOffset - glyphPositions[i].YOffset * scaleY),
                    Width = glyphPositions[i].XAdvance * scaleX
                };                
                
                xOffset += glyphPositions[i].XAdvance * scaleX;
                yOffset += glyphPositions[i].YAdvance * scaleY;
            }
            
            return new TextShapingResult(buffer.Direction, glyphs);
        }
        
        void PopulateBufferWithText(Buffer buffer, string text)
        {
            var encoding = Paint.TextEncoding;

            if (encoding == SKTextEncoding.Utf8)
                buffer.AddUtf8(text);
                
            else if (encoding == SKTextEncoding.Utf16)
                buffer.AddUtf16(text);

            else if (encoding == SKTextEncoding.Utf32)
                buffer.AddUtf32(text);

            else
                throw new NotSupportedException("TextEncoding of type GlyphId is not supported.");
        }
    }
    
    internal struct ShapedGlyph
    {
        public ushort Codepoint;
        public SKPoint Position;
        public float Width;
    }

    internal struct DrawTextCommand
    {
        public SKTextBlob SkTextBlob;
        public float TextOffsetX;
    }
    
    internal class TextShapingResult
    {
        private Direction Direction { get; }
        private ShapedGlyph[] Glyphs { get; }

        public int Length => Glyphs.Length;
        
        public ShapedGlyph this[int index] =>
            Direction == Direction.LeftToRight 
                ? Glyphs[index] 
                : Glyphs[Glyphs.Length - 1 - index];

        public TextShapingResult(Direction direction, ShapedGlyph[] glyphs)
        {
            Direction = direction;
            Glyphs = glyphs;
        }

        public int BreakText(int startIndex, float maxWidth)
        {
            return Direction switch
            {
                Direction.LeftToRight => BreakTextLeftToRight(),
                Direction.RightToLeft => BreakTextRightToLeft(),
                _ => throw new ArgumentOutOfRangeException()
            };

            int BreakTextLeftToRight()
            {
                var index = startIndex;
                maxWidth += Glyphs[startIndex].Position.X;

                while (index < Glyphs.Length)
                {
                    var glyph = Glyphs[index];
                
                    if (glyph.Position.X + glyph.Width > maxWidth + Size.Epsilon)
                        break;
                
                    index++;
                }

                return index - 1;
            }
            
            int BreakTextRightToLeft()
            {
                var index = startIndex;

                var startOffset = this[startIndex].Position.X + this[startIndex].Width;

                while (index < Glyphs.Length)
                {
                    if (startOffset - this[index].Position.X > maxWidth + Size.Epsilon)
                        break;
                
                    index++;
                }

                return index - 1;
            }
        }
        
        public float MeasureWidth(int startIndex, int endIndex)
        {
            if (Glyphs.Length == 0)
                return 0;
            
            var start = this[startIndex];
            var end = this[endIndex];

            return Direction switch
            {
                Direction.LeftToRight => end.Position.X - start.Position.X + end.Width,
                Direction.RightToLeft => start.Position.X - end.Position.X + start.Width,
                _ => throw new NotSupportedException()
            };
        }
        
        public DrawTextCommand? PositionText(int startIndex, int endIndex, TextStyle textStyle)
        {
            if (Glyphs.Length == 0)
                return null;

            if (startIndex > endIndex)
                return null;
            
            using var skTextBlobBuilder = new SKTextBlobBuilder();
            
            var positionedRunBuffer = skTextBlobBuilder.AllocatePositionedRun(textStyle.ToFont(), endIndex - startIndex + 1);
            var glyphSpan = positionedRunBuffer.GetGlyphSpan();
            var positionSpan = positionedRunBuffer.GetPositionSpan();
                
            for (var sourceIndex = startIndex; sourceIndex <= endIndex; sourceIndex++)
            {
                var runIndex = sourceIndex - startIndex;
                
                glyphSpan[runIndex] = this[sourceIndex].Codepoint;
                positionSpan[runIndex] = this[sourceIndex].Position;
            }

            var firstVisualCharacterIndex = Direction == Direction.LeftToRight
                ? startIndex
                : endIndex;
            
            return new DrawTextCommand
            {
                SkTextBlob = skTextBlobBuilder.Build(),
                TextOffsetX = -this[firstVisualCharacterIndex].Position.X
            };
        }
    }
    
}