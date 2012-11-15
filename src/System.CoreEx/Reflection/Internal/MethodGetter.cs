// from automapper
namespace System.Reflection.Internal
{
    internal class MethodGetter : MemberGetter
    {
        private readonly LateBoundMethod _lateBoundMethod;
        private readonly Type _memberType;
        private readonly MethodInfo _methodInfo;
        private readonly string _name;

        public MethodGetter(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            _name = _methodInfo.Name;
            _memberType = _methodInfo.ReturnType;
            _lateBoundMethod = DelegateFactory.CreateGet(methodInfo);
        }

        public bool Equals(MethodGetter other)
        {
            if (object.ReferenceEquals(null, other))
                return false;
            return (object.ReferenceEquals(this, other) || object.Equals(other._methodInfo, _methodInfo));
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
                return false;
            if (object.ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(MethodGetter))
                return false;
            return this.Equals((MethodGetter)obj);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _methodInfo.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _methodInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override int GetHashCode()
        {
            return _methodInfo.GetHashCode();
        }

        public override object GetValue(object source)
        {
            return ((_memberType == null) ? null : _lateBoundMethod(source, new object[0]));
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _methodInfo.IsDefined(attributeType, inherit);
        }

        public override MemberInfo MemberInfo
        {
            get { return _methodInfo; }
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