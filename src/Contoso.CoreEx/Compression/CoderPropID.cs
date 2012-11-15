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
namespace Contoso.Compression
{
    /// <summary>
    /// Provides the fields that represent properties idenitifiers for compressing.
    /// </summary>
    public enum CoderPropID
    {
        /// <summary>
        /// Specifies default property.
        /// </summary>
        DefaultProp = 0,
        /// <summary>
        /// Specifies size of dictionary.
        /// </summary>
        DictionarySize,
        /// <summary>
        /// Specifies size of memory for PPM*.
        /// </summary>
        UsedMemorySize,
        /// <summary>
        /// Specifies order for PPM methods.
        /// </summary>
        Order,
        /// <summary>
        /// Specifies Block Size.
        /// </summary>
        BlockSize,
        /// <summary>
        /// Specifies number of postion state bits for LZMA (0 &lt;= x &lt;= 4).
        /// </summary>
        PosStateBits,
        /// <summary>
        /// Specifies number of literal context bits for LZMA (0 &lt;= x &lt;= 8).
        /// </summary>
        LitContextBits,
        /// <summary>
        /// Specifies number of literal position bits for LZMA (0 &lt;= x &lt;= 4).
        /// </summary>
        LitPosBits,
        /// <summary>
        /// Specifies number of fast bytes for LZ*.
        /// </summary>
        NumFastBytes,
        /// <summary>
        /// Specifies match finder. LZMA: "BT2", "BT4" or "BT4B".
        /// </summary>
        MatchFinder,
        /// <summary>
        /// Specifies the number of match finder cyckes.
        /// </summary>
        MatchFinderCycles,
        /// <summary>
        /// Specifies number of passes.
        /// </summary>
        NumPasses,
        /// <summary>
        /// Specifies number of algorithm.
        /// </summary>
        Algorithm,
        /// <summary>
        /// Specifies the number of threads.
        /// </summary>
        NumThreads,
        /// <summary>
        /// Specifies mode with end marker.
        /// </summary>
        EndMarker
    }
}
