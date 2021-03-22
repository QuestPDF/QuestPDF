using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Helpers;
using QuestPDF.ReportSample.Layouts;

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
                
                LogoData = Helpers.GetImage("logo.png"),
                Sections = Enumerable.Range(0, 8).Select(x => GenerateSection()).ToList(),
                Photos = Enumerable.Range(0, 8).Select(x => GetReportPhotos()).ToList()
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
                    Title = TextPlaceholder.Label(),
                    Parts = Enumerable.Range(0, sectionLength).Select(x => GetRandomElement()).ToList()
                };
            }

            ReportSectionElement GetRandomElement()
            {
                var random = Helpers.Random.NextDouble();

                if (random < 0.8f)
                    return GetTextElement();
                
                if (random < 0.9f)
                    return GetMapElement();
                
                return GetPhotosElement();
            }
            
            ReportSectionText GetTextElement()
            {
                return new ReportSectionText
                {
                    Label = TextPlaceholder.Label(),
                    Text = TextPlaceholder.Paragraph()
                };
            }
            
            ReportSectionMap GetMapElement()
            {
                var rnd = Helpers.Random.Next(0, 64);
                    
                return new ReportSectionMap
                {
                    Label = "Location",
                    ImageSource = x => Helpers.GetDocumentMap($"{rnd}.jpg"),
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
                        .Select(x => Helpers.GetPhoto($"{x}.jpg"))
                        .ToList()
                };
            }

            ReportPhoto GetReportPhotos()
            {
                var photoId = Helpers.Random.Next(0, 128);
                var mapId = Helpers.Random.Next(0, 64);

                return new ReportPhoto()
                {
                    PhotoData = Helpers.GetPhoto($"{photoId}.jpg"),

                    Comments = TextPlaceholder.Paragraph(),
                    Date = DateTime.Now - TimeSpan.FromDays(Helpers.Random.NextDouble() * 100),
                    Location = Helpers.RandomLocation(),

                    MapContextSource = x => Helpers.GetContextMap($"{mapId}.jpg"),
                    MapDetailsSource = x => Helpers.GetDetailsMap($"{mapId}.jpg")
                };
            }
        }
    }
}