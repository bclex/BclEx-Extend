#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Collections;
namespace System
{
    /// <summary>
    /// ObservableExtensions
    /// </summary>
#if !COREINTERNAL
    public
#endif
 static class ObservableExtensions
    {
        /// <summary>
        /// Subscribes the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TSource>(this IObservable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return source.Subscribe<TSource>(delegate(TSource _) { }, delegate(Exception exception) { throw exception.PrepareForRethrow(); }, delegate { });
        }

        /// <summary>
        /// Subscribes the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TSource>(this IObservable<TSource> source, Action<TSource> onNext)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (onNext == null)
                throw new ArgumentNullException("onNext");
            return source.Subscribe<TSource>(onNext, delegate(Exception exception) { throw exception.PrepareForRethrow(); }, delegate { });
        }

        /// <summary>
        /// Subscribes the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <param name="onError">The on error.</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TSource>(this IObservable<TSource> source, Action<TSource> onNext, Action<Exception> onError)
        {
            if (onNext == null)
                throw new ArgumentNullException("onNext");
            if (onError == null)
                throw new ArgumentNullException("onError");
            return source.Subscribe<TSource>(onNext, onError, delegate { });
        }

        /// <summary>
        /// Subscribes the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <param name="onCompleted">The on completed.</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TSource>(this IObservable<TSource> source, Action<TSource> onNext, Action onCompleted)
        {
            if (onNext == null)
                throw new ArgumentNullException("onNext");
            if (onCompleted == null)
                throw new ArgumentNullException("onCompleted");
            return source.Subscribe<TSource>(onNext, delegate(Exception exception) { throw exception.PrepareForRethrow(); }, onCompleted);
        }

        /// <summary>
        /// Subscribes the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <param name="onError">The on error.</param>
        /// <param name="onCompleted">The on completed.</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TSource>(this IObservable<TSource> source, Action<TSource> onNext, Action<Exception> onError, Action onCompleted)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (onNext == null)
                throw new ArgumentNullException("onNext");
            if (onError == null)
                throw new ArgumentNullException("onError");
            if (onCompleted == null)
                throw new ArgumentNullException("onCompleted");
            return source.Subscribe(new AnonymousObserver<TSource>(onNext, onError, onCompleted));
        }
    }
}
#endif