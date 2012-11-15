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
using System.Reflection;
namespace System.Configuration
{
    /// <summary>
    /// ConfigurationElementSynHelper
    /// </summary>
    internal static class ConfigurationElementSynHelper
    {
        internal delegate bool FastPropertiesFromType(Type type, out ConfigurationPropertyCollection result);
        //
        internal static readonly FastPropertiesFromType PropertiesFromType = (FastPropertiesFromType)Delegate.CreateDelegate(typeof(FastPropertiesFromType), typeof(ConfigurationElement).GetMethod("PropertiesFromType", BindingFlags.NonPublic | BindingFlags.Static));
        internal static readonly MethodInfo ApplyInstanceAttributesMethod = typeof(ConfigurationElement).GetMethod("ApplyInstanceAttributes", BindingFlags.NonPublic | BindingFlags.Static);
        internal static readonly MethodInfo ApplyValidatorsRecursiveMethod = typeof(ConfigurationElement).GetMethod("ApplyValidatorsRecursive", BindingFlags.NonPublic | BindingFlags.Static);
        internal static readonly MethodInfo SetPropertyValueMethod = typeof(ConfigurationElement).GetMethod("SetPropertyValue", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly PropertyInfo ItemProperty = typeof(ConfigurationElement).GetProperty("Item", BindingFlags.NonPublic | BindingFlags.Instance, null, typeof(object), new[] { typeof(ConfigurationProperty) }, null);
        internal static readonly PropertyInfo PropertiesProperty = typeof(ConfigurationElement).GetProperty("Properties", BindingFlags.NonPublic | BindingFlags.Instance, null, typeof(ConfigurationPropertyCollection), new Type[0], null);
        //public static TSyn ToSyn<T, TSyn>(this T element)
        //    where TSyn : ConfigurationElementSyn<T>, new()
        //    where T : ConfigurationElement
        //{
        //    if (element == null)
        //        return null;
        //    var syn = new TSyn();
        //    syn.Syn = element;
        //    return syn;
        //}
    }
}
