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
using System.Globalization;
namespace System
{
    static partial class CoreExtensions
    {
        /// <summary>
        /// Dates the diff.
        /// </summary>
        /// <param name="startDateTime">The start date time.</param>
        /// <param name="datePart">The date part.</param>
        /// <param name="endDateTime">The end date time.</param>
        /// <returns></returns>
        public static int DateDiff(this DateTime startDateTime, DateTimeEx.DatePart datePart, DateTime endDateTime)
        {
            Calendar calendar;
            switch (datePart)
            {
                case DateTimeEx.DatePart.Day:
                    startDateTime = startDateTime.AddTicks(-startDateTime.TimeOfDay.Ticks);
                    endDateTime = endDateTime.AddTicks(-endDateTime.TimeOfDay.Ticks);
                    return (int)(((TimeSpan)(endDateTime - startDateTime)).TotalDays);
                case DateTimeEx.DatePart.DayOfYear:
                    throw new NotImplementedException();
                case DateTimeEx.DatePart.Hour:
                    startDateTime = startDateTime.AddTicks(-startDateTime.TimeOfDay.Ticks).AddHours(startDateTime.Hour);
                    endDateTime = endDateTime.AddTicks(-endDateTime.TimeOfDay.Ticks).AddHours(endDateTime.Hour);
                    return (int)(((TimeSpan)(endDateTime - startDateTime)).TotalHours);
                case DateTimeEx.DatePart.Millisecond:
                    return (int)(((TimeSpan)(endDateTime - startDateTime)).TotalMilliseconds);
                case DateTimeEx.DatePart.Minute:
                    startDateTime = startDateTime.AddTicks(-startDateTime.TimeOfDay.Ticks).AddHours(startDateTime.Hour).AddMinutes(startDateTime.Minute);
                    endDateTime = endDateTime.AddTicks(-endDateTime.TimeOfDay.Ticks).AddHours(endDateTime.Hour).AddMinutes(endDateTime.Minute);
                    return (int)(((TimeSpan)(endDateTime - startDateTime)).TotalMinutes);
                case DateTimeEx.DatePart.Month:
                    return endDateTime.Month - startDateTime.Month + ((endDateTime.Year - startDateTime.Year) * 12);
                case DateTimeEx.DatePart.Quarter:
                    return (int)(Math.Floor((decimal)(endDateTime.Month - 1) / 3)) - (int)(Math.Floor((decimal)(startDateTime.Month - 1) / 3)) + ((endDateTime.Year - startDateTime.Year) * 4);
                case DateTimeEx.DatePart.Second:
                    startDateTime = startDateTime.AddMilliseconds(-startDateTime.Millisecond);
                    endDateTime = endDateTime.AddMilliseconds(-endDateTime.Millisecond);
                    return (int)(((TimeSpan)(endDateTime - startDateTime)).TotalSeconds);
                case DateTimeEx.DatePart.Week:
                    calendar = CultureInfo.CurrentCulture.Calendar;
                    return calendar.GetWeekOfYear(endDateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - calendar.GetWeekOfYear(startDateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                case DateTimeEx.DatePart.Year:
                    return endDateTime.Year - startDateTime.Year;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Shifts the date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="shiftDateMethod">The shift date method.</param>
        /// <returns></returns>
        public static DateTime ShiftDate(this DateTime date, DateTimeEx.ShiftDateMethod shiftDateMethod)
        {
            switch (shiftDateMethod)
            {
                case DateTimeEx.ShiftDateMethod.EndOfMonth:
                    return date.AddTicks(-date.TimeOfDay.Ticks).AddMonths(1).AddDays(-date.Day);
                case DateTimeEx.ShiftDateMethod.FirstOfMonth:
                    return date.AddTicks(-date.TimeOfDay.Ticks).AddDays(-date.Day + 1);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
