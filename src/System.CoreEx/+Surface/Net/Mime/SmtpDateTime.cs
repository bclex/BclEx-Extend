#region Foreign-License
// .Net40 Surface
#endregion
using System.Collections.Generic;
using System.Globalization;

namespace System.Net.Mime
{
    internal class SmtpDateTime
    {
        internal static readonly char[] _allowedWhiteSpaceChars = new char[] { ' ', '\t' };
        private readonly DateTime _date;
        internal const string _dateFormatWithDayOfWeek = "ddd, dd MMM yyyy HH:mm:ss";
        internal const string _dateFormatWithDayOfWeekAndNoSeconds = "ddd, dd MMM yyyy HH:mm";
        internal const string _dateFormatWithoutDayOfWeek = "dd MMM yyyy HH:mm:ss";
        internal const string _dateFormatWithoutDayOfWeekAndNoSeconds = "dd MMM yyyy HH:mm";
        internal const int _maxMinuteValue = 0x3b;
        internal const int _offsetLength = 5;
        internal static readonly int _offsetMaxValue = 0x26e7;
        internal static readonly long _timeSpanMaxTicks = 0x3460cf55a00L;
        private readonly TimeSpan _timeZone;
        internal static readonly IDictionary<string, TimeSpan> _timeZoneOffsetLookup = InitializeShortHandLookups();
        private readonly bool _unknownTimeZone;
        internal const string _unknownTimeZoneDefaultOffset = "-0000";
        internal const string _utcDefaultTimeZoneOffset = "+0000";
        internal static readonly string[] _validDateTimeFormats = new string[] { "ddd, dd MMM yyyy HH:mm:ss", "dd MMM yyyy HH:mm:ss", "ddd, dd MMM yyyy HH:mm", "dd MMM yyyy HH:mm" };

        public SmtpDateTime(DateTime value)
        {
            _date = value;
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    _unknownTimeZone = true;
                    return;

                case DateTimeKind.Utc:
                    _timeZone = TimeSpan.Zero;
                    return;

                case DateTimeKind.Local:
                    {
                        var utcOffset = TimeZoneInfo.Local.GetUtcOffset(value);
                        _timeZone = ValidateAndGetSanitizedTimeSpan(utcOffset);
                        return;
                    }
            }
        }

        public SmtpDateTime(string value)
        {
            string str;
            _date = ParseValue(value, out str);
            if (!TryParseTimeZoneString(str, out _timeZone))
                _unknownTimeZone = true;
        }

        internal string FormatDate(DateTime value)
        {
            return value.ToString("ddd, dd MMM yyyy H:mm:ss", CultureInfo.InvariantCulture);
        }

        internal static IDictionary<string, TimeSpan> InitializeShortHandLookups()
        {
            var dictionary = new Dictionary<string, TimeSpan>();
            dictionary.Add("UT", TimeSpan.Zero);
            dictionary.Add("GMT", TimeSpan.Zero);
            dictionary.Add("EDT", new TimeSpan(-4, 0, 0));
            dictionary.Add("EST", new TimeSpan(-5, 0, 0));
            dictionary.Add("CDT", new TimeSpan(-5, 0, 0));
            dictionary.Add("CST", new TimeSpan(-6, 0, 0));
            dictionary.Add("MDT", new TimeSpan(-6, 0, 0));
            dictionary.Add("MST", new TimeSpan(-7, 0, 0));
            dictionary.Add("PDT", new TimeSpan(-7, 0, 0));
            dictionary.Add("PST", new TimeSpan(-8, 0, 0));
            return dictionary;
        }

        public DateTime ParseValue(string data, out string timeZone)
        {
            DateTime time;
            if (string.IsNullOrEmpty(data))
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            var index = data.IndexOf(':');
            if (index == -1)
                throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
            var length = data.IndexOfAny(_allowedWhiteSpaceChars, index);
            if (length == -1)
                throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
            if (!DateTime.TryParseExact(data.Substring(0, length).Trim(), _validDateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out time))
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            var str2 = data.Substring(length).Trim();
            var num3 = str2.IndexOfAny(_allowedWhiteSpaceChars);
            if (num3 != -1)
                str2 = str2.Substring(0, num3);
            if (string.IsNullOrEmpty(str2))
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            timeZone = str2;
            return time;
        }

        public string TimeSpanToOffset(TimeSpan span)
        {
            if (span.Ticks == 0L)
                return "+0000";
            var num = (uint)Math.Abs(Math.Floor(span.TotalHours));
            var num2 = (uint)Math.Abs(span.Minutes);
            var str = (span.Ticks > 0L ? "+" : "-");
            if (num < 10)
                str = str + "0";
            str = str + num.ToString();
            if (num2 < 10)
                str = str + "0";
            return (str + num2.ToString());
        }

        public override string ToString()
        {
            if (_unknownTimeZone)
                return string.Format("{0} {1}", FormatDate(_date), "-0000");
            return string.Format("{0} {1}", FormatDate(_date), TimeSpanToOffset(_timeZone));
        }

        public bool TryParseTimeZoneString(string timeZoneString, out TimeSpan timeZone)
        {
            timeZone = TimeSpan.Zero;
            if (timeZoneString != "-0000")
            {
                if (timeZoneString[0] == '+' || timeZoneString[0] == '-')
                {
                    bool positive;
                    int hours;
                    int minutes;
                    ValidateAndGetTimeZoneOffsetValues(timeZoneString, out positive, out hours, out minutes);
                    if (!positive)
                    {
                        if (hours != 0)
                            hours *= -1;
                        else if (minutes != 0)
                            minutes *= -1;
                    }
                    timeZone = new TimeSpan(hours, minutes, 0);
                    return true;
                }
                ValidateTimeZoneShortHandValue(timeZoneString);
                if (_timeZoneOffsetLookup.ContainsKey(timeZoneString))
                {
                    timeZone = _timeZoneOffsetLookup[timeZoneString];
                    return true;
                }
            }
            return false;
        }

        internal TimeSpan ValidateAndGetSanitizedTimeSpan(TimeSpan span)
        {
            var span2 = new TimeSpan(span.Days, span.Hours, span.Minutes, 0, 0);
            if (Math.Abs(span2.Ticks) > _timeSpanMaxTicks)
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            return span2;
        }

        internal void ValidateAndGetTimeZoneOffsetValues(string offset, out bool positive, out int hours, out int minutes)
        {
            if (offset.Length != 5)
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            positive = offset.StartsWith("+");
            if (!int.TryParse(offset.Substring(1, 2), NumberStyles.None, CultureInfo.InvariantCulture, out hours))
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            if (!int.TryParse(offset.Substring(3, 2), NumberStyles.None, CultureInfo.InvariantCulture, out minutes))
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            if (minutes > 0x3b)
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
        }

        internal void ValidateTimeZoneShortHandValue(string value)
        {
            for (int i = 0; i < value.Length; i++)
                if (!char.IsLetter(value, i))
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
        }

        public DateTime Date
        {
            get
            {
                if (_unknownTimeZone)
                    return DateTime.SpecifyKind(_date, DateTimeKind.Unspecified);
                var offset = new DateTimeOffset(_date, _timeZone);
                return offset.LocalDateTime;
            }
        }
    }
}

