// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

// Adapted to use pooled arrays from the .NET Collections Extensions project
/// <summary>
/// A lightweight Dictionary with three principal differences compared to <see cref="Dictionary{TKey, TValue}"/>,
/// rewritten to use pooled arrays to avoid heap allocations after initial creation.
///
/// 1) It is possible to do "get or add" in a single lookup using <see cref="GetOrAddValueRef(TKey)"/>. For
///    values that are value types, this also saves a copy of the value.
/// 2) It assumes it is cheap to equate values.
/// 3) It assumes the keys implement <see cref="IEquatable{TKey}"/> or else Equals() and they are cheap and sufficient.
/// 4) Buckets and entries arrays are rented from <see cref="ArrayPool{T}"/>, and returned when cleared or resized,
///    minimizing allocations.
/// </summary>
[DebuggerTypeProxy(typeof(DictionarySlimPooledDebugView<,>))]
[DebuggerDisplay("Count = {Count}")]
internal sealed class DictionarySlimPooled<TKey, TValue> : IDisposable, IReadOnlyCollection<KeyValuePair<TKey, TValue>> where TKey : IEquatable<TKey>
{
    private static readonly Entry[] InitialEntries = new Entry[1];
    private static readonly int[] InitialBuckets = HashHelpers.SizeOneIntArray;

    private int _count;
    private int _freeList = -1; // 0-based index into _entries of head of free chain; -1 means empty
    private int[] _buckets;
    private Entry[] _entries;

    private readonly ArrayPool<int> _bucketsPool;
    private readonly ArrayPool<Entry> _entriesPool;
    private int _size;

    [DebuggerDisplay("({key}, {value})->{next}")]
    private struct Entry
    {
        public TKey key;
        public TValue value;
        // 0-based index of next entry in chain: -1 means end of chain.
        // If negative less than -1, it encodes that this entry is on the free list:
        //    next = -3 - previousFreeIndex. E.g. -3 means index 0 is free, -4 means index 1 is free, etc.
        public int next;
    }

    /// <summary>
    /// Construct with default capacity.
    /// </summary>
    public DictionarySlimPooled()
    {
        _bucketsPool = ArrayPool<int>.Shared;
        _entriesPool = ArrayPool<Entry>.Shared;
        _buckets = InitialBuckets;
        _entries = InitialEntries;
    }

    /// <summary>
    /// Construct with at least the specified capacity for
    /// entries before resizing must occur.
    /// </summary>
    /// <param name="capacity">Requested minimum capacity</param>
    public DictionarySlimPooled(int capacity)
    {
        if (capacity < 0)
            ThrowHelper.ThrowCapacityArgumentOutOfRangeException();

        if (capacity < 2)
            capacity = 2; // 1 would indicate the dummy array

        _size = HashHelpers.PowerOf2(capacity);

        _bucketsPool = ArrayPool<int>.Shared;
        _entriesPool = ArrayPool<Entry>.Shared;

        _buckets = _bucketsPool.Rent(_size);
        _entries = _entriesPool.Rent(_size);

        Array.Clear(_buckets, 0, _size);
        Array.Clear(_entries, 0, _size);
    }

    /// <summary>
    /// Count of entries in the dictionary.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Clears the dictionary. Note that this invalidates any active enumerators.
    /// Returns rented arrays to their pools.
    /// </summary>
    public void Clear()
    {
        if (_entries != InitialEntries)
        {
            // Return arrays to the pool before resetting to static empty
            _entriesPool.Return(_entries);
            _bucketsPool.Return(_buckets);

            _entries = InitialEntries;
            _buckets = InitialBuckets;
        }

        _count = 0;
        _freeList = -1;
    }

    /// <summary>
    /// Looks for the specified key in the dictionary.
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <returns>true if the key is present, otherwise false</returns>
    public bool ContainsKey(TKey key)
    {
        if (key == null) ThrowHelper.ThrowKeyArgumentNullException();

        var entries = _entries;
        int[] buckets = _buckets;
        int bucketMask = _size - 1;
        int hash = key.GetHashCode() & bucketMask;

        int collisionCount = 0;
        for (int i = buckets[hash] - 1; (uint)i < (uint)_size; i = entries[i].next)
        {
            if (key.Equals(entries[i].key))
                return true;

            if (collisionCount == _size)
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();

            collisionCount++;
        }

        return false;
    }

    /// <summary>
    /// Gets the value if present for the specified key.
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <param name="value">Value found, otherwise default(TValue)</param>
    /// <returns>true if the key is present, otherwise false</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null) ThrowHelper.ThrowKeyArgumentNullException();

        var entries = _entries;
        int[] buckets = _buckets;
        int bucketMask = _size - 1;
        int hash = key.GetHashCode() & bucketMask;

