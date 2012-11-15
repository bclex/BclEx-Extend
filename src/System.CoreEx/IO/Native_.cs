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
using Microsoft.Win32;
using System.Runtime.InteropServices;
namespace System.IO
{
    /// <summary>
    /// Native_
    /// </summary>
    internal static class Native_
    {
        public const uint SYMBLOC_LINK_FLAG_FILE = 0x0;
        public const uint SYMBLOC_LINK_FLAG_DIRECTORY = 0x1;

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, uint dwFlags);
    }
}