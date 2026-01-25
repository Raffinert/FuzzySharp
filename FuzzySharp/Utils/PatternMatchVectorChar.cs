using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Raffinert.FuzzySharp.Utils;

/// <summary>
/// PatternMatchVector specialized for char with RapidFuzz-style fast path:
/// - ASCII/extended-ASCII chars (0..255) stored in a dense array (no hashing)
/// - Non-ASCII chars stored in a pooled dictionary + pooled dense buffer
///
/// Each char maps to a mask of length <see cref="Blocks"/> ulongs.
/// Bit position indicates where that char occurs in the pattern.
/// </summary>
public sealed class PatternMatchVectorChar : IPatternMatchVector<char>
{
    private readonly ArrayPool<ulong> _pool;
    private readonly DictionarySlimPooled<char, int> _indexMap; // non-ASCII only (1-based)

    private readonly ulong[] _asciiMasks; // 256 * blocks
    private ulong[] _buffer;     // capacity * blocks for non-ASCII

    private readonly int _blocks;
    private int _capacity;
    private int _next;

    private readonly ulong[] _zeroMask; // blocks
    private bool _disposed;

    public int Blocks => _blocks;

    public PatternMatchVectorChar(int estimatedNonAsciiCharCount, int blocks, ArrayPool<ulong>? pool = null)
    {
        if (blocks < 0) throw new ArgumentOutOfRangeException(nameof(blocks));
        if (estimatedNonAsciiCharCount < 0) throw new ArgumentOutOfRangeException(nameof(estimatedNonAsciiCharCount));

        _pool = pool ?? ArrayPool<ulong>.Shared;
        _blocks = blocks;

        // ASCII (0..255)
        _asciiMasks = _pool.Rent(256 * _blocks);
        Array.Clear(_asciiMasks, 0, 256 * _blocks);

        // Non-ASCII buffer
        _capacity = Math.Max(2, estimatedNonAsciiCharCount);
        _buffer = _pool.Rent(_capacity * _blocks);
        // Intentionally not clearing entire _buffer. Each new key slice is cleared once.

        _zeroMask = _pool.Rent(_blocks);
        Array.Clear(_zeroMask, 0, _blocks);

        _indexMap = new DictionarySlimPooled<char, int>(estimatedNonAsciiCharCount);
        _next = 0;
    }

    /// <summary>
    /// Builds a PMV for a pattern (source) span.
    /// </summary>
    public static PatternMatchVectorChar Create(ReadOnlySpan<char> source, ArrayPool<ulong>? pool = null)
    {
        int blocks = (source.Length + 63) >> 6;

        // Estimate non-ASCII count cheaply: usually 0, so don't over-allocate.
        // If you want a better estimate, you can scan and count >255 chars.
        var pmv = new PatternMatchVectorChar(estimatedNonAsciiCharCount: 8, blocks: blocks, pool: pool);

        for (int i = 0; i < source.Length; i++)
        {
            pmv.AddBit(source[i], i);
        }

        return pmv;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddBit(char key, int position)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVectorChar));

        int block = position >> 6;
        int offset = position & 63;

        // Fast path: ASCII / extended ASCII
        if ((uint)key <= 255u)
        {
            _asciiMasks[(key * _blocks) + block] |= 1UL << offset;
            return;
        }

        // Non-ASCII: dictionary -> index -> buffer slice
        ref int index = ref _indexMap.GetOrAddValueRef(key);

        if (index == 0)
        {
            if (_next >= _capacity)
                GrowBuffer();

            index = ++_next;

            // Clear this character's slice once
            Array.Clear(_buffer, (index - 1) * _blocks, _blocks);
        }

        _buffer[(index - 1) * _blocks + block] |= 1UL << offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetMask(char key, out ReadOnlySpan<ulong> mask)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVectorChar));

        if ((uint)key <= 255u)
        {
            int start = key * _blocks;

            if (!IsAllZero(_asciiMasks, start, _blocks))
            {
                mask = new ReadOnlySpan<ulong>(_asciiMasks, start, _blocks);
                return true;
            }

            mask = default;
            return false;
        }

        if (_indexMap.TryGetValue(key, out int index))
        {
            mask = new ReadOnlySpan<ulong>(_buffer, (index - 1) * _blocks, _blocks);
            return true;
        }

        mask = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<ulong> GetOrZero(char key)
    {
        return TryGetMask(key, out var mask) ? mask : _zeroMask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<ulong> GetOrDefault(char key, ReadOnlySpan<ulong> fallback)
    {
        return TryGetMask(key, out var mask) ? mask : fallback;
    }

    /// <summary>
    /// Useful helper if you want to check "known key".
    /// For ASCII we infer it by "any bits set".
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(char key)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVectorChar));

        if ((uint)key <= 255u)
            return !IsAllZero(_asciiMasks, key * _blocks, _blocks);

        return _indexMap.ContainsKey(key);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowBuffer()
    {
        int newCapacity = _capacity * 2;
        ulong[] newBuffer = _pool.Rent(newCapacity * _blocks);

        // Copy existing non-ASCII masks
        Array.Copy(_buffer, 0, newBuffer, 0, _capacity * _blocks);

        _pool.Return(_buffer);

        _buffer = newBuffer;
        _capacity = newCapacity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAllZero(ulong[] array, int start, int length)
    {
        for (int i = 0; i < length; i++)
        {
            if (array[start + i] != 0)
                return false;
        }
        return true;
    }

    public void Dispose()
    {
        if (_disposed) return;

        _indexMap.Dispose();

        _pool.Return(_asciiMasks);
        _pool.Return(_buffer);
        _pool.Return(_zeroMask);

        _disposed = true;
    }
}
