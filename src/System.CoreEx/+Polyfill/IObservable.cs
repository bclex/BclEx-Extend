#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
namespace System
{
    /// <summary>
    /// IObservable
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if !COREINTERNAL
    public
#endif
 interface IObservable<T>
    {
        /// <summary>
        /// Subscribes the specified observer.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns></returns>
        IDisposable Subscribe(IObserver<T> observer);
    }
}
#endif