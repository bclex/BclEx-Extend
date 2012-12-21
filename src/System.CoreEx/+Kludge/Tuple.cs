#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Collections;
using System.Text;
using System.Collections.Generic;
namespace System
{
    /// <summary>
    /// ITuple
    /// </summary>
    internal interface ITuple
    {
        int GetHashCode(IEqualityComparer comparer);
        string ToString(StringBuilder sb);
        int Size { get; }
    }

    /// <summary>
    /// Tuple
    /// </summary>
#if !COREINTERNAL
    public
#endif
 static class Tuple
    {
        internal static int CombineHashCodes(int h1, int h2) { return (((h1 << 5) + h1) ^ h2); }
        internal static int CombineHashCodes(int h1, int h2, int h3) { return CombineHashCodes(CombineHashCodes(h1, h2), h3); }
        internal static int CombineHashCodes(int h1, int h2, int h3, int h4) { return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4)); }
        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5) { return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5); }
        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6) { return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6)); }
        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7) { return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7)); }
        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8) { return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7, h8)); }
        /// <summary>
        /// Creates the specified item1.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="item1">The item1.</param>
        /// <returns></returns>
        public static Tuple<T1> Create<T1>(T1 item1) { return new Tuple<T1>(item1); }
        /// <summary>
        /// Creates the specified item1.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <returns></returns>
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) { return new Tuple<T1, T2>(item1, item2); }
        /// <summary>
        /// Creates the specified item1.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <returns></returns>
        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) { return new Tuple<T1, T2, T3>(item1, item2, item3); }
        /// <summary>
        /// Creates the specified item1.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <param name="item4">The item4.</param>
        /// <returns></returns>
        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) { return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4); }
        /// <summary>
        /// Creates the specified item1.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <param name="item4">The item4.</param>
        /// <param name="item5">The item5.</param>
        /// <returns></returns>
        public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) { return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5); }
        /// <summary>
        /// Creates the specified item1.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <param name="item4">The item4.</param>
        /// <param name="item5">The item5.</param>
        /// <param name="item6">The item6.</param>
        /// <returns></returns>
        public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) { return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6); }
        /// <summary>
        /// Creates the specified item1.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <param name="item4">The item4.</param>
        /// <param name="item5">The item5.</param>
        /// <param name="item6">The item6.</param>
        /// <param name="item7">The item7.</param>
        /// <returns></returns>
        public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) { return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7); }
        /// <summary>
        /// Creates the specified item1.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <typeparam name="T8">The type of the 8.</typeparam>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <param name="item4">The item4.</param>
        /// <param name="item5">The item5.</param>
        /// <param name="item6">The item6.</param>
        /// <param name="item7">The item7.</param>
        /// <param name="item8">The item8.</param>
        /// <returns></returns>
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8) { return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8>(item8)); }
    }

    #region Tuple<T1>
    /// <summary>
    /// Tuple
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    [Serializable]
#if !COREINTERNAL
    public
#endif
 class Tuple<T1> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        private readonly T1 _Item1;

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T1&gt;"/> class.
        /// </summary>
        /// <param name="item1">The item1.</param>
        public Tuple(T1 item1) { _Item1 = item1; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj) { return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default); }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() { return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default); }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null)
                return 1;
            var tuple = (other as Tuple<T1>);
            if (tuple == null)
                throw new ArgumentException(EnvironmentEx.GetResourceString("ArgumentException_TupleIncorrectType", new object[] { base.GetType().ToString() }), "other");
            return comparer.Compare(this._Item1, tuple._Item1);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
                return false;
            var tuple = (other as Tuple<T1>);
            if (tuple == null)
                return false;
            return comparer.Equals(_Item1, tuple._Item1);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) { return comparer.GetHashCode(_Item1); }

        int IComparable.CompareTo(object obj) { return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default); }

        int ITuple.GetHashCode(IEqualityComparer comparer) { return ((IStructuralEquatable)this).GetHashCode(comparer); }

        string ITuple.ToString(StringBuilder sb)
        {
            sb.Append(this._Item1);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            return ((ITuple)this).ToString(sb);
        }

        /// <summary>
        /// Gets the item1.
        /// </summary>
        public T1 Item1
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item1; }
        }

        int ITuple.Size
        {
            get { return 1; }
        }
    }
    #endregion

    #region Tuple<T1, T2>
    /// <summary>
    /// Tuple
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    [Serializable]
#if !COREINTERNAL
    public
