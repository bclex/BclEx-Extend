#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
namespace System.IO
{
    partial class KludgeExtensions
    {
        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyTo(this Stream source, Stream destination)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (!source.CanRead && !source.CanWrite)
                throw new ObjectDisposedException(null, SR.GetResourceString("ObjectDisposed_StreamClosed"));
            if (!destination.CanRead && !destination.CanWrite)
                throw new ObjectDisposedException("destination", SR.GetResourceString("ObjectDisposed_StreamClosed"));
            if (!source.CanRead)
                throw new NotSupportedException(SR.GetResourceString("NotSupported_UnreadableStream"));
            if (!destination.CanWrite)
                throw new NotSupportedException(SR.GetResourceString("NotSupported_UnwritableStream"));
            InternalCopyTo(source, destination, 0x14000);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public static void CopyTo(this Stream source, Stream destination, int bufferSize)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize", SR.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
            if (!source.CanRead && !source.CanWrite)
                throw new ObjectDisposedException(null, SR.GetResourceString("ObjectDisposed_StreamClosed"));
            if (!destination.CanRead && !destination.CanWrite)
                throw new ObjectDisposedException("destination", SR.GetResourceString("ObjectDisposed_StreamClosed"));
            if (!source.CanRead)
                throw new NotSupportedException(SR.GetResourceString("NotSupported_UnreadableStream"));
            if (!destination.CanWrite)
                throw new NotSupportedException(SR.GetResourceString("NotSupported_UnwritableStream"));
            InternalCopyTo(source, destination, bufferSize);
        }

        private static void InternalCopyTo(this Stream source, Stream destination, int bufferSize)
        {
            var buffer = new byte[bufferSize];
            int num;
            while ((num = source.Read(buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, num);
        }
    }
}
#endif