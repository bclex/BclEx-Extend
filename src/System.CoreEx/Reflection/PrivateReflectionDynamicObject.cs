#region Foreign-License
//http://blogs.msdn.com/b/davidebb/archive/2010/01/18/use-c-4-0-dynamic-to-drastically-simplify-your-private-reflection-code.aspx
#endregion
#if CLR4
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace System.Reflection
{
    /// <summary>
    /// PrivateReflectionDynamicObject
    /// </summary>
    public class PrivateReflectionDynamicObject : DynamicObject
    {
        private static IDictionary<Type, IDictionary<string, IProperty>> _propertiesOnType = new ConcurrentDictionary<Type, IDictionary<string, IProperty>>();
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // Simple abstraction to make field and property access consistent
        private interface IProperty
        {
            string Name { get; }
            object GetValue(object obj, object[] index);
            void SetValue(object obj, object val, object[] index);
        }

        // IProperty implementation over a PropertyInfo
        private class Property : IProperty
        {
            internal PropertyInfo PropertyInfo { get; set; }

            string IProperty.Name
            {
                get { return PropertyInfo.Name; }
            }
            object IProperty.GetValue(object obj, object[] index) { return PropertyInfo.GetValue(obj, index); }
            void IProperty.SetValue(object obj, object val, object[] index) { PropertyInfo.SetValue(obj, val, index); }
        }

        // IProperty implementation over a FieldInfo
        private class Field : IProperty
        {
            internal FieldInfo FieldInfo { get; set; }

            string IProperty.Name
            {
                get { return FieldInfo.Name; }
            }
            object IProperty.GetValue(object obj, object[] index) { return FieldInfo.GetValue(obj); }
            void IProperty.SetValue(object obj, object val, object[] index) { FieldInfo.SetValue(obj, val); }
        }


        private object RealObject { get; set; }

        internal static object WrapIfNeeded(object o)
        {
            // Don't wrap primitive types, which don't have many interesting internal APIs
            return (((o != null) || o.GetType().IsPrimitive || (o is string)) ? o : new PrivateReflectionDynamicObject() { RealObject = o });
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // Get the property value
            result = GetProperty(binder.Name).GetValue(RealObject, index: null);
            // Wrap the sub object if necessary. This allows nested anonymous objects to work.
            result = WrapIfNeeded(result);
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Set the property value
            GetProperty(binder.Name).SetValue(RealObject, value, index: null);
            return true;
        }
        /// <summary>
        /// Provides the implementation for operations that get a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for indexing operations.
        /// </summary>
        /// <param name="binder">Provides information about the operation.</param>
        /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] operation in C# (sampleObject(3) in Visual Basic), where sampleObject is derived from the DynamicObject class, indexes[0] is equal to 3.</param>
        /// <param name="result">The result of the index operation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            // The indexed property is always named "Item" in C#
            result = GetIndexProperty().GetValue(RealObject, indexes);
            // Wrap the sub object if necessary. This allows nested anonymous objects to work.
            result = WrapIfNeeded(result);
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that access objects by a specified index.
        /// </summary>
        /// <param name="binder">Provides information about the operation.</param>
        /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, indexes[0] is equal to 3.</param>
        /// <param name="value">The value to set to the object that has the specified index. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="value"/> is equal to 10.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
        /// </returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            // The indexed property is always named "Item" in C#
            GetIndexProperty().SetValue(RealObject, value, indexes);
            return true;
        }

        // Called when a method is called
        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as calling a method.
        /// </summary>
        /// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, args[0] is equal to 100.</param>
        /// <param name="result">The result of the member invocation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = InvokeMemberOnType(RealObject.GetType(), RealObject, binder.Name, args);
            // Wrap the sub object if necessary. This allows nested anonymous objects to work.
            result = WrapIfNeeded(result);
            return true;
        }

        /// <summary>
        /// Provides implementation for type conversion operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.
        /// </summary>
        /// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Type returns the <see cref="T:System.String"/> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
        /// <param name="result">The result of the type conversion operation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = Convert.ChangeType(RealObject, binder.Type);
            return true;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return RealObject.ToString();
        }

        private IProperty GetIndexProperty()
        {
            // The index property is always named "Item" in C#
            return GetProperty("Item");
        }

        private IProperty GetProperty(string propertyName)
        {
            // Get the list of properties and fields for this type
            var typeProperties = GetTypeProperties(RealObject.GetType());
            // Look for the one we want
            IProperty property;
            if (typeProperties.TryGetValue(propertyName, out property))
                return property;
            // The property doesn't exist
            // Get a list of supported properties and fields and show them as part of the exception message
            // For fields, skip the auto property backing fields (which name start with <)
            var propNames = typeProperties.Keys.Where(name => name[0] != '<').OrderBy(name => name);
            throw new ArgumentException(string.Format("The property {0} doesn't exist on type {1}. Supported properties are: {2}", propertyName, RealObject.GetType(), string.Join(", ", propNames)));
        }

        private static IDictionary<string, IProperty> GetTypeProperties(Type type)
        {
            // First, check if we already have it cached
            IDictionary<string, IProperty> typeProperties;
            if (_propertiesOnType.TryGetValue(type, out typeProperties))
                return typeProperties;
            // Not cache, so we need to build it
            typeProperties = new ConcurrentDictionary<string, IProperty>();
            // First, add all the properties
            foreach (PropertyInfo prop in type.GetProperties(bindingFlags).Where(p => p.DeclaringType == type))
                typeProperties[prop.Name] = new Property() { PropertyInfo = prop };
            // Now, add all the fields
            foreach (FieldInfo field in type.GetFields(bindingFlags).Where(p => p.DeclaringType == type))
                typeProperties[field.Name] = new Field() { FieldInfo = field };
            // Finally, recurse on the base class to add its fields
            if (type.BaseType != null)
                foreach (IProperty prop in GetTypeProperties(type.BaseType).Values)
                    typeProperties[prop.Name] = prop;
            // Cache it for next time
            _propertiesOnType[type] = typeProperties;
            return typeProperties;
        }

        private static object InvokeMemberOnType(Type type, object target, string name, object[] args)
        {
            try
            {
                // Try to incoke the method
                return type.InvokeMember(name, BindingFlags.InvokeMethod | bindingFlags, null, target, args);
            }
            catch (MissingMethodException)
            {
                // If we couldn't find the method, try on the base class
                if (type.BaseType != null)
                    return InvokeMemberOnType(type.BaseType, target, name, args);
                // allow methods to not exist
                return null;
            }
        }
    }
}
#endif