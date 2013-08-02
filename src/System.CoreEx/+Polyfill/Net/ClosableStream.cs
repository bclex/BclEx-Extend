#region Foreign-License
// .Net40 Polyfill
#endregion
using System.IO;
using System.Runtime;
using System.Threading;

namespace System.Net
{
    internal class ClosableStream : DelegatedStream
    {
        private int _closed;
        private EventHandler _onClose;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal ClosableStream(Stream stream, EventHandler onClose)
            : base(stream)
        {
            _onClose = onClose;
        }

        public override void Close()
        {
            if (Interlocked.Increment(ref _closed) == 1 && _onClose != null)
                _onClose(this, new EventArgs());
        }
    }
}
