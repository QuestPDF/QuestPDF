using System.Collections.Generic;

namespace QuestPdfSchedulerExample
{
    public class SchedulerMonth
    {
        public int MonthNo { get; set; }
        public List<SchedulerDay> Days { get; } = new();

        public SchedulerMonth(int month)
        {
            this.MonthNo = month;
        }
    }
}