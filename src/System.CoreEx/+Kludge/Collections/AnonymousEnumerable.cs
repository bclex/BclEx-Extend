#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Collections.Generic;
namespace System.Collections
{
    /// <summary>
    /// AnonymousEnumerable
    /// </summary>
	internal class AnonymousEnumerable<T> : IEnumerable<T>, IEnumerable
	{
		private Func<IEnumerator<T>> _getEnumerator;

		public AnonymousEnumerable(Func<IEnumerator<T>> getEnumerator)
		{
			_getEnumerator = getEnumerator;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _getEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}
}
#endif