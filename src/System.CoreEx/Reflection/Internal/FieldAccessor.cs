// from automapper
namespace System.Reflection.Internal
{
    internal class FieldAccessor : FieldGetter, IMemberAccessor, IMemberGetter, IMemberResolver, IValueResolver, ICustomAttributeProvider
    {
        private readonly LateBoundFieldSet _lateBoundFieldSet;

        public FieldAccessor(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            _lateBoundFieldSet = DelegateFactory.CreateSet(fieldInfo);
        }

        public void SetValue(object destination, object value)
        {
            _lateBoundFieldSet(destination, value);
        }
    }
}