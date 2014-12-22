using Microsoft.Win32.TaskScheduler;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.Win32.TaskScheduler
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Selects the specified source.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> Select<TResult>(this ICollection<Trigger> source, Func<DateTime, TResult> selector) { return Select(source, selector, DateTime.Today); }
        /// <summary>
        /// Selects the specified source.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> Select<TResult>(this ICollection<Trigger> source, Func<DateTime, TResult> selector, DateTime date)
        {
            return CreateTiggerCollectionIterator(source, selector, date);
        }

        static IEnumerable<TResult> CreateTiggerCollectionIterator<TResult>(ICollection<Trigger> source, Func<DateTime, TResult> selector, DateTime date)
        {
            var triggers = source.Select(x => x.Select(y => y, date).GetEnumerator()).ToArray();
            var d = DateTime.MinValue;
            do
            {
                var minD = DateTime.MaxValue;
                foreach (var trigger in triggers)
                    if (trigger.Current <= d)
                        if (trigger.MoveNext() && minD < trigger.Current)
                            minD = trigger.Current;
                if (minD == DateTime.MaxValue)
                    yield break; // exit if no advancement made
                d = minD;
                yield return selector(d);
            } while (true);
        }

        /// <summary>
        /// Selects the specified source.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> Select<TResult>(this Trigger source, Func<DateTime, TResult> selector) { return Select(source, selector, DateTime.Today); }
        /// <summary>
        /// Selects the specified source.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> Select<TResult>(this Trigger source, Func<DateTime, TResult> selector, DateTime date)
        {
            return CreateTiggerIterator(source, selector, date);
        }

        #region Trigger Enumerator

        static DateTime AddRandomDelay(DateTime date, TimeSpan randomDelay)
        {
            return date.AddMinutes(new Random().Next(randomDelay.Minutes));
        }

        static IEnumerable<TResult> CreateTiggerIterator<TResult>(Trigger source, Func<DateTime, TResult> selector, DateTime date)
        {
            if (!source.Enabled || date > source.EndBoundary) return Enumerable.Empty<TResult>();
            var sb = source.StartBoundary;
            date = (date > sb ? sb.AddYears(date.Year - sb.Year).AddMonths(date.Month - sb.Month).AddDays(date.Day - sb.Day) : sb);
            IEnumerable<DateTime> trigger;
            switch (source.TriggerType)
            {
                case TaskTriggerType.Event: throw new NotSupportedException();
                case TaskTriggerType.Time: trigger = CreateTiggerIterator((TimeTrigger)source, date); break;
                case TaskTriggerType.Daily: trigger = CreateTiggerIterator((DailyTrigger)source, date); break;
                case TaskTriggerType.Weekly: trigger = CreateTiggerIterator((WeeklyTrigger)source, date); break;
                case TaskTriggerType.Monthly: trigger = CreateTiggerIterator((MonthlyTrigger)source, date); break;
                case TaskTriggerType.MonthlyDOW: trigger = CreateTiggerIterator((MonthlyDOWTrigger)source, date); break;
                case TaskTriggerType.Idle: throw new NotSupportedException();
                case TaskTriggerType.Registration: throw new NotSupportedException();
                case TaskTriggerType.Boot: throw new NotSupportedException();
                case TaskTriggerType.Logon: throw new NotSupportedException();
                case TaskTriggerType.SessionStateChange: throw new NotSupportedException();
                case TaskTriggerType.Custom: throw new NotSupportedException();
                default: throw new NotSupportedException();
            }
            var repetition = source.Repetition;
            return (repetition.Duration == TimeSpan.Zero ? trigger : CreateTiggerIterator(trigger, repetition, source.EndBoundary, date)).Select(selector);
        }

        static IEnumerable<DateTime> CreateTiggerIterator(IEnumerable<DateTime> source, RepetitionPattern repetition, DateTime endDate, DateTime date)
        {
            var e = source.GetEnumerator();
            e.MoveNext();
            var d = e.Current;
            do
            {
                e.MoveNext();
                d = e.Current;
                var lastD = d;
                yield return lastD;
                foreach (var r in CreateRepetitionterator(repetition, (d < endDate ? d : endDate), lastD))
                    yield return r;
            } while (d < endDate);
        }

        static IEnumerable<DateTime> CreateRepetitionterator(RepetitionPattern source, DateTime endDate, DateTime date)
        {
            if (source.Interval == TimeSpan.Zero || source.Duration == TimeSpan.Zero)
                throw new ArgumentNullException("source");
            //
            var startDate = date;
            while ((date += source.Interval) <= endDate && date - startDate < source.Duration)
                yield return date;
        }

        static IEnumerable<DateTime> CreateTiggerIterator(TimeTrigger source, DateTime date)
        {
            if (source.RandomDelay != TimeSpan.Zero) date = AddRandomDelay(date, source.RandomDelay); //: RandomDelay
            yield return date;
        }

        static IEnumerable<DateTime> CreateTiggerIterator(DailyTrigger source, DateTime date)
        {
            if (source.DaysInterval < 1 || source.DaysInterval > 999)
                throw new ArgumentNullException("source");
            if (source.RandomDelay != TimeSpan.Zero) date = AddRandomDelay(date, source.RandomDelay); //: RandomDelay
            //
            var offset = ((date - source.StartBoundary).Days % source.DaysInterval);
            var d = (offset == 0 ? date : date.AddDays(-offset)); // realign to start-boundry granulatity
            do
            {
                if (d >= date)
                    yield return d;
                d = d.AddDays(source.DaysInterval); //: DaysInterval
            } while (d < source.EndBoundary);
        }

        static IEnumerable<DateTime> CreateTiggerIterator(WeeklyTrigger source, DateTime date)
        {
            if (source.RandomDelay != TimeSpan.Zero) date = AddRandomDelay(date, source.RandomDelay); //: RandomDelay
            //
            var offset = ((date - source.StartBoundary).Days % (source.WeeksInterval * 7));
            var d = (offset == 0 ? date : date.AddDays(-offset)); // realign to start-boundry granulatity
            var dow = source.DaysOfWeek; //: DaysOfWeek
            do
            {
                for (DaysOfTheWeek i = (DaysOfTheWeek)(1 << (int)d.DayOfWeek); i <= DaysOfTheWeek.Saturday; i = (DaysOfTheWeek)(1 << (int)i), d = d.AddDays(1))
                    if ((dow & i) != 0 && d >= date)
                        yield return d;
            } while (d < source.EndBoundary);
        }

        static IEnumerable<DateTime> CreateTiggerIterator(MonthlyTrigger source, DateTime date)
        {
            if (source.RandomDelay != TimeSpan.Zero) date = AddRandomDelay(date, source.RandomDelay); //: RandomDelay
            var d = date.AddDays(1 - date.Day); // move to month
            var moy = source.MonthsOfYear; //: MonthsOfYear
            do
            {
                for (MonthsOfTheYear i = (MonthsOfTheYear)(1 << d.Month - 1); i <= MonthsOfTheYear.December;
                    i = (MonthsOfTheYear)(1 << (int)i), d = d.AddMonths(1))
                    if ((moy & i) != 0 && d >= date)
                    {
                        var d2 = d;
                        var lastD = d.AddDays(1 - d.Day).AddMonths(1).AddDays(-1); // find last day of week;
                        foreach (var dom in source.DaysOfMonth.OrderBy(x => x)) //: DaysOfMonth
                            if ((d2 = d.AddDays(dom)) >= date && d2.Month == d.Month)
                            {
                                if (d2 == lastD)
                                    lastD = DateTime.MaxValue;
                                yield return d2;
                            }
                        if (source.RunOnLastDayOfMonth) //: RunOnLastDayOfMonth
                            if (lastD != DateTime.MaxValue && lastD >= date)
                                yield return lastD;
                    }
            } while (d < source.EndBoundary);
        }

        static IEnumerable<DateTime> CreateTiggerIterator(MonthlyDOWTrigger source, DateTime date)
        {
            if (source.RandomDelay != TimeSpan.Zero) date = AddRandomDelay(date, source.RandomDelay); //: RandomDelay
            var d = date.AddDays(1 - date.Day); // move to month
            var moy = source.MonthsOfYear; //: MonthsOfYear
            var wom = source.WeeksOfMonth; //: WeeksOfMonth
            var dow = source.DaysOfWeek; //: DaysOfWeek
            do
            {
                for (MonthsOfTheYear i = (MonthsOfTheYear)(1 << d.Month - 1); i <= MonthsOfTheYear.December; i = (MonthsOfTheYear)(1 << (int)i), d = d.AddMonths(1))
                    if ((moy & i) != 0)
                    {
                        var d2 = d.AddDays(-(int)d.DayOfWeek); // move to first day of week - sunday
                        var lastD = d2.AddDays(5 * 7); // find last week of month;
                        if (lastD.Month != d.Month)
                            lastD = lastD.AddDays(-7);
                        for (WhichWeek i2 = WhichWeek.FirstWeek; i2 <= WhichWeek.LastWeek; i2 = (WhichWeek)(1 << (int)i2), d2 = d2.AddDays(7))
                        {
                            if ((wom & i2) != 0 && d2 <= lastD)
                            {
                                if (d2 == lastD)
                                    lastD = DateTime.MaxValue;
                                var d3 = d2;
                                for (DaysOfTheWeek i3 = (DaysOfTheWeek)(1 << (int)d.DayOfWeek); i3 <= DaysOfTheWeek.Saturday; i3 = (DaysOfTheWeek)(1 << (int)i3), d3 = d3.AddDays(1))
                                    if ((dow & i3) != 0 && d3 >= date && d3.Month == d.Month)
                                        yield return d3;
                            }
                        }
                        if (source.RunOnLastWeekOfMonth) //: RunOnLastWeekOfMonth
                            if (lastD != DateTime.MaxValue && lastD >= date)
                            {
                                var d3 = lastD;
                                for (DaysOfTheWeek i3 = (DaysOfTheWeek)(1 << (int)d.DayOfWeek); i3 <= DaysOfTheWeek.Saturday; i3 = (DaysOfTheWeek)(1 << (int)i3), d3 = d3.AddDays(1))
                                    if ((dow & i3) != 0 && d3 >= date && d3.Month == d.Month)
                                        yield return d3;
                            }
                    }
            } while (d < source.EndBoundary);
        }

        #endregion
    }
}
