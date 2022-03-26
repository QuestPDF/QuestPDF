using System;
using System.IO;
using System.Collections.Generic;
using QuestPdfSchedulerExample.icons;
using QuestPdfSchedulerExample.Layouter;
using QuestPDF.Fluent;
using System.Diagnostics;

namespace QuestPdfSchedulerExample
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || !int.TryParse(args[0], out int year))
                year = DateTime.Now.Year;

            Console.WriteLine("Creates a year scheduler as pdf...");

            YearSchedulerModell schedulerModel = new(year);

            // Define a few Holydays
            schedulerModel.AddHolyDays(new List<Holyday>()
            {
                new Holyday()
                {
                    Holydate = new DateTime(year,12,24),
                    Name = "Heiligabend"
                },
                new Holyday()
                {
                    Holydate = new DateTime(year,12,25),
                    Name = "1. Weihnachtstag"
                },
                new Holyday()
                {
                    Holydate = new DateTime(year,12,26),
                    Name = "2. Weihnachtstag"
                },
                new Holyday()
                {
                    Holydate = new DateTime(year,1,1),
                    Name = "Neujahr"
                },
                new Holyday()
                {
                    Holydate = new DateTime(year,12,31),
                    Name = "Sylvester"
                }
            });

            Dictionary<string, byte[]> icons = IconReader.ReadIcons();

            // Add a few Icons
            schedulerModel.AddIconToDayOfTheWeek(DayOfWeek.Monday, new SchedulerIcon()
            {
                Img = icons["banana"],
                Name = "banana"
            });

            schedulerModel.AddIconToDayOfTheWeek(DayOfWeek.Monday, new SchedulerIcon()
            {
                Img = icons["apple"],
                Name = "apple"
            });

            schedulerModel.AddIconToDayOfTheWeek(DayOfWeek.Friday, new SchedulerIcon()
            {
                Img = icons["fish"],
                Name = "fish"
            });

            schedulerModel.AddIcon(new DateTime(year, 8, 28), new SchedulerIcon()
            {
               Img = icons["goethe"],
               Name = "Goethe's Birthday"
            });

            string filePath = "scheduler.pdf";

            YearScheduleLayouter document = new(schedulerModel);
            document.GeneratePdf(filePath);

            _ = Process.Start("explorer.exe", filePath);
        }
    }
}
