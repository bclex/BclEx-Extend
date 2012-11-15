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
using System.Text;
namespace Contoso.Text.Html
{
    /// <summary>
    /// HtmlStringReader
    /// </summary>
    public class HtmlStringReader : IDisposable
    {
        private string _text;
        private int _textLength;
        private StringBuilder _b;
        private int _startIndex = 0;
        private int _valueStartIndex;
        private int _valueEndIndex;
        private int _openIndex = -1;
        private int _closeIndex;
        private string _lastParseElementName;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlStringReader"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public HtmlStringReader(string text)
            : this(text, new StringBuilder()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlStringReader"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="b">The b.</param>
        public HtmlStringReader(string text, StringBuilder b)
        {
            _text = text;
            _textLength = text.Length;
            _b = b;
        }

        /// <summary>
        /// Appends the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Append(string value)
        {
            _b.Append(value);
        }

        /// <summary>
        /// Gets the found element.
        /// </summary>
        /// <returns></returns>
        public string GetFoundElement()
        {
            if (_openIndex == -1)
                throw new InvalidOperationException();
            // find matching > to the open anchor tag
            int openCloseBraceIndex = _text.IndexOf(">", _openIndex);
            if (openCloseBraceIndex == -1)
                openCloseBraceIndex = _textLength;
            // extract tag
            return _text.Substring(_openIndex, openCloseBraceIndex - _openIndex) + ">";
        }

        /// <summary>
        /// Determines whether [is attribute in element] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>
        /// 	<c>true</c> if [is attribute in element] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAttributeInElement(string element, string attribute)
        {
            int attributeLength = attribute.Length;
            int targetStartKey = 0;
            // if the tag contains the text "attribute"
            int targetAttribIndex;
            while ((targetAttribIndex = element.IndexOf(attribute, targetStartKey, StringComparison.OrdinalIgnoreCase)) > -1)
                // determine whether "attribute" is inside of a string
                if (((element.Substring(0, targetAttribIndex).Length - element.Substring(0, targetAttribIndex).Replace("\"", string.Empty).Length) % 2) == 0)
                    // "attribute" is not inside of a string -- attribute exists
                    return true;
                else
                    // skip "attribute" text and look for next match
                    targetStartKey = targetAttribIndex + attributeLength;
            return false;
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int IndexOf(string value)
        {
            return _text.IndexOf(value, _startIndex, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Sets the search value.
        /// </summary>
        /// <param name="valueStartIndex">Start index of the value.</param>
        /// <param name="valueEndIndex">End index of the value.</param>
        public void SetSearchValue(int valueStartIndex, int valueEndIndex)
        {
            _valueStartIndex = valueStartIndex;
            _valueEndIndex = valueEndIndex;
        }

        /// <summary>
        /// Searches for any element.
        /// </summary>
        /// <returns></returns>
        public bool SearchIfInElement()
        {
            _lastParseElementName = null;
            // look for the closest open tag (<) to the left of the url 
            _openIndex = _text.LastIndexOf("<", _valueStartIndex, _valueStartIndex - _startIndex + 1, StringComparison.OrdinalIgnoreCase);
            // open tag found
            if (_openIndex > -1)
            {
                // find the corresponding close tag ([</][/>]) to the left of the url
                _closeIndex = _text.LastIndexOf("</", _valueStartIndex, _valueStartIndex - _openIndex + 1, StringComparison.OrdinalIgnoreCase);
                int close2Index = _text.LastIndexOf("/>", _valueStartIndex, _valueStartIndex - _openIndex + 1, StringComparison.OrdinalIgnoreCase);
                if ((close2Index > -1) && (close2Index < _closeIndex))
                    _closeIndex = close2Index;
                // false:close tag found -- url is not enclosed in an anchor tag
                // true:close tag not found -- url is already linked (enclosed in an anchor tag)
                return (_closeIndex == -1);
            }
            // open tag not found
            else
                return false;
        }

        /// <summary>
        /// Searches for element.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public bool SearchIfInElement(string elementName)
        {
            _lastParseElementName = elementName.ToLowerInvariant();
            // look for the closest open element tag (<a>) to the left of the url 
            _openIndex = _text.LastIndexOf("<" + elementName + " ", _valueStartIndex, _valueStartIndex - _startIndex + 1, StringComparison.OrdinalIgnoreCase);
            // open tag found
            if (_openIndex > -1)
            {
                // find the corresponding close tag (</a>) to the left of the url
                _closeIndex = _text.LastIndexOf("</" + elementName + ">", _valueStartIndex, _valueStartIndex - _openIndex + 1, StringComparison.OrdinalIgnoreCase);
                // false:close tag found -- value is not enclosed in an element tag
                // true:close tag not found -- value is already elemented (enclosed in an anchor tag)
                return (_closeIndex == -1);
            }
            // open tag not found
            else
                return false;
        }

        #region Stream
        /// <summary>
        /// Streams to end of first element.
        /// </summary>
        public void StreamToEndOfFirstElement()
        {
            // find the close tag (>) to the left of the url
            int closeIndex = _text.IndexOf(">", _valueEndIndex, StringComparison.OrdinalIgnoreCase);
            if (closeIndex > -1)
            {
                _b.Append(_text.Substring(_startIndex, closeIndex - _startIndex + 1));
                Advance(closeIndex + 1);
            }
            else
            {
                _b.Append(_text.Substring(_startIndex));
                Advance(_textLength);
            }
        }

        /// <summary>
        /// Streams the found element fragment.
        /// </summary>
        /// <param name="elementBuilder">The element builder.</param>
        public void StreamFoundElementFragment(Func<string, string> elementBuilder)
        {
            if (_openIndex == -1)
                throw new InvalidOperationException();
            int elementOffset = _lastParseElementName.Length + 2;
            // look for closing anchor tag
            int closeIndex = _text.IndexOf("</" + _lastParseElementName + ">", _valueEndIndex, StringComparison.OrdinalIgnoreCase);
            // close anchor tag found
            if (closeIndex > -1)
            {
                closeIndex += elementOffset;
                _b.Append(elementBuilder("<" + _lastParseElementName + " " + _text.Substring(_openIndex + elementOffset, closeIndex - _openIndex - elementOffset - 1)));
                Advance(closeIndex + 1);
            }
            // close anchor tag not found
            else
            {
                Advance(_textLength);
                _b.Append(elementBuilder("<" + _lastParseElementName + " " + _text.Substring(_openIndex + elementOffset, _startIndex - _openIndex - elementOffset)));
            }
        }

        /// <summary>
        /// Streams to end of value.
        /// </summary>
        public void StreamToEndOfValue()
        {
            _b.Append(_text.Substring(_startIndex, _valueStartIndex - _startIndex));
            Advance(_valueEndIndex + 1);
        }

        /// <summary>
        /// Advances the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        protected void Advance(int index)
        {
            _startIndex = index;
            _openIndex = -1;
        }

        /// <summary>
        /// Streams to begin element.
        /// </summary>
        public void StreamToBeginElement()
        {
            if (_openIndex == -1)
                throw new InvalidOperationException();
            // stream text to left of url
            _b.Append(_text.Substring(_startIndex, _openIndex - _startIndex));
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            // append trailing text, if any
            if (_startIndex < _textLength)
            {
                _b.Append(_text.Substring(_startIndex));
                Advance(_textLength);
            }
            return _b.ToString();
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
