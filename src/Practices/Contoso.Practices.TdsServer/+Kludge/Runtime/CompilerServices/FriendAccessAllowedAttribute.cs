#region Foreign-License
// .Net40 Kludge
#endregion
namespace System.Runtime.CompilerServices
{
    [FriendAccessAllowed, AttributeUsage(AttributeTargets.Interface | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class FriendAccessAllowedAttribute : Attribute { }
}
