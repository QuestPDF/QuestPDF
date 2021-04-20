using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Helpers;

namespace QuestPDF.ReportSample
{
    public static class DataSource
    {
        public static int SectionCounter { get; set; }
        public static int FieldCounter { get; set; }
        
        public static ReportModel GetReport()
        {
            return new ReportModel
            {
                Title = "Sample Report Document",
                HeaderFields = HeaderFields(),
                
                LogoData = Helpers.GetImage("Logo.png"),
                Sections = Enumerable.Range(0, 8).Select(x => GenerateSection()).ToList(),
                Photos = Enumerable.Range(2, 6).Select(x => GetReportPhotos()).ToList()
            };

            List<ReportHeaderField> HeaderFields()
            {
                return new List<ReportHeaderField>
                {
                    new ReportHeaderField()
                    {
                        Label = "Scope",
                        Value = "Internal activities"
                    },
                    new ReportHeaderField()
                    {
                        Label = "Author",
                        Value = "Marcin Ziąbek"
                    },
                    new ReportHeaderField()
                    {
                        Label = "Date",
                        Value = DateTime.Now.ToString("g")
                    },
                    new ReportHeaderField()
                    {
                        Label = "Status",
                        Value = "Completed, found 2 issues"
                    }
                };
            }
            
            ReportSection GenerateSection()
            {
                var sectionLength = Helpers.Random.NextDouble() > 0.75
                    ? Helpers.Random.Next(10, 20)
                    : Helpers.Random.Next(3, 11);
                
                return new ReportSection
                {
                    Title = Placeholders.Label(),
                    Parts = Enumerable.Range(0, sectionLength).Select(x => GetRandomElement()).ToList()
                };
            }

            ReportSectionElement GetRandomElement()
            {
                var random = Helpers.Random.NextDouble();

                if (random < 0.9f)
                    return GetTextElement();
                
                if (random < 0.95f)
                    return GetMapElement();
                
                return GetPhotosElement();
            }
            
            ReportSectionText GetTextElement()
            {
                return new ReportSectionText
                {
                    Label = Placeholders.Label(),
                    Text = Placeholders.Paragraph()
                };
            }
            
            ReportSectionMap GetMapElement()
            {
                return new ReportSectionMap
                {
                    Label = "Location",
                    ImageSource = Placeholders.Image,
                    Location = Helpers.RandomLocation()
                };
            }
            
            ReportSectionPhotos GetPhotosElement()
            {
                return new ReportSectionPhotos
                {    
                    Label = "Photos",
                    Photos = Enumerable
                        .Range(0, Helpers.Random.Next(1, 10))
                        .Select(x => Helpers.Random.Next(0, 128))
                        .Select(x => Placeholders.Image(400, 300))
                        .ToList()
                };
            }

            ReportPhoto GetReportPhotos()
            {
                return new ReportPhoto()
                {
                    PhotoData = Placeholders.Image(400, 300),

                    Comments = Placeholders.Sentence(),
                    Date = DateTime.Now - TimeSpan.FromDays(Helpers.Random.NextDouble() * 100),
                    Location = Helpers.RandomLocation(),

                    MapContextSource = x => Placeholders.Image(400, 300),
                    MapDetailsSource = x => Placeholders.Image(400, 300)
                };
            }
        }
    }
}