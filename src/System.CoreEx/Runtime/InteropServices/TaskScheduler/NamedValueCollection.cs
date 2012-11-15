#region Foreign-License
// x
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class NamedValueCollection : IDisposable, IEnumerable
    {
        private Dictionary<string, string> _unboundDict;
        private ITaskNamedValueCollection _v2Coll;

        internal NamedValueCollection()
        {
            _unboundDict = new Dictionary<string, string>(5);
        }

        internal NamedValueCollection(ITaskNamedValueCollection iColl)
        {
            _v2Coll = iColl;
        }

        public void Add(string Name, string Value)
        {
            if (_v2Coll != null)
                _v2Coll.Create(Name, Value);
            else
                _unboundDict.Add(Name, Value);
        }

        internal void Bind(ITaskNamedValueCollection iTaskNamedValueCollection)
        {
            _v2Coll = iTaskNamedValueCollection;
            _v2Coll.Clear();
            foreach (KeyValuePair<string, string> pair in _unboundDict)
                _v2Coll.Create(pair.Key, pair.Value);
        }

        public void Clear()
        {
            if (_v2Coll != null)
                _v2Coll.Clear();
            else
                _unboundDict.Clear();
        }

        public void CopyTo(NamedValueCollection destCollection)
        {
            if (_v2Coll != null)
            {
                for (int i = 1; i <= Count; i++)
                    destCollection.Add(_v2Coll[i].Name, _v2Coll[i].Value);
            }
            else
            {
                foreach (KeyValuePair<string, string> pair in _unboundDict)
                    destCollection.Add(pair.Key, pair.Value);
            }
        }

        public void Dispose()
        {
            if (_v2Coll != null)
                Marshal.ReleaseComObject(_v2Coll);
        }

        public IEnumerator GetEnumerator()
        {
            if (_v2Coll != null)
                return _v2Coll.GetEnumerator();
            return _unboundDict.GetEnumerator();
        }

        public void RemoveAt(int index)
        {
            _v2Coll.Remove(index);
        }

        internal bool Bound
        {
            get { return (_v2Coll != null); }
        }

        public int Count
        {
            get
            {
                if (_v2Coll == null)
                    return _unboundDict.Count;
                return _v2Coll.Count;
            }
        }

        public string this[string key]
        {
            get
            {
                if (_v2Coll != null)
                {
                    foreach (ITaskNamedValuePair pair in _v2Coll)
                        if (string.Compare(pair.Name, key, false) == 0)
                            return pair.Value;
                    return null;
                }
                string str;
                _unboundDict.TryGetValue(key, out str);
                return str;
            }
        }

        public string this[int index]
        {
            get
            {
                if (_v2Coll != null)
                    return _v2Coll[++index].Value;
                var array = new string[_unboundDict.Count];
                _unboundDict.Keys.CopyTo(array, 0);
                return _unboundDict[array[index]];
            }
        }
    }
}