#endif
 class Tuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        private readonly T1 _Item1;
        private readonly T2 _Item2;

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T1, T2&gt;"/> class.
        /// </summary>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        public Tuple(T1 item1, T2 item2)
        {
            _Item1 = item1;
            _Item2 = item2;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null)
                return 1;
            var tuple = (other as Tuple<T1, T2>);
            if (tuple == null)
                throw new ArgumentException(EnvironmentEx.GetResourceString("ArgumentException_TupleIncorrectType", new object[] { base.GetType().ToString() }), "other");
            int num = 0;
            num = comparer.Compare(_Item1, tuple._Item1);
            if (num != 0)
                return num;
            return comparer.Compare(_Item2, tuple._Item2);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
                return false;
            var tuple = (other as Tuple<T1, T2>);
            if (tuple == null)
                return false;
            return (comparer.Equals(_Item1, tuple._Item1) && comparer.Equals(_Item2, tuple._Item2));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) { return Tuple.CombineHashCodes(comparer.GetHashCode(_Item1), comparer.GetHashCode(_Item2)); }

        int IComparable.CompareTo(object obj) { return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default); }

        int ITuple.GetHashCode(IEqualityComparer comparer) { return ((IStructuralEquatable)this).GetHashCode(comparer); }

        string ITuple.ToString(StringBuilder sb)
        {
            sb.Append(_Item1);
            sb.Append(", ");
            sb.Append(_Item2);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            return ((ITuple)this).ToString(sb);
        }

        /// <summary>
        /// Gets the item1.
        /// </summary>
        public T1 Item1
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item1; }
        }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        public T2 Item2
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item2; }
        }

        int ITuple.Size
        {
            get { return 2; }
        }
    }
    #endregion

    #region Tuple<T1, T2, T3>
    /// <summary>
    /// Tuple
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    [Serializable]
#if !COREINTERNAL
    public
#endif
 class Tuple<T1, T2, T3> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        private readonly T1 _Item1;
        private readonly T2 _Item2;
        private readonly T3 _Item3;

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T1, T2, T3&gt;"/> class.
        /// </summary>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            _Item1 = item1;
            _Item2 = item2;
            _Item3 = item3;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj) { return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default); }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() { return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default); }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null)
                return 1;
            var tuple = (other as Tuple<T1, T2, T3>);
            if (tuple == null)
                throw new ArgumentException(EnvironmentEx.GetResourceString("ArgumentException_TupleIncorrectType", new object[] { base.GetType().ToString() }), "other");
            int num = 0;
            num = comparer.Compare(_Item1, tuple._Item1);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item2, tuple._Item2);
            if (num != 0)
                return num;
            return comparer.Compare(_Item3, tuple._Item3);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
                return false;
            var tuple = (other as Tuple<T1, T2, T3>);
            if (tuple == null)
                return false;
            return ((comparer.Equals(_Item1, tuple._Item1) && comparer.Equals(_Item2, tuple._Item2)) && comparer.Equals(_Item3, tuple._Item3));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) { return Tuple.CombineHashCodes(comparer.GetHashCode(_Item1), comparer.GetHashCode(_Item2), comparer.GetHashCode(_Item3)); }

        int IComparable.CompareTo(object obj) { return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default); }

        int ITuple.GetHashCode(IEqualityComparer comparer) { return ((IStructuralEquatable)this).GetHashCode(comparer); }

        string ITuple.ToString(StringBuilder sb)
        {
            sb.Append(_Item1);
            sb.Append(", ");
            sb.Append(_Item2);
            sb.Append(", ");
            sb.Append(_Item3);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            return ((ITuple)this).ToString(sb);
        }

        /// <summary>
        /// Gets the item1.
        /// </summary>
        public T1 Item1
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item1; }
        }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        public T2 Item2
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item2; }
        }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        public T3 Item3
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item3; }
        }

        int ITuple.Size
        {
            get { return 3; }
        }
    }
    #endregion

    #region Tuple<T1, T2, T3, T4>
    /// <summary>
    /// Tuple
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    [Serializable]
#if !COREINTERNAL
    public
