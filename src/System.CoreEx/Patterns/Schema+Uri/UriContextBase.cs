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
namespace System.Patterns.Schema
{
    /// <summary>
    /// UriContextBase
    /// </summary>
    public abstract class UriContextBase
    {
        private UriSchemaBase _schema;

        public UriContextBase(UriSchemaBase schema, Uri uri)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");
            if (uri == null)
                throw new ArgumentNullException("uri");
            Schema = schema;
            OriginalUri = uri;
            Path = (!uri.IsAbsoluteUri ? uri.OriginalString : uri.LocalPath);
        }

        public abstract string Path { get; protected set; }

        public abstract bool HasOverflowed { get; protected set; }

        public virtual UriSchemaBase Schema
        {
            get { return _schema; }
            protected set
            {
                _schema = value;
                Parts = new UriPart[_schema.Parts.Count];
            }
        }
        public Uri OriginalUri { get; private set; }

        public abstract Uri Uri { get; protected set; }

        #region Create
        public string CreatePath(string uri, Nparams param)
        {
            var schema = Schema;
            if (schema == null)
                throw new NullReferenceException("Schema");
            return schema.CreatePath(this, uri, param);
        }
        public Uri CreateUri(Uri uri, Nparams param)
        {
            var schema = Schema;
            if (schema == null)
                throw new NullReferenceException("Schema");
            return schema.CreateUri(this, uri, param);
        }
        public string CreateVirtualPath(string virtualPath, Nparams param)
        {
            var schema = Schema;
            if (schema == null)
                throw new NullReferenceException("Schema");
            return schema.CreateVirtualPath(this, virtualPath, param);
        }
        #endregion

        protected internal UriPart[] Parts { get; private set; }
    }
}
#endif