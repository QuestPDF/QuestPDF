using System;
using System.Collections.Generic;

namespace QuestPDF.ReportSample
{
    public class ReportModel
    {
        public string Title { get; set; }
        public byte[] LogoData { get; set; }
        public List<ReportHeaderField> HeaderFields { get; set; }

        public List<ReportSection> Sections { get; set; }
        public List<ReportPhoto> Photos { get; set; }
    }

    public class ReportHeaderField
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
    
    public class ReportSection
    {
        public string Title { get; set; }
        public List<ReportSectionElement> Parts { get; set; }
    }

    public abstract class ReportSectionElement
    {
        public string Label { get; set; }
    }

    public class ReportSectionText : ReportSectionElement
    {
        public string Text { get; set; }
    }
    
    public class ReportSectionMap : ReportSectionElement
    {
        public Location Location { get; set; }
    }

    public class ReportSectionPhotos : ReportSectionElement
    {
        public int PhotoCount { get; set; }
    }

    public class ReportPhoto
    {
        public Location Location { get; set; }
        
        public DateTime? Date { get; set; }
        public string Comments { get; set; }
    }
}
