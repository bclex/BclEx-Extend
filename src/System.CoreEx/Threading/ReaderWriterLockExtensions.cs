#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System.Collections.Generic;
namespace System.Threading
{
	/// <summary>
	/// ReaderWriterLockExtensions
	/// </summary>
	public static class ReaderWriterLockExtensions
	{
#if !CLRSQL
		#region THREADED
		/// <summary>
		/// Threadeds the atomic method.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TList">The type of the list.</typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="list">The list.</param>
		/// <param name="key">The key.</param>
		/// <param name="method">The method.</param>
		public static void ThreadedAtomicMethod<TKey, TList>(this ReaderWriterLockSlim rwLock, TList list, TKey key, Action method)
			where TList : IList<TKey>
		{
			// blocking calls
			throw new NotImplementedException();
		}
		/// <summary>
		/// Threadeds the atomic method.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TList">The type of the list.</typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="list">The list.</param>
		/// <param name="key">The key.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public static TValue ThreadedAtomicMethod<TValue, TKey, TList>(this ReaderWriterLockSlim rwLock, TList list, TKey key, Func<TValue> method)
			where TList : IList<TKey>
		{
			// blocking calls
			throw new NotImplementedException();
		}

		/// <summary>
		/// Threadeds the atomic method async.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TList">The type of the list.</typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="list">The list.</param>
		/// <param name="key">The key.</param>
		/// <param name="method">The method.</param>
		public static void ThreadedAtomicMethodAsync<TKey, TList>(this ReaderWriterLockSlim rwLock, TList list, TKey key, Action method)
			where TList : IList<TKey>
		{
			rwLock.EnterUpgradeableReadLock();
			try
			{
				if (!list.Contains(key))
				{
					// set to running
					rwLock.EnterWriteLock();
					try
					{
						if (list.Contains(key))
							return;
						list.Add(key);
					}
					finally { rwLock.ExitWriteLock(); }
					// run queue
					try { method(); }
					catch
					{
						list.Remove(key);
						throw;
					}
					// set to idle
					rwLock.EnterWriteLock();
					try
					{
						//if (!list.Contains(key))
						//    return default(TValue);
						list.Remove(key);
					}
					finally { rwLock.ExitWriteLock(); }
					return;
				}
			}
			finally { rwLock.ExitUpgradeableReadLock(); }
			return;
		}

		/// <summary>
		/// Threadeds the atomic method async.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TList">The type of the list.</typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="list">The list.</param>
		/// <param name="key">The key.</param>
		/// <param name="method">The method.</param>
		/// <param name="contention">The contention.</param>
		public static void ThreadedAtomicMethodAsync<TKey, TList>(this ReaderWriterLockSlim rwLock, TList list, TKey key, Action method, Action contention)
			where TList : IList<TKey>
		{
			rwLock.EnterUpgradeableReadLock();
			try
			{
				if (!list.Contains(key))
				{
					// set to running
					rwLock.EnterWriteLock();
					try
					{
						if (list.Contains(key))
						{
							contention();
							return;
						}
						list.Add(key);
					}
					finally { rwLock.ExitWriteLock(); }
					// run queue
					try { method(); }
					catch
					{
						list.Remove(key);
						throw;
					}
					// set to idle
					rwLock.EnterWriteLock();
					try
					{
						//if (!list.Contains(key))
						//    return default(TValue);
						list.Remove(key);
					}
					finally { rwLock.ExitWriteLock(); }
					return;
				}
			}
			finally { rwLock.ExitUpgradeableReadLock(); }
			contention();
		}
		/// <summary>
		/// Threadeds the atomic method.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TList">The type of the list.</typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="list">The list.</param>
		/// <param name="key">The key.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public static TValue ThreadedAtomicMethodAsync<TValue, TKey, TList>(this ReaderWriterLockSlim rwLock, TList list, TKey key, Func<TValue> method)
			where TList : IList<TKey>
		{
			rwLock.EnterUpgradeableReadLock();
			try
			{
				if (!list.Contains(key))
				{
					// set to running
					rwLock.EnterWriteLock();
					try
					{
						if (list.Contains(key))
							return default(TValue);
						list.Add(key);
					}
					finally { rwLock.ExitWriteLock(); }
					// run queue
					TValue value;
					try { value = method(); }
					catch
					{
						list.Remove(key);
						throw;
					}
					// set to idle
					rwLock.EnterWriteLock();
					try
					{
						//if (!list.Contains(key))
						//    return default(TValue);
						list.Remove(key);
					}
					finally { rwLock.ExitWriteLock(); }
					return value;
				}
			}
			finally { rwLock.ExitUpgradeableReadLock(); }
			return default(TValue);
		}
		/// <summary>
		/// Threadeds the atomic method async.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TList">The type of the list.</typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="list">The list.</param>
		/// <param name="key">The key.</param>
		/// <param name="method">The method.</param>
		/// <param name="contention">The contention.</param>
		/// <returns></returns>
		public static TValue ThreadedAtomicMethodAsync<TValue, TKey, TList>(this ReaderWriterLockSlim rwLock, TList list, TKey key, Func<TValue> method, Func<TValue> contention)
			where TList : IList<TKey>
		{
			rwLock.EnterUpgradeableReadLock();
			try
			{
				if (!list.Contains(key))
				{
					// set to running
					rwLock.EnterWriteLock();
					try
					{
						if (list.Contains(key))
							return contention();
						list.Add(key);
					}
					finally { rwLock.ExitWriteLock(); }
					// run queue
					TValue value;
					try { value = method(); }
					catch
					{
						list.Remove(key);
						throw;
					}
					// set to idle
					rwLock.EnterWriteLock();
					try
					{
						//if (!list.Contains(key))
						//    return default(TValue);
						list.Remove(key);
					}
					finally { rwLock.ExitWriteLock(); }
					return value;
				}
			}
			finally { rwLock.ExitUpgradeableReadLock(); }
			return contention();
		}

