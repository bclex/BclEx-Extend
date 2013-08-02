#region Foreign-License
// .Net40 Polyfill
#endregion
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime;

namespace System.Net
{
    internal class TrackingValidationObjectDictionary : StringDictionary
    {
        private IDictionary<string, object> _internalObjects;
        private readonly IDictionary<string, ValidateAndParseValue> _validators;

        internal delegate object ValidateAndParseValue(object valueToValidate);

        internal TrackingValidationObjectDictionary(IDictionary<string, ValidateAndParseValue> validators)
        {
            IsChanged = false;
            _validators = validators;
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public override void Add(string key, string value)
        {
            PersistValue(key, value, true);
        }

        public override void Clear()
        {
            if (_internalObjects != null)
                _internalObjects.Clear();
            base.Clear();
            IsChanged = true;
        }

        internal object InternalGet(string key)
        {
            if (_internalObjects != null && _internalObjects.ContainsKey(key))
                return _internalObjects[key];
            return base[key];
        }

        internal void InternalSet(string key, object value)
        {
            if (_internalObjects == null)
                _internalObjects = new Dictionary<string, object>();
            _internalObjects[key] = value;
            base[key] = value.ToString();
            IsChanged = true;
        }

        private void PersistValue(string key, string value, bool addValue)
        {
            key = key.ToLowerInvariant();
            if (!string.IsNullOrEmpty(value))
            {
                if (_validators != null && _validators.ContainsKey(key))
                {
                    var obj2 = _validators[key](value);
                    if (_internalObjects == null)
                        _internalObjects = new Dictionary<string, object>();
                    if (addValue)
                    {
                        _internalObjects.Add(key, obj2);
                        base.Add(key, obj2.ToString());
                    }
                    else
                    {
                        _internalObjects[key] = obj2;
                        base[key] = obj2.ToString();
                    }
                }
                else if (addValue)
                    base.Add(key, value);
                else
                    base[key] = value;
                IsChanged = true;
            }
        }

        public override void Remove(string key)
        {
            if (_internalObjects != null && _internalObjects.ContainsKey(key))
                _internalObjects.Remove(key);
            base.Remove(key);
            IsChanged = true;
        }

        internal bool IsChanged { get; set; }

        public override string this[string key]
        {
            get { return base[key]; }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set { PersistValue(key, value, false); }
        }
    }
}

