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
    public sealed class TriggerCollection : IEnumerable<Trigger>, IEnumerable, IDisposable
    {
        private ITask _v1Task;
        private ITriggerCollection _v2Coll;
        private ITaskDefinition _v2Def;

        internal TriggerCollection(ITask iTask)
        {
            _v1Task = iTask;
        }
        internal TriggerCollection(ITaskDefinition iTaskDef)
        {
            _v2Def = iTaskDef;
            _v2Coll = _v2Def.Triggers;
        }

        public Trigger Add(Trigger unboundTrigger)
        {
            if (_v2Def != null)
            {
                unboundTrigger.Bind(_v2Def);
                return unboundTrigger;
            }
            unboundTrigger.Bind(_v1Task);
            return unboundTrigger;
        }

        public Trigger AddNew(TaskTriggerType taskTriggerType)
        {
            if (_v1Task != null)
            {
                ushort num;
                return Trigger.CreateTrigger(_v1Task.CreateTrigger(out num), Trigger.ConvertToV1TriggerType(taskTriggerType));
            }
            return Trigger.CreateTrigger(_v2Coll.Create(taskTriggerType));
        }

        internal void Bind()
        {
            foreach (Trigger trigger in this)
                trigger.SetV1TriggerData();
        }

        public void Clear()
        {
            if (_v2Coll != null)
                _v2Coll.Clear();
            else
                for (int i = Count - 1; i >= 0; i--)
                    RemoveAt(i);
        }

        public void Dispose()
        {
            if (_v2Coll != null)
                Marshal.ReleaseComObject(_v2Coll);
            _v2Def = null;
            _v1Task = null;
        }

        public IEnumerator<Trigger> GetEnumerator()
        {
            if (_v1Task != null)
                return new V1TriggerEnumerator(_v1Task);
            return new V2TriggerEnumerator(_v2Coll);
        }

        public void Insert(int index, Trigger trigger)
        {
            var triggerArray = new Trigger[Count - index];
            for (var i = index; i < Count; i++)
                triggerArray[i - index] = (Trigger)this[i].Clone();
            for (var j = Count - 1; j >= index; j--)
                RemoveAt(j);
            Add(trigger);
            for (var k = 0; k < triggerArray.Length; k++)
                Add(triggerArray[k]);
        }

        public void RemoveAt(int index)
        {
            if (index >= Count)
                throw new ArgumentOutOfRangeException("index", index, "Failed to remove Trigger. Index out of range.");
            if (_v2Coll != null)
                _v2Coll.Remove(++index);
            else
                _v1Task.DeleteTrigger((ushort)index);
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public override string ToString()
        {
            if (Count == 1)
                return this[0].ToString();
            if (Count > 1)
                return Local.MultipleTriggers;
            return string.Empty;
        }

        public int Count
        {
            get
            {
                if (_v2Coll != null)
                    return _v2Coll.Count;
                return _v1Task.GetTriggerCount();
            }
        }

        public Trigger this[int index]
        {
            get
            {
                if (_v2Coll != null)
                    return Trigger.CreateTrigger(_v2Coll[++index]);
                return Trigger.CreateTrigger(_v1Task.GetTrigger((ushort)index));
            }
            set
            {
                if (Count <= index)
                    throw new ArgumentOutOfRangeException("index", index, "Index is not a valid index in the TriggerCollection");
                RemoveAt(index);
                Insert(index, value);
            }
        }

        internal sealed class V1TriggerEnumerator : IEnumerator<Trigger>, IDisposable, IEnumerator
        {
            private short _curItem = -1;
            private ITask _iTask;

            internal V1TriggerEnumerator(ITask task)
            {
                _iTask = task;
            }

            public void Dispose()
            {
                _iTask = null;
            }

            public bool MoveNext()
            {
                if ((_curItem = (short)(_curItem + 1)) >= _iTask.GetTriggerCount())
                    return false;
                return true;
            }

            public void Reset()
            {
                _curItem = -1;
            }

            public Trigger Current
            {
                get { return Trigger.CreateTrigger(_iTask.GetTrigger((ushort)_curItem)); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        internal sealed class V2TriggerEnumerator : IEnumerator<Trigger>, IDisposable, IEnumerator
        {
            private IEnumerator _iEnum;

            internal V2TriggerEnumerator(ITriggerCollection iColl)
            {
                _iEnum = iColl.GetEnumerator();
            }

            public void Dispose()
            {
                _iEnum = null;
            }

            public bool MoveNext() { return _iEnum.MoveNext(); }

            public void Reset() { _iEnum.Reset(); }

            public Trigger Current
            {
                get { return Trigger.CreateTrigger((ITrigger)_iEnum.Current); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}

