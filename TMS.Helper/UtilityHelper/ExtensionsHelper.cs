using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TMS.Helper.UtilityHelper
{
    public static class ExtensionsHelper
    {

        public static bool IsDateTimeBetween(this DateTime? dateToCheck, DateTime? from, DateTime? to)
        {
            return dateToCheck?.Ticks >= from?.Ticks && dateToCheck?.Ticks <= to?.Ticks;
        }

        public static bool IsDateTimeAfter(this DateTime? dateToCheck, DateTime? from)
        {
            return dateToCheck?.Ticks >= from?.Ticks;
        }

        public static TimeSpan? GetTimeSpan(this string timeSpanString)
        {

            bool isNegative = timeSpanString.StartsWith("-"); 
            var digitsString = Regex.Replace(timeSpanString, "[^0-9]", " ");
            var s = digitsString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            switch (s.Length)
            {
                case 1:
                    hours = int.Parse(s[0]);
                    break;

                case 2:
                    hours = int.Parse(s[0]);
                    minutes = int.Parse(s[1]);
                    break;

                case 4:
                    days = int.Parse(s[0]);
                    hours = int.Parse(s[1]);
                    minutes = int.Parse(s[2]);
                    seconds = int.Parse(s[3]);
                    break;

                default:
                    hours = int.Parse(s[0]);
                    minutes = int.Parse(s[1]);
                    seconds = int.Parse(s[2]);
                    break;
            }

            TimeSpan ts;

            if (isNegative)
            {
                ts = new TimeSpan(-days, -hours, -minutes, -seconds);
            }
            else
            {
                ts = new TimeSpan(days, hours, minutes, seconds);
            }

            return ts;
        }
    }
}
