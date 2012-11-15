//#region License
// /*
//The MIT License

//Copyright (c) 2008 Sky Morey

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//*/
//#endregion
//namespace System.Collections.Generic
//{
//    public class WeakKeyedDictionary<K, V>
//    {
//        private Dictionary<int, List<Pair>> dic = new Dictionary<int, List<Pair>>();

//        public class Pair
//        {
//            public WeakReference Key;
//            public Object Value;
//        }

//        public void Add(K key, V value)
//        {
//            if (value == null)
//            {
//                this.Remove(key);
//                return;
//            }

//            List<Pair> list = null;
//            dic.TryGetValue(key.GetHashCode(), out list);
//            if (list == null)
//            {
//                list = new List<Pair>();
//                dic.Add(key.GetHashCode(), list);
//            }

//            bool isDirty = false;
//            foreach (Pair p in list)
//            {
//                if (p.Key.Target == null)
//                {
//                    isDirty = true;
//                    continue;
//                }
//                if (p.Key.Target == (Object)key)
//                {
//                    p.Value = (Object)value;
//                    if (isDirty) cleanList(list);
//                    return;
//                }
//            }
//            if (isDirty)
//                cleanList(list);
//            list.Add(new Pair
//            {
//                Key = new WeakReference(key),
//                Value = value,
//            });
            
//        }

//        public bool ContainsKey(K key)
//        {
//            List<Pair> list = null;
//            dic.TryGetValue(key.GetHashCode(), out list);
//            if (list == null) return false;

//            bool isDirty = false;
//            foreach (Pair p in list)
//            {
//                if (p.Key.Target == null)
//                {
//                    isDirty = true;
//                    continue;
//                }
//                if (p.Key.Target == (Object)key)
//                {
//                    if (isDirty)
//                        cleanList(list);
//                    return true;
//                }
//            }
//            if (isDirty)
//                cleanList(list);
//            return false;
//        }

//        private void cleanList(List<Pair> list)
//        {
//            var temp = (from Pair p in list where p.Key.Target != null select p);
//            list.Clear();
//            list.AddRange(temp);
//        }

//        public bool Remove(K key)
//        {
//            List<Pair> list = null;
//            dic.TryGetValue(key.GetHashCode(), out list);
//            if (list == null) return true;

//            foreach (Pair p in list)
//                if (p.Key.Target == (Object)key)
//                {
//                    p.Value = null;
//                    break;
//                }
//            cleanList(list);
//            return true;
//        }

//        public V this[K key]
//        {
//            get
//            {
//                List<Pair> list = null;
//                dic.TryGetValue(key.GetHashCode(), out list);
//                if (list == null)
//                    return default(V);
//                bool isDirty = false;
//                foreach (Pair p in list)
//                {
//                    if (p.Key.Target == null)
//                    {
//                        isDirty = true;
//                        continue;
//                    }
//                    if (p.Key.Target == (Object)key)
//                    {
//                        if (isDirty)
//                            cleanList(list);
//                        return (V)p.Value;
//                    }
//                }
//                if (isDirty)
//                    cleanList(list);
//                return default(V);
//            }
//            set { Add(key, value); }
//        }


//        public void Add(KeyValuePair<K, V> item)
//        {
//            throw new NotImplementedException();
//        }

//        public void Clear()
//        {
//            dic.Clear();
//        }

//        public bool Contains(KeyValuePair<K, V> item)
//        {
//            throw new NotImplementedException();
//        }

//        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
//        {
//            throw new NotImplementedException();
//        }

//        public int Count
//        {
//            get { throw new NotImplementedException(); /* dic.Count */ }
//        }

//        public bool IsReadOnly
//        {
//            get { return false; }
//        }

//        public bool Remove(KeyValuePair<K, V> item)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() { throw new NotImplementedException(); /* return dic.GetEnumerator(); */ }


//        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
//        //    return ((System.Collections.IEnumerable)dic).GetEnumerator();
//        //}
//    }
//}


//Sudo code - won't really compile:
//public class WeakDictionary <TKey,TValue> : IDictionary<TKey,TValue>
//{
//    private IDictionary<TKey,WeakReference> _innerDictionary = new Dictionary<TKey,WeakReference>();


//    public TValue Index[ TKey key ]
//    {
//        get{
//            var reference = _innerDictionary[ key ];
//            if( reference.IsAlive )
//                return (TValue)reference.Target;
//            throw new InvalidOperation( "Key not found." );
//        }

//    }

//    private void Cull()
//    {
//        var deadKeys = new List<TKey>();
//        foreach( var pair in _innerDictionary )
//        {
//            if( ! pair.Value.IsAlive )
//                deadKeys.Add( pair.Key );
//        }

//        foreach( var key in deadKeys )
//            _innerDictionary.Remove( key );
//    }
//}

//http://blogs.msdn.com/b/nicholg/archive/2006/06/04/617466.aspx
//http://stackoverflow.com/questions/2047591/compacting-a-weakreference-dictionary
//http://blogs.msdn.com/b/jaredpar/archive/2009/03/03/building-a-weakreference-hashtable.aspx
