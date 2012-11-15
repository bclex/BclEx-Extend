// from automapper
namespace System.Reflection
{
    /// <summary>
    /// IMemberGetter
    /// </summary>
    public interface IMemberGetter : IMemberResolver, IValueResolver, ICustomAttributeProvider
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        object GetValue(object source);
        /// <summary>
        /// Gets the member info.
        /// </summary>
        MemberInfo MemberInfo { get; }
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }
    }
}
