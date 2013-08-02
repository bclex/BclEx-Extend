#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Collections.Generic;
using System.Security;
using System.Collections;
using System.Threading;
namespace System.IO
{
    /// <summary>
    /// Iterator
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    internal abstract class Iterator<TSource> : IEnumerable<TSource>, IEnumerable, IEnumerator<TSource>, IDisposable, IEnumerator
    {
        internal TSource current;
        internal int state;
        private int threadId;

        public Iterator()
        {
            threadId = Thread.CurrentThread.ManagedThreadId;
        }

        protected abstract Iterator<TSource> Clone();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            current = default(TSource);
            state = -1;
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            if (threadId == Thread.CurrentThread.ManagedThreadId && state == 0)
            {
                state = 1;
                return this;
            }
            var iterator = Clone();
            iterator.state = 1;
            return iterator;
        }

        public abstract bool MoveNext();
        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

        void IEnumerator.Reset() { throw new NotSupportedException(); }

        // Properties
        public TSource Current
        {
            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return current; }
        }

        object IEnumerator.Current
        {
            get { return this.Current; }
        }
    }
}
#endif