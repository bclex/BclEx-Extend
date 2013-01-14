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
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading;

namespace System
{
    internal partial class SR
    {
        private static SR _loader;
        private ResourceManager _resources;

        internal SR()
        {
            _resources = new ResourceManager("System", base.GetType().Assembly);
        }

        private static SR GetLoader()
        {
            if (_loader == null)
            {
                var sr = new SR();
                Interlocked.CompareExchange<SR>(ref _loader, sr, null);
            }
            return _loader;
        }

        public static object GetObject(string name)
        {
            var loader = GetLoader();
            return (loader == null ? null : loader._resources.GetObject(name, Culture));
        }

        public static string GetString(string name)
        {
            var loader = GetLoader();
            return (loader == null ? null : loader._resources.GetString(name, Culture));
        }
        public static string GetString(string name, out bool usedFallback)
        {
            usedFallback = false;
            return GetString(name);
        }
        public static string GetString(string name, params object[] args)
        {
            var loader = GetLoader();
            if (loader == null)
                return null;
            var format = loader._resources.GetString(name, Culture);
            if (args == null || args.Length <= 0)
                return format;
            for (int i = 0; i < args.Length; i++)
            {
                var str2 = (args[i] as string);
                if (str2 != null && str2.Length > 0x400)
                    args[i] = str2.Substring(0, 0x3fd) + "...";
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static CultureInfo Culture
        {
            get { return null; }
        }

        public static ResourceManager Resources
        {
            get { return GetLoader()._resources; }
        }
    }
}

