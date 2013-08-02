#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Runtime;
using System.Runtime.InteropServices;
namespace System.Threading
{
    internal class SparselyPopulatedArray<T>
        where T : class
    {
        private readonly SparselyPopulatedArrayFragment<T> m_head;
        private volatile SparselyPopulatedArrayFragment<T> m_tail;

        internal SparselyPopulatedArray(int initialSize)
        {
            m_head = m_tail = new SparselyPopulatedArrayFragment<T>(initialSize);
        }

        internal SparselyPopulatedArrayAddInfo<T> Add(T element)
        {
            while (true)
            {
                var tail = m_tail;
                while (tail.m_next != null)
                    m_tail = tail = tail.m_next;
                for (SparselyPopulatedArrayFragment<T> fragment2 = tail; fragment2 != null; fragment2 = fragment2.m_prev)
                {
                    if (fragment2.m_freeCount < 1)
                        fragment2.m_freeCount--;
                    if (fragment2.m_freeCount > 0 || fragment2.m_freeCount < -10)
                    {
                        var length = fragment2.Length;
                        var num2 = (length - fragment2.m_freeCount) % length;
                        if (num2 < 0)
                        {
                            num2 = 0;
                            fragment2.m_freeCount--;
                        }
                        for (int i = 0; i < length; i++)
                        {
                            var index = (num2 + i) % length;
                            if (fragment2.m_elements[index] == null)
                            {
                                var comparand = default(T);
                                if (Interlocked.CompareExchange<T>(ref fragment2.m_elements[index], element, comparand) == null)
                                {
                                    var num5 = fragment2.m_freeCount - 1;
                                    fragment2.m_freeCount = (num5 > 0 ? num5 : 0);
                                    return new SparselyPopulatedArrayAddInfo<T>(fragment2, index);
                                }
                            }
                        }
                    }
                }
                var fragment3 = new SparselyPopulatedArrayFragment<T>(tail.m_elements.Length == 0x1000 ? 0x1000 : (tail.m_elements.Length * 2), tail);
                if (Interlocked.CompareExchange<SparselyPopulatedArrayFragment<T>>(ref tail.m_next, fragment3, null) == null)
                    m_tail = fragment3;
            }
        }

        internal SparselyPopulatedArrayFragment<T> Tail
        {
            get { return m_tail; }
        }
    }
}
#endif