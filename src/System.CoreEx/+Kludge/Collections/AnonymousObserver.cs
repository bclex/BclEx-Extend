#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
namespace System.Collections
{
    /// <summary>
    /// AnonymousObserver
    /// </summary>
	internal class AnonymousObserver<T> : AbstractObserver<T>
	{
		private Action _onCompleted;
		private Action<Exception> _onError;
		private Action<T> _onNext;

		public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
		{
			_onNext = onNext;
			_onError = onError;
			_onCompleted = onCompleted;
		}

		protected override void Completed()
		{
			_onCompleted();
		}

		protected override void Error(Exception exception)
		{
			_onError(exception);
		}

		protected override void Next(T value)
		{
			_onNext(value);
		}
	}
}
#endif