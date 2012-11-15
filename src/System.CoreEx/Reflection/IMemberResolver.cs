// from automapper
namespace System.Reflection
{
    /// <summary>
    /// IMemberResolver
    /// </summary>
    public interface IMemberResolver : IValueResolver
    {
        /// <summary>
        /// Gets the type of the member.
        /// </summary>
        /// <value>
        /// The type of the member.
        /// </value>
        Type MemberType { get; }
    }
}
