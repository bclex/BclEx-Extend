using System;
using System.Collections.Generic;
using System.IO;

namespace System.Text.Lalr
{
    internal static class Extensions
    {
        public static bool AddRange<T>(this HashSet<T> source, HashSet<T> set)
        {
            var change = false;
            foreach (var item in set)
                change |= source.Add(item);
            return change;
        }

        public static void CheckedWrite(this StreamWriter source, ref int line, string format) { source.Write(format); line += (format.Length - format.Replace("\n", string.Empty).Length); }
        public static void CheckedWrite(this StreamWriter source, ref int line, string format, object arg0) { source.Write(format, arg0); line += (format.Length - format.Replace("\n", string.Empty).Length); }
        public static void CheckedWrite(this StreamWriter source, ref int line, string format, object arg0, object arg1) { source.Write(format, arg0, arg1); line += (format.Length - format.Replace("\n", string.Empty).Length); }
        public static void CheckedWrite(this StreamWriter source, ref int line, string format, object arg0, object arg1, object arg2) { source.Write(format, arg0, arg1, arg2); line += (format.Length - format.Replace("\n", string.Empty).Length); }
        public static void CheckedWrite(this StreamWriter source, ref int line, string format, params object[] arg) { source.Write(format, arg); line += (format.Length - format.Replace("\n", string.Empty).Length); }
        public static void CheckedWriteLine(this StreamWriter source, ref int line, string format) { source.WriteLine(format); line += (format.Length - format.Replace("\n", string.Empty).Length); line++; }
        public static void CheckedWriteLine(this StreamWriter source, ref int line, string format, object arg0) { source.WriteLine(format, arg0); line += (format.Length - format.Replace("\n", string.Empty).Length); line++; }
        public static void CheckedWriteLine(this StreamWriter source, ref int line, string format, object arg0, object arg1) { source.WriteLine(format, arg0, arg1); line += (format.Length - format.Replace("\n", string.Empty).Length); line++; }
        public static void CheckedWriteLine(this StreamWriter source, ref int line, string format, object arg0, object arg1, object arg2) { source.WriteLine(format, arg0, arg1, arg2); line += (format.Length - format.Replace("\n", string.Empty).Length); line++; }
        public static void CheckedWriteLine(this StreamWriter source, ref int line, string format, params object[] arg) { source.WriteLine(format, arg); line += (format.Length - format.Replace("\n", string.Empty).Length); line++; }

        public static void WriteLine(this StreamWriter source, ref int line) { source.WriteLine(); line++; }
        public static void WriteLine(this StreamWriter source, ref int line, string format) { source.WriteLine(format); line++; }
        public static void WriteLine(this StreamWriter source, ref int line, string format, object arg0) { source.WriteLine(format, arg0); line++; }
        public static void WriteLine(this StreamWriter source, ref int line, string format, object arg0, object arg1) { source.WriteLine(format, arg0, arg1); line++; }
        public static void WriteLine(this StreamWriter source, ref int line, string format, object arg0, object arg1, object arg2) { source.WriteLine(format, arg0, arg1, arg2); line++; }
        public static void WriteLine(this StreamWriter source, ref int line, string format, params object[] arg) { source.WriteLine(format, arg); line++; }
    }
}
