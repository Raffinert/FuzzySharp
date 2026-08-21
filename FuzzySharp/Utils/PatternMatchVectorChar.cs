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
internal sealed class PatternMatchVectorChar : IPatternMatchVectorImpl<char>
{
    private readonly ArrayPool<ulong> _pool;
    private DictionarySlimPooled<char, int> _indexMap; // non-ASCII only (1-based), allocated lazily

    private readonly ulong[] _fixedData; // Single rental: [asciiMasks (256*blocks) | asciiPresence (4) | zeroMask (blocks)]
    private readonly int _asciiMasksOffset;
    private readonly int _asciiPresenceOffset;
    private readonly int _zeroMaskOffset;

    private ulong[] _buffer;     // capacity * blocks for non-ASCII (rented lazily, can grow)

    private readonly int _blocks;
    private int _capacity;
    private int _next;

    private bool _disposed;

    public int Length { get; }
    public int Blocks => _blocks;

    public PatternMatchVectorChar(int length, int estimatedNonAsciiCharCount, int blocks, ArrayPool<ulong> pool = null)
    {
        if (blocks < 0) throw new ArgumentOutOfRangeException(nameof(blocks));
        if (estimatedNonAsciiCharCount < 0) throw new ArgumentOutOfRangeException(nameof(estimatedNonAsciiCharCount));

        Length = length;

        _pool = pool ?? ArrayPool<ulong>.Shared;
        _blocks = blocks;

        // Single rental for all fixed-size data:
        // Layout: [asciiMasks (256*blocks) | asciiPresence (4) | zeroMask (blocks)]
        int totalFixedSize = (256 * _blocks) + 4 + _blocks;
        _fixedData = _pool.Rent(totalFixedSize);
        
        _asciiMasksOffset = 0;
        _asciiPresenceOffset = 256 * _blocks;
        _zeroMaskOffset = _asciiPresenceOffset + 4;

        // Clear all fixed data
        Array.Clear(_fixedData, 0, totalFixedSize);

        // Non-ASCII state is allocated on first use so ASCII-only patterns avoid
        // an extra object allocation and three pool operations.
        _capacity = Math.Max(2, estimatedNonAsciiCharCount);
        _next = 0;
    }


    public void Populate(ReadOnlySpan<char> source)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVectorChar));

        if (_blocks == 1)
        {
            PopulateSingleBlock(source);
            return;
        }

        for (var block = 0; block < _blocks; block++)
        {
            var blockStart = block << 6;
            var blockLength = Math.Min(64, source.Length - blockStart);

            for (var offset = 0; offset < blockLength; offset++)
            {
                var key = source[blockStart + offset];
                var bit = 1UL << offset;

                if (key <= 255u)
                {
                    _fixedData[_asciiMasksOffset + (key * _blocks) + block] |= bit;
                    _fixedData[_asciiPresenceOffset + (key >> 6)] |= 1UL << (key & 63);
                }
                else
                {
                    AddNonAsciiBit(key, block, bit);
                }
            }
        }
    }

    private void PopulateSingleBlock(ReadOnlySpan<char> source)
    {
        for (var position = 0; position < source.Length; position++)
        {
            var key = source[position];
            var bit = 1UL << position;

            if (key <= 255u)
            {
                _fixedData[_asciiMasksOffset + key] |= bit;
                _fixedData[_asciiPresenceOffset + (key >> 6)] |= 1UL << (key & 63);
            }
            else
            {
                AddNonAsciiBit(key, 0, bit);
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddNonAsciiBit(char key, int block, ulong bit)
    {
        if (_indexMap == null)
        {
            InitializeNonAsciiStorage();
        }

        ref int index = ref _indexMap.GetOrAddValueRef(key);

        if (index == 0)
        {
            if (_next >= _capacity)
                GrowBuffer();

            index = ++_next;

            // Clear this character's slice once
            Array.Clear(_buffer, (index - 1) * _blocks, _blocks);
        }

        _buffer[(index - 1) * _blocks + block] |= bit;
    }

    private void InitializeNonAsciiStorage()
    {
        _buffer = _pool.Rent(_capacity * _blocks);
        // Intentionally do not clear the entire buffer. Each new key slice is cleared once.
        _indexMap = new DictionarySlimPooled<char, int>(_capacity);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetMask(char key, out ReadOnlySpan<ulong> mask)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVectorChar));

        if (key <= 255u)
        {
            // Check presence bitmap instead of scanning the mask
            int presenceIndex = key >> 6;
            int presenceOffset = key & 63;

            if ((_fixedData[_asciiPresenceOffset + presenceIndex] & (1UL << presenceOffset)) != 0)
            {
                mask = new ReadOnlySpan<ulong>(_fixedData, _asciiMasksOffset + (key * _blocks), _blocks);
                return true;
            }

            mask = default;
            return false;
        }

        if (_indexMap != null && _indexMap.TryGetValue(key, out int index))
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
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVectorChar));

        // Dense ASCII masks are already zero for absent characters, so the
        // presence-bitmap lookup performed by TryGetMask is unnecessary here.
        if ((uint)key <= 255u)
        {
            return new ReadOnlySpan<ulong>(_fixedData, _asciiMasksOffset + (key * _blocks), _blocks);
        }

        if (_indexMap != null && _indexMap.TryGetValue(key, out int index))
        {
            return new ReadOnlySpan<ulong>(_buffer, (index - 1) * _blocks, _blocks);
        }

        return new ReadOnlySpan<ulong>(_fixedData, _zeroMaskOffset, _blocks);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<ulong> GetOrDefault(char key, ReadOnlySpan<ulong> fallback)
    {
        return TryGetMask(key, out var mask) ? mask : fallback;
    }

    /// <summary>
    /// Useful helper if you want to check "known key".
    /// For ASCII we check the presence bitmap.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(char key)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PatternMatchVectorChar));

        if (key <= 255u)
        {
            int presenceIndex = key >> 6;
            int presenceOffset = key & 63;
            return (_fixedData[_asciiPresenceOffset + presenceIndex] & (1UL << presenceOffset)) != 0;
        }

        return _indexMap != null && _indexMap.ContainsKey(key);
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

    public void Dispose()
    {
        if (_disposed) return;

        _pool.Return(_fixedData);

        if (_indexMap != null)
        {
            _indexMap.Dispose();
            _pool.Return(_buffer);
        }

        _disposed = true;
    }
}
