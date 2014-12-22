using System.Runtime.InteropServices;
using System.Text;
namespace System.Globalization
{
    internal static class NumberSpanParse
    {
        private static readonly NumberSpanToken _zero = new NumberSpanToken(0);

        [StructLayout(LayoutKind.Sequential)]
        private struct NumberSpanResult
        {
            internal NumberSpan _parsedNumberSpan;
            internal NumberSpanParse.NumberSpanThrowStyle _throwStyle;
            internal NumberSpanParse.ParseFailureKind _failure;
            internal string _failureMessageID;
            internal object _failureMessageFormatArgument;
            internal string _failureArgumentName;

            internal void Init(NumberSpanParse.NumberSpanThrowStyle throwStyle)
            {
                _parsedNumberSpan = new NumberSpan();
                _throwStyle = throwStyle;
            }

            internal void SetFailure(NumberSpanParse.ParseFailureKind failure, string failureMessageID) { SetFailure(failure, failureMessageID, null, null); }
            internal void SetFailure(NumberSpanParse.ParseFailureKind failure, string failureMessageID, object failureMessageFormatArgument) { SetFailure(failure, failureMessageID, failureMessageFormatArgument, null); }
            internal void SetFailure(NumberSpanParse.ParseFailureKind failure, string failureMessageID, object failureMessageFormatArgument, string failureArgumentName)
            {
                _failure = failure;
                _failureMessageID = failureMessageID;
                _failureMessageFormatArgument = failureMessageFormatArgument;
                _failureArgumentName = failureArgumentName;
                if (_throwStyle != NumberSpanParse.NumberSpanThrowStyle.None)
                    throw GetNumberSpanParseException();
            }

            internal Exception GetNumberSpanParseException()
            {
                switch (_failure)
                {
                    case NumberSpanParse.ParseFailureKind.ArgumentNull: return new ArgumentNullException(_failureArgumentName, _failureMessageID);
                    case NumberSpanParse.ParseFailureKind.Format: return new FormatException(_failureMessageID);
                    case NumberSpanParse.ParseFailureKind.FormatWithParameter: return new FormatException(string.Format(_failureMessageID, _failureMessageFormatArgument));
                    case NumberSpanParse.ParseFailureKind.Overflow: return new OverflowException(_failureMessageID);
                }
                return new FormatException("Format_InvalidString");
            }
        }

        [Flags]
        private enum NumberSpanStandardStyles
        {
            None,
            Invariant,
            Localized,
            Any,
            RequireFull
        }

