using System;
using System.Diagnostics;
namespace Tests
{
    public class NumberSpanTests
    {
        public void TestNew()
        {
            var span1 = new NumberSpan(1, 3);
            Debug.Assert(span1.Current == 1);
            Debug.Assert(span1.Total == 3);
            //
            var span2 = new NumberSpan(3, 3);
            Debug.Assert(span2.Current == 3);
            Debug.Assert(span2.Total == 3);
            //
            var span3 = new NumberSpan(4);
            Debug.Assert(span3.Current == 4);
            Debug.Assert(span3.Total == 0);
        }

        public void TestParse()
        {
            var span1 = NumberSpan.Parse("1 of 3");
            Debug.Assert(span1.Current == 1);
            Debug.Assert(span1.Total == 3);
            //
            var span2 = NumberSpan.Parse("3 Of 3");
            Debug.Assert(span2.Current == 3);
            Debug.Assert(span2.Total == 3);
            //
            var span3 = NumberSpan.Parse("4");
            Debug.Assert(span3.Current == 4);
            Debug.Assert(span3.Total == 0);
        }

        public void TestFormat()
        {
            var span1 = new NumberSpan(1, 3);
            Debug.Assert("1 of 3" == span1.ToString());
            //
            var span2 = new NumberSpan(3, 3);
            Debug.Assert("3 of 3" == span2.ToString());

            var span3 = new NumberSpan(4);
            Debug.Assert("4" == span2.ToString());
        }

        public void RunAllTests()
        {
            TestNew();
            TestParse();
            TestFormat();
        }
    }
}

