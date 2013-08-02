#region Foreign-License
// .Net40 Surface
#endregion
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace System.Net.Mime
{
    internal static class MailBnfHelper
    {
        internal static readonly int Ascii7bitMaxValue = 0x7f;
        internal static readonly char At = '@';
        internal static bool[] Atext = new bool[0x80];
        internal static readonly char Backslash = '\\';
        internal static readonly char Comma = ',';
        internal static readonly char CR = '\r';
        internal static bool[] Ctext = new bool[0x80];
        internal static readonly char Dot = '.';
        internal static bool[] Dtext = new bool[0x80];
        internal static readonly char EndAngleBracket = '>';
        internal static readonly char EndComment = ')';
        internal static readonly char EndSquareBracket = ']';
        internal static bool[] Ftext = new bool[0x80];
        internal static readonly char LF = '\n';
        internal static bool[] Qtext = new bool[0x80];
        internal static readonly char Quote = '"';
        private static string[] s_months;
        internal static readonly char Space = ' ';
        internal static readonly char StartAngleBracket = '<';
        internal static readonly char StartComment = '(';
        internal static readonly char StartSquareBracket = '[';
        internal static readonly char Tab = '\t';
        internal static bool[] Ttext = new bool[0x80];
        internal static readonly IList<char> Whitespace;

        static MailBnfHelper()
        {
            var strArray = new string[13];
            strArray[1] = "Jan";
            strArray[2] = "Feb";
            strArray[3] = "Mar";
            strArray[4] = "Apr";
            strArray[5] = "May";
            strArray[6] = "Jun";
            strArray[7] = "Jul";
            strArray[8] = "Aug";
            strArray[9] = "Sep";
            strArray[10] = "Oct";
            strArray[11] = "Nov";
            strArray[12] = "Dec";
            s_months = strArray;
            Whitespace = new List<char>();
            Whitespace.Add(Tab);
            Whitespace.Add(Space);
            Whitespace.Add(CR);
            Whitespace.Add(LF);
            for (var i = 0x30; i <= 0x39; i++)
                Atext[i] = true;
            for (var j = 0x41; j <= 90; j++)
                Atext[j] = true;
            for (var k = 0x61; k <= 0x7a; k++)
                Atext[k] = true;
            Atext[0x21] = true;
            Atext[0x23] = true;
            Atext[0x24] = true;
            Atext[0x25] = true;
            Atext[0x26] = true;
            Atext[0x27] = true;
            Atext[0x2a] = true;
            Atext[0x2b] = true;
            Atext[0x2d] = true;
            Atext[0x2f] = true;
            Atext[0x3d] = true;
            Atext[0x3f] = true;
            Atext[0x5e] = true;
            Atext[0x5f] = true;
            Atext[0x60] = true;
            Atext[0x7b] = true;
            Atext[0x7c] = true;
            Atext[0x7d] = true;
            Atext[0x7e] = true;
            for (var m = 1; m <= 9; m++)
                Qtext[m] = true;
            Qtext[11] = true;
            Qtext[12] = true;
            for (var n = 14; n <= 0x21; n++)
                Qtext[n] = true;
            for (var num6 = 0x23; num6 <= 0x5b; num6++)
                Qtext[num6] = true;
            for (var num7 = 0x5d; num7 <= 0x7f; num7++)
                Qtext[num7] = true;
            for (var num8 = 1; num8 <= 8; num8++)
                Dtext[num8] = true;
            Dtext[11] = true;
            Dtext[12] = true;
            for (var num9 = 14; num9 <= 0x1f; num9++)
                Dtext[num9] = true;
            for (var num10 = 0x21; num10 <= 90; num10++)
                Dtext[num10] = true;
            for (var num11 = 0x5e; num11 <= 0x7f; num11++)
                Dtext[num11] = true;
            for (var num12 = 0x21; num12 <= 0x39; num12++)
                Ftext[num12] = true;
            for (var num13 = 0x3b; num13 <= 0x7e; num13++)
                Ftext[num13] = true;
            for (var num14 = 0x21; num14 <= 0x7e; num14++)
                Ttext[num14] = true;
            Ttext[40] = false;
            Ttext[0x29] = false;
            Ttext[60] = false;
            Ttext[0x3e] = false;
            Ttext[0x40] = false;
            Ttext[0x2c] = false;
            Ttext[0x3b] = false;
            Ttext[0x3a] = false;
            Ttext[0x5c] = false;
            Ttext[0x22] = false;
            Ttext[0x2f] = false;
            Ttext[0x5b] = false;
            Ttext[0x5d] = false;
            Ttext[0x3f] = false;
            Ttext[0x3d] = false;
            for (var num15 = 1; num15 <= 8; num15++)
                Ctext[num15] = true;
            Ctext[11] = true;
            Ctext[12] = true;
            for (var num16 = 14; num16 <= 0x1f; num16++)
                Ctext[num16] = true;
            for (var num17 = 0x21; num17 <= 0x27; num17++)
                Ctext[num17] = true;
            for (var num18 = 0x2a; num18 <= 0x5b; num18++)
                Ctext[num18] = true;
            for (var num19 = 0x5d; num19 <= 0x7f; num19++)
                Ctext[num19] = true;
        }

        private static bool CheckForUnicode(char ch, bool allowUnicode)
        {
            if (ch < Ascii7bitMaxValue)
                return false;
            if (!allowUnicode)
                throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", new object[] { ch }));
            return true;
        }

        internal static string GetDateTimeString(DateTime value, StringBuilder builder)
        {
            var builder2 = (builder != null ? builder : new StringBuilder());
            builder2.Append(value.Day);
            builder2.Append(' ');
            builder2.Append(s_months[value.Month]);
            builder2.Append(' ');
            builder2.Append(value.Year);
            builder2.Append(' ');
            if (value.Hour <= 9)
                builder2.Append('0');
            builder2.Append(value.Hour);
            builder2.Append(':');
            if (value.Minute <= 9)
                builder2.Append('0');
            builder2.Append(value.Minute);
            builder2.Append(':');
            if (value.Second <= 9)
                builder2.Append('0');
            builder2.Append(value.Second);
            var str = TimeZone.CurrentTimeZone.GetUtcOffset(value).ToString();
            if (str[0] != '-')
                builder2.Append(" +");
            else
                builder2.Append(" ");
            var strArray = str.Split(new char[] { ':' });
            builder2.Append(strArray[0]);
            builder2.Append(strArray[1]);
            if (builder == null)
                return builder2.ToString();
            return null;
        }

        internal static void GetTokenOrQuotedString(string data, StringBuilder builder, bool allowUnicode)
        {
            var index = 0;
            var startIndex = 0;
            while (index < data.Length)
            {
                if (!CheckForUnicode(data[index], allowUnicode) && (!Ttext[data[index]] || data[index] == ' '))
                {
                    builder.Append('"');
                    while (index < data.Length)
                    {
                        if (!CheckForUnicode(data[index], allowUnicode))
                        {
                            if (IsFWSAt(data, index))
                            {
                                index++;
                                index++;
                            }
                            else if (!Qtext[data[index]])
                            {
                                builder.Append(data, startIndex, index - startIndex);
                                builder.Append('\\');
                                startIndex = index;
                            }
                        }
                        index++;
                    }
                    builder.Append(data, startIndex, index - startIndex);
                    builder.Append('"');
                    return;
                }
                index++;
            }
            if (data.Length == 0)
                builder.Append("\"\"");
            builder.Append(data);
        }

        internal static bool HasCROrLF(string data)
        {
            for (var i = 0; i < data.Length; i++)
                if (data[i] == '\r' || data[i] == '\n')
                    return true;
            return false;
        }

        internal static bool IsFWSAt(string data, int index)
        {
            if (data[index] != CR || (index + 2) >= data.Length || data[index + 1] != LF)
                return false;
            if (data[index + 2] != Space)
                return (data[index + 2] == Tab);
            return true;
        }

        internal static string ReadParameterAttribute(string data, ref int offset, StringBuilder builder)
        {
            if (!SkipCFWS(data, ref offset))
                return null;
            return ReadToken(data, ref offset, null);
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal static string ReadQuotedString(string data, ref int offset, StringBuilder builder)
        {
            return ReadQuotedString(data, ref offset, builder, false, false);
        }

        internal static string ReadQuotedString(string data, ref int offset, StringBuilder builder, bool doesntRequireQuotes, bool permitUnicodeInDisplayName)
        {
            if (!doesntRequireQuotes)
                offset++;
            var startIndex = offset;
            var builder2 = (builder != null ? builder : new StringBuilder());
            while (offset < data.Length)
            {
                if (data[offset] == '\\')
                {
                    builder2.Append(data, startIndex, offset - startIndex);
                    startIndex = ++offset;
                }
                else
                {
                    if (data[offset] == '"')
                    {
                        builder2.Append(data, startIndex, offset - startIndex);
                        offset++;
                        if (builder == null)
                            return builder2.ToString();
                        return null;
                    }
                    if (((data[offset] == '=' && data.Length > (offset + 3)) && (data[offset + 1] == '\r' && data[offset + 2] == '\n')) && (data[offset + 3] == ' ' || data[offset + 3] == '\t'))
                        offset += 3;
                    else if (permitUnicodeInDisplayName)
                    {
                        if (data[offset] <= Ascii7bitMaxValue && !Qtext[data[offset]])
                            throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", new object[] { data[offset] }));
                    }
                    else if (data[offset] > Ascii7bitMaxValue || !Qtext[data[offset]])
                        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", new object[] { data[offset] }));
                }
                offset++;
            }
            if (!doesntRequireQuotes)
                throw new FormatException(SR.GetString("MailHeaderFieldMalformedHeader"));
            builder2.Append(data, startIndex, offset - startIndex);
            if (builder == null)
                return builder2.ToString();
            return null;
        }

        internal static string ReadToken(string data, ref int offset, StringBuilder builder)
        {
            var startIndex = offset;
            while (offset < data.Length)
            {
                if (data[offset] > Ascii7bitMaxValue)
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", new object[] { data[offset] }));
                if (!Ttext[data[offset]])
                    break;
                offset++;
            }
            if (startIndex == offset)
                throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", new object[] { data[offset] }));
            return data.Substring(startIndex, offset - startIndex);
        }

        internal static bool SkipCFWS(string data, ref int offset)
        {
            var num = 0;
            while (offset < data.Length)
            {
                if (data[offset] > '\x007f')
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", new object[] { data[offset] }));
                if (data[offset] == '\\' && num > 0)
                    offset += 2;
                else if (data[offset] == '(')
                    num++;
                else if (data[offset] == ')')
                    num--;
                else if (data[offset] != ' ' && data[offset] != '\t' && num == 0)
                    return true;
                if (num < 0)
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", new object[] { data[offset] }));
                offset++;
            }
            return false;
        }

        internal static void ValidateHeaderName(string data)
        {
            var num = 0;
            while (num < data.Length)
            {
                if (data[num] > Ftext.Length || !Ftext[data[num]])
                    throw new FormatException(SR.GetString("InvalidHeaderName"));
                num++;
            }
            if (num == 0)
                throw new FormatException(SR.GetString("InvalidHeaderName"));
        }
    }
}

