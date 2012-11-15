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
#if xEXPERIMENTAL
namespace System.Primitives
{
    /// <summary>
    /// Abstract class used as a base for all text merging types in Instinct.
    /// </summary>
    public interface ITextProcess
    {
        string Process(string[] text, Nattrib attrib);
        string[] Tokenize(string text, Nattrib attrib);
    }

    /// <summary>
    /// Abstract class used as a base for all text merging types in Instinct.
    /// </summary>
    public abstract class TextProcessBase : SimpleFactoryBase<TextProcessBase>, ITextProcess
    {
        /// <summary>
        /// Creates the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected static TextProcessBase Create<T>(IAppUnit appUnit)
            where T : TextProcessBase
        {
            return ServiceLocator.Resolve<T>();
        }

        /// <summary>
        /// Decodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Decoded <paramref name="text"/> value.</returns>
        public string Process(object[] text) { return (text == null ? string.Empty : Process(ConvertEx.ToStrings<object>(text), null)); }
        /// <summary>
        /// Decodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Decoded <paramref name="text"/> value.</returns>
        public string Process(string[] text) { return (text == null ? string.Empty : Process(text, null)); }
        /// <summary>
        /// Decodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="attrib">The attributes.</param>
        /// <returns>Decoded <paramref name="text"/> value.</returns>
        public string Process(object[] text, Nattrib attrib) { return (text == null ? string.Empty : Process(ConvertEx.ToStrings<object>(text), attrib)); }
        /// <summary>
        /// Decodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="attrib">The attributes.</param>
        /// <returns>Decoded <paramref name="text"/> value.</returns>
        public virtual string Process(string[] text, Nattrib attrib)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Encodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Encoded <paramref name="text"/> value.</returns>
        public string[] Tokenize(object text) { return (text == null ? null : Tokenize(text.ToString(), null)); }
        /// <summary>
        /// Encodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Encoded <paramref name="text"/> value.</returns>
        public virtual string[] Tokenize(string text) { return (text == null ? null : Tokenize(text, null)); }
        /// <summary>
        /// Encodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="attrib">The attrib.</param>
        /// <returns>Encoded <paramref name="text"/> value.</returns>
        public string[] Tokenize(object text, Nattrib attrib) { return (text == null ? null : Tokenize(text.ToString(), attrib)); }
        /// <summary>
        /// Encodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="attrib">The attrib.</param>
        /// <returns>Encoded <paramref name="text"/> value.</returns>
        public virtual string[] Tokenize(string text, Nattrib attrib)
        {
            throw new NotSupportedException();
        }
    }
}
#endif