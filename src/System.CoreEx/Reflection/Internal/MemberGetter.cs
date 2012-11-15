// from automapper
namespace System.Reflection.Internal
{
    internal abstract class MemberGetter : IMemberGetter, IMemberResolver, IValueResolver, ICustomAttributeProvider
    {
        protected MemberGetter()
        {
        }

        public abstract object[] GetCustomAttributes(bool inherit);
        public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);
        public abstract object GetValue(object source);
        public abstract bool IsDefined(Type attributeType, bool inherit);

        //public ResolutionResult Resolve(ResolutionResult source)
        //{
        //    return ((source.Value == null) ? source.New(source.Value, MemberType) : source.New(GetValue(source.Value), MemberType));
        //}

        public abstract MemberInfo MemberInfo { get; }

        public abstract Type MemberType { get; }

        public abstract string Name { get; }
    }
}