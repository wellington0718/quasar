using System;

namespace SharedKernel.ValueObjects
{
    public class DateTimeRange
    {
        public DateTimeRange()
        {
            Start = DateTime.Now.Date;
            End = DateTime.Now.Date;
        }

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        public DateTimeRange(DateTime? start, DateTime? end)
        {
            Start = start?.Date;
            End = end?.Date;
        }

        public bool IsDateRangeInValid()
        {
            return End < Start || Start > End;
        }
    }
}
