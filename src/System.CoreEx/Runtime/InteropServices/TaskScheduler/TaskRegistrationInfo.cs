#region Foreign-License
// x
#endregion
using System.IO;
using System.Runtime.InteropServices.TaskScheduler.V1Interop;
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class TaskRegistrationInfo : IDisposable
    {
        private ITask _v1Task;
        private IRegistrationInfo _v2RegInfo;

        internal TaskRegistrationInfo(ITask iTask)
        {
            _v1Task = iTask;
        }
        internal TaskRegistrationInfo(IRegistrationInfo iRegInfo)
        {
            _v2RegInfo = iRegInfo;
        }

        public void Dispose()
        {
            _v1Task = null;
            if (_v2RegInfo != null)
                Marshal.ReleaseComObject(_v2RegInfo);
        }

        internal static object GetTaskData(ITask v1Task)
        {
            try
            {
                ushort num;
                IntPtr ptr;
                v1Task.GetWorkItemData(out num, out ptr);
                var destination = new byte[num];
                Marshal.Copy(ptr, destination, 0, num);
                var serializationStream = new MemoryStream(destination, false);
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(serializationStream);
            }
            catch { }
            return string.Empty;
        }

        internal static void SetTaskData(ITask v1Task, object value)
        {
            var formatter = new BinaryFormatter();
            var serializationStream = new MemoryStream();
            formatter.Serialize(serializationStream, value);
            v1Task.SetWorkItemData((ushort)serializationStream.Length, serializationStream.ToArray());
        }

        public string Author
        {
            get
            {
                if (_v2RegInfo != null)
                    return _v2RegInfo.Author;
                return (string)_v1Task.GetCreator();
            }
            set
            {
                if (_v2RegInfo != null)
                    _v2RegInfo.Author = value;
                else
                    _v1Task.SetCreator(value);
            }
        }

        public DateTime Date
        {
            get
            {
                if (_v2RegInfo != null)
                {
                    var date = _v2RegInfo.Date;
                    if (!string.IsNullOrEmpty(date))
                        return DateTime.Parse(date);
                    return DateTime.MinValue;
                }
                var str2 = Task.GetV1Path(_v1Task);
                if (!string.IsNullOrEmpty(str2) && File.Exists(str2))
                    return File.GetLastWriteTime(str2);
                return DateTime.MinValue;
            }
            set
            {
                if (_v2RegInfo != null)
                    _v2RegInfo.Date = value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFK");
                else
                {
                    var str = Task.GetV1Path(_v1Task);
                    if (string.IsNullOrEmpty(str) || !File.Exists(str))
                        throw new NotV1SupportedException("This property cannot be set on an unregistered task.");
                    File.SetLastWriteTime(str, value);
                }
            }
        }

        public string Description
        {
            get
            {
                if (_v2RegInfo != null)
                    return _v2RegInfo.Description;
                return (string)_v1Task.GetComment();
            }
            set
            {
                if (_v2RegInfo != null)
                    _v2RegInfo.Description = value;
                else
                    _v1Task.SetComment(value);
            }
        }

        public string Documentation
        {
            get
            {
                if (_v2RegInfo != null)
                    return _v2RegInfo.Documentation;
                return GetTaskData(_v1Task).ToString();
            }
            set
            {
                if (_v2RegInfo != null)
                    _v2RegInfo.Documentation = value;
                else
                    SetTaskData(_v1Task, value);
            }
        }

        public GenericSecurityDescriptor SecurityDescriptor
        {
            get { return new RawSecurityDescriptor(SecurityDescriptorSddlForm); }
            set { SecurityDescriptorSddlForm = value.GetSddlForm(AccessControlSections.All); }
        }

        public string SecurityDescriptorSddlForm
        {
            get
            {
                object securityDescriptor = null;
                if (_v2RegInfo != null)
                    securityDescriptor = _v2RegInfo.SecurityDescriptor;
                if (securityDescriptor != null)
                    return securityDescriptor.ToString();
                return null;
            }
            set
            {
                if (_v2RegInfo == null)
                    throw new NotV1SupportedException();
                _v2RegInfo.SecurityDescriptor = value;
            }
        }

        public string Source
        {
            get
            {
                if (_v2RegInfo != null)
                    return _v2RegInfo.Source;
                return null;
            }
            set
            {
                if (_v2RegInfo == null)
                    throw new NotV1SupportedException();
                _v2RegInfo.Source = value;
            }
        }

        public Uri URI
        {
            get
            {
                string uRI = null;
                if (_v2RegInfo != null)
                    uRI = _v2RegInfo.URI;
                if (string.IsNullOrEmpty(uRI))
                    return null;
                return new Uri(uRI);
            }
            set
            {
                if (_v2RegInfo == null)
                    throw new NotV1SupportedException();
                _v2RegInfo.URI = value.ToString();
            }
        }

        public Version Version
        {
            get
            {
                if (_v2RegInfo != null)
                    try { return new Version(_v2RegInfo.Version); }
                    catch { }
                return new System.Version(1, 0);
            }
            set
            {
                if (_v2RegInfo == null)
                    throw new NotV1SupportedException();
                _v2RegInfo.Version = value.ToString();
            }
        }

        public string XmlText
        {
            get
            {
                if (_v2RegInfo == null)
                    throw new NotV1SupportedException();
                return _v2RegInfo.XmlText;
            }
            set
            {
                if (_v2RegInfo == null)
                    throw new NotV1SupportedException();
                _v2RegInfo.XmlText = value;
            }
        }
    }
}

