using System;
using System.Tests;
namespace System
{
    class Program
    {
        static void Main(string[] args)
        {
            //new NumberSpanTests().RunAllTests();
            new ScheduleTests().RunAllTests();

            //var scheduleJ = "{int:\"mm\",is:2,days:\"mon tue\"}".Dump();
            //var stateJ = "{}".Dump();
            //var state = ScheduleService.ParseState(stateJ).Dump();
            //var node = ScheduleService.Parse(scheduleJ).Dump();
            //node.Next(state, DateTime.Today).Dump();
            //state.ToString().Dump();
        }
    }
}
