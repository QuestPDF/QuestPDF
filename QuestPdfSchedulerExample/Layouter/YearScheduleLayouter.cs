using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using System;
using System.Globalization;

namespace QuestPdfSchedulerExample.Layouter
{
    public class YearScheduleLayouter : IDocument
    {
        private readonly YearSchedulerModell year;

        public YearScheduleLayouter(YearSchedulerModell year)
        {
            this.year = year;
        }

        public void Compose(IDocumentContainer container)
        {
            _ = container
                .Page(page =>
                {
                    page.Size(PageSizes.A3.Landscape());

                    page.Margin(mm2point(5));

                    page.Header().AlignCenter()
                        .Text($"{year.FirstDay.ToShortDateString()} - {year.LastDay.ToShortDateString()}", TextStyle.Default.Size(mm2point(20)).SemiBold().Color(Colors.Red.Darken2));

                    page.Content()
                        .Element(ComposeContent);

                    page.Footer().AlignRight()
                        .Text("QuestPDF Scheduler Example 2021 by Ervin Peters");
                });
        }

        private void ComposeContent(IContainer container)
        {
            DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;

            container.Row(row =>
                this.year.SchedulerMonths.ForEach(month =>
                    row.RelativeColumn().Stack(daysStack =>
                        {
                            daysStack.Item()
                                .Background(Colors.Green.Lighten3)
                                .Border(0.5f)
                                .AlignCenter()
                                .Text(dateTimeFormatInfo.GetMonthName(month.MonthNo), TextStyle.Default.Size(10));

                            month.Days.ForEach(day =>
                                daysStack.Item()
                                .Background(BackgroundColorOfDay(day))
                                .Border(0.5f)
                                .AlignLeft()
                                .Inlined(inlined =>
                                {
                                    inlined.HorizontalSpacing(mm2point(1));
                                    inlined.BaselineMiddle();
                                    inlined.Item()
                                        .Text($"{day.Day.Day} {dateTimeFormatInfo.GetAbbreviatedDayName(day.Day.DayOfWeek)}");

                                    day.Icons.ForEach(icon =>
                                        inlined.Item().Height(mm2point(4f))
                                            .Image(icon.Img, ImageScaling.FitArea));
                                })
                            );
                        }
                    )
                )
            );
        }

        private string BackgroundColorOfDay(SchedulerDay day)
        {
            return Colors.Transparent;
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata()
            {
                DocumentLayoutExceptionThreshold = 5,
                Author = "Ervin Peters",
                Creator = "QuestPDF",
                CreationDate = DateTime.Now,
                Subject = $"from {year.FirstDay} to {year.LastDay}.",
                Title = "Scheduler"
            };
        }

        public static float mm2point(double mm)
            => (float)(mm / 25.4 * 72.0);
    }
}
