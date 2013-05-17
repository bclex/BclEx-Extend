//#region Foreign-License
//// .Net40 Kludge
//#endregion
//#if !CLR4
//using System.Globalization;
//using System.Runtime.InteropServices;
//namespace System
//{
//    [Serializable, ComVisible(true)]
//    public sealed class Version : ICloneable, IComparable, IComparable<Version>, IEquatable<Version>
//    {
//        private int _Build;
//        private int _Major;
//        private int _Minor;
//        private int _Revision;

//        public Version()
//        {
//            _Build = -1;
//            _Revision = -1;
//            _Major = 0;
//            _Minor = 0;
//        }

//        public Version(string version)
//        {
//            _Build = -1;
//            _Revision = -1;
//            var version2 = Parse(version);
//            _Major = version2.Major;
//            _Minor = version2.Minor;
//            _Build = version2.Build;
//            _Revision = version2.Revision;
//        }

//        public Version(int major, int minor)
//        {
//            _Build = -1;
//            _Revision = -1;
//            if (major < 0)
//                throw new ArgumentOutOfRangeException("major", SR.GetResourceString("ArgumentOutOfRange_Version"));
//            if (minor < 0)
//                throw new ArgumentOutOfRangeException("minor", SR.GetResourceString("ArgumentOutOfRange_Version"));
//            _Major = major;
//            _Minor = minor;
//        }

//        public Version(int major, int minor, int build)
//        {
//            _Build = -1;
//            _Revision = -1;
//            if (major < 0)
//                throw new ArgumentOutOfRangeException("major", SR.GetResourceString("ArgumentOutOfRange_Version"));
//            if (minor < 0)
//                throw new ArgumentOutOfRangeException("minor", SR.GetResourceString("ArgumentOutOfRange_Version"));
//            if (build < 0)
//                throw new ArgumentOutOfRangeException("build", SR.GetResourceString("ArgumentOutOfRange_Version"));
//            _Major = major;
//            _Minor = minor;
//            _Build = build;
//        }

//        public Version(int major, int minor, int build, int revision)
//        {
//            _Build = -1;
//            _Revision = -1;
//            if (major < 0)
//                throw new ArgumentOutOfRangeException("major", SR.GetResourceString("ArgumentOutOfRange_Version"));
//            if (minor < 0)
//                throw new ArgumentOutOfRangeException("minor", SR.GetResourceString("ArgumentOutOfRange_Version"));
//            if (build < 0)
//                throw new ArgumentOutOfRangeException("build", SR.GetResourceString("ArgumentOutOfRange_Version"));
//            if (revision < 0)
//                throw new ArgumentOutOfRangeException("revision", SR.GetResourceString("ArgumentOutOfRange_Version"));
//            _Major = major;
//            _Minor = minor;
//            _Build = build;
//            _Revision = revision;
//        }

//        public object Clone()
//        {
//            var version = new Version();
//            version._Major = _Major;
//            version._Minor = _Minor;
//            version._Build = _Build;
//            version._Revision = _Revision;
//            return version;
//        }

//        public int CompareTo(object version)
//        {
//            if (version == null)
//                return 1;
//            var version2 = version as Version;
//            if (version2 == null)
//                throw new ArgumentException(SR.GetResourceString("Arg_MustBeVersion"));
//            if (_Major != version2._Major)
//            {
//                if (_Major > version2._Major)
//                    return 1;
//                return -1;
//            }
//            if (_Minor != version2._Minor)
//            {
//                if (_Minor > version2._Minor)
//                    return 1;
//                return -1;
//            }
//            if (_Build != version2._Build)
//            {
//                if (_Build > version2._Build)
//                    return 1;
//                return -1;
//            }
//            if (_Revision == version2._Revision)
//                return 0;
//            if (_Revision > version2._Revision)
//                return 1;
//            return -1;
//        }

