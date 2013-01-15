#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Runtime;
namespace System.Threading
{
    internal class SparselyPopulatedArrayFragment<T>
        where T : class
    {
        internal readonly T[] m_elements;
        internal volatile int m_freeCount;
        internal volatile SparselyPopulatedArrayFragment<T> m_next;
        internal volatile SparselyPopulatedArrayFragment<T> m_prev;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal SparselyPopulatedArrayFragment(int size)
            : this(size, null) { }

        internal SparselyPopulatedArrayFragment(int size, SparselyPopulatedArrayFragment<T> prev)
        {
            m_elements = new T[size];
            m_freeCount = size;
            m_prev = prev;
        }

        internal T SafeAtomicRemove(int index, T expectedElement)
        {
            var local = Interlocked.CompareExchange<T>(ref m_elements[index], default(T), expectedElement);
            if (local != null)
                m_freeCount++;
            return local;
        }

        internal T this[int index]
        {
            get { return Volatile.Read<T>(ref m_elements[index]); }
        }

        internal int Length
        {
            get { return m_elements.Length; }
        }

        internal SparselyPopulatedArrayFragment<T> Prev
        {
            get { return m_prev; }
        }
    }
}
#endif