        int collisionCount = 0;
        for (int i = buckets[hash] - 1; (uint)i < (uint)_size; i = entries[i].next)
        {
            if (key.Equals(entries[i].key))
            {
                value = entries[i].value;
                return true;
            }

            if (collisionCount == _size)
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();

            collisionCount++;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Removes the entry if present with the specified key.
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <returns>true if the key is present, false if it is not</returns>
    public bool Remove(TKey key)
    {
        if (key == null) ThrowHelper.ThrowKeyArgumentNullException();

        var entries = _entries;
        int[] buckets = _buckets;
        int bucketMask = _size - 1;
        int hash = key.GetHashCode() & bucketMask;

        int last = -1;
        int i = buckets[hash] - 1;
        int collisionCount = 0;

        while (i != -1)
        {
            ref Entry candidate = ref entries[i];
            if (candidate.key.Equals(key))
            {
                if (last != -1)
                {
                    entries[last].next = candidate.next;
                }
                else
                {
                    buckets[hash] = candidate.next + 1;
                }

                // Clear entry and add to free list
                candidate = default;
                candidate.next = -3 - _freeList;
                _freeList = i;
                _count--;
                return true;
            }

            last = i;
            i = candidate.next;

            if (collisionCount == _size)
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();

            collisionCount++;
        }

        return false;
    }

    /// <summary>
    /// Gets the value for the specified key, or, if the key is not present,
    /// adds an entry and returns the value by ref. This makes it possible to
    /// add or update a value in a single look up operation.
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <returns>Reference to the new or existing value</returns>
    public ref TValue GetOrAddValueRef(TKey key)
    {
        if (key == null) ThrowHelper.ThrowKeyArgumentNullException();

        var entries = _entries;
        int[] buckets = _buckets;
        int bucketMask = _size - 1;
        int hash = key.GetHashCode() & bucketMask;

        int collisionCount = 0;
        for (int i = buckets[hash] - 1; (uint)i < (uint)_size; i = entries[i].next)
        {
            if (key.Equals(entries[i].key))
                return ref entries[i].value;

            if (collisionCount == _size)
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();

            collisionCount++;
        }

        return ref AddKey(key, hash);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private ref TValue AddKey(TKey key, int bucketIndex)
    {
        var entries = _entries;
        int entryIndex;

        if (_freeList != -1)
        {
            entryIndex = _freeList;
            _freeList = -3 - entries[_freeList].next;
        }
        else
        {
            if (_entries == InitialEntries || _count == _size)
            {
                entries = Resize();
                bucketIndex = key.GetHashCode() & (_size - 1);
                // entry indexes were not changed by Resize
            }

            entryIndex = _count;
        }

        entries[entryIndex].key = key;
        entries[entryIndex].next = _buckets[bucketIndex] - 1;
        _buckets[bucketIndex] = entryIndex + 1;
        _count++;
        return ref entries[entryIndex].value;
    }

    private Entry[] Resize()
    {
        int oldSize = _entries == InitialEntries ? 0 : _size;
        int newSize = oldSize == 0 ? 2 : oldSize * 2;
        if (unchecked((uint)newSize > (uint)int.MaxValue)) // uint cast handles overflow
        {
            throw new InvalidOperationException("DictionarySlimPooled: Capacity overflow.");
        }

        // Rent new arrays
        var newBuckets = _bucketsPool.Rent(newSize);
        var newEntries = _entriesPool.Rent(newSize);

        int count = _count;
        Array.Clear(newBuckets, 0, newSize);
        Array.Clear(newEntries, count, newSize - count);

        Array.Copy(_entries, 0, newEntries, 0, count);

        // Recompute buckets
        while (count-- > 0)
        {
            int bucketIndex = newEntries[count].key.GetHashCode() & (newSize - 1);
            newEntries[count].next = newBuckets[bucketIndex] - 1;
            newBuckets[bucketIndex] = count + 1;
        }

        // Return old arrays to pool
        _entriesPool.Return(_entries);
        _bucketsPool.Return(_buckets);

        _buckets = newBuckets;
        _entries = newEntries;

        _size = newSize;

        return _entries;
    }

    /// <summary>
    /// Gets an enumerator over the dictionary
    /// </summary>
    public Enumerator GetEnumerator() => new Enumerator(this);

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    /// <summary>
    /// Enumerator
    /// </summary>
    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly DictionarySlimPooled<TKey, TValue> _dictionary;
        private int _index;
        private int _remaining; // remaining count to enumerate

        internal Enumerator(DictionarySlimPooled<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _index = 0;
            _remaining = dictionary._count;
            Current = default;
        }

        /// <summary>
        /// Move to next
        /// </summary>
        public bool MoveNext()
        {
            if (_remaining == 0)
            {
                Current = default;
                return false;
            }

            var entries = _dictionary._entries;
            while (_index < _dictionary._size && entries[_index].next < -1)
            {
                _index++;
            }

            if (_index >= _dictionary._size)
            {
                Current = default;
                return false;
            }

            Current = new KeyValuePair<TKey, TValue>(
                entries[_index].key,
                entries[_index].value);

            _index++;
            _remaining--;
            return true;
        }

        public KeyValuePair<TKey, TValue> Current { get; private set; }

        object IEnumerator.Current => Current;
        public void Reset()
        {
            _index = 0;
            _remaining = _dictionary._count;
            Current = default;
        }
        public void Dispose() { }
    }

    public void Dispose()
    {
        Clear();
        GC.SuppressFinalize(this);
    }

    ~DictionarySlimPooled()
    {
        Clear();
    }
}

internal sealed class DictionarySlimPooledDebugView<K, V>(DictionarySlimPooled<K, V> dictionary)
    where K : IEquatable<K>
{
    private readonly DictionarySlimPooled<K, V> _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<K, V>[] Items => _dictionary.ToArray();
}