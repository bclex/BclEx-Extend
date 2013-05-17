#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Security;
namespace System.IO
{
    /// <summary>
    /// SearchResultHandler
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    internal abstract class SearchResultHandler<TSource>
    {
        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected SearchResultHandler() { }

        [SecurityCritical]
        internal abstract TSource CreateObject(SearchResult result);
        [SecurityCritical]
        internal abstract bool IsResultIncluded(SearchResult result);
    }
}
#endif