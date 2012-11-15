#region Foreign-License
// x
#endregion
using System.Runtime.InteropServices.TaskScheduler.V2Interop;
namespace System.Runtime.InteropServices.TaskScheduler
{
    public sealed class NetworkSettings : IDisposable
    {
        private INetworkSettings _v2Settings;

        internal NetworkSettings() { }
        internal NetworkSettings(INetworkSettings iSettings)
        {
            _v2Settings = iSettings;
        }

        public void Dispose()
        {
            if (_v2Settings != null)
                Marshal.ReleaseComObject(_v2Settings);
        }

        public Guid Id
        {
            get
            {
                string id = null;
                if (_v2Settings != null)
                    id = _v2Settings.Id;
                if (!string.IsNullOrEmpty(id))
                    return new Guid(id);
                return Guid.Empty;
            }
            set
            {
                if (_v2Settings == null)
                    throw new NotV1SupportedException();
                _v2Settings.Id = (value == Guid.Empty ? null : value.ToString());
            }
        }

        public string Name
        {
            get
            {
                if (_v2Settings != null)
                    return _v2Settings.Name;
                return null;
            }
            set
            {
                if (_v2Settings == null)
                    throw new NotV1SupportedException();
                _v2Settings.Name = value;
            }
        }
    }
}

