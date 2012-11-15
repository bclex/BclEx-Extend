#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class TaskCollection : IEnumerable<Task>, IEnumerable, IDisposable
    {
        private Regex _filter;
        private TaskFolder _fld;
        private TaskService _svc;
        private ITaskScheduler _v1TS;
        private IRegisteredTaskCollection _v2Coll;

        internal TaskCollection(TaskService svc, Regex filter = null)
        {
            _svc = svc;
            Filter = filter;
            _v1TS = svc._v1TaskScheduler;
        }
        internal TaskCollection(TaskFolder folder, IRegisteredTaskCollection iTaskColl, Regex filter = null)
        {
            _svc = folder.TaskService;
            Filter = filter;
            _fld = folder;
            _v2Coll = iTaskColl;
        }

        public void Dispose()
        {
            _v1TS = null;
            if (_v2Coll != null)
                Marshal.ReleaseComObject(_v2Coll);
        }

        public IEnumerator<Task> GetEnumerator()
        {
            if (_v1TS != null)
                return new V1TaskEnumerator(_svc, _filter);
            return new V2TaskEnumerator(_fld, _v2Coll, _filter);
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int Count
        {
            get
            {
                var num = 0;
                if (_v2Coll != null)
                {
                    if (_filter == null)
                        return _v2Coll.Count;
                    var enumerator = new V2TaskEnumerator(_fld, _v2Coll, _filter);
                    while (enumerator.MoveNext())
                        num++;
                    return num;
                }
                var enumerator2 = new V1TaskEnumerator(_svc, _filter);
                return enumerator2.TaskNames.Length;
            }
        }

        private Regex Filter
        {
            get { return _filter; }
            set
            {
                switch (value == null ? string.Empty : value.ToString().TrimStart(new char[] { '^' }).TrimEnd(new char[] { '$' }))
                {
                    case "":
                    case "*":
                        _filter = null;
                        return;
                }
                if (value.ToString().TrimEnd(new char[] { '$' }).EndsWith(@"\.job", StringComparison.InvariantCultureIgnoreCase))
                    _filter = new Regex(value.ToString().Replace(@"\.job", ""));
                else
                    _filter = value;
            }
        }

        public Task this[string name]
        {
            get
            {
                if (_v2Coll != null)
                    return new Task(_svc, _v2Coll[name]);
                var task = _svc.GetTask(name);
                if (task == null)
                    throw new ArgumentOutOfRangeException();
                return task;
            }
        }

        public Task this[int index]
        {
            get
            {
                var num = 0;
                if (_v2Coll != null)
                {
                    if (_filter == null)
                        return new Task(_svc, _v2Coll[++index]);
                    var enumerator = new V2TaskEnumerator(_fld, _v2Coll, _filter);
                    while (enumerator.MoveNext())
                        if (num++ == index)
                            return enumerator.Current;
                }
                else
                {
                    var enumerator2 = new V1TaskEnumerator(_svc, _filter);
                    while (enumerator2.MoveNext())
                        if (num++ == index)
                            return enumerator2.Current;
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        internal class V1TaskEnumerator : IEnumerator<Task>, IEnumerator, IDisposable
        {
            private string _curItem;
            private Regex _filter;
            private Guid _iTaskGuid = Marshal.GenerateGuidForType(typeof(ITask));
            private ITaskScheduler _ts;
            private TaskService _svc;
            private IEnumWorkItems _wienum;

            internal V1TaskEnumerator(TaskService svc, Regex filter = null)
            {
                _svc = svc;
                _filter = filter;
                _ts = svc._v1TaskScheduler;
                _wienum = _ts.Enum();
                Reset();
            }

            public void Dispose()
            {
                if (_wienum != null)
                    Marshal.ReleaseComObject(_wienum);
                _ts = null;
            }

            public bool MoveNext()
            {
                var zero = IntPtr.Zero;
                var flag = false;
                do
                {
                    _curItem = null;
                    uint fetched = 0;
                    try
                    {
                        _wienum.Next(1, out zero, out fetched);
                        if (fetched != 1)
                            break;
                        using (var str = new CoTaskMemString(Marshal.ReadIntPtr(zero)))
                            _curItem = str.ToString();
                        if (_curItem.EndsWith(".job", StringComparison.InvariantCultureIgnoreCase))
                            _curItem = _curItem.Remove(_curItem.Length - 4);
                    }
                    catch { }
                    finally { Marshal.FreeCoTaskMem(zero); zero = IntPtr.Zero; }
                    if (_filter == null || _filter.IsMatch(_curItem))
                        try { var iCurrent = ICurrent; flag = true; }
                        catch { flag = false; }
                }
                while (!flag);
                return (_curItem != null);
            }

            public void Reset()
            {
                _curItem = null;
                _wienum.Reset();
            }

            public Task Current
            {
                get { return new Task(_svc, ICurrent); }
            }

            internal ITask ICurrent
            {
                get { return _ts.Activate(_curItem, ref _iTaskGuid); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            internal string[] TaskNames
            {
                get
                {
                    var list = new List<string>();
                    var zero = IntPtr.Zero;
                    var flag = false;
                    do
                    {
                        uint fetched = 0;
                        try
                        {
                            _wienum.Next(50, out zero, out fetched);
                            if (fetched == 0)
                                break;
                            var ptr = zero;
                            for (var i = 0; i < fetched; i++)
                            {
                                using (var str = new CoTaskMemString(Marshal.ReadIntPtr(ptr)))
                                {
                                    var input = str.ToString();
                                    if (input.EndsWith(".job", StringComparison.InvariantCultureIgnoreCase))
                                        input = input.Remove(input.Length - 4);
                                    if (_filter == null || _filter.IsMatch(input))
                                        list.Add(input);
                                }
                                ptr = (IntPtr)(((long)ptr) + Marshal.SizeOf(ptr));
                            }
                        }
                        catch { }
                        finally { Marshal.FreeCoTaskMem(zero); zero = IntPtr.Zero; }
                    }
                    while (!flag);
                    Reset();
                    return list.ToArray();
                }
            }
        }

        internal class V2TaskEnumerator : IEnumerator<Task>, IEnumerator, IDisposable
        {
            private Regex _filter;
            private TaskFolder _fld;
            private IEnumerator _iEnum;

            internal V2TaskEnumerator(TaskFolder folder, IRegisteredTaskCollection iTaskColl, Regex filter = null)
            {
                _fld = folder;
                _iEnum = iTaskColl.GetEnumerator();
                _filter = filter;
            }

            public void Dispose()
            {
                _iEnum = null;
            }

            public bool MoveNext()
            {
                var flag = _iEnum.MoveNext();
                if (flag)
                {
                    while (flag && _filter != null)
                    {
                        if (_filter.IsMatch(Current.Name))
                            return flag;
                        flag = _iEnum.MoveNext();
                    }
                    return flag;
                }
                return false;
            }

            public void Reset()
            {
                _iEnum.Reset();
            }

            public Task Current
            {
                get { return new Task(_fld.TaskService, (IRegisteredTask)_iEnum.Current); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}

