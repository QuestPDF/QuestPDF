using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Fluent
{
    public static class ObjectElementExtensions
    {
        /// <summary>
        /// Add an inspecific object. To use this feature, your ICanvas must implement DrawObject().
        /// </summary>
        public static void Object(this IContainer parent, object Object, AspectRatioOption fitSpaceOption = AspectRatioOption.FitWidth)
        {
            parent.Element(
                new ObjectElement()
                {
                    Object = Object,
                    SpaceFitBehavior = fitSpaceOption
                });
        }
    }
}