using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Helpers;

namespace QuestPDF.ReportSample
{
    public static class DataSource
    {
        public static ReportModel GetReport()
        {
            return new ReportModel
            {
                Title = "Sample Report Document",
                HeaderFields = HeaderFields(),
                
                LogoData = Helpers.GetImage("Logo.png"),
                Sections = Enumerable.Range(0, 40).Select(x => GenerateSection()).ToList(),
                Photos = Enumerable.Range(0, 25).Select(x => GetReportPhotos()).ToList()
            };

            List<ReportHeaderField> HeaderFields()
            {
                return new List<ReportHeaderField>
                {
                    new ReportHeaderField
                    {
                        Label = "Scope",
                        Value = "Internal activities"
                    },
                    new ReportHeaderField
                    {
                        Label = "Author",
                        Value = "Marcin Ziąbek"
                    },
                    new ReportHeaderField
                    {
                        Label = "Date",
                        Value = DateTime.Now.ToString("g")
                    },
                    new ReportHeaderField
                    {
                        Label = "Status",
                        Value = "Completed, found 2 issues"
                    }
                };
            }
            
            ReportSection GenerateSection()
            {
                var sectionLength = Helpers.Random.NextDouble() > 0.75
                    ? Helpers.Random.Next(20, 40)
                    : Helpers.Random.Next(5, 10);
                
                return new ReportSection
                {
                    Title = Placeholders.Label(),
                    Parts = Enumerable.Range(0, sectionLength).Select(x => GetRandomElement()).ToList()
                };
            }

            ReportSectionElement GetRandomElement()
            {
                var random = Helpers.Random.NextDouble();
                return random switch
                {
                    < 0.9f => GetTextElement(),
                    < 0.95f => GetMapElement(),
                    _ => GetPhotosElement(),
                };
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
                    Location = Helpers.RandomLocation()
                };
            }
            
            ReportSectionPhotos GetPhotosElement()
            {
                return new ReportSectionPhotos
                {    
                    Label = "Photos",
                    PhotoCount = Helpers.Random.Next(1, 15)
                };
            }

            ReportPhoto GetReportPhotos()
            {
                return new ReportPhoto()
                {
                    Comments = Placeholders.Sentence(),
                    Date = DateTime.Now - TimeSpan.FromDays(Helpers.Random.NextDouble() * 100),
                    Location = Helpers.RandomLocation()
                };
            }
        }
    }
}