using System.IO;

namespace System.Text.Lalr.Emitters
{
    public partial class EmitterJS
    {
        /// <summary>
        /// Template
        /// </summary>
        public class Template
        {
            /* The first function transfers data from "in" to "out" until a line is seen which begins with "%%".  The line number is tracked.
            * if name!=0, then any word that begin with "Parse" is changed to begin with *name instead. */
            internal static void Transfer(string name, StreamReader r, StreamWriter w, ref int lineno)
            {
                string line;
                while ((line = r.ReadLine()) != null && (line.Length == 0 || line[0] != '%' || line[1] != '%'))
                {
                    lineno++;
                    var iStart = 0;
                    if (!string.IsNullOrEmpty(name))
                        for (var i = 0; i < line.Length; i++)
                            if (line[i] == 'P' && (i + 4) <= line.Length && line.Substring(i, 5) == "Parse" && (i == 0 || !char.IsLetter(line[i - 1])))
                            {
                                if (i > iStart) w.Write(line.Substring(iStart, i - iStart));
                                w.Write(name);
                                i += 4;
                                iStart = i + 1;
                            }
                    w.WriteLine(line.Substring(iStart));
                }
            }

            /* Print a #line directive line to the output file. */
            internal static void WriteLineInfo(ref int lineno, StreamWriter w, int sourceLineno, string source) { w.WriteLine(ref lineno, "#line {0} \"{1}\"", sourceLineno, source.Replace("\\", "\\\\")); }

            /* Print a string to the file and keep the linenumber up to date */
            internal static void Write(StreamWriter w, Context ctx, string value, ref int lineno, string filePath)
            {
                if (value == null) return;
                w.CheckedWrite(ref lineno, value);
                if (!value.EndsWith("\n"))
                    w.WriteLine(ref lineno);
                if (!ctx.NoShowLinenos) WriteLineInfo(ref lineno, w, lineno, filePath);
            }
        }
    }
}