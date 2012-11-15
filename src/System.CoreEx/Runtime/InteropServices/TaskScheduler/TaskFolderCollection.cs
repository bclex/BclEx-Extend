#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace System.Runtime.InteropServices.TaskScheduler
{

    public sealed class TaskFolderCollection : IEnumerable<TaskFolder>, IEnumerable
    {
        private TaskFolder _parent;
        private TaskFolder[] _v1FolderList;
        private ITaskFolderCollection _v2FolderList;

        internal TaskFolderCollection()
        {
            _v1FolderList = new TaskFolder[0];
        }
        internal TaskFolderCollection(TaskFolder v1Folder)
        {
            _parent = v1Folder;
            _v1FolderList = new TaskFolder[] { v1Folder };
        }
        internal TaskFolderCollection(TaskFolder folder, ITaskFolderCollection iCollection)
        {
            _parent = folder;
            _v2FolderList = iCollection;
        }

        internal void CopyTo(TaskFolder[] array, int arrayIndex)
        {
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            if (array == null)
                throw new ArgumentNullException();
            if (_v2FolderList != null)
            {
                if ((arrayIndex + Count) > array.Length)
                    throw new ArgumentException();
                foreach (var folder in _v2FolderList)
                    array[arrayIndex++] = new TaskFolder(_parent.TaskService, folder);
            }
            else
            {
                if ((arrayIndex + _v1FolderList.Length) > array.Length)
                    throw new ArgumentException();
                _v1FolderList.CopyTo(array, arrayIndex);
            }
        }

        public void Dispose()
        {
            if (_v1FolderList != null && _v1FolderList.Length > 0)
            {
                _v1FolderList[0].Dispose();
                _v1FolderList[0] = null;
            }
            if (_v2FolderList != null)
                Marshal.ReleaseComObject(_v2FolderList);
        }

        public IEnumerator<TaskFolder> GetEnumerator()
        {
            var array = new TaskFolder[Count];
            CopyTo(array, 0);
            return new TaskFolderEnumerator(array);
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int Count
        {
            get
            {
                if (_v2FolderList == null)
                    return _v1FolderList.Length;
                return _v2FolderList.Count;
            }
        }

        public TaskFolder this[int index]
        {
            get
            {
                if (_v2FolderList != null)
                    return new TaskFolder(_parent.TaskService, _v2FolderList[++index]);
                return _v1FolderList[index];
            }
        }

        public TaskFolder this[string path]
        {
            get
            {
                if (_v2FolderList != null)
                    return new TaskFolder(_parent.TaskService, _v2FolderList[path]);
                if ((_v1FolderList == null || _v1FolderList.Length <= 0) || (!(path == string.Empty) && !(path == @"\")))
                    throw new ArgumentException("Path not found");
                return _v1FolderList[0];
            }
        }

        private class TaskFolderEnumerator : IEnumerator<TaskFolder>, IDisposable, IEnumerator
        {
            private TaskFolder[] _folders;
            private IEnumerator _iEnum;

            internal TaskFolderEnumerator(TaskFolder[] f)
            {
                _folders = f;
                _iEnum = f.GetEnumerator();
            }

            public void Dispose() { }

            public bool MoveNext() { return _iEnum.MoveNext(); }

            public void Reset() { _iEnum.Reset(); }

            public TaskFolder Current
            {
                get { return (_iEnum.Current as TaskFolder); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}

