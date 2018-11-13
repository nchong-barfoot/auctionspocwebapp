using System;
using System.Globalization;

namespace BT.Auctions.API.Models.Helpers
{
    public static class DateTimeHelper
    {
        public const string DEFAULT_DATE_FORMAT = "dd/MM/yyyy HH:mm:ss.fff";
        public const string DEFAULT_CULTURE = "en-NZ";
        public const string DEFAULT_TIMEZONE = "New Zealand Standard Time";

        /// <summary>
        /// Determines whether is a valid server date and matches the predefined date format 
        /// for frontend date communications
        /// </summary>
        /// <param name="date">The date to validate.</param>
        /// <returns>(bool if is valid, DateTime of the resolved date)</returns>
        public static (bool, DateTime) IsValidServerDate(string date)
        {
            return (DateTime.TryParseExact(date, DEFAULT_DATE_FORMAT, CultureInfo.GetCultureInfo(DEFAULT_CULTURE),
                DateTimeStyles.None, out var resolvedDate), resolvedDate);
        }

        /// <summary>
        /// Gets the server date time.
        /// </summary>
        /// <returns>DateTime of server time when object called, all datetimes formatted to NZ Timezone</returns>
        public static DateTime GetCurrentServerDateTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(DEFAULT_TIMEZONE));
        }

        /// <summary>
        /// Gets the server date time.
        /// </summary>
        /// <returns>DateTime of server time when object called, all datetimes formatted to NZ Timezone</returns>
        public static string GetCurrentServerDateTimeFormatted(string format = DEFAULT_DATE_FORMAT)
        {
            return GetCurrentServerDateTime().ToString(format, CultureInfo.GetCultureInfo(DEFAULT_CULTURE));
        }

        /// <summary>
        /// Converts to local date time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns></returns>
        public static DateTimeOffset ConvertToLocalDateTime(DateTimeOffset date, string timeZone)
        {
            if (string.IsNullOrWhiteSpace(timeZone))
            {
                timeZone = DEFAULT_TIMEZONE;
            }

            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, timeZone);
        }

        /// <summary>
        /// Checks the date is within range.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="targetDate">The targeted date.</param>
        /// <returns></returns>
        public static bool CheckDateIsWithinRange(DateTimeOffset startDate, DateTimeOffset targetDate)
        {
            return startDate.Date.CompareTo(targetDate.Date) == 0;
        }

        /// <summary>
        /// Checks the date1 is within range or not of date2
        /// </summary>
        /// <param name="startDate1">The start date1.</param>
        /// <param name="finishDate1">The finish date1.</param>
        /// <param name="startDate2">The start date2.</param>
        /// <param name="finishDate2">The finish date2.</param>
        /// <returns>bool of whether the date is in range</returns>
        public static bool CheckDatesAreWithinRange(DateTimeOffset startDate1, DateTimeOffset finishDate1, DateTimeOffset startDate2, DateTimeOffset finishDate2)
        {
            return startDate1 >= startDate2 && startDate1 <= finishDate2 || finishDate1 >= startDate2 && finishDate1 <= finishDate2
                || startDate2 >= startDate1 && startDate2 <= finishDate1 || finishDate2 >= startDate1 && finishDate2 <= finishDate1;
        }
    }
}