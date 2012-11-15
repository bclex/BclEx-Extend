#region Foreign-License
// x
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class ActionCollection : IEnumerable<Action>, IEnumerable, IDisposable
    {
        private ITask _v1Task;
        private IActionCollection _v2Coll;
        private ITaskDefinition _v2Def;

        internal ActionCollection(ITask task)
        {
            _v1Task = task;
        }
        internal ActionCollection(ITaskDefinition iTaskDef)
        {
            _v2Def = iTaskDef;
            _v2Coll = iTaskDef.Actions;
        }

        public Action Add(Action action)
        {
            if (_v2Def != null)
            {
                action.Bind(_v2Def);
                return action;
            }
            action.Bind(_v1Task);
            return action;
        }

        public Action AddNew(TaskActionType actionType)
        {
            if (_v1Task != null)
                return new ExecAction(_v1Task);
            return Action.CreateAction(_v2Coll.Create(actionType));
        }

        public void Clear()
        {
            if (_v2Coll != null)
                _v2Coll.Clear();
            else
                Add(new ExecAction());
        }

        public void Dispose()
        {
            _v1Task = null;
            _v2Def = null;
            _v2Coll = null;
        }

        public IEnumerator<Action> GetEnumerator()
        {
            if (_v2Coll != null)
                return new Enumerator(this);
            return new Enumerator(_v1Task);
        }

        public void Insert(int index, Action action)
        {
            if (_v2Coll == null && Count > 0)
                throw new NotV1SupportedException("Only a single action is allowed.");
            var actionArray = new Action[Count - index];
            for (var i = index; i < Count; i++)
                actionArray[i - index] = (Action)this[i].Clone();
            for (var j = Count - 1; j >= index; j--)
                RemoveAt(j);
            Add(action);
            for (var k = 0; k < actionArray.Length; k++)
                Add(actionArray[k]);
        }

        public void RemoveAt(int index)
        {
            if (index >= Count)
                throw new ArgumentOutOfRangeException("index", index, "Failed to remove action. Index out of range.");
            if (_v2Coll != null)
                _v2Coll.Remove(++index);
            else
            {
                if (index != 0)
                    throw new NotV1SupportedException("There can be only a single action and it cannot be removed.");
                Add(new ExecAction());
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public override string ToString()
        {
            if (Count == 1)
                return this[0].ToString();
            if (Count > 1)
                return Local.MultipleActions;
            return string.Empty;
        }

        public string Context
        {
            get
            {
                if (_v2Coll != null)
                    return _v2Coll.Context;
                return string.Empty;
            }
            set
            {
                if (_v2Coll == null)
                    throw new NotV1SupportedException();
                _v2Coll.Context = value;
            }
        }

        public int Count
        {
            get
            {
                if (_v2Coll != null)
                    return _v2Coll.Count;
                if (_v1Task.GetApplicationName().Length != 0)
                    return 1;
                return 0;
            }
        }

        public Action this[int index]
        {
            get
            {
                if (_v2Coll != null)
                    return Action.CreateAction(_v2Coll[++index]);
                if (index != 0)
                    throw new ArgumentOutOfRangeException();
                return new ExecAction((string)_v1Task.GetApplicationName(), (string)_v1Task.GetParameters(), (string)_v1Task.GetWorkingDirectory());
            }
            set
            {
                if (Count <= index)
                    throw new ArgumentOutOfRangeException("index", index, "Index is not a valid index in the ActionCollection");
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public string XmlText
        {
            get
            {
                if (_v2Coll == null)
                    throw new NotV1SupportedException();
                return _v2Coll.XmlText;
            }
            set
            {
                if (_v2Coll == null)
                    throw new NotV1SupportedException();
                _v2Coll.XmlText = value;
            }
        }

        internal class Enumerator : IEnumerator<Action>, IDisposable, IEnumerator
        {
            private ActionCollection _parent;
            private int _v1Pos;
            private ITask _v1Task;
            private IEnumerator _v2Enum;

            internal Enumerator(ActionCollection iColl)
            {
                _v1Pos = -1;
                _parent = iColl;
                if (iColl._v2Coll != null)
                    _v2Enum = iColl._v2Coll.GetEnumerator();
            }

            internal Enumerator(ITask task)
            {
                _v1Pos = -1;
                _v1Task = task;
            }

            public void Dispose()
            {
                _v1Task = null;
                _v2Enum = null;
            }

            public bool MoveNext()
            {
                if (_v2Enum != null)
                    return _v2Enum.MoveNext();
                return (++_v1Pos == 0);
            }

            public void Reset()
            {
                if (_v2Enum != null)
                    _v2Enum.Reset();
                _v1Pos = -1;
            }

            public Action Current
            {
                get
                {
                    if (_v2Enum != null)
                    {
                        var current = _v2Enum.Current as IAction;
                        if (current != null)
                            return Action.CreateAction(current);
                    }
                    if (_v1Pos != 0)
                        throw new InvalidOperationException();
                    return new ExecAction((string)_v1Task.GetApplicationName(), (string)_v1Task.GetParameters(), (string)_v1Task.GetWorkingDirectory());
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}

