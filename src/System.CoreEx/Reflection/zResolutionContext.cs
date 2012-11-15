//// from automapper
//using System.Collections.Generic;
//namespace System.Reflection
//{
//    public class ResolutionContext : IEquatable<ResolutionContext>
//    {
//        private ResolutionContext(ResolutionContext context, object sourceValue)
//        {
//            ArrayIndex = context.ArrayIndex;
//            TypeMap = context.TypeMap;
//            PropertyMap = context.PropertyMap;
//            SourceType = context.SourceType;
//            SourceValue = sourceValue;
//            DestinationValue = context.DestinationValue;
//            Parent = context;
//            DestinationType = context.DestinationType;
//            InstanceCache = context.InstanceCache;
//        }

//        private ResolutionContext(ResolutionContext context, object sourceValue, Type sourceType)
//        {
//            ArrayIndex = context.ArrayIndex;
//            TypeMap = context.TypeMap;
//            PropertyMap = context.PropertyMap;
//            SourceType = sourceType;
//            SourceValue = sourceValue;
//            DestinationValue = context.DestinationValue;
//            Parent = context;
//            DestinationType = context.DestinationType;
//            InstanceCache = context.InstanceCache;
//        }

//        public ResolutionContext(TypeMap typeMap, object source, Type sourceType, Type destinationType)
//            : this(typeMap, source, null, sourceType, destinationType)
//        {
//        }

//        private ResolutionContext(ResolutionContext context, TypeMap memberTypeMap, object sourceValue, Type sourceType, Type destinationType)
//        {
//            TypeMap = memberTypeMap;
//            SourceValue = sourceValue;
//            Parent = context;
//            AssignTypes(memberTypeMap, sourceType, destinationType);
//            InstanceCache = context.InstanceCache;
//        }

//        private ResolutionContext(ResolutionContext context, object sourceValue, object destinationValue, TypeMap memberTypeMap, PropertyMap propertyMap)
//        {
//            TypeMap = memberTypeMap;
//            PropertyMap = propertyMap;
//            SourceValue = sourceValue;
//            DestinationValue = destinationValue;
//            Parent = context;
//            InstanceCache = context.InstanceCache;
//            SourceType = memberTypeMap.SourceType;
//            DestinationType = memberTypeMap.DestinationType;
//        }

//        private ResolutionContext(ResolutionContext context, object sourceValue, object destinationValue, Type sourceType, PropertyMap propertyMap)
//        {
//            PropertyMap = propertyMap;
//            SourceType = sourceType;
//            SourceValue = sourceValue;
//            DestinationValue = destinationValue;
//            Parent = context;
//            DestinationType = propertyMap.DestinationProperty.MemberType;
//            InstanceCache = context.InstanceCache;
//        }

//        public ResolutionContext(TypeMap typeMap, object source, object destination, Type sourceType, Type destinationType)
//        {
//            TypeMap = typeMap;
//            SourceValue = source;
//            DestinationValue = destination;
//            AssignTypes(typeMap, sourceType, destinationType);
//            InstanceCache = new Dictionary<ResolutionContext, object>();
//        }

//        private ResolutionContext(ResolutionContext context, object sourceValue, TypeMap typeMap, Type sourceType, Type destinationType, int arrayIndex)
//        {
//            ArrayIndex = new int?(arrayIndex);
//            TypeMap = typeMap;
//            PropertyMap = context.PropertyMap;
//            SourceValue = sourceValue;
//            Parent = context;
//            InstanceCache = context.InstanceCache;
//            AssignTypes(typeMap, sourceType, destinationType);
//        }

//        private void AssignTypes(TypeMap typeMap, Type sourceType, Type destinationType)
//        {
//            if (typeMap != null)
//            {
//                SourceType = typeMap.SourceType;
//                DestinationType = typeMap.DestinationType;
//            }
//            else
//            {
//                SourceType = sourceType;
//                DestinationType = destinationType;
//            }
//        }

//        public ResolutionContext CreateElementContext(TypeMap elementTypeMap, object item, Type sourceElementType, Type destinationElementType, int arrayIndex)
//        {
//            return new ResolutionContext(this, item, elementTypeMap, sourceElementType, destinationElementType, arrayIndex);
//        }

