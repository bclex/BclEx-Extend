#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
namespace System.Collections
{
    /// <summary>
    /// AbstractObserver
    /// </summary>
	internal abstract class AbstractObserver<T> : IObserver<T>
	{
		public AbstractObserver()
		{
			IsStopped = false;
		}

		protected abstract void Completed();

		protected abstract void Error(Exception exception);

		protected abstract void Next(T value);

		public void OnCompleted()
		{
			if (!IsStopped)
			{
				IsStopped = true;
				Completed();
			}
		}

		public void OnError(Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException("exception");
			if (!IsStopped)
			{
				IsStopped = true;
				Error(exception);
			}
		}

		public void OnNext(T value)
		{
			if (!IsStopped)
				Next(value);
		}

		protected void Stop()
		{
			IsStopped = true;
		}

		protected bool IsStopped { get; set; }
	}
}
#endif