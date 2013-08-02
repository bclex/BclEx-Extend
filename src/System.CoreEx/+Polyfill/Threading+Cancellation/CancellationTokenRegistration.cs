#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
namespace System.Threading
{
    /// <summary>
    /// CancellationTokenRegistration
    /// </summary>
    [StructLayout(LayoutKind.Sequential), HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public struct CancellationTokenRegistration : IEquatable<CancellationTokenRegistration>, IDisposable
    {
        private readonly CancellationCallbackInfo _callbackInfo;
        private readonly SparselyPopulatedArrayAddInfo<CancellationCallbackInfo> _registrationInfo;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal CancellationTokenRegistration(CancellationCallbackInfo callbackInfo, SparselyPopulatedArrayAddInfo<CancellationCallbackInfo> registrationInfo)
        {
            _callbackInfo = callbackInfo;
            _registrationInfo = registrationInfo;
        }

        [FriendAccessAllowed]
        internal bool TryDeregister()
        {
            if (_registrationInfo.Source == null)
                return false;
            if (_registrationInfo.Source.SafeAtomicRemove(_registrationInfo.Index, _callbackInfo) != _callbackInfo)
                return false;
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var flag = TryDeregister();
            var callbackInfo = _callbackInfo;
            if (callbackInfo != null)
            {
                var cancellationTokenSource = callbackInfo.CancellationTokenSource;
                if (cancellationTokenSource.IsCancellationRequested && !cancellationTokenSource.IsCancellationCompleted && !flag && cancellationTokenSource.ThreadIDExecutingCallbacks != Thread.CurrentThread.ManagedThreadId)
                    cancellationTokenSource.WaitForCallbackToComplete(_callbackInfo);
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(CancellationTokenRegistration left, CancellationTokenRegistration right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(CancellationTokenRegistration left, CancellationTokenRegistration right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is CancellationTokenRegistration && Equals((CancellationTokenRegistration)obj));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(CancellationTokenRegistration other)
        {
            return (_callbackInfo == other._callbackInfo && _registrationInfo.Source == other._registrationInfo.Source && _registrationInfo.Index == other._registrationInfo.Index);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (_registrationInfo.Source != null)
                return (_registrationInfo.Source.GetHashCode() ^ _registrationInfo.Index.GetHashCode());
            return _registrationInfo.Index.GetHashCode();
        }
    }
}
#endif