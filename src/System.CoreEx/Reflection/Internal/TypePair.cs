// from automapper
using System.Runtime.InteropServices;
namespace System.Reflection.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TypePair : IEquatable<TypePair>
    {
        private readonly Type _sourceType;
        private readonly Type _destinationType;
        private readonly int _hashcode;

        public TypePair(Type sourceType, Type destinationType)
        {
            this = new TypePair();
            _sourceType = sourceType;
            _destinationType = destinationType;
            _hashcode = (_sourceType.GetHashCode() * 0x18d) ^ _destinationType.GetHashCode();
        }

        public bool Equals(TypePair other)
        {
            return (object.Equals(other._sourceType, this._sourceType) && object.Equals(other._destinationType, _destinationType));
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
                return false;
            if (obj.GetType() != typeof(TypePair))
                return false;
            return Equals((TypePair)obj);
        }

        public override int GetHashCode()
        {
            return _hashcode;
        }
    }
}