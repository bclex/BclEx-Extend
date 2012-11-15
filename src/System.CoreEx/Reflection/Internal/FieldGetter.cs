// from automapper
namespace System.Reflection.Internal
{
    internal class FieldGetter : MemberGetter
    {
        private readonly FieldInfo _fieldInfo;
        private readonly LateBoundFieldGet _lateBoundFieldGet;
        private readonly Type _memberType;
        private readonly string _name;

        public FieldGetter(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
            _name = fieldInfo.Name;
            _memberType = fieldInfo.FieldType;
            _lateBoundFieldGet = DelegateFactory.CreateGet(fieldInfo);
        }

        public bool Equals(FieldGetter other)
        {
            if (object.ReferenceEquals(null, other))
                return false;
            return (object.ReferenceEquals(this, other) || object.Equals(other._fieldInfo, _fieldInfo));
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
                return false;
            if (object.ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(FieldGetter))
                return false;
            return Equals((FieldGetter)obj);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _fieldInfo.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _fieldInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override int GetHashCode()
        {
            return _fieldInfo.GetHashCode();
        }

        public override object GetValue(object source)
        {
            return _lateBoundFieldGet(source);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _fieldInfo.IsDefined(attributeType, inherit);
        }

        public override MemberInfo MemberInfo
        {
            get { return _fieldInfo; }
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