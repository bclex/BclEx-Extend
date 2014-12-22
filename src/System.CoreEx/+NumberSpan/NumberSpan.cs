using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct NumberSpan : IComparable, IComparable<NumberSpan>, IEquatable<NumberSpan>, IFormattable
    {
        internal long _ticks;

        static NumberSpan()
        {
            Zero = new NumberSpan(0L);
            MaxValue = new NumberSpan(long.MaxValue);
            MinValue = new NumberSpan(long.MinValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberSpan"/> struct.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        public NumberSpan(long ticks)
        {
            _ticks = ticks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberSpan"/> struct.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="total">The total.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">null;Overflow_TimeSpanTooLong</exception>
        public NumberSpan(int current, int total)
        {
            long num = (total * 0xc92a69c000L) + current;
            if (num >= long.MaxValue || num <= long.MinValue)
                throw new ArgumentOutOfRangeException(null, "Overflow_TimeSpanTooLong");
            _ticks = num;
        }

        /// <summary>
        /// Adds the specified ts.
        /// </summary>
        /// <param name="ts">The ts.</param>
        /// <returns></returns>
        public NumberSpan Add(NumberSpan ts)
        {
            long num = _ticks + ts._ticks;
            return new NumberSpan(num);
        }

        /// <summary>
        /// Compares the specified t1.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns></returns>
        public static int Compare(NumberSpan t1, NumberSpan t2)
        {
            if (t1._ticks > t2._ticks) return 1;
            if (t1._ticks < t2._ticks) return -1;
            return 0;
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Arg_MustBeNumberSpan</exception>
        public int CompareTo(object value)
        {
            if (value == null) return 1;
            if (!(value is NumberSpan))
                throw new ArgumentException("Arg_MustBeNumberSpan");
            long num = ((NumberSpan)value)._ticks;
            if (_ticks > num) return 1;
            if (_ticks < num) return -1;
            return 0;
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int CompareTo(NumberSpan value)
        {
            long num = value._ticks;
            if (_ticks > num)
                return 1;
            if (_ticks < num) return -1;
            return 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            return (value is NumberSpan && _ticks == ((NumberSpan)value)._ticks);
        }

        /// <summary>
        /// Equalses the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public bool Equals(NumberSpan obj)
        {
            return (_ticks == obj._ticks);
        }

        /// <summary>
        /// Equalses the specified t1.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns></returns>
        public static bool Equals(NumberSpan t1, NumberSpan t2)
        {
            return (t1._ticks == t2._ticks);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _ticks.GetHashCode();
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static NumberSpan operator +(NumberSpan t1, NumberSpan t2)
        {
            return t1.Add(t2);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(NumberSpan t1, NumberSpan t2)
        {
            return (t1._ticks == t2._ticks);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(NumberSpan t1, NumberSpan t2)
        {
            return (t1._ticks > t2._ticks);
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >=(NumberSpan t1, NumberSpan t2)
        {
            return (t1._ticks >= t2._ticks);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(NumberSpan t1, NumberSpan t2)
        {
            return (t1._ticks != t2._ticks);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(NumberSpan t1, NumberSpan t2)
        {
            return (t1._ticks < t2._ticks);
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <=(NumberSpan t1, NumberSpan t2)
        {
            return (t1._ticks <= t2._ticks);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static NumberSpan operator -(NumberSpan t1, NumberSpan t2)
        {
            return t1.Subtract(t2);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static NumberSpan operator -(NumberSpan t)
        {
            return new NumberSpan(-t._ticks);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static NumberSpan operator +(NumberSpan t)
        {
            return t;
        }

        /// <summary>
        /// Parses the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static NumberSpan Parse(string s)
        {
            return NumberSpanParse.Parse(s, null);
        }

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns></returns>
        public static NumberSpan Parse(string input, IFormatProvider formatProvider)
        {
            return NumberSpanParse.Parse(input, formatProvider);
        }

        /// <summary>
        /// Parses the exact.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns></returns>
        public static NumberSpan ParseExact(string input, string format, IFormatProvider formatProvider)
        {
            return NumberSpanParse.ParseExact(input, format, formatProvider, NumberSpanStyles.None);
        }

        /// <summary>
        /// Parses the exact.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="formats">The formats.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns></returns>
        public static NumberSpan ParseExact(string input, string[] formats, IFormatProvider formatProvider)
        {
            return NumberSpanParse.ParseExactMultiple(input, formats, formatProvider, NumberSpanStyles.None);
        }

        /// <summary>
        /// Parses the exact.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="styles">The styles.</param>
        /// <returns></returns>
        public static NumberSpan ParseExact(string input, string format, IFormatProvider formatProvider, NumberSpanStyles styles)
        {
            NumberSpanParse.ValidateStyles(styles, "styles");
            return NumberSpanParse.ParseExact(input, format, formatProvider, styles);
        }

        /// <summary>
        /// Parses the exact.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="formats">The formats.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="styles">The styles.</param>
        /// <returns></returns>
        public static NumberSpan ParseExact(string input, string[] formats, IFormatProvider formatProvider, NumberSpanStyles styles)
        {
            NumberSpanParse.ValidateStyles(styles, "styles");
            return NumberSpanParse.ParseExactMultiple(input, formats, formatProvider, styles);
        }

        /// <summary>
        /// Subtracts the specified ts.
        /// </summary>
        /// <param name="ts">The ts.</param>
        /// <returns></returns>
        public NumberSpan Subtract(NumberSpan ts)
        {
            long num = _ticks - ts._ticks;
            return new NumberSpan(num);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return NumberSpanFormat.Format(this, null, null);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return NumberSpanFormat.Format(this, format, formatProvider);
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParse(string s, out NumberSpan result)
        {
            return NumberSpanParse.TryParse(s, null, out result);
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParse(string input, IFormatProvider formatProvider, out NumberSpan result)
        {
            return NumberSpanParse.TryParse(input, formatProvider, out result);
        }

        /// <summary>
        /// Tries the parse exact.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParseExact(string input, string format, IFormatProvider formatProvider, out NumberSpan result)
        {
            return NumberSpanParse.TryParseExact(input, format, formatProvider, NumberSpanStyles.None, out result);
        }

        /// <summary>
        /// Tries the parse exact.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="formats">The formats.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParseExact(string input, string[] formats, IFormatProvider formatProvider, out NumberSpan result)
        {
            return NumberSpanParse.TryParseExactMultiple(input, formats, formatProvider, NumberSpanStyles.None, out result);
        }

        /// <summary>
        /// Tries the parse exact.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="styles">The styles.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParseExact(string input, string format, IFormatProvider formatProvider, NumberSpanStyles styles, out NumberSpan result)
        {
            NumberSpanParse.ValidateStyles(styles, "styles");
            return NumberSpanParse.TryParseExact(input, format, formatProvider, styles, out result);
        }

        /// <summary>
        /// Tries the parse exact.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="formats">The formats.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="styles">The styles.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParseExact(string input, string[] formats, IFormatProvider formatProvider, NumberSpanStyles styles, out NumberSpan result)
        {
            NumberSpanParse.ValidateStyles(styles, "styles");
            return NumberSpanParse.TryParseExactMultiple(input, formats, formatProvider, styles, out result);
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public int Current
        {
            get { return (int)(_ticks % 0xc92a69c000L); }
        }

        /// <summary>
        /// Gets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public int Total
        {
            get { return (int)(_ticks / 0xc92a69c000L); }
        }

        /// <summary>
        /// Gets the ticks.
        /// </summary>
        /// <value>
        /// The ticks.
        /// </value>
        public long Ticks
        {
            get { return _ticks; }
        }

        /// <summary>
        /// The maximum value
        /// </summary>
        public static readonly NumberSpan MaxValue;
        /// <summary>
        /// The minimum value
        /// </summary>
        public static readonly NumberSpan MinValue;
        /// <summary>
        /// The zero
        /// </summary>
        public static readonly NumberSpan Zero;
    }
}

