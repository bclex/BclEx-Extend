using System.Runtime.InteropServices;
using System.Text;
namespace System.Globalization
{
    internal static class NumberSpanFormat
    {
        internal static readonly FormatLiterals NegativeInvariantFormatLiterals = FormatLiterals.InitInvariant(true);
        internal static readonly FormatLiterals PositiveInvariantFormatLiterals = FormatLiterals.InitInvariant(false);

        [StructLayout(LayoutKind.Sequential)]
        internal struct FormatLiterals
        {
            internal int _cc;
            internal int _tt;
            private string[] _literals;

            internal string Start
            {
                get { return _literals[0]; }
            }

            internal string OfSep
            {
                get { return _literals[1]; }
            }

            internal string End
            {
                get { return _literals[2]; }
            }

            internal static NumberSpanFormat.FormatLiterals InitInvariant(bool isNegative)
            {
                return new NumberSpanFormat.FormatLiterals
                {
                    _literals = new[] { 
                        (isNegative ? "-" : string.Empty),
                        " of ",
                        string.Empty },
                    _cc = 2,
                    _tt = 2,
                };
            }

            internal void Init(string format, bool useInvariantFieldLengths)
            {
                _literals = new string[3];
                for (int i = 0; i < _literals.Length; i++)
                    _literals[i] = string.Empty;
                _cc = 0;
                _tt = 0;
                StringBuilder sb = StringBuilderCache.Acquire(0x10);
                bool inquote = false;
                char ch = '\'';
                int index = 0;
                for (int j = 0; j < format.Length; j++)
                {
                    switch (format[j])
                    {
                        case '\'':
                        case '"':
                            {
                                if (inquote && ch == format[j])
                                {
                                    if (index < 0 || index > 3)
                                        return;
                                    _literals[index] = sb.ToString();
                                    sb.Length = 0;
                                    inquote = false;
                                }
                                else if (!inquote)
                                {
                                    ch = format[j];
                                    inquote = true;
                                }
                                continue;
                            }
                        case 'c':
                            {
                                if (!inquote)
                                {
                                    index = 1;
                                    _cc++;
                                }
                                continue;
                            }
                        case 't':
                            {
                                if (!inquote)
                                {
                                    index = 2;
                                    _tt++;
                                }
                                continue;
                            }
                        case '\\':
                            {
                                if (inquote)
                                    break;
                                j++;
                                continue;
                            }
                    }
                    sb.Append(format[j]);
                }
                if (useInvariantFieldLengths)
                {
                    _cc = 2;
                    _tt = 2;
                }
                else
                {
                    if (_cc < 1 || _cc > 2)
                        _cc = 2;
                    if (_tt < 1 || _tt > 2)
                        _tt = 2;
                }
                StringBuilderCache.Release(sb);
            }
        }

        internal enum Pattern
        {
            None,
            Minimum,
            Full
        }

        internal static string Format(NumberSpan value, string format, IFormatProvider formatProvider)
        {
            Pattern minimum;
            if (format == null || format.Length == 0)
                format = "c";
            //if (format.Length != 1)
            //    return FormatCustomized(value, format, DateTimeFormatInfo.GetInstance(formatProvider));
            char ch = format[0];
            switch (ch)
            {
                case 'c':
                case 't':
                case 'T':
                    return FormatStandard(value, true, format, Pattern.Minimum);
            }
            if (ch != 'g' && ch != 'G')
                throw new FormatException("Format_InvalidString");
            //DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(formatProvider);
            //if (value._current < 0L)
            //    format = instance.FullTimeSpanNegativePattern;
            //else
            //    format = instance.FullTimeSpanPositivePattern;
            if (ch == 'g')
                minimum = Pattern.Minimum;
            else
                minimum = Pattern.Full;
            return FormatStandard(value, false, format, minimum);
        }

