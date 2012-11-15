using System.Collections;
using System.Runtime.InteropServices;

namespace System.DirectoryServices
{
    /// <summary>
    /// DistinguishedName
    /// </summary>
    public class DistinguishedName
    {
        private Component[] _components;

        /// <summary>
        /// Component
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Component
        {
            /// <summary>
            /// 
            /// </summary>
            public string Name;
            /// <summary>
            /// 
            /// </summary>
            public string Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistinguishedName"/> class.
        /// </summary>
        /// <param name="dn">The dn.</param>
        public DistinguishedName(string dn)
        {
            _components = GetDNComponents(dn);
        }

        /// <summary>
        /// Equalses the specified dn.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public bool Equals(DistinguishedName dn)
        {
            if (dn == null || _components.Length != dn.Components.Length)
                return false;
            for (var i = 0; i < _components.Length; i++)
                if (!string.Equals(_components[i].Name, dn.Components[i].Name) || !string.Equals(_components[i].Value, dn.Components[i].Value))
                    return false;
            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj)
        {
            return (obj != null && obj is DistinguishedName && this.Equals((DistinguishedName)obj));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var num = 0;
            for (var i = 0; i < _components.Length; i++)
                num = (num + _components[i].Name.ToUpperInvariant().GetHashCode()) + _components[i].Value.ToUpperInvariant().GetHashCode();
            return num;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var str = _components[0].Name + "=" + _components[0].Value;
            for (var i = 1; i < _components.Length; i++)
                str = str + "," + _components[i].Name + "=" + _components[i].Value;
            return str;
        }

        /// <summary>
        /// Gets the components.
        /// </summary>
        public Component[] Components
        {
            get { return _components; }
        }

        internal static Component[] GetDNComponents(string distinguishedName)
        {
            var strArray = Split(distinguishedName, ',');
            var componentArray = new Component[strArray.GetLength(0)];
            for (var i = 0; i < strArray.GetLength(0); i++)
            {
                var strArray2 = Split(strArray[i], '=');
                if (strArray2.GetLength(0) != 2)
                    throw new ArgumentException(Local.InvalidDNFormat, "distinguishedName");
                componentArray[i].Name = strArray2[0].Trim();
                if (componentArray[i].Name.Length == 0)
                    throw new ArgumentException(Local.InvalidDNFormat, "distinguishedName");
                componentArray[i].Value = strArray2[1].Trim();
                if (componentArray[i].Value.Length == 0)
                    throw new ArgumentException(Local.InvalidDNFormat, "distinguishedName");
            }
            return componentArray;
        }

        internal static string[] Split(string distinguishedName, char delim)
        {
            var flag = false;
            var ch2 = '"';
            var ch3 = '\\';
            var startIndex = 0;
            var list = new ArrayList();
            for (var i = 0; i < distinguishedName.Length; i++)
            {
                var ch = distinguishedName[i];
                if (ch == ch2)
                    flag = !flag;
                else if (ch == ch3)
                {
                    if (i < (distinguishedName.Length - 1))
                        i++;
                }
                else if (!flag && ch == delim)
                {
                    list.Add(distinguishedName.Substring(startIndex, i - startIndex));
                    startIndex = i + 1;
                }
                if (i == (distinguishedName.Length - 1))
                {
                    if (flag)
                        throw new ArgumentException(Local.InvalidDNFormat, "distinguishedName");
                    list.Add(distinguishedName.Substring(startIndex, (i - startIndex) + 1));
                }
            }
            var strArray = new string[list.Count];
            for (var j = 0; j < list.Count; j++)
                strArray[j] = (string)list[j];
            return strArray;
        }
    }
}