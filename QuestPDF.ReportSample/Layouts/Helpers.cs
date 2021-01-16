using System;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample.Layouts
{
    public static class Helpers
    {
        static IContainer Cell(this IContainer container, string color)
        {
            return container
                .Border(0.5f)
                .Background(color)
                .Padding(5);
        }
        
        public static IContainer LightCell(this IContainer container)
        {
            return container.Cell("#0000");
        }
        
        public static IContainer DarkCell(this IContainer container)
        {
            return container.Cell("#1000");
        }
        
        public static string Format(this Location location)
        {
            if (location == null)
                return string.Empty;
            
            var lon = location.Longitude;
            var lat = location.Latitude;
            
            var typeLon = lon > 0 ? "E" : "W";
            lon = Math.Abs(lon);
            
            var typeLat = lat > 0 ? "N" : "S";
            lat = Math.Abs(lat);
            
            return $"{lat:F5}° {typeLat}   {lon:F5}° {typeLon}";
        }
    }
}