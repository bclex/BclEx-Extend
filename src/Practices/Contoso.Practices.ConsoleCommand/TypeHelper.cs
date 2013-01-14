#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Contoso
{
    internal static class TypeHelper
    {
        public static object ChangeType(object value, Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (value == null)
            {
                if (TypeAllowsNull(type))
                    return null;
                return Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
            }
            type = RemoveNullableFromType(type);
            if (value.GetType() == type)
                return value;
            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(value.GetType()))
                return converter.ConvertFrom(value);
            var converter2 = TypeDescriptor.GetConverter(value.GetType());
            if (!converter2.CanConvertTo(type))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Local.UnableToConvertTypeError, new object[] { value.GetType(), type }));
            return converter2.ConvertTo(value, type);
        }

        //public static Type GetDictionaryType(Type type) { return GetInterfaceType(type, typeof(IDictionary<,>)); }
        //public static Type GetGenericCollectionType(Type type) { return GetInterfaceType(type, typeof(ICollection<>)); }
        public static Type GetInterface(this Type type, params Type[] interfaceTypes) { return (type.IsGenericType && interfaceTypes.Contains(type.GetGenericTypeDefinition()) ? type : type.GetInterfaces().SingleOrDefault(x => x.IsGenericType && interfaceTypes.Contains(type.GetGenericTypeDefinition()))); }
        //public static bool IsEnumProperty(this PropertyInfo property) { return property.PropertyType.IsEnum; }
        //public static bool IsKeyValueProperty(this PropertyInfo property) { return (GetInterfaceType(property.PropertyType, typeof(IDictionary<,>)) != null); }
        //public static bool IsMultiValuedProperty(this PropertyInfo property) { return (GetInterfaceType(property.PropertyType, typeof(ICollection<>)) == null ? IsKeyValueProperty(property) : true); }
        public static Type RemoveNullableFromType(Type type) { return (Nullable.GetUnderlyingType(type) ?? type); }
        public static bool TypeAllowsNull(Type type) { return (Nullable.GetUnderlyingType(type) == null ? !type.IsValueType : true); }
    }
}

