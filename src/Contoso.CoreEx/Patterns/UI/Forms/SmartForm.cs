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
namespace Contoso.Patterns.UI.Forms
{
    /// <summary>
    /// Represents a advanced form processing type that maintains state, processing functionality, and TextColumn/Element support.
    /// </summary>
    public class SmartForm
    {
        private Dictionary<string, string> _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, string> _replaceTags = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified key.
        /// </summary>
        public string this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");
                string value;
                return (_values.TryGetValue(key, out value) ? value : string.Empty);
            }
            set
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");
                _values[key] = (value ?? string.Empty);
            }
        }

        /// <summary>
        /// Executes the contract.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <param name="method">The method.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public object ExecuteContract(ISmartFormContract contract, string method, params object[] args)
        {
            if (contract == null)
                throw new ArgumentNullException("contract");
            return contract.Execute(this, method, args);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public bool HasError { get; set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public Dictionary<string, string> Values
        {
            get { return _values; }
        }

        /// <summary>
        /// Gets the replace tags.
        /// </summary>
        public Dictionary<string, string> ReplaceTags
        {
            get { return _replaceTags; }
        }

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>
        /// The meta.
        /// </value>
        public SmartFormMeta Meta { get; protected set; }

        /// <summary>
        /// Creates the merged text.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string CreateMergedText(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            string value;
            if (!_values.TryGetValue(key, out value))
                return string.Empty;
            if (!string.IsNullOrEmpty(value))
                foreach (string replaceTagKey in _replaceTags.Keys)
                    value = value.Replace("[:" + replaceTagKey + ":]", _replaceTags[replaceTagKey]);
            return value;
        }
    }
}
