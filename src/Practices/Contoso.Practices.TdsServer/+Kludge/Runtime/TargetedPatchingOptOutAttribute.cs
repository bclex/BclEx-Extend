#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
namespace System.Runtime
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class TargetedPatchingOptOutAttribute : Attribute
    {
        private string _reason;

        private TargetedPatchingOptOutAttribute() { }
        public TargetedPatchingOptOutAttribute(string reason)
        {
            _reason = reason;
        }

        public string Reason
        {
            get { return _reason; }
        }
    }
}
#endif