//        public int CompareTo(Version value)
//        {
//            if (value == null)
//                return 1;
//            if (_Major != value._Major)
//            {
//                if (_Major > value._Major)
//                    return 1;
//                return -1;
//            }
//            if (_Minor != value._Minor)
//            {
//                if (_Minor > value._Minor)
//                    return 1;
//                return -1;
//            }
//            if (_Build != value._Build)
//            {
//                if (_Build > value._Build)
//                    return 1;
//                return -1;
//            }
//            if (_Revision == value._Revision)
//                return 0;
//            if (_Revision > value._Revision)
//                return 1;
//            return -1;
//        }

//        public override bool Equals(object obj)
//        {
//            var version = (obj as Version);
//            if (version == null)
//                return false;
//            return (((_Major == version._Major) && (_Minor == version._Minor)) && ((_Build == version._Build) && (_Revision == version._Revision)));
//        }

//        public bool Equals(Version obj)
//        {
//            if (obj == null)
//                return false;
//            return (((_Major == obj._Major) && (_Minor == obj._Minor)) && ((_Build == obj._Build) && (_Revision == obj._Revision)));
//        }

//        public override int GetHashCode()
//        {
//            int num = 0;
//            num |= (_Major & 15) << 0x1c;
//            num |= (_Minor & 0xff) << 20;
//            num |= (_Build & 0xff) << 12;
//            return (num | (_Revision & 0xfff));
//        }

//        public static bool operator ==(Version v1, Version v2)
//        {
//            if (object.ReferenceEquals(v1, null))
//                return object.ReferenceEquals(v2, null);
//            return v1.Equals(v2);
//        }

//        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        public static bool operator >(Version v1, Version v2) { return (v2 < v1); }

//        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        public static bool operator >=(Version v1, Version v2) { return (v2 <= v1); }

//        public static bool operator !=(Version v1, Version v2) { return !(v1 == v2); }

//        public static bool operator <(Version v1, Version v2)
//        {
//            if (v1 == null)
//                throw new ArgumentNullException("v1");
//            return (v1.CompareTo(v2) < 0);
//        }

//        public static bool operator <=(Version v1, Version v2)
//        {
//            if (v1 == null)
//                throw new ArgumentNullException("v1");
//            return (v1.CompareTo(v2) <= 0);
//        }

//        public static Version Parse(string input)
//        {
//            if (input == null)
//                throw new ArgumentNullException("input");
//            var result = new VersionResult();
//            result.Init("input", true);
//            if (!TryParseVersion(input, ref result))
//                throw result.GetVersionParseException();
//            return result._parsedVersion;
//        }

//        public override string ToString()
//        {
//            if (_Build == -1)
//                return ToString(2);
//            if (_Revision == -1)
//                return ToString(3);
//            return ToString(4);
//        }

//        public string ToString(int fieldCount)
//        {
//            switch (fieldCount)
//            {
//                case 0:
//                    return string.Empty;
//                case 1:
//                    return (_Major);
//                case 2:
//                    return (_Major + "." + _Minor);
//            }
//            if (_Build == -1)
//                throw new ArgumentException(SR.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper", new object[] { "0", "2" }), "fieldCount");
//            if (fieldCount == 3)
//                return string.Concat(new object[] { _Major, ".", _Minor, ".", _Build });
//            if (_Revision == -1)
//                throw new ArgumentException(SR.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper", new object[] { "0", "3" }), "fieldCount");
//            if (fieldCount != 4)
//                throw new ArgumentException(SR.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper", new object[] { "0", "4" }), "fieldCount");
//            return string.Concat(new object[] { Major, ".", _Minor, ".", _Build, ".", _Revision });
//        }

//        public static bool TryParse(string input, out Version result)
//        {
//            var result2 = new VersionResult();
//            result2.Init("input", false);
//            bool flag = TryParseVersion(input, ref result2);
//            result = result2._parsedVersion;
//            return flag;
//        }

