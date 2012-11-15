#region Foreign-License
// x
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class TaskPrincipalPrivileges : IList<string>, ICollection<string>, IEnumerable<string>, IEnumerable
    {
        private IPrincipal2 _v2Principal2;

        internal TaskPrincipalPrivileges(IPrincipal2 iPrincipal2 = null)
        {
            _v2Principal2 = iPrincipal2;
        }

        public void Add(string item)
        {
            if (_v2Principal2 == null)
                throw new NotV1SupportedException();
            _v2Principal2.AddRequiredPrivilege(item);
        }

        public void Clear() { throw new NotImplementedException(); }

        public bool Contains(string item) { return (IndexOf(item) != -1); }

        public void CopyTo(string[] array, int arrayIndex)
        {
            var enumerator = GetEnumerator();
            for (var i = arrayIndex; i < array.Length; i++)
            {
                if (!enumerator.MoveNext())
                    return;
                array[i] = enumerator.Current;
            }
        }

        public IEnumerator<string> GetEnumerator() { return new TaskPrincipalPrivilegesEnumerator(_v2Principal2); }

        public int IndexOf(string item)
        {
            for (var i = 0; i < Count; i++)
                if (string.Compare(item, this[i], true) == 0)
                    return i;
            return -1;
        }

        public void Insert(int index, string item) { throw new NotImplementedException(); }

        public bool Remove(string item) { throw new NotImplementedException(); }

        public void RemoveAt(int index) { throw new NotImplementedException(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int Count
        {
            get
            {
                if (_v2Principal2 == null)
                    return 0;
                return (int)_v2Principal2.RequiredPrivilegeCount;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public string this[int index]
        {
            get
            {
                if (_v2Principal2 == null)
                    throw new IndexOutOfRangeException();
                return _v2Principal2.GetRequiredPrivilege((long)(index + 1));
            }
            set { throw new NotImplementedException(); }
        }

        public sealed class TaskPrincipalPrivilegesEnumerator : IEnumerator<string>, IDisposable, IEnumerator
        {
            private int _cur;
            private string _curString;
            private IPrincipal2 _v2Principal2;

            internal TaskPrincipalPrivilegesEnumerator(IPrincipal2 iPrincipal2 = null)
            {
                _v2Principal2 = iPrincipal2;
                Reset();
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_v2Principal2 != null && _cur < _v2Principal2.RequiredPrivilegeCount)
                {
                    _cur++;
                    _curString = _v2Principal2.GetRequiredPrivilege((long)_cur);
                    return true;
                }
                _curString = null;
                return false;
            }

            public void Reset()
            {
                _cur = 0;
                _curString = null;
            }

            public string Current
            {
                get { return _curString; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}

