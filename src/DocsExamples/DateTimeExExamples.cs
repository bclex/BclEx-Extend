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
using System;
namespace Example
{
	public class DateTimeExExamples
	{
		public void DateAnalysis()
		{
			// finding the days left in this month
			var daysLeftInMonth = DateTime.Now.DateDiff(DateTimeEx.DatePart.Day, DateTime.Now.AddMonths(1));
			// finding the quaters left in this year
			var quartersLeftInYear = DateTime.Now.DateDiff(DateTimeEx.DatePart.Quarter, DateTime.Now.AddYears(1));

			// finding all date-times, in hours, left in this year
			var hoursLeftInYear = DateTimeEx.GetDatesInRange(DateTime.Now, DateTime.Now.AddYears(1), x => x.AddHours(1));

			// print remaining weeks of this year
			foreach (var weekOfMonth in DateTimeEx.GetWeeksOfMonth(DateTime.Now, DateTime.Now.AddYears(1)))
				Console.WriteLine("week #{0} starts on {1} and ends on {2}."
					, weekOfMonth.Week
					, weekOfMonth.StartDate
					, weekOfMonth.EndDate);
		}

		public void DateModifications()
		{
			// finding the first of the month
			var firstOfMonth = DateTime.Now.ShiftDate(DateTimeEx.ShiftDateMethod.FirstOfMonth);
			// finding the end of the month
			var endOfMonth = DateTime.Now.ShiftDate(DateTimeEx.ShiftDateMethod.EndOfMonth);
		}
	}
}