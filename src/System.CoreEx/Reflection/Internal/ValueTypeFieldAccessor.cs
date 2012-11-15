// from automapper
namespace System.Reflection.Internal
{
    internal class ValueTypeFieldAccessor : FieldGetter, IMemberAccessor, IMemberGetter, IMemberResolver, IValueResolver, ICustomAttributeProvider
    {
        private readonly FieldInfo _lateBoundFieldSet;

        public ValueTypeFieldAccessor(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            _lateBoundFieldSet = fieldInfo;
        }

        public void SetValue(object destination, object value)
        {
            _lateBoundFieldSet.SetValue(destination, value);
        }
    }
}