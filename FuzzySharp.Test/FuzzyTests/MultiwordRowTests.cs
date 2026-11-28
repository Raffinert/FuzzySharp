using System;
using System.Linq;
using Raffinert.FuzzySharp.Utils;
using Xunit;

namespace Raffinert.FuzzySharp.Test.FuzzyTests;

public class MultiwordRowTests
{
    [Fact]
    public void RowUpdatePropagatesEveryByteCarryCombination()
    {
        var state = new ulong[3];
        var matches = new ulong[3];
        const ulong low = (1UL << 56) - 1;
        for (int incoming = 0; incoming <= 1; incoming++)
        for (int value = 0; value < 256; value++)
        for (int mask = 0; mask < 256; mask++)
        {
            // The first word produces the incoming carry. Ones in the low
            // 56 bits propagate it into the byte under test. The final word
            // exposes the outgoing carry instead of discarding it.
            state[0] = incoming == 0 ? 0 : ulong.MaxValue;
            state[1] = ((ulong)value << 56) | low;
            state[2] = 0;
            matches[0] = (ulong)incoming;
            matches[1] = (ulong)mask << 56;
            int u = value & mask;
            int sum = value + u + incoming;
            ulong expected = ((ulong)((sum & 255) | (value - u)) << 56) | low;

            LongestCommonSubsequence.AdvanceMultiWordRow(state, matches);

            Assert.Equal(expected, state[1]);
            Assert.Equal((ulong)(sum >> 8), state[2]);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(8)]
    [InlineData(32)]
    public void RowUpdateMatchesIndependentBitwiseFullAdder(int words)
    {
        var random = new Random(390004 + words);
        var bytes = new byte[8];
        for (int sample = 0; sample < 1000; sample++)
        {
            var state = new ulong[words];
            var matches = new ulong[words];
            for (int i = 0; i < words; i++)
            {
                random.NextBytes(bytes);
                state[i] = BitConverter.ToUInt64(bytes, 0);
                random.NextBytes(bytes);
                matches[i] = BitConverter.ToUInt64(bytes, 0);
            }
            var expected = new ulong[words];
            ulong carry = 0;
            for (int word = 0; word < words; word++)
            {
                ulong s = state[word], u = s & matches[word], sum = 0;
                for (int bit = 0; bit < 64; bit++)
                {
                    ulong total = ((s >> bit) & 1) + ((u >> bit) & 1) + carry;
                    sum |= (total & 1) << bit;
                    carry = total >> 1;
                }
                expected[word] = sum | (s - u);
            }

            LongestCommonSubsequence.AdvanceMultiWordRow(state, matches);

            Assert.Equal(expected, state);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(65)]
    [InlineData(257)]
    [InlineData(1025)]
    public void InternalMaskLookupReturnsPresenceAndTheOriginalMask(int length)
    {
        const string alphabet = "abяєґ";
        var pattern = Enumerable.Range(0, length).Select(i => alphabet[i % alphabet.Length]).ToArray();
        pattern[length - 1] = 'Z'; // Present only in the final word for long patterns.
        using var characters = PatternMatchVector.Create<char>(pattern);
        using var generic = PatternMatchVector.Create<int>(Array.ConvertAll(pattern, c => (int)c));
        var characterLookup = (IPatternMatchVectorImpl<char>)characters;
        var genericLookup = (IPatternMatchVectorImpl<int>)generic;
        foreach (char symbol in alphabet + "Zz")
        {
            bool expected = Array.IndexOf(pattern, symbol) >= 0;
            Assert.Equal(expected, characterLookup.TryGetMask(symbol, out var characterMask));
            Assert.Equal(expected, genericLookup.TryGetMask(symbol, out var genericMask));
            if (expected)
            {
                Assert.Equal(characters.GetOrZero(symbol).ToArray(), characterMask.ToArray());
                Assert.Equal(characterMask.ToArray(), genericMask.ToArray());
            }
            else
            {
                Assert.True(characterMask.IsEmpty);
                Assert.True(genericMask.IsEmpty);
            }
        }
    }
}