#endif
 class Tuple<T1, T2, T3, T4> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        private readonly T1 _Item1;
        private readonly T2 _Item2;
        private readonly T3 _Item3;
        private readonly T4 _Item4;

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T1, T2, T3, T4&gt;"/> class.
        /// </summary>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <param name="item4">The item4.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            _Item1 = item1;
            _Item2 = item2;
            _Item3 = item3;
            _Item4 = item4;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj) { return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default); }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() { return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default); }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null)
                return 1;
            var tuple = (other as Tuple<T1, T2, T3, T4>);
            if (tuple == null)
                throw new ArgumentException(EnvironmentEx.GetResourceString("ArgumentException_TupleIncorrectType", new object[] { base.GetType().ToString() }), "other");
            int num = 0;
            num = comparer.Compare(_Item1, tuple._Item1);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item2, tuple._Item2);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item3, tuple._Item3);
            if (num != 0)
                return num;
            return comparer.Compare(_Item4, tuple._Item4);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
                return false;
            var tuple = (other as Tuple<T1, T2, T3, T4>);
            if (tuple == null)
                return false;
            return (((comparer.Equals(_Item1, tuple._Item1) && comparer.Equals(_Item2, tuple._Item2)) && comparer.Equals(_Item3, tuple._Item3)) && comparer.Equals(_Item4, tuple._Item4));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) { return Tuple.CombineHashCodes(comparer.GetHashCode(_Item1), comparer.GetHashCode(_Item2), comparer.GetHashCode(_Item3), comparer.GetHashCode(_Item4)); }

        int IComparable.CompareTo(object obj) { return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default); }

        int ITuple.GetHashCode(IEqualityComparer comparer) { return ((IStructuralEquatable)this).GetHashCode(comparer); }

        string ITuple.ToString(StringBuilder sb)
        {
            sb.Append(_Item1);
            sb.Append(", ");
            sb.Append(_Item2);
            sb.Append(", ");
            sb.Append(_Item3);
            sb.Append(", ");
            sb.Append(_Item4);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            return ((ITuple)this).ToString(sb);
        }

        /// <summary>
        /// Gets the item1.
        /// </summary>
        public T1 Item1
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item1; }
        }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        public T2 Item2
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item2; }
        }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        public T3 Item3
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item3; }
        }

        /// <summary>
        /// Gets the item4.
        /// </summary>
        public T4 Item4
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item4; }
        }

        int ITuple.Size
        {
            get { return 4; }
        }
    }
    #endregion

    #region Tuple<T1, T2, T3, T4, T5>
    /// <summary>
    /// Tuple
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    [Serializable]
#if !COREINTERNAL
    public
