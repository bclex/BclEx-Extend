namespace System.Reflection
{
    /// <summary>
    /// MemberInfoExtensions
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets the type of the member.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            if (memberInfo is MethodInfo)
                return ((MethodInfo)memberInfo).ReturnType;
            if (memberInfo is PropertyInfo)
                return ((PropertyInfo)memberInfo).PropertyType;
            if (memberInfo is FieldInfo)
                return ((FieldInfo)memberInfo).FieldType;
            return null;
        }

        /// <summary>
        /// Toes the member accessor.
        /// </summary>
        /// <param name="accessorCandidate">The accessor candidate.</param>
        /// <returns></returns>
        public static IMemberAccessor ToMemberAccessor(this MemberInfo accessorCandidate)
        {
            FieldInfo fieldInfo = (accessorCandidate as FieldInfo);
            if (fieldInfo != null)
                return (accessorCandidate.DeclaringType.IsValueType ? ((IMemberAccessor)new Internal.ValueTypeFieldAccessor(fieldInfo)) : ((IMemberAccessor)new Internal.FieldAccessor(fieldInfo)));
            PropertyInfo propertyInfo = (accessorCandidate as PropertyInfo);
            if (propertyInfo != null)
                return (accessorCandidate.DeclaringType.IsValueType ? ((IMemberAccessor)new Internal.ValueTypePropertyAccessor(propertyInfo)) : ((IMemberAccessor)new Internal.PropertyAccessor(propertyInfo)));
            return null;
        }

        /// <summary>
        /// Toes the member getter.
        /// </summary>
        /// <param name="accessorCandidate">The accessor candidate.</param>
        /// <returns></returns>
        public static IMemberGetter ToMemberGetter(this MemberInfo accessorCandidate)
        {
            if (accessorCandidate != null)
            {
                if (accessorCandidate is PropertyInfo)
                    return new Internal.PropertyGetter((PropertyInfo)accessorCandidate);
                if (accessorCandidate is FieldInfo)
                    return new Internal.FieldGetter((FieldInfo)accessorCandidate);
                if (accessorCandidate is MethodInfo)
                    return new Internal.MethodGetter((MethodInfo)accessorCandidate);
            }
            return null;
        }
    }
}