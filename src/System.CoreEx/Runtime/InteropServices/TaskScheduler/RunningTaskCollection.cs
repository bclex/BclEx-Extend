#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class RunningTaskCollection : IEnumerable<RunningTask>, IEnumerable, IDisposable
    {
        private TaskService _svc;
        private ITaskScheduler _v1TS;
        private IRunningTaskCollection _v2Coll;
        private ITaskService _v2Svc;

        internal RunningTaskCollection(TaskService svc)
        {
            _svc = svc;
            _v1TS = svc._v1TaskScheduler;
        }
        internal RunningTaskCollection(TaskService svc, IRunningTaskCollection iTaskColl)
        {
            _svc = svc;
            _v2Svc = svc._v2TaskService;
            _v2Coll = iTaskColl;
        }

        public void Dispose()
        {
            _v1TS = null;
            _v2Svc = null;
            if (_v2Coll != null)
                Marshal.ReleaseComObject(_v2Coll);
        }

        public IEnumerator<RunningTask> GetEnumerator()
        {
            if (_v2Coll != null)
                return new RunningTaskEnumerator(_svc, _v2Coll);
            return new V1RunningTaskEnumerator(_svc);
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int Count
        {
            get
            {
                if (_v2Coll != null)
                    return _v2Coll.Count;
                var num = 0;
                var enumerator = new V1RunningTaskEnumerator(_svc);
                while (enumerator.MoveNext())
                    num++;
                return num;
            }
        }

        public RunningTask this[int index]
        {
            get
            {
                if (_v2Coll != null)
                {
                    var iRunningTask = _v2Coll[++index];
                    return new RunningTask(_svc, TaskService.GetTask(_svc._v2TaskService, iRunningTask.Path), iRunningTask);
                }
                var num = 0;
                var enumerator = new V1RunningTaskEnumerator(_svc);
                while (enumerator.MoveNext())
                    if (num++ == index)
                        return enumerator.Current;
                throw new ArgumentOutOfRangeException();
            }
        }

        internal class RunningTaskEnumerator : IEnumerator<RunningTask>, IEnumerator, IDisposable
        {
            private IEnumerator _iEnum;
            private TaskService _svc;
            private ITaskService _v2Svc;

            internal RunningTaskEnumerator(TaskService svc, IRunningTaskCollection iTaskColl)
            {
                _svc = svc;
                _v2Svc = svc._v2TaskService;
                _iEnum = iTaskColl.GetEnumerator();
            }

            public void Dispose()
            {
                _v2Svc = null;
                _iEnum = null;
            }

            public bool MoveNext()
            {
                return _iEnum.MoveNext();
            }

            public void Reset()
            {
                _iEnum.Reset();
            }

            public RunningTask Current
            {
                get
                {
                    var current = (IRunningTask)_iEnum.Current;
                    IRegisteredTask iTask = null;
                    try { iTask = TaskService.GetTask(_v2Svc, current.Path); }
                    catch { }
                    if (iTask == null)
                        return null;
                    return new RunningTask(this._svc, iTask, current);
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        internal class V1RunningTaskEnumerator : IEnumerator<RunningTask>, IDisposable, IEnumerator
        {
            private TaskService _svc;
            private TaskCollection.V1TaskEnumerator _tEnum;

            internal V1RunningTaskEnumerator(TaskService svc)
            {
                _svc = svc;
                _tEnum = new TaskCollection.V1TaskEnumerator(svc, null);
            }

            public void Dispose()
            {
                _tEnum.Dispose();
            }

            public bool MoveNext()
            {
                if (!_tEnum.MoveNext())
                    return false;
                return (_tEnum.Current.State == TaskState.Running || MoveNext());
            }

            public void Reset() { _tEnum.Reset(); }

            public RunningTask Current
            {
                get { return new RunningTask(_svc, _tEnum.ICurrent); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}