        //internal static string FormatCustomized(NumberSpan value, string format, DateTimeFormatInfo dtfi)
        //{
        //    int num = (int)(value._current / 0xc92a69c000L);
        //    long num2 = value._current % 0xc92a69c000L;
        //    if (value._current < 0L)
        //    {
        //        num = -num;
        //        num2 = -num2;
        //    }
        //    int num3 = (int)((num2 / 0x861c46800L) % 0x18L);
        //    int num4 = (int)((num2 / 0x23c34600L) % 60L);
        //    int num5 = (int)((num2 / 0x989680L) % 60L);
        //    int num6 = (int)(num2 % 0x989680L);
        //    long num7 = 0L;
        //    int pos = 0;
        //    StringBuilder outputBuffer = StringBuilderCache.Acquire(0x10);
        //    while (pos < format.Length)
        //    {
        //        int num9;
        //        int num10;
        //        char patternChar = format[pos];
        //        switch (patternChar)
        //        {
        //            case '%':
        //                {
        //                    num10 = DateTimeFormat.ParseNextChar(format, pos);
        //                    if ((num10 < 0) || (num10 == 0x25))
        //                    {
        //                        throw new FormatException("Format_InvalidString");
        //                    }
        //                    char ch3 = (char)num10;
        //                    outputBuffer.Append(FormatCustomized(value, ch3.ToString(), dtfi));
        //                    num9 = 2;
        //                    goto Label_035F;
        //                }
        //            case '\'':
        //            case '"':
        //                {
        //                    StringBuilder result = new StringBuilder();
        //                    num9 = DateTimeFormat.ParseQuoteString(format, pos, result);
        //                    outputBuffer.Append(result);
        //                    goto Label_035F;
        //                }
        //            case 'F':
        //                {
        //                    num9 = DateTimeFormat.ParseRepeatPattern(format, pos, patternChar);
        //                    if (num9 > 7)
        //                    {
        //                        throw new FormatException("Format_InvalidString");
        //                    }
        //                    num7 = num6;
        //                    num7 /= (long)Math.Pow(10.0, (double)(7 - num9));
        //                    int num11 = num9;
        //                    while (num11 > 0)
        //                    {
        //                        if ((num7 % 10L) != 0L)
        //                        {
        //                            break;
        //                        }
        //                        num7 /= 10L;
        //                        num11--;
        //                    }
        //                    if (num11 > 0)
        //                    {
        //                        outputBuffer.Append(num7.ToString(DateTimeFormat.fixedNumberFormats[num11 - 1], CultureInfo.InvariantCulture));
        //                    }
        //                    goto Label_035F;
        //                }
        //            case 'm':
        //                num9 = DateTimeFormat.ParseRepeatPattern(format, pos, patternChar);
        //                if (num9 > 2)
        //                    throw new FormatException("Format_InvalidString");
        //                DateTimeFormat.FormatDigits(outputBuffer, num4, num9);
        //                goto Label_035F;

        //            case 's':
        //                num9 = DateTimeFormat.ParseRepeatPattern(format, pos, patternChar);
        //                if (num9 > 2)
        //                    throw new FormatException("Format_InvalidString");
        //                DateTimeFormat.FormatDigits(outputBuffer, num5, num9);
        //                goto Label_035F;

        //            case 'd':
        //                num9 = DateTimeFormat.ParseRepeatPattern(format, pos, patternChar);
        //                if (num9 > 8)
        //                    throw new FormatException("Format_InvalidString");
        //                goto Label_02A9;

        //            case 'f':
        //                num9 = DateTimeFormat.ParseRepeatPattern(format, pos, patternChar);
        //                if (num9 > 7)
        //                    throw new FormatException("Format_InvalidString");
        //                goto Label_01BA;

        //            case 'h':
        //                num9 = DateTimeFormat.ParseRepeatPattern(format, pos, patternChar);
        //                if (num9 > 2)
        //                    throw new FormatException("Format_InvalidString");
        //                break;

        //            case '\\':
        //                num10 = DateTimeFormat.ParseNextChar(format, pos);
        //                if (num10 < 0)
        //                    throw new FormatException("Format_InvalidString");
        //                outputBuffer.Append((char)num10);
        //                num9 = 2;
        //                goto Label_035F;

        //            default:
        //                throw new FormatException("Format_InvalidString");
        //        }
        //        DateTimeFormat.FormatDigits(outputBuffer, num3, num9);
        //        goto Label_035F;
        //    Label_01BA:
        //        num7 = num6;
        //        outputBuffer.Append((num7 / ((long)Math.Pow(10.0, (double)(7 - num9)))).ToString(DateTimeFormat.fixedNumberFormats[num9 - 1], CultureInfo.InvariantCulture));
        //        goto Label_035F;
        //    Label_02A9:
        //        DateTimeFormat.FormatDigits(outputBuffer, num, num9, true);
        //    Label_035F:
        //        pos += num9;
        //    }
        //    return StringBuilderCache.GetStringAndRelease(outputBuffer);
        //}

        private static string FormatStandard(NumberSpan value, bool isInvariant, string format, Pattern pattern)
        {
            StringBuilder sb = StringBuilderCache.Acquire(0x10);
            int num = (int)(value._ticks / 0xc92a69c000L);
            int num2 = (int)(value._ticks % 0xc92a69c000L);
            if (value._ticks < 0L)
            {
                num = -num;
                num2 = -num2;
            }
            FormatLiterals formatLiterals;
            if (isInvariant)
                formatLiterals = (value._ticks < 0L ? NegativeInvariantFormatLiterals : PositiveInvariantFormatLiterals);
            else
            {
                formatLiterals = new FormatLiterals();
                formatLiterals.Init(format, pattern == Pattern.Full);
            }
            sb.Append(formatLiterals.Start);
            sb.Append(IntToString(num2, formatLiterals._cc));
            if (pattern == Pattern.Full || num != 0)
            {
                sb.Append(formatLiterals.OfSep);
                sb.Append(IntToString(num, formatLiterals._tt));
            }
            sb.Append(formatLiterals.End);
            return StringBuilderCache.GetStringAndRelease(sb);
        }

        private static string IntToString(int n, int digits)
        {
            return n.ToString();
        }
    }
}