// from automapper
namespace System.Reflection.Internal
{
    internal class PropertyGetter : MemberGetter
    {
        private readonly LateBoundPropertyGet _lateBoundPropertyGet;
        private readonly Type _memberType;
        private readonly string _name;
        private readonly PropertyInfo _propertyInfo;

        // Methods
        public PropertyGetter(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            _name = _propertyInfo.Name;
            _memberType = _propertyInfo.PropertyType;
            if (_propertyInfo.GetGetMethod(true) != null)
                _lateBoundPropertyGet = DelegateFactory.CreateGet(propertyInfo);
        }

        public bool Equals(PropertyGetter other)
        {
            if (object.ReferenceEquals(null, other))
                return false;
            return (object.ReferenceEquals(this, other) || object.Equals(other._propertyInfo, _propertyInfo));
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
                return false;
            if (object.ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(PropertyGetter))
                return false;
            return Equals((PropertyGetter)obj);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _propertyInfo.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _propertyInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override int GetHashCode()
        {
            return _propertyInfo.GetHashCode();
        }

        public override object GetValue(object source)
        {
            return _lateBoundPropertyGet(source);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _propertyInfo.IsDefined(attributeType, inherit);
        }

        public override MemberInfo MemberInfo
        {
            get { return _propertyInfo; }
        }

        public override Type MemberType
        {
            get { return _memberType; }
        }

        public override string Name
        {
            get { return _name; }
        }
    }
}