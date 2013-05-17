#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
namespace System
{
    /// <summary>
    /// IObserver
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if !COREINTERNAL
    public
#endif
 interface IObserver<T>
    {
        /// <summary>
        /// Called when [completed].
        /// </summary>
        void OnCompleted();
        /// <summary>
        /// Called when [error].
        /// </summary>
        /// <param name="exception">The exception.</param>
        void OnError(Exception exception);
        /// <summary>
        /// Called when [next].
        /// </summary>
        /// <param name="value">The value.</param>
        void OnNext(T value);
    }
}
#endif