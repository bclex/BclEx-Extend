using Microsoft.Win32.TaskScheduler;
using System;
using System.Linq;
using System.Diagnostics;
namespace System.Tests
{
    public class ScheduleTests
    {
        public void TestParse()
        {
            var span1 = LocalTriggerCollection.Parse("Monthly");
            //var nextDate1 = span1.Select((a, b) => a).Take(5);
            //Debug.Assert(span1.Current == 1);
            //
            var span2 = LocalTriggerCollection.Parse("Quarterly");
            //var nextDate2 = span2.FindNextDate();
            //Debug.Assert(span2.Current == 1);
            //
            var span3 = LocalTriggerCollection.Parse("Yearly");
            //var nextDate3 = span3.FindNextDate();
            //Debug.Assert(span3.Current == 3);
        }

        public void TestTimeSelect()
        {
            var span1 = new TimeTrigger();
            span1.StartBoundary = DateTime.Parse("12/7/2014 6:00 PM");
            var r = span1.Select(x => x).Take(5).ToArray();
            Debug.Assert(r.Length == 1);
            Debug.Assert(r[0].Date == DateTime.Today);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            // repeat interval
            span1.Repetition.Interval = new TimeSpan(1, 0, 0);
            span1.Repetition.Duration = new TimeSpan(3, 0, 0);
            r = span1.Select(x => x).Take(5).ToArray();
            Debug.Assert(r.Length == 3);
            Debug.Assert(r[0].Date == DateTime.Today);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == DateTime.Today);
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
            Debug.Assert(r[2].TimeOfDay == new TimeSpan(20, 0, 0));
            // end boundry
            span1.EndBoundary = DateTime.Today.AddHours(19);
            r = span1.Select(x => x).Take(5).ToArray();
            Debug.Assert(r.Length == 2);
            Debug.Assert(r[0].Date == DateTime.Today);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == DateTime.Today);
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
        }

        public void TestDailySelect()
        {
            var span1 = new DailyTrigger();
            span1.StartBoundary = DateTime.Parse("12/7/2014 6:00 PM");
            var r = span1.Select(x => x).Take(3).ToArray();
            Debug.Assert(r.Length == 3);
            Debug.Assert(r[0].Date == DateTime.Today);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == r[0].Date.AddDays(1));
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[2].Date == r[1].Date.AddDays(1));
            Debug.Assert(r[2].TimeOfDay == new TimeSpan(18, 0, 0));
            // repeat interval
            span1.Repetition.Interval = new TimeSpan(1, 0, 0);
            span1.Repetition.Duration = new TimeSpan(3, 0, 0);
            r = span1.Select(x => x).Take(6).ToArray();
            Debug.Assert(r.Length == 6);
            Debug.Assert(r[0].Date == DateTime.Today);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == r[0].Date);
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
            Debug.Assert(r[2].Date == r[1].Date);
            Debug.Assert(r[2].TimeOfDay == new TimeSpan(20, 0, 0));
            Debug.Assert(r[3].Date == r[2].Date.AddDays(1));
            Debug.Assert(r[3].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[4].Date == r[3].Date);
            Debug.Assert(r[4].TimeOfDay == new TimeSpan(19, 0, 0));
            Debug.Assert(r[5].Date == r[4].Date);
            Debug.Assert(r[5].TimeOfDay == new TimeSpan(20, 0, 0));
            // end boundry
            span1.EndBoundary = DateTime.Today.AddDays(1).AddHours(19);
            r = span1.Select(x => x).Take(6).ToArray();
            Debug.Assert(r.Length == 5);
            Debug.Assert(r[0].Date == DateTime.Today);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == r[0].Date);
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
            Debug.Assert(r[1].Date == r[1].Date);
            Debug.Assert(r[2].TimeOfDay == new TimeSpan(20, 0, 0));
            Debug.Assert(r[3].Date == r[2].Date.AddDays(1));
            Debug.Assert(r[3].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[4].Date == r[3].Date);
            Debug.Assert(r[4].TimeOfDay == new TimeSpan(19, 0, 0));
            // every other week
            span1.EndBoundary = DateTime.MaxValue;
            span1.DaysInterval = 2;
            for (var i = 0; i < 2; i++)
            {
                r = span1.Select(x => x, DateTime.Today.AddDays(i)).Take(5).ToArray();
                Debug.Assert(r.Length == 5);
                Debug.Assert(r[0].Date.Day % 2 == 1);
                Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
                Debug.Assert(r[1].Date == r[0].Date);
                Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
                Debug.Assert(r[2].Date == r[1].Date);
                Debug.Assert(r[2].TimeOfDay == new TimeSpan(20, 0, 0));
                Debug.Assert(r[3].Date == r[2].Date.AddDays(2));
                Debug.Assert(r[3].TimeOfDay == new TimeSpan(18, 0, 0));
                Debug.Assert(r[4].Date == r[3].Date);
                Debug.Assert(r[4].TimeOfDay == new TimeSpan(19, 0, 0));
            }
        }

        public void TestWeeklySelect()
        {
            var span1 = new WeeklyTrigger(DaysOfTheWeek.Tuesday);
            span1.StartBoundary = DateTime.Parse("12/7/2014 6:00 PM");
            var r = span1.Select(x => x).Take(3).ToArray();
            Debug.Assert(r.Length == 3);
            Debug.Assert(r[0].Date.DayOfWeek == DayOfWeek.Tuesday);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == r[0].Date.AddDays(7));
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[2].Date == r[1].Date.AddDays(7));
            Debug.Assert(r[2].TimeOfDay == new TimeSpan(18, 0, 0));
            // repeat interval
            span1.Repetition.Interval = new TimeSpan(1, 0, 0);
            span1.Repetition.Duration = new TimeSpan(3, 0, 0);
            r = span1.Select(x => x).Take(6).ToArray();
            Debug.Assert(r.Length == 6);
            Debug.Assert(r[0].Date.DayOfWeek == DayOfWeek.Tuesday);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == r[0].Date);
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
            Debug.Assert(r[2].Date == r[1].Date);
            Debug.Assert(r[2].TimeOfDay == new TimeSpan(20, 0, 0));
            Debug.Assert(r[3].Date == r[2].Date.AddDays(7));
            Debug.Assert(r[3].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[4].Date == r[3].Date);
            Debug.Assert(r[4].TimeOfDay == new TimeSpan(19, 0, 0));
            Debug.Assert(r[5].Date == r[4].Date);
            Debug.Assert(r[5].TimeOfDay == new TimeSpan(20, 0, 0));
            // end boundry
            span1.EndBoundary = DateTime.Today.AddDays(3).AddDays(7).AddHours(19); // move back to a tuesday, then advance
            r = span1.Select(x => x).Take(6).ToArray();
            Debug.Assert(r.Length == 5);
            Debug.Assert(r[0].Date.DayOfWeek == DayOfWeek.Tuesday);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == r[0].Date);
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
            Debug.Assert(r[1].Date == r[1].Date);
            Debug.Assert(r[2].TimeOfDay == new TimeSpan(20, 0, 0));
            Debug.Assert(r[3].Date == r[2].Date.AddDays(7));
            Debug.Assert(r[3].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[4].Date == r[3].Date);
            Debug.Assert(r[4].TimeOfDay == new TimeSpan(19, 0, 0));
            // every other week
            span1.EndBoundary = DateTime.MaxValue;
            span1.WeeksInterval = 2;
            for (var i = 0; i < 2; i++)
            {
                r = span1.Select(x => x, DateTime.Today.AddDays(i * 7)).Take(5).ToArray();
                Debug.Assert(r.Length == 5);
                Debug.Assert(r[0].Date.DayOfWeek == DayOfWeek.Tuesday);
                Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
                Debug.Assert(r[1].Date == r[0].Date);
                Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
                Debug.Assert(r[2].Date == r[1].Date);
                Debug.Assert(r[2].TimeOfDay == new TimeSpan(20, 0, 0));
                Debug.Assert(r[3].Date == r[2].Date.AddDays(14));
                Debug.Assert(r[3].TimeOfDay == new TimeSpan(18, 0, 0));
                Debug.Assert(r[4].Date == r[3].Date);
                Debug.Assert(r[4].TimeOfDay == new TimeSpan(19, 0, 0));
            }
        }

        public void TestMonthlySelect()
        {
            var span1 = new MonthlyTrigger(2, MonthsOfTheYear.February);
            span1.StartBoundary = DateTime.Parse("12/7/2014 6:00 PM");
            var r = span1.Select(x => x).Take(3).ToArray();
            Debug.Assert(r.Length == 3);
            Debug.Assert(r[0].Date.Month == 2);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == r[0].Date.AddYears(1));
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[2].Date == r[1].Date.AddYears(1));
            Debug.Assert(r[2].TimeOfDay == new TimeSpan(18, 0, 0));
            // repeat interval
            span1.Repetition.Interval = new TimeSpan(1, 0, 0);
            span1.Repetition.Duration = new TimeSpan(3, 0, 0);
            r = span1.Select(x => x).Take(6).ToArray();
            Debug.Assert(r.Length == 6);
            Debug.Assert(r[0].Date.Month == 2);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == r[0].Date);
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
            Debug.Assert(r[2].Date == r[1].Date);
            Debug.Assert(r[2].TimeOfDay == new TimeSpan(20, 0, 0));
            Debug.Assert(r[3].Date == r[2].Date.AddYears(1));
            Debug.Assert(r[3].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[4].Date == r[3].Date);
            Debug.Assert(r[4].TimeOfDay == new TimeSpan(19, 0, 0));
            Debug.Assert(r[5].Date == r[4].Date);
            Debug.Assert(r[5].TimeOfDay == new TimeSpan(20, 0, 0));
            // end boundry
            span1.EndBoundary = DateTime.Today.AddDays(-1).AddYears(1).AddHours(19); // move back to a januaray, then advance
            r = span1.Select(x => x).Take(6).ToArray();
            Debug.Assert(r.Length == 5);
            Debug.Assert(r[0].Date.Month == 2);
            Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[1].Date == r[0].Date);
            Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
            Debug.Assert(r[1].Date == r[1].Date);
            Debug.Assert(r[2].TimeOfDay == new TimeSpan(20, 0, 0));
            Debug.Assert(r[3].Date == r[2].Date.AddYears(1));
            Debug.Assert(r[3].TimeOfDay == new TimeSpan(18, 0, 0));
            Debug.Assert(r[4].Date == r[3].Date);
            Debug.Assert(r[4].TimeOfDay == new TimeSpan(19, 0, 0));
            // every other week
            span1.EndBoundary = DateTime.MaxValue;
            for (var i = 0; i < 2; i++)
            {
                r = span1.Select(x => x, DateTime.Today.AddDays(i * 7)).Take(5).ToArray();
                Debug.Assert(r.Length == 5);
                Debug.Assert(r[0].Date.DayOfWeek == DayOfWeek.Tuesday);
                Debug.Assert(r[0].TimeOfDay == new TimeSpan(18, 0, 0));
                Debug.Assert(r[1].Date == r[0].Date);
                Debug.Assert(r[1].TimeOfDay == new TimeSpan(19, 0, 0));
                Debug.Assert(r[2].Date == r[1].Date);
                Debug.Assert(r[2].TimeOfDay == new TimeSpan(20, 0, 0));
                Debug.Assert(r[3].Date == r[2].Date.AddDays(14));
                Debug.Assert(r[3].TimeOfDay == new TimeSpan(18, 0, 0));
                Debug.Assert(r[4].Date == r[3].Date);
                Debug.Assert(r[4].TimeOfDay == new TimeSpan(19, 0, 0));
            }
        }

        public void RunAllTests()
        {
            //TestParse();
            //TestTimeSelect();
            //TestDailySelect();
            TestWeeklySelect();
            TestMonthlySelect();
        }
    }
}

