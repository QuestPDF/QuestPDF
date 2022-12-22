using System.Collections.Generic;
using SkiaSharp;

namespace QuestPDF.Helpers
{
    public static class BezierCurveHelper
    {
        internal static void PopulateBezierPoints(SKPoint point1, SKPoint point2, SKPoint point3, 
            int currentIteration, IList<SKPoint> list)
        {
            if (currentIteration < 6) // ~6 iterations give good result
            {
                //calculate next mid points
                var midPoint1 = MidPoint(point1, point2);
                var midPoint2 = MidPoint(point2, point3);
                var midPoint3 = MidPoint(midPoint1, midPoint2); 
                currentIteration++;

                PopulateBezierPoints(point1, midPoint1, midPoint3, currentIteration, list);
                list.Add(midPoint3); 
                
                PopulateBezierPoints(midPoint3, midPoint2, point3, currentIteration, list);
            }
        }
        
        private static SKPoint MidPoint(SKPoint controlPoint1, SKPoint controlPoint2)
        {
            return new SKPoint(
                (controlPoint1.X + controlPoint2.X) / 2,
                (controlPoint1.Y + controlPoint2.Y) / 2
            );
        }
    }
}