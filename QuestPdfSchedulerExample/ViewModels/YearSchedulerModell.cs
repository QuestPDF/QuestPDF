using System;
using System.Linq;
using System.Collections.Generic;

namespace QuestPdfSchedulerExample
{
    public class YearSchedulerModell
    {
        public DateTime FirstDay { get; set; }
        public DateTime LastDay { get; set; }

        public List<SchedulerMonth> SchedulerMonths { get; private set; } = new();

        public YearSchedulerModell(int year)
        {
            this.FirstDay = new(year, 1, 1);
            this.LastDay = new(year, 12, 31);

            DateTime day = FirstDay;
            SchedulerMonth month = null;

            while (day <= LastDay)
            {
                if (month is null || month.MonthNo != day.Month)
                {
                    month = new SchedulerMonth(day.Month);
                    this.SchedulerMonths.Add(month);
                }

                month.Days.Add(new SchedulerDay(day));
                day = day.AddDays(1);
            }
        }

        public void AddHolyDays(List<Holyday> holydays)
        {
            holydays.ForEach(holyDay =>
            {
                SchedulerDay day = this.SchedulerMonths[holyDay.Holydate.Month - 1].Days[holyDay.Holydate.Day - 1];
                day.HolyDayName = holyDay.Name;
            });
        }

        public void AddIcon(DateTime dayToAddIcon, SchedulerIcon icon)
        {
            SchedulerDay day = this.SchedulerMonths[dayToAddIcon.Month].Days[dayToAddIcon.Day];
            day.Icons.Add(icon);
        }

        public void AddIconToDayOfTheWeek(DayOfWeek friday, SchedulerIcon schedulerIcon)
        {
            this.SchedulerMonths.ForEach(month =>
                month.Days
                    .Where(day => day.Day.DayOfWeek == friday)
                    .ToList()
                    .ForEach(day => day.Icons.Add(schedulerIcon))
            );
        }
    }
}