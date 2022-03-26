using System;
using System.Collections.Generic;

namespace QuestPdfSchedulerExample
{
    public class SchedulerDay
    {
        public string HolyDayName { get; set; }

        public DateTime Day { get; set; }

        public List<SchedulerIcon> Icons { get; set; } = new();

        public SchedulerDay(DateTime day)
        {
            this.Day = day;
        }
    }
}