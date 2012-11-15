// from automapper
namespace System.Reflection
{
    /// <summary>
    /// IMemberAccessor
    /// </summary>
    public interface IMemberAccessor : IMemberGetter, IMemberResolver, IValueResolver, ICustomAttributeProvider
    {
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="value">The value.</param>
        void SetValue(object destination, object value);
    }
}



