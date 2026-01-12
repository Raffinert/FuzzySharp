using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

public sealed class CharMaskBuffer<T> : IDisposable where T : notnull, IEquatable<T>
{
    private readonly ArrayPool<ulong> _pool;
    private readonly DictionarySlimPooled<T, int> _indexMap;
    private ulong[] _buffer;
    private readonly int _blocks;
    private int _capacity;
    private int _next;
    private readonly ulong[] _zeroMask;
    private bool _disposed;

    public CharMaskBuffer(int estimatedCharCount, int blocks, ArrayPool<ulong> pool = null)
    {
        _pool = pool ?? ArrayPool<ulong>.Shared;
        _blocks = blocks;
        _capacity = estimatedCharCount;
        _buffer = _pool.Rent(_capacity * _blocks);
        _zeroMask = _pool.Rent(_blocks);
        Array.Clear(_zeroMask, 0, _blocks);
        _indexMap = new DictionarySlimPooled<T, int>(estimatedCharCount);
        _next = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddBit(T key, int position)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(CharMaskBuffer<T>));

        ref var index = ref _indexMap.GetOrAddValueRef(key);

        if (index == 0)
        {
            if (_next >= _capacity)
            {
                GrowBuffer();
            }

            index = ++_next;

            Array.Clear(_buffer, (index - 1) * _blocks, _blocks);
        }

        int block = position >> 6;
        int offset = position & 63;

        _buffer[(index - 1) * _blocks + block] |= 1UL << offset;
    }

    private void GrowBuffer()
    {
        int newCapacity = _capacity * 2;
        ulong[] newBuffer = _pool.Rent(newCapacity * _blocks);

        // Copy existing masks
        Array.Copy(_buffer, 0, newBuffer, 0, _capacity * _blocks);

        // Return old buffer
        _pool.Return(_buffer);

        _buffer = newBuffer;
        _capacity = newCapacity;
    }

    public bool ContainsKey(T key)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(CharMaskBuffer<T>));

        return _indexMap.ContainsKey(key);
    }

    public bool TryGetMask(T key, out ReadOnlySpan<ulong> mask)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(CharMaskBuffer<T>));

        if (_indexMap.TryGetValue(key, out var index))
        {
            mask = new ReadOnlySpan<ulong>(_buffer, (index - 1) * _blocks, _blocks);
            return true;
        }
        mask = default;
        return false;
    }

    public ReadOnlySpan<ulong> GetOrZero(T key)
    {
        return TryGetMask(key, out var mask) ? mask : _zeroMask;
    }

    public ReadOnlySpan<ulong> GetOrDefault(T key, ReadOnlySpan<ulong> fallback)
    {
        return TryGetMask(key, out var mask) ? mask : fallback;
    }

    ~CharMaskBuffer()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _indexMap.Dispose();
            _pool.Return(_buffer);
            _pool.Return(_zeroMask);
        }

        _disposed = true;
    }
}