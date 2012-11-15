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
using System.IO;
namespace Contoso.Compression
{
    /// <summary>
    /// ICoder
    /// </summary>
    public interface ICoder
    {
        /// <summary>
        /// Codes the specified in stream.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        /// <param name="outStream">The out stream.</param>
        /// <param name="inSize">Size of the in.</param>
        /// <param name="outSize">Size of the out.</param>
        /// <param name="progress">The progress.</param>
        void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress);
    }
}

//    uint dictionarySize = 16;
//    var encoder = new Encoder();
//    CoderPropID[] propIDs = 
//    { 
//        CoderPropID.DictionarySize,
//    };
//    object[] properties = 
//    {
//        (int)(dictionarySize),
//    };

//    uint kBufferSize = dictionarySize + kAdditionalSize;
//    uint kCompressedBufferSize = (kBufferSize / 2) + kCompressedAdditionalSize;
//    encoder.SetCoderProperties(propIDs, properties);
//    var propStream = new MemoryStream();
//    encoder.WriteCoderProperties(propStream);
//    byte[] propArray = propStream.ToArray();