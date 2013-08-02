#region Foreign-License
// .Net40 Polyfill
#endregion
using System.IO;
using System.Net.Sockets;
using System.Runtime;

namespace System.Net
{
    internal class DelegatedStream : Stream
    {
        private NetworkStream _netStream;
        private Stream _stream;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected DelegatedStream() { }

        protected DelegatedStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            _stream = stream;
            _netStream = (stream as NetworkStream);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (!CanRead)
                throw new NotSupportedException(SR.GetString("ReadNotSupported"));
            if (_netStream != null)
                return _netStream.BeginRead(buffer, offset, count, callback, state);
            return _stream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (!CanWrite)
                throw new NotSupportedException(SR.GetString("WriteNotSupported"));
            if (_netStream != null)
                return _netStream.BeginWrite(buffer, offset, count, callback, state);
            return _stream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void Close()
        {
            _stream.Close();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            if (!CanRead)
                throw new NotSupportedException(SR.GetString("ReadNotSupported"));
            return _stream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (!CanWrite)
                throw new NotSupportedException(SR.GetString("WriteNotSupported"));
            _stream.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
                throw new NotSupportedException(SR.GetString("ReadNotSupported"));
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!CanSeek)
                throw new NotSupportedException(SR.GetString("SeekNotSupported"));
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            if (!CanSeek)
                throw new NotSupportedException(SR.GetString("SeekNotSupported"));
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
                throw new NotSupportedException(SR.GetString("WriteNotSupported"));
            _stream.Write(buffer, offset, count);
        }

        protected Stream BaseStream
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return _stream; }
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override long Length
        {
            get
            {
                if (!CanSeek)
                    throw new NotSupportedException(SR.GetString("SeekNotSupported"));
                return _stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                if (!CanSeek)
                    throw new NotSupportedException(SR.GetString("SeekNotSupported"));
                return _stream.Position;
            }
            set
            {
                if (!CanSeek)
                    throw new NotSupportedException(SR.GetString("SeekNotSupported"));
                _stream.Position = value;
            }
        }

#if CLR45
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _stream.FlushAsync(cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (!CanRead)
                throw new NotSupportedException(SR.GetString("ReadNotSupported"));
            return _stream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (!CanWrite)
                throw new NotSupportedException(SR.GetString("WriteNotSupported"));
            return _stream.WriteAsync(buffer, offset, count, cancellationToken);
        }
#endif
    }
}

