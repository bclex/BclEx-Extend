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
namespace System.Collections.Generic
{
    /// <summary>
    /// DictionaryExtensions
    /// </summary>
    public static class DictionaryExtensions
    {
        //public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int index, TValue defaultValue)
        //{
        //    return defaultValue;
        //}

        /// <summary>
        /// Appends the specified value array to the underlying instance of <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="values">String array of default values to use in initializing the collection.</param>
        /// <returns></returns>
        public static Dictionary<string, string> Insert(this Dictionary<string, string> dictionary, string[] values)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if ((values == null) || (values.Length == 0))
                return dictionary;
            foreach (string value in values)
            {
                if (value == null)
                    throw new NullReferenceException(Local.InvalidArrayNullItem);
                if (value.Length > 0)
                {
                    int valueEqualIndex = value.IndexOf('=');
                    if (valueEqualIndex >= 0)
                        dictionary[value.Substring(0, valueEqualIndex)] = value.Substring(valueEqualIndex + 1);
                    else
                        dictionary[value] = value;
                }
            }
            return dictionary;
        }
        /// <summary>
        /// Appends the specified value array to the underlying instance of <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="values">String array of default values to use in initializing the collection.</param>
        /// <param name="startIndex">The initial index value to use as a starting point when processing the specified <c>values</c></param>
        /// <returns></returns>
        public static Dictionary<string, string> Insert(this Dictionary<string, string> dictionary, string[] values, int startIndex)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");
            if ((values == null) || (startIndex >= values.Length))
                return dictionary;
            for (int valueIndex = startIndex; valueIndex < values.Length; valueIndex++)
            {
                string value = values[valueIndex];
                if (value == null)
                    throw new NullReferenceException(Local.InvalidArrayNullItem);
                if (value.Length > 0)
                {
                    int valueEqualIndex = value.IndexOf('=');
                    if (valueEqualIndex >= 0)
                        dictionary[value.Substring(0, valueEqualIndex)] = value.Substring(valueEqualIndex + 1);
                    else
                        dictionary[value] = value;
                }
            }
            return dictionary;
        }
        /// <summary>
        /// Appends the specified value array to the underlying instance of <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="values">String array of default values to use in initializing the collection.</param>
        /// <param name="startIndex">The initial index value to use as a starting point when processing the specified <c>values</c></param>
        /// <param name="count">The number of value from <c>values</c> to process, starting at the position indicated by <c>startIndex</c>.</param>
        /// <returns></returns>
        public static Dictionary<string, string> Insert(this Dictionary<string, string> dictionary, string[] values, int startIndex, int count)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if ((values == null) || (startIndex >= count))
                return dictionary;
            if (count > values.Length)
                count = values.Length;
            for (int valueIndex = startIndex; valueIndex < count; valueIndex++)
            {
                string value = values[valueIndex];
                if (value == null)
                    throw new InvalidOperationException(Local.InvalidArrayNullItem);
                if (value.Length > 0)
                {
                    int valueEqualIndex = value.IndexOf('=');
                    if (valueEqualIndex >= 0)
                        dictionary[value.Substring(0, valueEqualIndex)] = value.Substring(valueEqualIndex + 1);
                    else
                        dictionary[value] = value;
                }
            }
            return dictionary;
        }
        /// <summary>
        /// Appends the specified value array to the underlying instance of <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="values">String array of default values to use in initializing the collection.</param>
        /// <param name="namespace">A namespace prefix to search when setting initial values.</param>
        /// <returns></returns>
        /// <remarks>When processing <c>values</c>, the underlying instance of <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>
        /// is search for values. The <c>namespaceKey</c> + ":" is used as a prefix to the keys found in the internal dictionary to determine a match for initializing to the value provided by <c>values.</c>
        /// </remarks>
        public static Dictionary<string, string> Insert(this Dictionary<string, string> dictionary, string[] values, string @namespace)
        {
            if (string.IsNullOrEmpty(@namespace))
            {
                Insert(dictionary, values);
                return dictionary;
            }
            if (values == null)
                return dictionary;
            @namespace += ":";
            int namespaceKeyLength = @namespace.Length;
            int valueDataIndex;
            foreach (string value in values)
            {
                if (value == null)
                    throw new InvalidOperationException(Local.InvalidArrayNullItem);
                if ((value.Length > -1) && ((valueDataIndex = value.IndexOf(@namespace, StringComparison.OrdinalIgnoreCase)) > -1))
                {
                    valueDataIndex += namespaceKeyLength;
                    int valueEqualIndex = value.IndexOf('=', valueDataIndex);
                    if (valueEqualIndex >= 0)
                        dictionary[value.Substring(valueDataIndex, valueEqualIndex - valueDataIndex)] = value.Substring(valueDataIndex + valueEqualIndex + 1);
                    else
                    {
                        string value2 = value.Substring(valueDataIndex);
                        dictionary[value2] = value2;
                    }
                }
            }
            return dictionary;
        }
        /// <summary>
        /// Appends the specified value array to the underlying instance of<see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="values">String array of default values to use in initializing the collection.</param>
        /// <param name="namespace">A namespace prefix to search when setting initial values.</param>
        /// <param name="startIndex">The initial index value to use as a starting point when processing the specified <c>values</c></param>
        /// <param name="count">The number of value from <c>values</c> to process, starting at the position indicated by <c>startIndex</c>.</param>
        /// <returns></returns>
        /// <remarks>When processing <c>values</c>, the underlying instance of <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>
        /// is search for values. The <c>namespaceKey</c> + ":" is used as a prefix to the keys found in the internal dictionary to determine a match for initializing to the value provided by <c>values.</c>
        /// </remarks>
        public static Dictionary<string, string> Insert(this Dictionary<string, string> dictionary, string[] values, string @namespace, int startIndex, int count)
        {
            if (string.IsNullOrEmpty(@namespace))
            {
                Insert(dictionary, values, startIndex, count);
                return dictionary;
            }
            if ((values == null) || (startIndex >= count))
                return dictionary;
            if (count > values.Length)
                count = values.Length;
            @namespace += ":";
            int namespaceKeyLength = @namespace.Length;
            int valueDataIndex;
            for (int valueIndex = startIndex; valueIndex < count; valueIndex++)
            {
                string value = values[valueIndex];
                if (value == null)
                    throw new InvalidOperationException(Local.InvalidArrayNullItem);
                if ((value.Length > -1) && ((valueDataIndex = value.IndexOf(@namespace, StringComparison.OrdinalIgnoreCase)) > -1))
                {
                    valueDataIndex += namespaceKeyLength;
                    int valueEqualIndex = value.IndexOf('=', valueDataIndex);
                    if (valueEqualIndex >= 0)
                        dictionary[value.Substring(valueDataIndex, valueEqualIndex - valueDataIndex)] = value.Substring(valueDataIndex + valueEqualIndex + 1);
                    else
                    {
                        value = value.Substring(valueDataIndex);
                        dictionary[value] = value;
                    }
                }
            }
            return dictionary;
        }
        /// <summary>
        /// Appends the specified hash to the underlying instance of <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="sourceDictionary">The source dictionary.</param>
        /// <returns></returns>
        public static Dictionary<string, string> Insert(this Dictionary<string, string> dictionary, IDictionary<string, string> sourceDictionary)
        {
            if (sourceDictionary == null)
                return dictionary;
            foreach (string key in sourceDictionary.Keys)
                dictionary[key] = sourceDictionary[key];
            return dictionary;
        }

        /// <summary>
        /// Gets the bit.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool GetBool(this Dictionary<string, string> dictionary, string key)
        {
            string value;
            return ((dictionary.TryGetValue(key, out value)) && (string.Compare(value, key, StringComparison.OrdinalIgnoreCase) == 0));
        }

        /// <summary>
        /// Sets the bit.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void SetBool(this Dictionary<string, string> dictionary, string key, bool value)
        {
            if (value)
                dictionary[key] = key;
            else if (dictionary.ContainsKey(key))
                dictionary.Remove(key);
        }

        /// <summary>
        /// Slices or removes all keys prefixed with the provided <c>namespaceKey</c> value from the underlying
        /// <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> instance that contains
        /// the values stored within the collection object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="namespace">The namespace to target.</param>
        /// <returns></returns>
        public static T Slice<T>(this Dictionary<string, string> dictionary, string @namespace)
            where T : Dictionary<string, string>, new() { return Slice<T>(dictionary, @namespace, false); }
        /// <summary>
        /// Slices or removes all keys prefixed with the provided <c>namespaceKey</c> value from the underlying
        /// <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> instance that contains
        /// the values stored within the collection object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="namespace">The namespace to target.</param>
        /// <param name="returnNullIfNoMatch">if set to <c>true</c> [return null if no match].</param>
        /// <returns></returns>
        public static T Slice<T>(this Dictionary<string, string> dictionary, string @namespace, bool returnNullIfNoMatch)
            where T : Dictionary<string, string>, new()
        {
            T slicedDictionary = null;
            if (@namespace.Length > 0)
            {
                @namespace += ":";
                int namespaceLength = @namespace.Length;
                //+ return namespace set
                foreach (string key in new List<string>(dictionary.Keys))
                    if (key.StartsWith(@namespace))
                    {
                        if (slicedDictionary == null)
                            slicedDictionary = new T();
                        // add & remove
                        string value = dictionary[key];
                        if (key != value)
                            slicedDictionary[key.Substring(namespaceLength)] = value;
                        else
                        {
                            // isbit
                            value = key.Substring(namespaceLength);
                            slicedDictionary[value] = value;
                        }
                        dictionary.Remove(key);
                    }
            }
            // return root-namespace set
            else
                foreach (string key in new List<string>(dictionary.Keys))
                    if (key.IndexOf(":") == -1)
                    {
                        slicedDictionary[key] = dictionary[key];
                        dictionary.Remove(key);
                    }
            return ((slicedDictionary != null) || (returnNullIfNoMatch) ? slicedDictionary : new T());
        }
        /// <summary>
        /// Slices or removes all keys prefixed with the provided <c>namespaceKey</c> value from the underlying
        /// <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> instance that contains
        /// the values stored within the collection object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="namespace">The namespace to target.</param>
        /// <param name="isReturnNullIfNoMatch">TODO.</param>
        /// <param name="valueReplacerIfStartsWith">The value replacer if starts with.</param>
        /// <param name="valueReplacer">The value replacer.</param>
        /// <returns></returns>
        public static T Slice<T>(this Dictionary<string, string> dictionary, string @namespace, bool isReturnNullIfNoMatch, string valueReplacerIfStartsWith, Func<string, string> valueReplacer)
            where T : Dictionary<string, string>, new()
        {
            T slicedDictionary = null;
            if (@namespace.Length > 0)
            {
                @namespace += ":";
                int namespaceLength = @namespace.Length;
                string compositeNamespace = valueReplacerIfStartsWith + @namespace;
                int compositeNamespaceLength = compositeNamespace.Length;
                // return namespace set
                foreach (string key in new List<string>(dictionary.Keys))
                    if (key.StartsWith(compositeNamespace))
                    {
                        if (slicedDictionary == null)
                            slicedDictionary = new T();
                        // add & remove
                        string value = valueReplacer(dictionary[key]);
                        if (key != value)
                            slicedDictionary[key.Substring(compositeNamespaceLength)] = value;
                        else
                        {
                            // isbit
                            value = key.Substring(compositeNamespaceLength);
                            slicedDictionary[value] = value;
                        }
                        dictionary.Remove(key);
                    }
                    else if (key.StartsWith(@namespace))
                    {
                        if (slicedDictionary == null)
                            slicedDictionary = new T();
                        // add and remove
                        string value = dictionary[key];
                        if (key != value)
                            slicedDictionary[key.Substring(namespaceLength)] = value;
                        else
                        {
                            // isbit
                            value = key.Substring(namespaceLength);
                            slicedDictionary[value] = value;
                        }
                        dictionary.Remove(key);
                    }
            }
            // return root-namespace set
            else
                foreach (string key in new List<string>(dictionary.Keys))
                    if (key.IndexOf(":") == -1)
                    {
                        slicedDictionary[key] = (!key.StartsWith(valueReplacerIfStartsWith) ? dictionary[key] : valueReplacer(dictionary[key]));
                        dictionary.Remove(key);
                    }
            return ((slicedDictionary != null) || (isReturnNullIfNoMatch) ? slicedDictionary : new T());
        }

        /// <summary>
        /// Looks for the item in the underlying hash and if exists, removes it and returns it.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool SliceBool(this Dictionary<string, string> dictionary, string key)
        {
            string value;
            if (dictionary.TryGetValue(key, out value))
            {
                bool wasSliced = (string.Compare(value, key, StringComparison.OrdinalIgnoreCase) == 0);
                dictionary.Remove(key);
                return wasSliced;
            }
            return false;
        }
        /// <summary>
        /// Looks for the item in the underlying hash and if exists, removes it and returns it, or returns default value if not found.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static bool SliceBool(this Dictionary<string, string> dictionary, string key, bool defaultValue)
        {
            string value;
            if (dictionary.TryGetValue(key, out value))
            {
                bool wasSliced = (string.Compare(value, key, StringComparison.OrdinalIgnoreCase) == 0);
                dictionary.Remove(key);
                return wasSliced;
            }
            return defaultValue;
        }

        /// <summary>
        /// Converts the underlying collection of string representations.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>
        /// Returns a string array of values following the format "<c>key</c>=<c>value</c>"
        /// </returns>
        public static string[] ToStringArray(this Dictionary<string, string> dictionary)
        {
            var keys = dictionary.Keys;
            int keyIndex = 0;
            string[] array = new string[keys.Count];
            foreach (var entry in dictionary)
                array[keyIndex++] = entry.Key + "=" + entry.Value;
            return array;
        }
    }
}