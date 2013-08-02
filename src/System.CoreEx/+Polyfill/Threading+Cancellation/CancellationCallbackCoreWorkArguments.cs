#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Runtime;
using System.Runtime.InteropServices;
namespace System.Threading
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CancellationCallbackCoreWorkArguments
    {
        internal SparselyPopulatedArrayFragment<CancellationCallbackInfo> _currArrayFragment;
        internal int _currArrayIndex;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public CancellationCallbackCoreWorkArguments(SparselyPopulatedArrayFragment<CancellationCallbackInfo> currArrayFragment, int currArrayIndex)
        {
            _currArrayFragment = currArrayFragment;
            _currArrayIndex = currArrayIndex;
        }
    }
}
#endif