#endif
 class Tuple<T1, T2, T3, T4, T5> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        private readonly T1 _Item1;
        private readonly T2 _Item2;
        private readonly T3 _Item3;
        private readonly T4 _Item4;
        private readonly T5 _Item5;

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T1, T2, T3, T4, T5&gt;"/> class.
        /// </summary>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <param name="item4">The item4.</param>
        /// <param name="item5">The item5.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            _Item1 = item1;
            _Item2 = item2;
            _Item3 = item3;
            _Item4 = item4;
            _Item5 = item5;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj) { return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default); }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() { return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default); }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null)
                return 1;
            var tuple = (other as Tuple<T1, T2, T3, T4, T5>);
            if (tuple == null)
                throw new ArgumentException(EnvironmentEx.GetResourceString("ArgumentException_TupleIncorrectType", new object[] { base.GetType().ToString() }), "other");
            int num = 0;
            num = comparer.Compare(_Item1, tuple._Item1);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item2, tuple._Item2);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item3, tuple._Item3);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item4, tuple._Item4);
            if (num != 0)
                return num;
            return comparer.Compare(_Item5, tuple._Item5);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
                return false;
            var tuple = (other as Tuple<T1, T2, T3, T4, T5>);
            if (tuple == null)
                return false;
            return (((comparer.Equals(_Item1, tuple._Item1) && comparer.Equals(_Item2, tuple._Item2)) && (comparer.Equals(_Item3, tuple._Item3) && comparer.Equals(_Item4, tuple._Item4))) && comparer.Equals(_Item5, tuple._Item5));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) { return Tuple.CombineHashCodes(comparer.GetHashCode(_Item1), comparer.GetHashCode(_Item2), comparer.GetHashCode(_Item3), comparer.GetHashCode(_Item4), comparer.GetHashCode(_Item5)); }

        int IComparable.CompareTo(object obj) { return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default); }

        int ITuple.GetHashCode(IEqualityComparer comparer) { return ((IStructuralEquatable)this).GetHashCode(comparer); }

        string ITuple.ToString(StringBuilder sb)
        {
            sb.Append(_Item1);
            sb.Append(", ");
            sb.Append(_Item2);
            sb.Append(", ");
            sb.Append(_Item3);
            sb.Append(", ");
            sb.Append(_Item4);
            sb.Append(", ");
            sb.Append(_Item5);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            return ((ITuple)this).ToString(sb);
        }

        /// <summary>
        /// Gets the item1.
        /// </summary>
        public T1 Item1
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item1; }
        }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        public T2 Item2
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item2; }
        }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        public T3 Item3
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item3; }
        }

        /// <summary>
        /// Gets the item4.
        /// </summary>
        public T4 Item4
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item4; }
        }

        /// <summary>
        /// Gets the item5.
        /// </summary>
        public T5 Item5
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item5; }
        }

        int ITuple.Size
        {
            get { return 5; }
        }
    }
    #endregion

    #region Tuple<T1, T2, T3, T4, T5, T6>
    /// <summary>
    /// Tuple
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    [Serializable]
#if !COREINTERNAL
    public