//        private static bool TryParseComponent(string component, string componentName, ref VersionResult result, out int parsedComponent)
//        {
//            if (!int.TryParse(component, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedComponent))
//            {
//                result.SetFailure(ParseFailureKind.FormatException, component);
//                return false;
//            }
//            if (parsedComponent < 0)
//            {
//                result.SetFailure(ParseFailureKind.ArgumentOutOfRangeException, componentName);
//                return false;
//            }
//            return true;
//        }

//        private static bool TryParseVersion(string version, ref VersionResult result)
//        {
//            int num;
//            int num2;
//            if (version == null)
//            {
//                result.SetFailure(ParseFailureKind.ArgumentNullException);
//                return false;
//            }
//            string[] strArray = version.Split(new char[] { '.' });
//            int length = strArray.Length;
//            if ((length < 2) || (length > 4))
//            {
//                result.SetFailure(ParseFailureKind.ArgumentException);
//                return false;
//            }
//            if (!TryParseComponent(strArray[0], "version", ref result, out num))
//                return false;
//            if (!TryParseComponent(strArray[1], "version", ref result, out num2))
//                return false;
//            length -= 2;
//            if (length > 0)
//            {
//                int num3;
//                if (!TryParseComponent(strArray[2], "build", ref result, out num3))
//                    return false;
//                length--;
//                if (length > 0)
//                {
//                    int num4;
//                    if (!TryParseComponent(strArray[3], "revision", ref result, out num4))
//                        return false;
//                    result._parsedVersion = new Version(num, num2, num3, num4);
//                }
//                else
//                    result._parsedVersion = new Version(num, num2, num3);
//            }
//            else
//                result._parsedVersion = new Version(num, num2);
//            return true;
//        }

//        public int Build
//        {
//            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//            get { return _Build; }
//        }

//        public int Major
//        {
//            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//            get { return _Major; }
//        }

//        public short MajorRevision
//        {
//            get { return (short)(_Revision >> 0x10); }
//        }

//        public int Minor
//        {
//            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//            get { return _Minor; }
//        }

//        public short MinorRevision
//        {
//            get { return (short)(_Revision & 0xffff); }
//        }

//        public int Revision
//        {
//            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//            get { return _Revision; }
//        }

//        internal enum ParseFailureKind
//        {
//            ArgumentNullException,
//            ArgumentException,
//            ArgumentOutOfRangeException,
//            FormatException
//        }

//        [StructLayout(LayoutKind.Sequential)]
//        internal struct VersionResult
//        {
//            internal Version _parsedVersion;
//            internal Version.ParseFailureKind _failure;
//            internal string _exceptionArgument;
//            internal string _argumentName;
//            internal bool _canThrow;
//            internal void Init(string argumentName, bool canThrow)
//            {
//                _canThrow = canThrow;
//                _argumentName = argumentName;
//            }

//            internal void SetFailure(Version.ParseFailureKind failure) { SetFailure(failure, string.Empty); }
//            internal void SetFailure(Version.ParseFailureKind failure, string argument)
//            {
//                _failure = failure;
//                _exceptionArgument = argument;
//                if (_canThrow)
//                    throw GetVersionParseException();
//            }

//            internal Exception GetVersionParseException()
//            {
//                switch (_failure)
//                {
//                    case Version.ParseFailureKind.ArgumentNullException:
//                        return new ArgumentNullException(_argumentName);
//                    case Version.ParseFailureKind.ArgumentException:
//                        return new ArgumentException(SR.GetResourceString("Arg_VersionString"));
//                    case Version.ParseFailureKind.ArgumentOutOfRangeException:
//                        return new ArgumentOutOfRangeException(_exceptionArgument, SR.GetResourceString("ArgumentOutOfRange_Version"));
//                    case Version.ParseFailureKind.FormatException:
//                        try { int.Parse(_exceptionArgument, CultureInfo.InvariantCulture); }
//                        catch (FormatException exception) { return exception; }
//                        catch (OverflowException exception2) { return exception2; }
//                        return new FormatException(SR.GetResourceString("Format_InvalidString"));
//                }
//                return new ArgumentException(SR.GetResourceString("Arg_VersionString"));
//            }
//        }
//    }
//}
//#endif