//        public ResolutionContext CreateMemberContext(TypeMap memberTypeMap, object memberValue, object destinationValue, Type sourceMemberType, PropertyMap propertyMap)
//        {
//            return ((memberTypeMap != null) ? new ResolutionContext(this, memberValue, destinationValue, memberTypeMap, propertyMap) : new ResolutionContext(this, memberValue, destinationValue, sourceMemberType, propertyMap));
//        }

//        public ResolutionContext CreateTypeContext(TypeMap memberTypeMap, object sourceValue, Type sourceType, Type destinationType)
//        {
//            return new ResolutionContext(this, memberTypeMap, sourceValue, sourceType, destinationType);
//        }

//        public ResolutionContext CreateValueContext(object sourceValue)
//        {
//            return new ResolutionContext(this, sourceValue);
//        }

//        public ResolutionContext CreateValueContext(object sourceValue, Type sourceType)
//        {
//            return new ResolutionContext(this, sourceValue, sourceType);
//        }

//        public bool Equals(ResolutionContext other)
//        {
//            if (object.ReferenceEquals(null, other))
//                return false;
//            return (object.ReferenceEquals(this, other) || (((object.Equals(other.TypeMap, TypeMap) && object.Equals(other.SourceType, SourceType)) && object.Equals(other.DestinationType, DestinationType)) && object.Equals(other.SourceValue, SourceValue)));
//        }

//        public override bool Equals(object obj)
//        {
//            if (object.ReferenceEquals(null, obj))
//                return false;
//            if (object.ReferenceEquals(this, obj))
//                return true;
//            if (obj.GetType() != typeof(ResolutionContext))
//                return false;
//            return Equals((ResolutionContext)obj);
//        }

//        public PropertyMap GetContextPropertyMap()
//        {
//            PropertyMap propertyMap = PropertyMap;
//            for (ResolutionContext context = Parent; (propertyMap == null) && (context != null); context = context.Parent)
//                propertyMap = context.PropertyMap;
//            return propertyMap;
//        }

//        public TypeMap GetContextTypeMap()
//        {
//            TypeMap typeMap = TypeMap;
//            for (ResolutionContext context = Parent; (typeMap == null) && (context != null); context = context.Parent)
//                typeMap = context.TypeMap;
//            return typeMap;
//        }

//        public override int GetHashCode()
//        {
//            int num = (TypeMap != null) ? TypeMap.GetHashCode() : 0;
//            num = (num * 0x18d) ^ ((SourceType != null) ? SourceType.GetHashCode() : 0);
//            num = (num * 0x18d) ^ ((DestinationType != null) ? DestinationType.GetHashCode() : 0);
//            num = (num * 0x18d) ^ (ArrayIndex.HasValue ? ArrayIndex.Value : 0);
//            return ((num * 0x18d) ^ ((SourceValue != null) ? SourceValue.GetHashCode() : 0));
//        }

//        public static ResolutionContext New<TSource>(TSource sourceValue)
//        {
//            return new ResolutionContext(null, sourceValue, typeof(TSource), null);
//        }

//        public override string ToString()
//        {
//            return string.Format("Trying to map {0} to {1}.", SourceType.Name, DestinationType.Name);
//        }

//        public int? ArrayIndex { get; private set; }

//        public Type DestinationType { get; private set; }

//        public object DestinationValue { get; private set; }

//        public Dictionary<ResolutionContext, object> InstanceCache { get; private set; }

//        public bool IsSourceValueNull
//        {
//            get { return object.Equals(null, SourceValue); }
//        }

//        public string MemberName
//        {
//            get { return ((PropertyMap == null) ? string.Empty : (!ArrayIndex.HasValue ? PropertyMap.DestinationProperty.Name : (PropertyMap.DestinationProperty.Name + ArrayIndex.Value))); }
//        }

//        public ResolutionContext Parent { get; private set; }
//        public PropertyMap PropertyMap { get; private set; }
//        public Type SourceType { get; private set; }
//        public object SourceValue { get; private set; }
//        public TypeMap TypeMap { get; private set; }
//    }





//}