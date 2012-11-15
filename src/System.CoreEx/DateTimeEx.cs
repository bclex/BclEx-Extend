#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System.Collections.Generic;
namespace System
{
    /// <summary>
    /// DateTimeEx
    /// </summary>
#if !COREINTERNAL
    public
#endif
 static class DateTimeEx
    {
        /// <summary>
        /// Gets the dates in range.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="accumulator">The accumulator.</param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetDatesInRange(DateTime startDate, DateTime endDate, Func<DateTime, DateTime> accumulator)
        {
            for (var date = startDate; date <= endDate; date = accumulator(date))
                yield return date;
        }

        /// <summary>
        /// Gets the weeks of month.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public static IEnumerable<WeekOfMonth> GetWeeksOfMonth(DateTime startDate, DateTime endDate)
        {
            var startOfFirstMonth = startDate.AddDays(-startDate.Day + 1);
            var endOfLastMonth = startOfFirstMonth.AddMonths(1).AddDays(-1);
            var week = 1;
            for (DateTime startOfWeek = startOfFirstMonth, endOfWeek = startOfFirstMonth.AddDays(6 - (int)startOfFirstMonth.DayOfWeek);
                (startOfWeek <= endOfLastMonth);
                endOfWeek = endOfWeek.AddDays(7), startOfWeek = endOfWeek.AddDays(-6), week++)
            {
                var clippedEndOfWeek = (endOfWeek.Month == startOfWeek.Month ? endOfWeek : endOfLastMonth);
                var minStartDate = (startOfWeek < startDate ? startOfWeek : startDate);
                var minEndDate = (clippedEndOfWeek < endDate ? clippedEndOfWeek : endDate);
                if (startOfWeek <= endDate && endOfWeek >= startDate)
                    yield return new WeekOfMonth
                    {
                        StartDate = minStartDate,
                        EndDate = minEndDate,
                        Week = week,
                        LastWeekOfMonth = (endOfWeek.Month != startOfWeek.Month),
                    };
                // advance to next month
                if (endOfWeek.Month != startOfWeek.Month)
                    week = 1;
            }
        }

        /// <summary>
        /// WeekOfMonth
        /// </summary>
        public struct WeekOfMonth
        {
            /// <summary>
            /// StartDate
            /// </summary>
            public DateTime StartDate;
            /// <summary>
            /// EndDate
            /// </summary>
            public DateTime EndDate;
            /// <summary>
            /// Week
            /// </summary>
            public int Week;
            /// <summary>
            /// LastWeekOfMonth
            /// </summary>
            public bool LastWeekOfMonth;
        }

        /// <summary>
        /// DatePart
        /// </summary>
        public enum DatePart
        {
            /// <summary>
            /// Years
            /// </summary>
            Year,
            /// <summary>
            /// Quarters
            /// </summary>
            Quarter,
            /// <summary>
            /// Months
            /// </summary>
            Month,
            /// <summary>
            /// DaysOfYear
            /// </summary>
            DayOfYear,
            /// <summary>
            /// Days
            /// </summary>
            Day,
            /// <summary>
            /// Weeks
            /// </summary>
            Week,
            /// <summary>
            /// Hours
            /// </summary>
            Hour,
            /// <summary>
            /// Minutes
            /// </summary>
            Minute,
            /// <summary>
            /// Seconds
            /// </summary>
            Second,
            /// <summary>
            /// Milliseconds
            /// </summary>
            Millisecond,
        }

        /// <summary>
        /// ShiftDateMethod
        /// </summary>
        public enum ShiftDateMethod
        {
            /// <summary>
            /// EndOfMonth
            /// </summary>
            EndOfMonth,
            /// <summary>
            /// FirstOfMonth
            /// </summary>
            FirstOfMonth,
        }


        //public static DateTime GetValidForTimeZone(DateTime dateTime, string timeZoneId)
        //{
        //    var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        //    return GetValidForTimeZone(dateTime.AddMinutes(1), zone);
        //}
        //public static DateTime GetValidForTimeZone(DateTime dateTime, TimeZoneInfo timeZone)
        //{
        //    if (timeZone.IsInvalidTime(dateTime))
        //        return GetValidForTimeZone(dateTime.AddMinutes(1), timeZone);

        //    return dateTime;
        //}

    }
}
