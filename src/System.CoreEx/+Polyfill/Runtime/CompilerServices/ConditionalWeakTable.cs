#region Foreign-License
// .Net40 Polyfill
#endregion
#if !CLR4
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;
namespace System.Runtime.CompilerServices
{
    /*
    * Description: Compiler support for runtime-generated "object fields."
    *
    *    Lets DLR and other language compilers expose the ability to attach arbitrary "properties" to instanced managed objects at runtime.
    *
    *    We expose this support as a dictionary whose keys are the instanced objects and the values are the "properties."
    *
    *    Unlike a regular dictionary, ConditionalWeakTables will not keep keys alive.
    *
    *
    * Lifetimes of keys and values:
    *
    *    Inserting a key and value into the dictonary will not prevent the key from dying, even if the key is strongly reachable
    *    from the value.
    *
    *    Prior to ConditionalWeakTable, the CLR did not expose the functionality needed to implement this guarantee.
    *
    *    Once the key dies, the dictionary automatically removes the key/value entry.
    *
    *
    * Relationship between ConditionalWeakTable and Dictionary:
    *
    *    ConditionalWeakTable mirrors the form and functionality of the IDictionary interface for the sake of api consistency.
    *
    *    Unlike Dictionary, ConditionalWeakTable is fully thread-safe and requires no additional locking to be done by callers.
    *
    *    ConditionalWeakTable defines equality as Object.ReferenceEquals(). ConditionalWeakTable does not invoke GetHashCode() overrides.
    *
    *    It is not intended to be a general purpose collection and it does not formally implement IDictionary or
    *    expose the full public surface area.
    *
    *
    * Thread safety guarantees:
    *
    *    ConditionalWeakTable is fully thread-safe and requires no additional locking to be done by callers.
    *
    *
    * OOM guarantees:
    *
    *    Will not corrupt unmanaged handle table on OOM. No guarantees about managed weak table consistency. Native handles reclamation
    *    may be delayed until appdomain shutdown.
    */
    /// <summary>
    /// ConditionalWeakTable
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [ComVisible(false)]
#if !COREINTERNAL
    public
#endif
 sealed class ConditionalWeakTable<TKey, TValue>
        where TKey : class
        where TValue : class
    {
        private int[] _buckets; // _buckets[hashcode & _buckets.Length] contains index of first entry in bucket (-1 if empty)
        private Entry[] _entries;
        private int _freeList; // -1 = empty, else index of first unused Entry
        private const int _initialCapacity = 5;
        private bool _invalid; // flag detects if OOM or other background exception threw us out of the lock.
        private object _lock; // this could be a ReaderWriterLock but CoreCLR does not support RWLocks. 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public delegate TValue CreateValueCallback(TKey key);

        [StructLayout(LayoutKind.Sequential)]
        private struct Entry
        {
            // Holds key and value using a weak reference for the key and a strong reference
            // for the value that is traversed only if the key is reachable without going through the value. 
            public DependentHandle depHnd;
            public int hashCode; // Cached copy of key's hashcode
            public int next; // Index of next entry, -1 if last 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalWeakTable&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        [SecuritySafeCritical]
        public ConditionalWeakTable()
        {
            _buckets = new int[0];
            _entries = new Entry[0];
            _freeList = -1;
            _lock = new object();
            // Resize at once (so won't need "if initialized" checks all over)
            Resize();
        }
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ConditionalWeakTable&lt;TKey, TValue&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        [SecuritySafeCritical]
        ~ConditionalWeakTable()
        {
            // We're just freeing per-appdomain unmanaged handles here. If we're already shutting down the AD, don't bother.
            // (Despite its name, Environment.HasShutdownStart also returns true if the current AD is finalizing.) 
            if (!Environment.HasShutdownStarted && (_lock != null))
                lock (_lock)
                    if (!_invalid)
                    {
                        Entry[] entries = _entries;
                        // Make sure anyone sneaking into the table post-resurrection gets booted before they can damage the native handle table.
                        _invalid = true;
                        _entries = null;
                        _buckets = null;
                        for (int entriesIndex = 0; entriesIndex < entries.Length; entriesIndex++)
                            entries[entriesIndex].depHnd.Free();
                    }
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [SecuritySafeCritical]
        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            lock (_lock)
            {
                VerifyIntegrity();
                _invalid = true;
                if (FindEntry(key) != -1)
                {
                    _invalid = false;
                    throw new ArgumentException("Argument_AddingDuplicate", "key");
                }
                CreateEntry(key, value);
                _invalid = false;
            }
        }

        [SecurityCritical]
        private void CreateEntry(TKey key, TValue value)
        {
            if (_freeList == -1)
                Resize();
            int hashCode = RuntimeHelpers.GetHashCode(key) & 0x7fffffff;
            int bucket = hashCode % _buckets.Length;
            int newEntry = _freeList;
            _freeList = _entries[newEntry].next;
            _entries[newEntry].hashCode = hashCode;
            _entries[newEntry].depHnd = new DependentHandle(key, value);
            _entries[newEntry].next = _buckets[bucket];
            _buckets[bucket] = newEntry;
        }

        [SecurityCritical]
        private int FindEntry(TKey key)
        {
            int hashCode = RuntimeHelpers.GetHashCode(key) & 0x7fffffff;
            for (int entriesIndex = _buckets[hashCode % _buckets.Length]; entriesIndex != -1; entriesIndex = _entries[entriesIndex].next)
                if ((_entries[entriesIndex].hashCode == hashCode) && (_entries[entriesIndex].depHnd.GetPrimary() == key))
                    return entriesIndex;
            return -1;
        }

        // Kludge: CompilerServicesExtensions dependency
        internal int FindEntryForLazyValueHelper<TLazyKey>(TLazyKey key, bool isValueCreated)
        {
            Lazy<TLazyKey> lazy;
            for (int entriesIndex = 0; entriesIndex < _entries.Length; entriesIndex++)
            {
                var depHnd = _entries[entriesIndex].depHnd;
                if (depHnd.IsAllocated
                    && ((lazy = (Lazy<TLazyKey>)depHnd.GetPrimary()) != null)
                    && (lazy.IsValueCreated || isValueCreated)
                    && lazy.Value.Equals(key))
                    return entriesIndex;
            }
            return -1;
        }

        /// <summary>
        /// Gets the or create value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public TValue GetOrCreateValue(TKey key) { return GetValue(key, (TKey k) => Activator.CreateInstance<TValue>()); }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="createValueCallback">The create value callback.</param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public TValue GetValue(TKey key, CreateValueCallback createValueCallback)
        {
            TValue local;
            if (createValueCallback == null)
                throw new ArgumentNullException("createValueCallback");
            if (TryGetValue(key, out local))
                return local;
            // If we got here, the key is not currently in table. Invoke the callback (outside the lock) to generate the new value for the key.
            var newValue = createValueCallback(key);
            lock (_lock)
            {
                VerifyIntegrity();
                _invalid = true;
                // Now that we've retaken the lock, must recheck in case we lost a ---- to add the key.
                if (TryGetValueWorker(key, out local))
                {
                    _invalid = false;
                    return local;
                }
                // Verified in-lock that we won the ---- to add the key. Add it now. 
                CreateEntry(key, newValue);
                _invalid = false;
                return newValue;
            }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentException("key");
            lock (_lock)
            {
                VerifyIntegrity();
                _invalid = true;
                int hashCode = RuntimeHelpers.GetHashCode(key) & 0x7fffffff;
                int bucket = hashCode % _buckets.Length;
                int last = -1;
                for (int entriesIndex = _buckets[bucket]; entriesIndex != -1; entriesIndex = _entries[entriesIndex].next)
                {
                    if ((_entries[entriesIndex].hashCode == hashCode) && (_entries[entriesIndex].depHnd.GetPrimary() == key))
                    {
                        if (last == -1)
                            _buckets[bucket] = _entries[entriesIndex].next;
                        else
                            _entries[last].next = _entries[entriesIndex].next;
                        _entries[entriesIndex].depHnd.Free();
                        _entries[entriesIndex].next = _freeList;
                        _freeList = entriesIndex;
                        _invalid = false;
                        return true;
                    }
                    last = entriesIndex;
                }
                _invalid = false;
                return false;
            }
        }

        [SecurityCritical]
        private void Resize()
        {
            // Start by assuming we won't resize.
            int newSize = _buckets.Length;
            // If any expired keys exist, we won't resize.
            bool hasExpiredEntries = false;
            int entriesIndex;
            for (entriesIndex = 0; entriesIndex < _entries.Length; entriesIndex++)
                if (_entries[entriesIndex].depHnd.IsAllocated && (_entries[entriesIndex].depHnd.GetPrimary() == null))
                {
                    hasExpiredEntries = true;
                    break;
                }
            if (!hasExpiredEntries)
                newSize = HashHelpers.GetPrime((_buckets.Length == 0) ? 6 : (_buckets.Length * 2));
            // Reallocate both buckets and entries and rebuild the bucket and freelists from scratch.
            // This serves both to scrub entries with expired keys and to put the new entries in the proper bucket. 
            int newFreeList = -1;
            int[] newBuckets = new int[newSize];
            for (int bucketIndex = 0; bucketIndex < newSize; bucketIndex++)
                newBuckets[bucketIndex] = -1;
            var newEntries = new Entry[newSize];
            // Migrate existing entries to the new table. 
            for (entriesIndex = 0; entriesIndex < _entries.Length; entriesIndex++)
            {
                var depHnd = _entries[entriesIndex].depHnd;
                if (depHnd.IsAllocated && (depHnd.GetPrimary() != null))
                {
                    // Entry is used and has not expired. Link it into the appropriate bucket list.
                    int index = _entries[entriesIndex].hashCode % newSize;
                    newEntries[entriesIndex].depHnd = depHnd;
                    newEntries[entriesIndex].hashCode = _entries[entriesIndex].hashCode;
                    newEntries[entriesIndex].next = newBuckets[index];
                    newBuckets[index] = entriesIndex;
                }
                else
                {
                    // Entry has either expired or was on the freelist to begin with. Either way insert it on the new freelist. 
                    _entries[entriesIndex].depHnd.Free();
                    newEntries[entriesIndex].depHnd = new DependentHandle();
                    newEntries[entriesIndex].next = newFreeList;
                    newFreeList = entriesIndex;
                }
            }
            // Add remaining entries to freelist.
            while (entriesIndex != newEntries.Length)
            {
                newEntries[entriesIndex].depHnd = new DependentHandle();
                newEntries[entriesIndex].next = newFreeList;
                newFreeList = entriesIndex;
                entriesIndex++;
            }
            _buckets = newBuckets;
            _entries = newEntries;
            _freeList = newFreeList;
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentException("key");
            lock (_lock)
            {
                VerifyIntegrity();
                return TryGetValueWorker(key, out value);
            }
        }

        [SecurityCritical]
        private bool TryGetValueWorker(TKey key, out TValue value)
        {
            int index = FindEntry(key);
            if (index != -1)
            {
                object primary = null;
                object secondary = null;
                _entries[index].depHnd.GetPrimaryAndSecondary(out primary, out secondary);
                // Now that we've secured a strong reference to the secondary, must check the primary again to ensure it didn't expire
                // (otherwise, we open a ---- where TryGetValue misreports an expired key as a live key with a null value.) 
                if (primary != null)
                {
                    value = (TValue)secondary;
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        // Kludge: CompilerServicesExtensions dependency
        /// <summary>
        /// Tries the get value worker for lazy value helper.
        /// </summary>
        /// <typeparam name="TLazyKey">The type of the lazy key.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="isValueCreated">if set to <c>true</c> [is value created].</param>
        /// <returns></returns>
        [SecurityCritical]
        public bool TryGetValueWorkerForLazyValueHelper<TLazyKey>(TLazyKey key, out TValue value, bool isValueCreated)
        {
            int index = FindEntryForLazyValueHelper(key, isValueCreated);
            if (index != -1)
            {
                object primary = null;
                object secondary = null;
                _entries[index].depHnd.GetPrimaryAndSecondary(out primary, out secondary);
                // Now that we've secured a strong reference to the secondary, must check the primary again to ensure it didn't expire
                // (otherwise, we open a ---- where TryGetValue misreports an expired key as a live key with a null value.) 
                if (primary != null)
                {
                    value = (TValue)secondary;
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        private void VerifyIntegrity()
        {
            if (_invalid)
                throw new InvalidOperationException(SR.GetResourceString("CollectionCorrupted"));
        }
    }
}
#endif