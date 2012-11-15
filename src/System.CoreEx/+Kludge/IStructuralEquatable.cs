#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Collections;
namespace System
{
#if !COREINTERNAL
    /// <summary>
    /// IStructuralEquatable
    /// </summary>
    public
#endif
 interface IStructuralEquatable
    {
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        bool Equals(object other, IEqualityComparer comparer);
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        int GetHashCode(IEqualityComparer comparer);
    }
}
#endif