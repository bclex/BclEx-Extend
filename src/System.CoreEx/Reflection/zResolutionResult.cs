//// from automapper
//namespace System.Reflection
//{
//    public class ResolutionResult
//    {
//        private readonly ResolutionContext _context;
//        private readonly Type _memberType;
//        private readonly Type _type;
//        private readonly object _value;

//        public ResolutionResult(ResolutionContext context)
//            : this(context.SourceValue, context)
//        {
//        }

//        private ResolutionResult(object value, ResolutionContext context)
//        {
//            _value = value;
//            _context = context;
//            _type = this.ResolveType(value, typeof(object));
//            _memberType = _type;
//        }

//        private ResolutionResult(object value, ResolutionContext context, Type memberType)
//        {
//            _value = value;
//            _context = context;
//            _type = this.ResolveType(value, memberType);
//            _memberType = memberType;
//        }

//        public ResolutionResult New(object value)
//        {
//            return new ResolutionResult(value, this.Context);
//        }

//        public ResolutionResult New(object value, Type memberType)
//        {
//            return new ResolutionResult(value, this.Context, memberType);
//        }

//        private Type ResolveType(object value, Type memberType)
//        {
//            if (value == null)
//                return memberType;
//            return value.GetType();
//        }

//        public ResolutionContext Context
//        {
//            get { return _context; }
//        }

//        public Type MemberType
//        {
//            get { return _memberType; }
//        }

//        public Type Type
//        {
//            get { return _type; }
//        }

//        public object Value
//        {
//            get { return _value; }
//        }
//    }
//}