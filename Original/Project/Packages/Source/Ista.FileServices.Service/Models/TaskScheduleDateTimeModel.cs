using System;
using System.Linq;

namespace Ista.FileServices.Service.Models
{
    public class TaskScheduleDateTimeModel : TaskScheduleModel
    {
        public int[] MinutesConsidered { get; set; }
        public int[] HoursConsidered { get; set; }
        public int[] DaysConsidered { get; set; }

        public override DateTime IdentifyNextEntry(DateTime date)
        {
            var minuteValue = MinutesConsidered.First();
            var hourValue = HoursConsidered.First();

            if (date.Second != 0)
                date = date.AddMinutes(1).AddSeconds(-date.Second);

            if (MinutesConsidered.Any(x => x >= date.Minute))
            {
                minuteValue = MinutesConsidered.First(x => x >= date.Minute);
                date = date.AddMinutes((minuteValue - date.Minute));
            }
            else
            {
                date = date.AddMinutes(-date.Minute)
                    .AddMinutes(minuteValue)
                    .AddHours(1);

                return IdentifyNextEntry(date);
            }

            if (HoursConsidered.Any(x => x >= date.Hour))
            {
                hourValue = HoursConsidered.First(x => x >= date.Hour);
                date = date.AddHours((hourValue - date.Hour));
            }
            else
            {
                date = date.AddMinutes(-date.Minute)
                    .AddHours(-date.Hour)
                    .AddHours(hourValue)
                    .AddDays(1);

                return IdentifyNextEntry(date);
            }

            var dayOfWeek = (int)date.DayOfWeek;
            while (!DaysConsidered.Contains(dayOfWeek))
            {
                if (!date.Minute.Equals(MinutesConsidered[0]))
                    date = date.AddMinutes(-date.Minute).AddMinutes(MinutesConsidered[0]);

                if (!date.Hour.Equals(HoursConsidered[0]))
                    date = date.AddHours(-date.Hour).AddHours(HoursConsidered[0]);

                date = date.AddDays(1);
                dayOfWeek = (int)date.DayOfWeek;
            }

            return date.AddSeconds(-date.Second);
        }
    }
}
