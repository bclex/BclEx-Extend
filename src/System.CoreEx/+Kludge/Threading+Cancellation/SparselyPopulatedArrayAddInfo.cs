#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Runtime;
using System.Runtime.InteropServices;
namespace System.Threading
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SparselyPopulatedArrayAddInfo<T>
        where T : class
    {
        private SparselyPopulatedArrayFragment<T> m_source;
        private int m_index;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal SparselyPopulatedArrayAddInfo(SparselyPopulatedArrayFragment<T> source, int index)
        {
            m_source = source;
            m_index = index;
        }

        internal SparselyPopulatedArrayFragment<T> Source
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return m_source; }
        }
        internal int Index
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return m_index; }
        }
    }
}
#endif