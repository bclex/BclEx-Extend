// from automapper
namespace System.Reflection.Internal
{
    internal class ValueTypePropertyAccessor : PropertyGetter, IMemberAccessor, IMemberGetter, IMemberResolver, IValueResolver, ICustomAttributeProvider
    {
        private readonly bool _hasSetter;
        private readonly MethodInfo _lateBoundPropertySet;

        public ValueTypePropertyAccessor(PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            var setMethod = propertyInfo.GetSetMethod(true);
            _hasSetter = (setMethod != null);
            if (_hasSetter)
                _lateBoundPropertySet = setMethod;
        }

        public void SetValue(object destination, object value)
        {
            _lateBoundPropertySet.Invoke(destination, new object[] { value });
        }

        public bool HasSetter
        {
            get { return _hasSetter; }
        }
    }
}