		/// <summary>
		/// Threadeds the get with create.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="value">The value.</param>
		/// <param name="builder">The builder.</param>
		/// <returns></returns>
		public static T ThreadedGetWithCreate<T>(this ReaderWriterLockSlim rwLock, ref T value, Func<T> builder)
		{
			rwLock.EnterUpgradeableReadLock();
			try
			{
				if (value == null)
				{
					rwLock.EnterWriteLock();
					try
					{
						if (value == null)
							// create
							value = builder();
					}
					finally { rwLock.ExitWriteLock(); }
				}
			}
			finally { rwLock.ExitUpgradeableReadLock(); }
			return value;
		}

		/// <summary>
		/// Threadeds the get with create.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="THash">The type of the hash.</typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="hash">The hash.</param>
		/// <param name="key">The key.</param>
		/// <param name="builder">The builder.</param>
		/// <returns></returns>
		public static TValue ThreadedGetWithCreate<TValue, TKey, THash>(this ReaderWriterLockSlim rwLock, THash hash, TKey key, Func<TKey, TValue> builder)
			where THash : IDictionary<TKey, TValue>
		{
			rwLock.EnterUpgradeableReadLock();
			try
			{
				TValue value;
				if (!hash.TryGetValue(key, out value))
				{
					rwLock.EnterWriteLock();
					try
					{
						if (!hash.TryGetValue(key, out value))
						{
							// create
							value = builder(key);
							hash.Add(key, value);
						}
					}
					finally { rwLock.ExitWriteLock(); }
				}
				return value;
			}
			finally { rwLock.ExitUpgradeableReadLock(); }
		}

		/// <summary>
		/// Threadeds the remove.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="THash">The type of the hash.</typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="hash">The hash.</param>
		/// <param name="key">The key.</param>
		public static void ThreadedRemove<TValue, TKey, THash>(this ReaderWriterLockSlim rwLock, THash hash, TKey key)
			where THash : IDictionary<TKey, TValue>
		{
			rwLock.EnterUpgradeableReadLock();
			try
			{
				if (hash.ContainsKey(key))
				{
					rwLock.EnterWriteLock();
					try
					{
						if (hash.ContainsKey(key))
							// remove
							hash.Remove(key);
					}
					finally { rwLock.ExitWriteLock(); }
				}
			}
			finally { rwLock.ExitUpgradeableReadLock(); }
		}
		#endregion
#else
		#region SQL THREADED
		/// <summary>
		/// Threadeds the get with create.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="value">The value.</param>
		/// <param name="builder">The builder.</param>
		/// <returns></returns>
        public static T ThreadedGetWithCreate<T>(object rwLock, ref T value, Func<T> builder)
        {
            //readerWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
            try
            {
                if (value == null)
                {
                    //System.Threading.LockCookie lockCookie = readerWriterLock.UpgradeToWriterLock(System.Threading.Timeout.Infinite);
                    try
                    {
                        if (value == null)
                        {
                            // create
                            value = builder();
                        }
                    }
                    finally
                    {
                        //readerWriterLock.DowngradeFromWriterLock(ref lockCookie);
                    }
                }
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
            return value;
        }

		/// <summary>
		/// Threadeds the get with create.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="THash">The type of the hash.</typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="hash">The hash.</param>
		/// <param name="key">The key.</param>
		/// <param name="builder">The builder.</param>
		/// <returns></returns>
        public static TValue ThreadedGetWithCreate<TValue, TKey, THash>(object rwLock, THash hash, TKey key, Func<TKey, TValue> builder)
            where THash : IDictionary<TKey, TValue>
        {
            //readerWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
            try
            {
                TValue value;
                if (!hash.TryGetValue(key, out value))
                {
                    //System.Threading.LockCookie lockCookie = readerWriterLock.UpgradeToWriterLock(System.Threading.Timeout.Infinite);
                    try
                    {
                        if (!hash.TryGetValue(key, out value))
                        {
                            // create
                            value = builder(key);
                            hash.Add(key, value);
                        }
                    }
                    finally
                    {
                        //readerWriterLock.DowngradeFromWriterLock(ref lockCookie);
                    }
                }
                return value;
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
        }

		/// <summary>
		/// Threadeds the remove.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="THash">The type of the hash.</typeparam>
		/// <param name="rwLock">The reader writer lock.</param>
		/// <param name="hash">The hash.</param>
		/// <param name="key">The key.</param>
        public static void ThreadedRemove<TValue, TKey, THash>(object rwLock, THash hash, TKey key)
            where THash : IDictionary<TKey, TValue>
        {
            //readerWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
            try
            {
                if (hash.ContainsKey(key))
                {
                    //System.Threading.LockCookie lockCookie = readerWriterLock.UpgradeToWriterLock(System.Threading.Timeout.Infinite);
                    try
                    {
                        if (hash.ContainsKey(key))
                        {
                            // remove
                            hash.Remove(key);
                        }
                    }
                    finally
                    {
                        //readerWriterLock.DowngradeFromWriterLock(ref lockCookie);
                    }
                }
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
        }
		#endregion
#endif
	}
}