#endif
 class Tuple<T1, T2, T3, T4, T5, T6> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        private readonly T1 _Item1;
        private readonly T2 _Item2;
        private readonly T3 _Item3;
        private readonly T4 _Item4;
        private readonly T5 _Item5;
        private readonly T6 _Item6;

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T1, T2, T3, T4, T5, T6&gt;"/> class.
        /// </summary>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <param name="item4">The item4.</param>
        /// <param name="item5">The item5.</param>
        /// <param name="item6">The item6.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            _Item1 = item1;
            _Item2 = item2;
            _Item3 = item3;
            _Item4 = item4;
            _Item5 = item5;
            _Item6 = item6;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj) { return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default); }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() { return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default); }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null)
                return 1;
            var tuple = (other as Tuple<T1, T2, T3, T4, T5, T6>);
            if (tuple == null)
                throw new ArgumentException(EnvironmentEx.GetResourceString("ArgumentException_TupleIncorrectType", new object[] { base.GetType().ToString() }), "other");
            int num = 0;
            num = comparer.Compare(_Item1, tuple._Item1);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item2, tuple._Item2);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item3, tuple._Item3);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item4, tuple._Item4);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item5, tuple._Item5);
            if (num != 0)
                return num;
            return comparer.Compare(_Item6, tuple._Item6);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
                return false;
            var tuple = (other as Tuple<T1, T2, T3, T4, T5, T6>);
            if (tuple == null)
                return false;
            return ((((comparer.Equals(_Item1, tuple._Item1) && comparer.Equals(_Item2, tuple._Item2)) && (comparer.Equals(_Item3, tuple._Item3) && comparer.Equals(_Item4, tuple._Item4))) && comparer.Equals(_Item5, tuple._Item5)) && comparer.Equals(_Item6, tuple._Item6));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) { return Tuple.CombineHashCodes(comparer.GetHashCode(_Item1), comparer.GetHashCode(_Item2), comparer.GetHashCode(_Item3), comparer.GetHashCode(_Item4), comparer.GetHashCode(_Item5), comparer.GetHashCode(_Item6)); }

        int IComparable.CompareTo(object obj) { return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default); }

        int ITuple.GetHashCode(IEqualityComparer comparer) { return ((IStructuralEquatable)this).GetHashCode(comparer); }

        string ITuple.ToString(StringBuilder sb)
        {
            sb.Append(_Item1);
            sb.Append(", ");
            sb.Append(_Item2);
            sb.Append(", ");
            sb.Append(_Item3);
            sb.Append(", ");
            sb.Append(_Item4);
            sb.Append(", ");
            sb.Append(_Item5);
            sb.Append(", ");
            sb.Append(_Item6);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            return ((ITuple)this).ToString(sb);
        }

        /// <summary>
        /// Gets the item1.
        /// </summary>
        public T1 Item1
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item1; }
        }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        public T2 Item2
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item2; }
        }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        public T3 Item3
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item3; }
        }

        /// <summary>
        /// Gets the item4.
        /// </summary>
        public T4 Item4
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item4; }
        }

        /// <summary>
        /// Gets the item5.
        /// </summary>
        public T5 Item5
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item5; }
        }

        /// <summary>
        /// Gets the item6.
        /// </summary>
        public T6 Item6
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item6; }
        }

        int ITuple.Size
        {
            get { return 6; }
        }
    }
    #endregion

    #region Tuple<T1, T2, T3, T4, T5, T6, T7>
    /// <summary>
    /// Tuple
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    /// <typeparam name="T7">The type of the 7.</typeparam>
    [Serializable]
#if !COREINTERNAL
    public
#endif
 class Tuple<T1, T2, T3, T4, T5, T6, T7> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        private readonly T1 _Item1;
        private readonly T2 _Item2;
        private readonly T3 _Item3;
        private readonly T4 _Item4;
        private readonly T5 _Item5;
        private readonly T6 _Item6;
        private readonly T7 _Item7;

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T1, T2, T3, T4, T5, T6, T7&gt;"/> class.
        /// </summary>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <param name="item4">The item4.</param>
        /// <param name="item5">The item5.</param>
        /// <param name="item6">The item6.</param>
        /// <param name="item7">The item7.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            _Item1 = item1;
            _Item2 = item2;
            _Item3 = item3;
            _Item4 = item4;
            _Item5 = item5;
            _Item6 = item6;
            _Item7 = item7;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj) { return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default); }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() { return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default); }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null)
                return 1;
            var tuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7>;
            if (tuple == null)
                throw new ArgumentException(EnvironmentEx.GetResourceString("ArgumentException_TupleIncorrectType", new object[] { base.GetType().ToString() }), "other");
            int num = 0;
            num = comparer.Compare(_Item1, tuple._Item1);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item2, tuple._Item2);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item3, tuple._Item3);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item4, tuple._Item4);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item5, tuple._Item5);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item6, tuple._Item6);
            if (num != 0)
                return num;
            return comparer.Compare(_Item7, tuple._Item7);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
                return false;
            var tuple = (other as Tuple<T1, T2, T3, T4, T5, T6, T7>);
            if (tuple == null)
                return false;
            return ((((comparer.Equals(_Item1, tuple._Item1) && comparer.Equals(_Item2, tuple._Item2)) && (comparer.Equals(_Item3, tuple._Item3) && comparer.Equals(_Item4, tuple._Item4))) && (comparer.Equals(_Item5, tuple._Item5) && comparer.Equals(_Item6, tuple._Item6))) && comparer.Equals(_Item7, tuple._Item7));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) { return Tuple.CombineHashCodes(comparer.GetHashCode(_Item1), comparer.GetHashCode(_Item2), comparer.GetHashCode(_Item3), comparer.GetHashCode(_Item4), comparer.GetHashCode(_Item5), comparer.GetHashCode(_Item6), comparer.GetHashCode(_Item7)); }

        int IComparable.CompareTo(object obj) { return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default); }

        int ITuple.GetHashCode(IEqualityComparer comparer) { return ((IStructuralEquatable)this).GetHashCode(comparer); }

        string ITuple.ToString(StringBuilder sb)
        {
            sb.Append(_Item1);
            sb.Append(", ");
            sb.Append(_Item2);
            sb.Append(", ");
            sb.Append(_Item3);
            sb.Append(", ");
            sb.Append(_Item4);
            sb.Append(", ");
            sb.Append(_Item5);
            sb.Append(", ");
            sb.Append(_Item6);
            sb.Append(", ");
            sb.Append(_Item7);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            return ((ITuple)this).ToString(sb);
        }

        /// <summary>
        /// Gets the item1.
        /// </summary>
        public T1 Item1
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item1; }
        }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        public T2 Item2
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item2; }
        }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        public T3 Item3
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item3; }
        }

        /// <summary>
        /// Gets the item4.
        /// </summary>
        public T4 Item4
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item4; }
        }

        /// <summary>
        /// Gets the item5.
        /// </summary>
        public T5 Item5
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item5; }
        }

        /// <summary>
        /// Gets the item6.
        /// </summary>
        public T6 Item6
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item6; }
        }

        /// <summary>
        /// Gets the item7.
        /// </summary>
        public T7 Item7
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item7; }
        }

        int ITuple.Size
        {
            get { return 7; }
        }
    }
    #endregion

    #region Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>
    /// <summary>
    /// Tuple
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    /// <typeparam name="T7">The type of the 7.</typeparam>
    /// <typeparam name="TRest">The type of the rest.</typeparam>
    [Serializable]
