#region Foreign-License
// .Net40 Surface
#endregion
using System.Collections.Specialized;
using System.Net.Mail;
using System.Runtime;
using System.Text;

namespace System.Net.Mime
{
    internal class MimeBasePart
    {
        protected ContentDisposition _contentDisposition;
        protected ContentType _contentType;
        internal const string _defaultCharSet = "utf-8";
        private HeaderCollection _headers;

        internal class MimePartAsyncResult : LazyAsyncResult
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            internal MimePartAsyncResult(MimeBasePart part, object state, AsyncCallback callback)
                : base(part, state, callback) { }
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal MimeBasePart() { }
        internal virtual IAsyncResult BeginSend(BaseWriter writer, AsyncCallback callback, bool allowUnicode, object state)
        {
            throw new NotImplementedException();
        }

        internal static Encoding DecodeEncoding(string value)
        {
            if (value == null || value.Length == 0)
                return null;
            var strArray = value.Split(new char[] { '?', '\r', '\n' });
            if (strArray.Length < 5 || strArray[0] != "=" || strArray[4] != "=")
                return null;
            var name = strArray[1];
            return Encoding.GetEncoding(name);
        }

        internal static string DecodeHeaderValue(string value)
        {
            if (value == null || value.Length == 0)
                return string.Empty;
            var str = string.Empty;
            foreach (var str2 in value.Split(new char[] { '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var strArray2 = str2.Split(new char[] { '?' });
                if (strArray2.Length != 5 || strArray2[0] != "=" || strArray2[4] != "=")
                    return value;
                var name = strArray2[1];
                var flag = strArray2[2] == "B";
                var bytes = Encoding.ASCII.GetBytes(strArray2[3]);
                var factory = new EncodedStreamFactory();
                var count = factory.GetEncoderForHeader(Encoding.GetEncoding(name), flag, 0).DecodeBytes(bytes, 0, bytes.Length);
                var encoding = Encoding.GetEncoding(name);
                str = str + encoding.GetString(bytes, 0, count);
            }
            return str;
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal static string EncodeHeaderValue(string value, Encoding encoding, bool base64Encoding)
        {
            return EncodeHeaderValue(value, encoding, base64Encoding, 0);
        }

        internal static string EncodeHeaderValue(string value, Encoding encoding, bool base64Encoding, int headerLength)
        {
            new StringBuilder();
            if (IsAscii(value, false))
                return value;
            if (encoding == null)
                encoding = Encoding.GetEncoding("utf-8");
            var stream = new EncodedStreamFactory().GetEncoderForHeader(encoding, base64Encoding, headerLength);
            var bytes = encoding.GetBytes(value);
            stream.EncodeBytes(bytes, 0, bytes.Length);
            return stream.GetEncodedString();
        }

        internal void EndSend(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");
            var result = (asyncResult as MimePartAsyncResult);
            if (result == null || result.AsyncObject != this)
                throw new ArgumentException(SR.GetString("net_io_invalidasyncresult"), "asyncResult");
            if (result.EndCalled)
                throw new InvalidOperationException(SR.GetString("net_io_invalidendcall", new object[] { "EndSend" }));
            result.InternalWaitForCompletion();
            result.EndCalled = true;
            if (result.Result is Exception)
                throw ((Exception)result.Result);
        }

        internal static bool IsAnsi(string value, bool permitCROrLF)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            foreach (var ch in value)
            {
                if (ch > '\x00ff')
                    return false;
                if (!permitCROrLF && ((ch == '\r') || (ch == '\n')))
                    return false;
            }
            return true;
        }

        internal static bool IsAscii(string value, bool permitCROrLF)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            foreach (var ch in value)
            {
                if (ch > '\x007f')
                    return false;
                if (!permitCROrLF && ((ch == '\r') || (ch == '\n')))
                    return false;
            }
            return true;
        }

        internal void PrepareHeaders(bool allowUnicode)
        {
            _contentType.PersistIfNeeded((HeaderCollection)Headers, false);
            _headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentType), _contentType.Encode(allowUnicode));
            if (_contentDisposition != null)
            {
                _contentDisposition.PersistIfNeeded((HeaderCollection)Headers, false);
                _headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition), _contentDisposition.Encode(allowUnicode));
            }
        }

        internal virtual void Send(BaseWriter writer, bool allowUnicode)
        {
            throw new NotImplementedException();
        }

        internal static bool ShouldUseBase64Encoding(Encoding encoding)
        {
            if (encoding != Encoding.Unicode && encoding != Encoding.UTF8 && encoding != Encoding.UTF32 && encoding != Encoding.BigEndianUnicode)
                return false;
            return true;
        }

        internal string ContentID
        {
            get { return Headers[MailHeaderInfo.GetString(MailHeaderID.ContentID)]; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.ContentID));
                else
                    Headers[MailHeaderInfo.GetString(MailHeaderID.ContentID)] = value;
            }
        }

        internal string ContentLocation
        {
            get { return Headers[MailHeaderInfo.GetString(MailHeaderID.ContentLocation)]; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.ContentLocation));
                else
                    Headers[MailHeaderInfo.GetString(MailHeaderID.ContentLocation)] = value;
            }
        }

        internal ContentType ContentType
        {
            get
            {
                if (_contentType == null)
                    _contentType = new ContentType();
                return _contentType;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _contentType = value;
                _contentType.PersistIfNeeded((HeaderCollection)Headers, true);
            }
        }

        internal NameValueCollection Headers
        {
            get
            {
                if (_headers == null)
                    _headers = new HeaderCollection();
                if (_contentType == null)
                    _contentType = new ContentType();
                _contentType.PersistIfNeeded(_headers, false);
                if (_contentDisposition != null)
                    _contentDisposition.PersistIfNeeded(_headers, false);
                return _headers;
            }
        }
    }
}

