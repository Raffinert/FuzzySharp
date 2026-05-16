using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

public interface IPatternMatchVector<in TKey> : IDisposable where TKey : notnull, IEquatable<TKey>
{
    int Length { get; }
    int Blocks { get; }
    ReadOnlySpan<ulong> GetOrZero(TKey key);

    bool ContainsKey(TKey key);
}

internal interface IPatternMatchVectorImpl<in TKey> : IPatternMatchVector<TKey> where TKey : IEquatable<TKey>
{
    void AddBit(TKey key, int position);
}

public sealed class PatternMatchVector
{
    public static IPatternMatchVector<T> Create<T>(ReadOnlySpan<T> source) where T : notnull, IEquatable<T>
    {
        var blocks = (source.Length + 63) >> 6;

        var pmv = typeof(T) == typeof(char)
            ? (IPatternMatchVectorImpl<T>)(object)new PatternMatchVectorChar(source.Length, estimatedNonAsciiCharCount: 8, blocks: blocks)
            : new PatternMatchVector<T>(source.Length, 64, blocks);

        var i = 0;

        foreach (var item in source)
        {
            pmv.AddBit(item, i++);
        }

        return pmv;
    }
}

internal sealed class PatternMatchVector<T> : IPatternMatchVectorImpl<T> where T : notnull, IEquatable<T>
{
    private readonly ArrayPool<ulong> _pool;
    private readonly DictionarySlimPooled<T, int> _indexMap;
    private ulong[] _buffer;
    private int _capacity;
    private int _next;
    private readonly ulong[] _zeroMask;
    private bool _disposed;

    public PatternMatchVector(int length, int estimatedCharCount, int blocks, ArrayPool<ulong> pool = null)
    {
        _pool = pool ?? ArrayPool<ulong>.Shared;
        Length = length;
        Blocks = blocks;
        _capacity = estimatedCharCount;
        _buffer = _pool.Rent(_capacity * Blocks);
        _zeroMask = _pool.Rent(Blocks);
        Array.Clear(_zeroMask, 0, Blocks);
        _indexMap = new DictionarySlimPooled<T, int>(estimatedCharCount);
        _next = 0;
    }

    public int Length { get; }
    public int Blocks { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddBit(T key, int position)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVector<T>));

        ref var index = ref _indexMap.GetOrAddValueRef(key);

        if (index == 0)
        {
            if (_next >= _capacity)
            {
                GrowBuffer();
            }

            index = ++_next;

            Array.Clear(_buffer, (index - 1) * Blocks, Blocks);
        }

        int block = position >> 6;
        int offset = position & 63;

        _buffer[(index - 1) * Blocks + block] |= 1UL << offset;
    }

    private void GrowBuffer()
    {
        int newCapacity = _capacity * 2;
        ulong[] newBuffer = _pool.Rent(newCapacity * Blocks);

        // Copy existing masks
        Array.Copy(_buffer, 0, newBuffer, 0, _capacity * Blocks);

        // Return old buffer
        _pool.Return(_buffer);

        _buffer = newBuffer;
        _capacity = newCapacity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(T key)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVector<T>));

        return _indexMap.ContainsKey(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetMask(T key, out ReadOnlySpan<ulong> mask)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVector<T>));

        if (_indexMap.TryGetValue(key, out var index))
        {
            mask = new ReadOnlySpan<ulong>(_buffer, (index - 1) * Blocks, Blocks);
            return true;
        }
        mask = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<ulong> GetOrZero(T key)
    {
        return TryGetMask(key, out var mask) ? mask : _zeroMask;
    }

    public ReadOnlySpan<ulong> GetOrDefault(T key, ReadOnlySpan<ulong> fallback)
    {
        return TryGetMask(key, out var mask) ? mask : fallback;
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _indexMap.Dispose();
        _pool.Return(_buffer);
        _pool.Return(_zeroMask);

        _disposed = true;
    }
}