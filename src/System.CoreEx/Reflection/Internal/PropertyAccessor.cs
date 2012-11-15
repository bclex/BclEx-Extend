// from automapper
namespace System.Reflection.Internal
{
    internal class PropertyAccessor : PropertyGetter, IMemberAccessor, IMemberGetter, IMemberResolver, IValueResolver, ICustomAttributeProvider
    {
        private readonly bool _hasSetter;
        private readonly LateBoundPropertySet _lateBoundPropertySet;

        public PropertyAccessor(PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            _hasSetter = (propertyInfo.GetSetMethod(true) != null);
            if (_hasSetter)
                _lateBoundPropertySet = DelegateFactory.CreateSet(propertyInfo);
        }

        public virtual void SetValue(object destination, object value)
        {
            _lateBoundPropertySet(destination, value);
        }

        public bool HasSetter
        {
            get { return _hasSetter; }
        }
    }
}