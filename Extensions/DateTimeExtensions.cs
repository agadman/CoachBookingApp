using System;

namespace CoachBookingApp.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly TimeZoneInfo SwedenTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        public static string ToSwedishTime(this DateTime utcDateTime, string format = "yyyy-MM-dd HH:mm")
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, SwedenTimeZone).ToString(format);
        }
    }
}