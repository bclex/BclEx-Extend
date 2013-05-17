#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Collections;
namespace System
{
#if !COREINTERNAL
    /// <summary>
    /// IStructuralComparable
    /// </summary>
    public
#endif
 interface IStructuralComparable
    {
        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        int CompareTo(object other, IComparer comparer);
    }
}
#endif