        private enum NumberSpanThrowStyle
        {
            None,
            All
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NumberSpanToken
        {
            internal NumberSpanParse.TTT _ttt;
            internal int _num;
            internal int _zeroes;
            internal string _sep;

            public NumberSpanToken(int number)
            {
                _ttt = NumberSpanParse.TTT.Num;
                _num = number;
                _zeroes = 0;
                _sep = null;
            }

            public NumberSpanToken(int leadingZeroes, int number)
            {
                _ttt = NumberSpanParse.TTT.Num;
                _num = number;
                _zeroes = leadingZeroes;
                _sep = null;
            }

            public bool IsInvalidNumber(int maxValue, int maxPrecision)
            {
                if (_num > maxValue) return true;
                if (maxPrecision == -1) return false;
                return (_zeroes > maxPrecision || (_num != 0 && _zeroes != 0 && _num >= ((long)maxValue / ((long)Math.Pow(10.0, (double)(_zeroes - 1))))));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NumberSpanTokenizer
        {
            private int _pos;
            private string _value;

            internal void Init(string input) { Init(input, 0); }
            internal void Init(string input, int startPosition)
            {
                _pos = startPosition;
                _value = input;
            }

            internal NumberSpanParse.NumberSpanToken GetNextToken()
            {
                var token = new NumberSpanParse.NumberSpanToken();
                var currentChar = CurrentChar;
                if (currentChar == '\0')
                {
                    token._ttt = NumberSpanParse.TTT.End;
                    return token;
                }
                if (currentChar >= '0' && currentChar <= '9')
                {
                    token._ttt = NumberSpanParse.TTT.Num;
                    token._num = 0;
                    token._zeroes = 0;
                    do
                    {
                        if ((token._num & 0xf0000000L) != 0L)
                        {
                            token._ttt = NumberSpanParse.TTT.NumOverflow;
                            return token;
                        }
                        token._num = ((token._num * 10) + currentChar) - 0x30;
                        if (token._num == 0)
                            token._zeroes++;
                        if (token._num < 0)
                        {
                            token._ttt = NumberSpanParse.TTT.NumOverflow;
                            return token;
                        }
                        currentChar = NextChar;
                    }
                    while (currentChar >= '0' && currentChar <= '9');
                    return token;
                }
                token._ttt = NumberSpanParse.TTT.Sep;
                var pos = _pos;
                var length = 0;
                while (currentChar != '\0' && (currentChar < '0' || '9' < currentChar))
                {
                    currentChar = NextChar;
                    length++;
                }
                token._sep = _value.Substring(pos, length);
                return token;
            }

            internal bool EOL
            {
                get { return (_pos >= (_value.Length - 1)); }
            }

            internal void BackOne()
            {
                if (_pos > 0) _pos--;
            }

            internal char NextChar
            {
                get { _pos++; return CurrentChar; }
            }

            internal char CurrentChar
            {
                get { return (_pos > -1 && _pos < _value.Length ? _value[_pos] : '\0'); }
            }
        }

        private enum TTT
        {
            None,
            End,
            Num,
            Sep,
            NumOverflow
        }

        private enum ParseFailureKind
        {
            None,
            ArgumentNull,
            Format,
            FormatWithParameter,
            Overflow
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StringParser
        {
            private string _str;
            private char _ch;
            private int _pos;
            private int _len;

            internal void NextChar()
            {
                if (_pos < _len)
                    _pos++;
                _ch = (_pos < _len ? _str[_pos] : '\0');
            }

            internal char NextNonDigit()
            {
                for (int i = _pos; i < _len; i++)
                {
                    char ch = _str[i];
                    if (ch < '0' || ch > '9')
                        return ch;
                }
                return '\0';
            }

            internal bool TryParse(string input, ref NumberSpanParse.NumberSpanResult result)
            {
                result._parsedNumberSpan._ticks = 0L;
                if (input == null)
                {
                    result.SetFailure(NumberSpanParse.ParseFailureKind.ArgumentNull, "ArgumentNull_String", null, "input");
                    return false;
                }
                _str = input;
                _len = input.Length;
                _pos = -1;
                NextChar();
                SkipBlanks();
                var negative = false;
                if (_ch == '-')
                {
                    negative = true;
                    NextChar();
                }
                long num;
                if (NextNonDigit() == ':')
                {
                    if (!ParseNumber(out num, ref result))
                        return false;
                }
                else
                {
                    int num2;
                    if (!ParseInt(0xa2e3ff, out num2, ref result))
                        return false;
                    num = num2 * 0xc92a69c000L;
                    if (_ch == '.')
                    {
                        long num3;
                        NextChar();
                        if (!ParseNumber(out num3, ref result))
                            return false;
                        num += num3;
                    }
                }
                if (negative)
                {
                    num = -num;
                    if (num > 0L)
                    {
                        result.SetFailure(NumberSpanParse.ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                        return false;
                    }
                }
                else if (num < 0L)
                {
                    result.SetFailure(NumberSpanParse.ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                    return false;
                }
                SkipBlanks();
                if (_pos < _len)
                {
                    result.SetFailure(NumberSpanParse.ParseFailureKind.Format, "Format_BadNumberSpan");
                    return false;
                }
                result._parsedNumberSpan._ticks = num;
                return true;
            }

            internal bool ParseInt(int max, out int i, ref NumberSpanParse.NumberSpanResult result)
            {
                i = 0;
                var pos = _pos;
                while (_ch >= '0' && _ch <= '9')
                {
                    if ((((long)i) & 0xf0000000L) != 0L)
                    {
                        result.SetFailure(NumberSpanParse.ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                        return false;
                    }
                    i = ((i * 10) + _ch) - 0x30;
                    if (i < 0)
                    {
                        result.SetFailure(NumberSpanParse.ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                        return false;
                    }
                    NextChar();
                }
                if (pos == _pos)
                {
                    result.SetFailure(NumberSpanParse.ParseFailureKind.Format, "Format_BadNumberSpan");
                    return false;
                }
                if (i > max)
                {
                    result.SetFailure(NumberSpanParse.ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                    return false;
                }
                return true;
            }

            internal bool ParseNumber(out long number, ref NumberSpanParse.NumberSpanResult result)
            {
                int num;
                number = 0L;
                if (!ParseInt(0x17, out num, ref result))
                    return false;
                number = num * 0x861c46800L;
                if (_ch != ':')
                {
                    result.SetFailure(NumberSpanParse.ParseFailureKind.Format, "Format_BadNumberSpan");
                    return false;
                }
                NextChar();
                if (!ParseInt(0x3b, out num, ref result))
                    return false;
                number += num * 0x23c34600L;
                if (_ch == ':')
                {
                    NextChar();
                    if (_ch != '.')
                    {
                        if (!ParseInt(0x3b, out num, ref result))
                            return false;
                        number += num * 0x989680L;
                    }
                    if (_ch == '.')
                    {
                        NextChar();
                        int num2 = 0x989680;
                        while (num2 > 1 && _ch >= '0' && _ch <= '9')
                        {
                            num2 /= 10;
                            number += (_ch - '0') * num2;
                            NextChar();
                        }
                    }
                }
                return true;
            }

            internal void SkipBlanks()
            {
                while (_ch == ' ' || _ch == '\t')
                    NextChar();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NumberSpanRawInfo
        {
            //private const int MaxTokens = 11;
            //private const int MaxLiteralTokens = 6;
            //private const int MaxNumericTokens = 5;
            internal NumberSpanParse.TTT _lastSeenTTT;
            internal int _tokenCount;
            internal int _sepCount;
            internal int _numCount;
            internal string[] _literals;
            internal NumberSpanParse.NumberSpanToken[] _numbers;
            private NumberSpanFormat.FormatLiterals _posLoc;
            private NumberSpanFormat.FormatLiterals _negLoc;
            private bool _posLocInit;
            private bool _negLocInit;
            private string _fullPosPattern;
            private string _fullNegPattern;

            internal NumberSpanFormat.FormatLiterals PositiveInvariant
            {
                get { return NumberSpanFormat.PositiveInvariantFormatLiterals; }
            }

            internal NumberSpanFormat.FormatLiterals NegativeInvariant
            {
                get { return NumberSpanFormat.NegativeInvariantFormatLiterals; }
            }

            internal NumberSpanFormat.FormatLiterals PositiveLocalized
            {
                get
                {
                    if (!_posLocInit)
                    {
                        _posLoc = new NumberSpanFormat.FormatLiterals();
                        _posLoc.Init(_fullPosPattern, false);
                        _posLocInit = true;
                    }
                    return _posLoc;
                }
            }
            internal NumberSpanFormat.FormatLiterals NegativeLocalized
            {
                get
                {
                    if (!_negLocInit)
                    {
                        _negLoc = new NumberSpanFormat.FormatLiterals();
                        _negLoc.Init(_fullNegPattern, false);
                        _negLocInit = true;
                    }
                    return _negLoc;
                }
            }

            internal bool FullMatch(NumberSpanFormat.FormatLiterals pattern)
            {
                return (_sepCount == 6 && _numCount == 5 && pattern.Start == _literals[0] && string.Equals(pattern.OfSep, _literals[1], StringComparison.OrdinalIgnoreCase) && pattern.End == _literals[2]);
            }

            internal bool FullCMatch(NumberSpanFormat.FormatLiterals pattern)
            {
                return (_sepCount == 2 && _numCount == 1 && pattern.Start == _literals[0] && pattern.End == _literals[1]);
            }

            internal bool FullCTMatch(NumberSpanFormat.FormatLiterals pattern)
            {
                return (_sepCount == 3 && _numCount == 2 && pattern.Start == _literals[0] && string.Equals(pattern.OfSep, _literals[1], StringComparison.OrdinalIgnoreCase) && pattern.End == _literals[2]);
            }

            internal void Init()
            {
                _lastSeenTTT = NumberSpanParse.TTT.None;
                _tokenCount = 0;
                _sepCount = 0;
                _numCount = 0;
                _literals = new string[4];
                _numbers = new NumberSpanParse.NumberSpanToken[3];
                _fullPosPattern = "c of t";
                _fullNegPattern = "-c of t";
                _posLocInit = false;
                _negLocInit = false;
            }

            internal bool ProcessToken(ref NumberSpanParse.NumberSpanToken tok, ref NumberSpanParse.NumberSpanResult result)
            {
                if (tok._ttt == NumberSpanParse.TTT.NumOverflow)
                {
                    result.SetFailure(NumberSpanParse.ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge", null);
                    return false;
                }
                if (tok._ttt != NumberSpanParse.TTT.Sep && tok._ttt != NumberSpanParse.TTT.Num)
                {
                    result.SetFailure(NumberSpanParse.ParseFailureKind.Format, "Format_BadNumberSpan", null);
                    return false;
                }
                switch (tok._ttt)
                {
                    case NumberSpanParse.TTT.Num:
                        if (_tokenCount != 0 || AddSep(string.Empty, ref result))
                        {
                            if (!AddNum(tok, ref result))
                                return false;
                            break;
                        }
                        return false;
                    case NumberSpanParse.TTT.Sep:
                        if (AddSep(tok._sep, ref result))
                            break;
                        return false;
                }
                _lastSeenTTT = tok._ttt;
                return true;
            }

            private bool AddSep(string sep, ref NumberSpanParse.NumberSpanResult result)
            {
                if (_sepCount >= 6 || _tokenCount >= 11)
                {
                    result.SetFailure(NumberSpanParse.ParseFailureKind.Format, "Format_BadNumberSpan", null);
                    return false;
                }
                _literals[_sepCount++] = sep;
                _tokenCount++;
                return true;
            }

            private bool AddNum(NumberSpanParse.NumberSpanToken num, ref NumberSpanParse.NumberSpanResult result)
            {
                if (_numCount >= 5 || _tokenCount >= 11)
                {
                    result.SetFailure(NumberSpanParse.ParseFailureKind.Format, "Format_BadNumberSpan", null);
                    return false;
                }
                _numbers[_numCount++] = num;
                _tokenCount++;
                return true;
            }
        }

        internal static NumberSpan Parse(string input, IFormatProvider formatProvider)
        {
            NumberSpanResult result = new NumberSpanResult();
            result.Init(NumberSpanThrowStyle.All);
            if (!TryParseNumberSpan(input, NumberSpanStandardStyles.Any, formatProvider, ref result))
                throw result.GetNumberSpanParseException();
            return result._parsedNumberSpan;
        }

        internal static NumberSpan ParseExact(string input, string format, IFormatProvider formatProvider, NumberSpanStyles styles)
        {
            NumberSpanResult result = new NumberSpanResult();
            result.Init(NumberSpanThrowStyle.All);
            if (!TryParseExactNumberSpan(input, format, formatProvider, styles, ref result))
                throw result.GetNumberSpanParseException();
            return result._parsedNumberSpan;
        }

        private static bool ParseExactDigits(ref NumberSpanTokenizer tokenizer, int minDigitLength, out int result)
        {
            result = 0;
            int zeroes = 0;
            int maxDigitLength = (minDigitLength == 1 ? 2 : minDigitLength);
            return ParseExactDigits(ref tokenizer, minDigitLength, maxDigitLength, out zeroes, out result);
        }

        private static bool ParseExactDigits(ref NumberSpanTokenizer tokenizer, int minDigitLength, int maxDigitLength, out int zeroes, out int result)
        {
            result = 0;
            zeroes = 0;
            int num = 0;
            while (num < maxDigitLength)
            {
                char nextChar = tokenizer.NextChar;
                if (nextChar < '0' || nextChar > '9')
                {
                    tokenizer.BackOne();
                    break;
                }
                result = (result * 10) + (nextChar - '0');
                if (result == 0)
                    zeroes++;
                num++;
            }
            return (num >= minDigitLength);
        }

        private static bool ParseExactLiteral(ref NumberSpanTokenizer tokenizer, StringBuilder enquotedString)
        {
            for (int i = 0; i < enquotedString.Length; i++)
                if (enquotedString[i] != tokenizer.NextChar)
                    return false;
            return true;
        }

        internal static NumberSpan ParseExactMultiple(string input, string[] formats, IFormatProvider formatProvider, NumberSpanStyles styles)
        {
            NumberSpanResult result = new NumberSpanResult();
            result.Init(NumberSpanThrowStyle.All);
            if (!TryParseExactMultipleNumberSpan(input, formats, formatProvider, styles, ref result))
                throw result.GetNumberSpanParseException();
            return result._parsedNumberSpan;
        }

        private static bool ProcessTerminal_C(ref NumberSpanRawInfo raw, NumberSpanStandardStyles style, ref NumberSpanResult result)
        {
            if (raw._sepCount != 2 || raw._numCount != 1 || (style & NumberSpanStandardStyles.RequireFull) != NumberSpanStandardStyles.None)
            {
                result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
                return false;
            }
            bool invariant = (style & NumberSpanStandardStyles.Invariant) != NumberSpanStandardStyles.None;
            bool localized = (style & NumberSpanStandardStyles.Localized) != NumberSpanStandardStyles.None;
            bool positive = false;
            bool match = false;
            if (invariant)
            {
                if (raw.FullCMatch(raw.PositiveInvariant))
                {
                    match = true;
                    positive = true;
                }
                if (!match && raw.FullCMatch(raw.NegativeInvariant))
                {
                    match = true;
                    positive = false;
                }
            }
            if (localized)
            {
                if (!match && raw.FullCMatch(raw.PositiveLocalized))
                {
                    match = true;
                    positive = true;
                }
                if (!match && raw.FullCMatch(raw.NegativeLocalized))
                {
                    match = true;
                    positive = false;
                }
            }
            long num;
            if (match)
            {
                if (!TryNumberToTicks(positive, raw._numbers[0], _zero, _zero, out num))
                {
                    result.SetFailure(ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                    return false;
                }
                if (!positive)
                {
                    num = -num;
                    if (num > 0)
                    {
                        result.SetFailure(ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                        return false;
                    }
                }
                result._parsedNumberSpan._ticks = num;
                return true;
            }
            result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
            return false;
        }

        private static bool ProcessTerminal_FULL(ref NumberSpanRawInfo raw, NumberSpanStandardStyles style, ref NumberSpanResult result)
        {
            if (raw._sepCount != 3 || raw._numCount != 5)
            {
                result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
                return false;
            }
            bool invariant = (style & NumberSpanStandardStyles.Invariant) != NumberSpanStandardStyles.None;
            bool localized = (style & NumberSpanStandardStyles.Localized) != NumberSpanStandardStyles.None;
            bool positive = false;
            bool match = false;
            if (invariant)
            {
                if (raw.FullMatch(raw.PositiveInvariant))
                {
                    match = true;
                    positive = true;
                }
                if (!match && raw.FullMatch(raw.NegativeInvariant))
                {
                    match = true;
                    positive = false;
                }
            }
            if (localized)
            {
                if (!match && raw.FullMatch(raw.PositiveLocalized))
                {
                    match = true;
                    positive = true;
                }
                if (!match && raw.FullMatch(raw.NegativeLocalized))
                {
                    match = true;
                    positive = false;
                }
            }
            if (match)
            {
                long num;
                if (!TryNumberToTicks(positive, raw._numbers[0], raw._numbers[1], raw._numbers[2], out num))
                {
                    result.SetFailure(ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                    return false;
                }
                if (!positive)
                {
                    num = -num;
                    if (num > 0)
                    {
                        result.SetFailure(ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                        return false;
                    }
                }
                result._parsedNumberSpan._ticks = num;
                return true;
            }
            result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
            return false;
        }

        private static bool ProcessTerminal_CT(ref NumberSpanRawInfo raw, NumberSpanStandardStyles style, ref NumberSpanResult result)
        {
            if (raw._sepCount != 3 || raw._numCount != 2 || (style & NumberSpanStandardStyles.RequireFull) != NumberSpanStandardStyles.None)
            {
                result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
                return false;
            }
            bool invariant = (style & NumberSpanStandardStyles.Invariant) != NumberSpanStandardStyles.None;
            bool localized = (style & NumberSpanStandardStyles.Localized) != NumberSpanStandardStyles.None;
            bool positive = false;
            bool match = false;
            if (invariant)
            {
                if (raw.FullCTMatch(raw.PositiveInvariant))
                {
                    match = true;
                    positive = true;
                }
                if (!match && raw.FullCTMatch(raw.NegativeInvariant))
                {
                    match = true;
                    positive = false;
                }
            }
            if (localized)
            {
                if (!match && raw.FullCTMatch(raw.PositiveLocalized))
                {
                    match = true;
                    positive = true;
                }
                if (!match && raw.FullCTMatch(raw.NegativeLocalized))
                {
                    match = true;
                    positive = false;
                }
            }
            long num;
            if (match)
            {
                if (!TryNumberToTicks(positive, raw._numbers[0], raw._numbers[1], _zero, out num))
                {
                    result.SetFailure(ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                    return false;
                }
                if (!positive)
                {
                    num = -num;
                    if (num > 0)
                    {
                        result.SetFailure(ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
                        return false;
                    }
                }
                result._parsedNumberSpan._ticks = num;
                return true;
            }
            result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
            return false;
        }

        private static bool ProcessTerminalState(ref NumberSpanRawInfo raw, NumberSpanStandardStyles style, ref NumberSpanResult result)
        {
            if (raw._lastSeenTTT == TTT.Num)
            {
                var tok = new NumberSpanToken
                {
                    _ttt = TTT.Sep,
                    _sep = string.Empty
                };
                if (!raw.ProcessToken(ref tok, ref result))
                {
                    result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
                    return false;
                }
            }
            switch (raw._numCount)
            {
                case 1: return ProcessTerminal_C(ref raw, style, ref result);
                case 2: return ProcessTerminal_CT(ref raw, style, ref result);
                case 3: return ProcessTerminal_FULL(ref raw, style, ref result);
            }
            result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
            return false;
        }

        internal static bool TryParse(string input, IFormatProvider formatProvider, out NumberSpan result)
        {
            NumberSpanResult result2 = new NumberSpanResult();
            result2.Init(NumberSpanThrowStyle.None);
            if (TryParseNumberSpan(input, NumberSpanStandardStyles.Any, formatProvider, ref result2))
            {
                result = result2._parsedNumberSpan;
                return true;
            }
            result = new NumberSpan();
            return false;
        }

        //private static bool TryParseByFormat(string input, string format, NumberSpanStyles styles, ref NumberSpanResult result)
        //{
        //    bool flag = false;
        //    bool flag2 = false;
        //    bool flag3 = false;
        //    bool flag4 = false;
        //    bool flag5 = false;
        //    int num = 0;
        //    int num2 = 0;
        //    int num3 = 0;
        //    int num4 = 0;
        //    int zeroes = 0;
        //    int num6 = 0;
        //    int pos = 0;
        //    int returnValue = 0;
        //    NumberSpanTokenizer tokenizer = new NumberSpanTokenizer();
        //    tokenizer.Init(input, -1);
        //    while (pos < format.Length)
        //    {
        //        int num9;
        //        char failureMessageFormatArgument = format[pos];
        //        switch (failureMessageFormatArgument)
        //        {
        //            case '%':
        //                num9 = DateTimeFormat.ParseNextChar(format, pos);
        //                if (num9 < 0 || num9 == 0x25)
        //                    goto Label_0280;
        //                returnValue = 1;
        //                goto Label_02CA;

        //            case '\'':
        //            case '"':
        //                {
        //                    StringBuilder builder = new StringBuilder();
        //                    if (!DateTimeParse.TryParseQuoteString(format, pos, builder, out returnValue))
        //                    {
        //                        result.SetFailure(ParseFailureKind.FormatWithParameter, "Format_BadQuote", failureMessageFormatArgument);
        //                        return false;
        //                    }
        //                    if (ParseExactLiteral(ref tokenizer, builder))
        //                    {
        //                        goto Label_02CA;
        //                    }
        //                    result.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
        //                    return false;
        //                }
        //            case 'F':
        //                returnValue = DateTimeFormat.ParseRepeatPattern(format, pos, failureMessageFormatArgument);
        //                if ((returnValue > 7) || flag5)
        //                {
        //                    result.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
        //                    return false;
        //                }
        //                ParseExactDigits(ref tokenizer, returnValue, returnValue, out zeroes, out num6);
        //                flag5 = true;
        //                goto Label_02CA;

        //            case 'm':
        //                returnValue = DateTimeFormat.ParseRepeatPattern(format, pos, failureMessageFormatArgument);
        //                if (((returnValue > 2) || flag3) || !ParseExactDigits(ref tokenizer, returnValue, out num3))
        //                {
        //                    result.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
        //                    return false;
        //                }
        //                flag3 = true;
        //                goto Label_02CA;

        //            case 's':
        //                returnValue = DateTimeFormat.ParseRepeatPattern(format, pos, failureMessageFormatArgument);
        //                if (((returnValue > 2) || flag4) || !ParseExactDigits(ref tokenizer, returnValue, out num4))
        //                {
        //                    result.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
        //                    return false;
        //                }
        //                flag4 = true;
        //                goto Label_02CA;

        //            case 'd':
        //                {
        //                    returnValue = DateTimeFormat.ParseRepeatPattern(format, pos, failureMessageFormatArgument);
        //                    int num10 = 0;
        //                    if (((returnValue > 8) || flag) || !ParseExactDigits(ref tokenizer, (returnValue < 2) ? 1 : returnValue, (returnValue < 2) ? 8 : returnValue, out num10, out num))
        //                    {
        //                        result.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
        //                        return false;
        //                    }
        //                    flag = true;
        //                    goto Label_02CA;
        //                }
        //            case 'f':
        //                returnValue = DateTimeFormat.ParseRepeatPattern(format, pos, failureMessageFormatArgument);
        //                if (((returnValue <= 7) && !flag5) && ParseExactDigits(ref tokenizer, returnValue, returnValue, out zeroes, out num6))
        //                {
        //                    goto Label_0193;
        //                }
        //                result.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
        //                return false;

        //            case 'h':
        //                returnValue = DateTimeFormat.ParseRepeatPattern(format, pos, failureMessageFormatArgument);
        //                if (((returnValue <= 2) && !flag2) && ParseExactDigits(ref tokenizer, returnValue, out num2))
        //                {
        //                    break;
        //                }
        //                result.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
        //                return false;

        //            case '\\':
        //                num9 = DateTimeFormat.ParseNextChar(format, pos);
        //                if ((num9 >= 0) && (tokenizer.NextChar == ((char)num9)))
        //                {
        //                    returnValue = 2;
        //                    goto Label_02CA;
        //                }
        //                result.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
        //                return false;

        //            default:
        //                result.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
        //                return false;
        //        }
        //        flag2 = true;
        //        goto Label_02CA;
        //    Label_0193:
        //        flag5 = true;
        //        goto Label_02CA;
        //    Label_0280:
        //        result.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
        //        return false;
        //    Label_02CA:
        //        pos += returnValue;
        //    }
        //    if (!tokenizer.EOL)
        //    {
        //        result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
        //        return false;
        //    }
        //    int num11 = 0;
        //    bool positive = (styles & NumberSpanStyles.AssumeNegative) == NumberSpanStyles.None;
        //    if (TryNumberToTicks(positive, new NumberSpanToken(num), new NumberSpanToken(num2), new NumberSpanToken(num3), new NumberSpanToken(num4), new NumberSpanToken(zeroes, num6), out num11))
        //    {
        //        if (!positive)
        //            num11 = -num11;
        //        result._parsedNumberSpan._current = num11;
        //        return true;
        //    }
        //    result.SetFailure(ParseFailureKind.Overflow, "Overflow_NumberSpanElementTooLarge");
        //    return false;
        //}

        internal static bool TryParseExact(string input, string format, IFormatProvider formatProvider, NumberSpanStyles styles, out NumberSpan result)
        {
            NumberSpanResult result2 = new NumberSpanResult();
            result2.Init(NumberSpanThrowStyle.None);
            if (TryParseExactNumberSpan(input, format, formatProvider, styles, ref result2))
            {
                result = result2._parsedNumberSpan;
                return true;
            }
            result = new NumberSpan();
            return false;
        }

        internal static bool TryParseExactMultiple(string input, string[] formats, IFormatProvider formatProvider, NumberSpanStyles styles, out NumberSpan result)
        {
            NumberSpanResult result2 = new NumberSpanResult();
            result2.Init(NumberSpanThrowStyle.None);
            if (TryParseExactMultipleNumberSpan(input, formats, formatProvider, styles, ref result2))
            {
                result = result2._parsedNumberSpan;
                return true;
            }
            result = new NumberSpan();
            return false;
        }

        private static bool TryParseExactMultipleNumberSpan(string input, string[] formats, IFormatProvider formatProvider, NumberSpanStyles styles, ref NumberSpanResult result)
        {
            if (input == null)
            {
                result.SetFailure(ParseFailureKind.ArgumentNull, "ArgumentNull_String", null, "input");
                return false;
            }
            if (formats == null)
            {
                result.SetFailure(ParseFailureKind.ArgumentNull, "ArgumentNull_String", null, "formats");
                return false;
            }
            if (input.Length == 0)
            {
                result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
                return false;
            }
            if (formats.Length == 0)
            {
                result.SetFailure(ParseFailureKind.Format, "Format_BadFormatSpecifier");
                return false;
            }
            for (int i = 0; i < formats.Length; i++)
            {
                if (formats[i] == null || formats[i].Length == 0)
                {
                    result.SetFailure(ParseFailureKind.Format, "Format_BadFormatSpecifier");
                    return false;
                }
                NumberSpanResult result2 = new NumberSpanResult();
                result2.Init(NumberSpanThrowStyle.None);
                if (TryParseExactNumberSpan(input, formats[i], formatProvider, styles, ref result2))
                {
                    result._parsedNumberSpan = result2._parsedNumberSpan;
                    return true;
                }
            }
            result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
            return false;
        }

        private static bool TryParseExactNumberSpan(string input, string format, IFormatProvider formatProvider, NumberSpanStyles styles, ref NumberSpanResult result)
        {
            if (input == null)
            {
                result.SetFailure(ParseFailureKind.ArgumentNull, "ArgumentNull_String", null, "input");
                return false;
            }
            if (format == null)
            {
                result.SetFailure(ParseFailureKind.ArgumentNull, "ArgumentNull_String", null, "format");
                return false;
            }
            if (format.Length == 0)
            {
                result.SetFailure(ParseFailureKind.Format, "Format_BadFormatSpecifier");
                return false;
            }
            //if (format.Length != 1)
            //    return TryParseByFormat(input, format, styles, ref result);
            NumberSpanStandardStyles none = NumberSpanStandardStyles.None;
            if (format[0] == 'c' || format[0] == 't' || format[0] == 'T')
                return TryParseNumberSpanConstant(input, ref result);
            if (format[0] == 'g')
                none = NumberSpanStandardStyles.Localized;
            else if (format[0] == 'G')
                none = NumberSpanStandardStyles.RequireFull | NumberSpanStandardStyles.Localized;
            else
            {
                result.SetFailure(ParseFailureKind.Format, "Format_BadFormatSpecifier");
                return false;
            }
            return TryParseNumberSpan(input, none, formatProvider, ref result);
        }

        private static bool TryParseNumberSpan(string input, NumberSpanStandardStyles style, IFormatProvider formatProvider, ref NumberSpanResult result)
        {
            if (input == null)
            {
                result.SetFailure(ParseFailureKind.ArgumentNull, "ArgumentNull_String", null, "input");
                return false;
            }
            input = input.Trim();
            if (input == string.Empty)
            {
                result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
                return false;
            }
            var tokenizer = new NumberSpanTokenizer();
            tokenizer.Init(input);
            var raw = new NumberSpanRawInfo();
            raw.Init(); //DateTimeFormatInfo.GetInstance(formatProvider));
            for (var token = tokenizer.GetNextToken(); token._ttt != TTT.End; token = tokenizer.GetNextToken())
                if (!raw.ProcessToken(ref token, ref result))
                {
                    result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
                    return false;
                }
            if (!tokenizer.EOL)
            {
                result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
                return false;
            }
            if (!ProcessTerminalState(ref raw, style, ref result))
            {
                result.SetFailure(ParseFailureKind.Format, "Format_BadNumberSpan");
                return false;
            }
            return true;
        }

        private static bool TryParseNumberSpanConstant(string input, ref NumberSpanResult result)
        {
            var parser = new StringParser();
            return parser.TryParse(input, ref result);
        }

        private static bool TryNumberToTicks(bool positive, NumberSpanToken current, NumberSpanToken total, NumberSpanToken fraction, out long result)
        {
            if (current.IsInvalidNumber(0xa2e3ff, -1) || total.IsInvalidNumber(0x17, -1))
            {
                result = 0L;
                return false;
            }
            long num = (total._num * 0xc92a69c000L) + current._num;
            if (num >= long.MaxValue || num <= long.MinValue)
            {
                result = 0L;
                return false;
            }
            //long num2 = fraction._num;
            //if (num2 != 0L)
            //{
            //    long num3 = 0xf4240L;
            //    if (fraction._zeroes > 0)
            //    {
            //        long num4 = (long)Math.Pow(10.0, (double)fraction._zeroes);
            //        num3 /= num4;
            //    }
            //    while (num2 < num3)
            //        num2 *= 10L;
            //}
            result = num; // (num * 0x2710) + num2;
            if (positive && result < 0)
            {
                result = 0L;
                return false;
            }
            return true;
        }

        internal static void ValidateStyles(NumberSpanStyles style, string parameterName)
        {
            if (style != NumberSpanStyles.None && style != NumberSpanStyles.AssumeNegative)
                throw new ArgumentException("Argument_InvalidNumberSpanStyles", parameterName);
        }
    }
}