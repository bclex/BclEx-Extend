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
#if EXPERIMENTAL
using System.Collections.Generic;
namespace System.Patterns.Schema
{
    /// <summary>
    /// UriSchemaBase
    /// </summary>
    public abstract class UriSchemaBase
    {
        public class CreateUriAttrib
        {
            //    public const string NoUrlRoot = "noUrlRoot";
            //    public const string NoUrlState = "noUrlState";
        }

        public class CreateVirtualPathAttrib { }

        public abstract UriContextBase ParseUri(Uri uri);
        public abstract Dictionary<Type, UriPartBase> Parts { get; protected set; }
        public abstract string CreatePath(UriContextBase context, string uri, Nparams param);
        public abstract Uri CreateUri(UriContextBase context, Uri uri, Nparams param);
        public abstract string CreateVirtualPath(UriContextBase context, string virtualPath, Nparams param);
        //public abstract Action<IUriPartScanner> OnOverflow { get; set; }

        #region FluentConfig

        public abstract UriSchemaBase AddUriPart<T>(T part)
            where T : UriPartBase;
        public abstract UriSchemaBase MakeReadOnly();

        #endregion
    }
}
#endif