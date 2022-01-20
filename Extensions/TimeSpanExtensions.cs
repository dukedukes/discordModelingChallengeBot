using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToReadableFormat(this TimeSpan timeSpan)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (timeSpan.Days > 0)
            {
                stringBuilder.Append($"{timeSpan.Days} day{InsertPlural(timeSpan.Days)} ");
            }
            if (timeSpan.Hours > 0)
            {
                stringBuilder.Append($"{timeSpan.Hours} hour{InsertPlural(timeSpan.Hours)} ");
            }
            if (timeSpan.Minutes > 0)
            {
                stringBuilder.Append($"{timeSpan.Minutes} minute{InsertPlural(timeSpan.Minutes)} ");
            }
            if (timeSpan.Seconds > 0)
            {
                stringBuilder.Append($"{timeSpan.Seconds} second{InsertPlural(timeSpan.Seconds)}");
            }
            string output = stringBuilder.ToString();
            if (string.IsNullOrEmpty(output))
            {
                Log.Logger.Error($"possible no max duration. timespan: {timeSpan}");
                output = "Unknown";
            }
            return output;
        }

        private static string InsertPlural(int number)
        {
            if (number > 1)
            {
                return "s";
            }
            return "";
        }
    }
}