#if !COREINTERNAL
    public
#endif
 class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        private readonly T1 _Item1;
        private readonly T2 _Item2;
        private readonly T3 _Item3;
        private readonly T4 _Item4;
        private readonly T5 _Item5;
        private readonly T6 _Item6;
        private readonly T7 _Item7;
        private readonly TRest _Rest;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T1, T2, T3, T4, T5, T6, T7, TRest&gt;"/> class.
        /// </summary>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="item3">The item3.</param>
        /// <param name="item4">The item4.</param>
        /// <param name="item5">The item5.</param>
        /// <param name="item6">The item6.</param>
        /// <param name="item7">The item7.</param>
        /// <param name="rest">The rest.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
        {
            if (!(rest is ITuple))
                throw new ArgumentException(EnvironmentEx.GetResourceString("ArgumentException_TupleLastArgumentNotATuple"));
            _Item1 = item1;
            _Item2 = item2;
            _Item3 = item3;
            _Item4 = item4;
            _Item5 = item5;
            _Item6 = item6;
            _Item7 = item7;
            _Rest = rest;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj) { return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default); }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() { return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default); }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null)
                return 1;
            var tuple = (other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>);
            if (tuple == null)
                throw new ArgumentException(EnvironmentEx.GetResourceString("ArgumentException_TupleIncorrectType", new object[] { base.GetType().ToString() }), "other");
            int num = 0;
            num = comparer.Compare(_Item1, tuple._Item1);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item2, tuple._Item2);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item3, tuple._Item3);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item4, tuple._Item4);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item5, tuple._Item5);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item6, tuple._Item6);
            if (num != 0)
                return num;
            num = comparer.Compare(_Item7, tuple._Item7);
            if (num != 0)
                return num;
            return comparer.Compare(_Rest, tuple._Rest);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
                return false;
            var tuple = (other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>);
            if (tuple == null)
                return false;
            return ((((comparer.Equals(_Item1, tuple._Item1) && comparer.Equals(_Item2, tuple._Item2)) && (comparer.Equals(_Item3, tuple._Item3) && comparer.Equals(_Item4, tuple._Item4))) && ((comparer.Equals(_Item5, tuple._Item5) && comparer.Equals(_Item6, tuple._Item6)) && comparer.Equals(_Item7, tuple._Item7))) && comparer.Equals(_Rest, tuple._Rest));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            var rest = (ITuple)_Rest;
            if (rest.Size >= 8)
                return rest.GetHashCode(comparer);
            switch ((8 - rest.Size))
            {
                case 1:
                    return Tuple.CombineHashCodes(comparer.GetHashCode(_Item7), rest.GetHashCode(comparer));
                case 2:
                    return Tuple.CombineHashCodes(comparer.GetHashCode(_Item6), comparer.GetHashCode(_Item7), rest.GetHashCode(comparer));
                case 3:
                    return Tuple.CombineHashCodes(comparer.GetHashCode(_Item5), comparer.GetHashCode(_Item6), comparer.GetHashCode(_Item7), rest.GetHashCode(comparer));
                case 4:
                    return Tuple.CombineHashCodes(comparer.GetHashCode(_Item4), comparer.GetHashCode(_Item5), comparer.GetHashCode(_Item6), comparer.GetHashCode(_Item7), rest.GetHashCode(comparer));
                case 5:
                    return Tuple.CombineHashCodes(comparer.GetHashCode(_Item3), comparer.GetHashCode(_Item4), comparer.GetHashCode(_Item5), comparer.GetHashCode(_Item6), comparer.GetHashCode(_Item7), rest.GetHashCode(comparer));
                case 6:
                    return Tuple.CombineHashCodes(comparer.GetHashCode(_Item2), comparer.GetHashCode(_Item3), comparer.GetHashCode(_Item4), comparer.GetHashCode(_Item5), comparer.GetHashCode(_Item6), comparer.GetHashCode(_Item7), rest.GetHashCode(comparer));
                case 7:
                    return Tuple.CombineHashCodes(comparer.GetHashCode(_Item1), comparer.GetHashCode(_Item2), comparer.GetHashCode(_Item3), comparer.GetHashCode(_Item4), comparer.GetHashCode(_Item5), comparer.GetHashCode(_Item6), comparer.GetHashCode(_Item7), rest.GetHashCode(comparer));
            }
            return -1;
        }

        int IComparable.CompareTo(object obj) { return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default); }

        int ITuple.GetHashCode(IEqualityComparer comparer) { return ((IStructuralEquatable)this).GetHashCode(comparer); }

        string ITuple.ToString(StringBuilder sb)
        {
            sb.Append(_Item1);
            sb.Append(", ");
            sb.Append(_Item2);
            sb.Append(", ");
            sb.Append(_Item3);
            sb.Append(", ");
            sb.Append(_Item4);
            sb.Append(", ");
            sb.Append(_Item5);
            sb.Append(", ");
            sb.Append(_Item6);
            sb.Append(", ");
            sb.Append(_Item7);
            sb.Append(", ");
            return ((ITuple)_Rest).ToString(sb);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            return ((ITuple)this).ToString(sb);
        }

        /// <summary>
        /// Gets the item1.
        /// </summary>
        public T1 Item1
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item1; }
        }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        public T2 Item2
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item2; }
        }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        public T3 Item3
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item3; }
        }

        /// <summary>
        /// Gets the item4.
        /// </summary>
        public T4 Item4
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item4; }
        }

        /// <summary>
        /// Gets the item5.
        /// </summary>
        public T5 Item5
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item5; }
        }

        /// <summary>
        /// Gets the item6.
        /// </summary>
        public T6 Item6
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item6; }
        }

        /// <summary>
        /// Gets the item7.
        /// </summary>
        public T7 Item7
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Item7; }
        }

        /// <summary>
        /// Gets the rest.
        /// </summary>
        public TRest Rest
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _Rest; }
        }

        int ITuple.Size
        {
            get { return (7 + ((ITuple)_Rest).Size); }
        }
    }
    #endregion
}
#endif